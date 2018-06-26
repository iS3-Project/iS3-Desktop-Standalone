using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using System.Data.Odbc;

using IS3.Core;
using IS3.Core.Serialization;
using IS3.Geology;

namespace IS3.Geology.Serialization
{
    #region Copyright Notice
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************

    #endregion
    public class GeologyDbDataLoader : DbDataLoader
    {
        public GeologyDbDataLoader(DbContext dbContext)
            : base(dbContext)
        { }

        // Read boreholes
        //
        public bool ReadBoreholes(DGObjects objs, string tableNameSQL,
            List<int> objsIDs)
        {
            string conditionSQL = WhereSQL(objsIDs);

            return ReadBoreholes(objs, tableNameSQL, conditionSQL, null);
        }

        public bool ReadBoreholes(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL, 
            string orderSQL)
        {
            try
            {
                _ReadBoreholes(objs, tableNameSQL, conditionSQL, 
                    orderSQL);
                _ReadBoreholeGeologies2(objs);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadBoreholes(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                if (IsDbNull(row, "ID"))
                    continue;

                Borehole bh = new Borehole(row);
                bh.id = ReadInt(row, "ID").Value;
                bh.name = ReadString(row, "Name");
                bh.fullName = ReadString(row, "FullName");
                bh.description = ReadString(row, "Description");
                bh.shape = ReadShape(row);

                bh.Type = ReadString(row, "BoreholeType");
                bh.Top = ReadDouble(row, "TopElevation").Value;
                bh.Base = bh.Top - ReadDouble(row, "BoreholeLength").Value;
                bh.Mileage = ReadDouble(row, "Mileage");

                objs[bh.key] = bh;
            }
        }

        void _ReadBoreholeGeologies(DGObjects objs)
        {
            // method1: maybe very slow because linq is slow.
            DataTable dt = objs.rawDataSet.Tables[1];
            foreach (Borehole bh in objs.values)
            {
                var rows = from row in dt.AsEnumerable()
                           where (int)row["BoreholeID"] == bh.id
                           orderby row["ElevationOfStratumBottom"] descending
                           select row;

                double top = bh.Top;
                foreach (DataRow x in rows)
                {
                    //if (x["StratumID"].GetType() == typeof(System.DBNull)
                    //    || x["ElevationOfStratumBottom"].GetType() == typeof(System.DBNull))
                    if (IsDbNull(x, "StratumID") || IsDbNull(x, "ElevationOfStratumBottom"))
                    {
                        string error = string.Format(
                            "Data table [{0}] error: [StratumID] or [ElevationOfStratumBottom] can't be null, [BoreholeID] = {1}."
                            + Environment.NewLine
                            + "This record is ignore. Checking data is strongly recommended.",
                            dt.TableName, x["BoreholeID"]);
                        ErrorReport.Report(error);
                        continue;
                    }
                    BoreholeGeology bg = new BoreholeGeology();
                    bg.StratumID = ReadInt(x, "StratumID").Value;
                    bg.Top = top;
                    bg.Base = ReadDouble(x, "ElevationOfStratumBottom").Value;

                    top = bg.Base;
                    bh.Geologies.Add(bg);
                }
            }
        }

        void _ReadBoreholeGeologies2(DGObjects objs)
        {
            if (objs.rawDataSet.Tables.Count <= 1)
                return;

            // method2: index the stratra info
            DataTable dt = objs.rawDataSet.Tables[1];
            Dictionary<int, List<BoreholeGeology>> strata_dict =
                new Dictionary<int, List<BoreholeGeology>>();

            // put the strata information into the dictionary
            foreach (DataRow row in dt.Rows)
            {
                if (IsDbNull(row, "StratumID") || IsDbNull(row, "ElevationOfStratumBottom"))
                {
                    string error = string.Format(
                        "Data table [{0}] error: [StratumID] or [ElevationOfStratumBottom] can't be null, [BoreholeID] = {1}."
                        + Environment.NewLine
                        + "This record is ignore. Checking data is strongly recommended.",
                        dt.TableName, row["BoreholeID"]);
                    ErrorReport.Report(error);
                    continue;
                }

                int bhID = ReadInt(row, "BoreholeID").Value;
                List<BoreholeGeology> geo = null;
                if (strata_dict.ContainsKey(bhID))
                    geo = strata_dict[bhID];
                else
                {
                    geo = new List<BoreholeGeology>();
                    strata_dict[bhID] = geo;
                }
                BoreholeGeology bg = new BoreholeGeology();
                bg.StratumID = ReadInt(row, "StratumID").Value;
                bg.Base = ReadDouble(row, "ElevationOfStratumBottom").Value;
                geo.Add(bg);
            }

            // sort the borehole geology
            foreach (var geo in strata_dict.Values)
            {
                geo.Sort((x,y) => x.StratumID.CompareTo(y.StratumID));
            }

            // add the geology to borehole
            foreach (Borehole bh in objs.values)
            {
                List<BoreholeGeology> geo = null;
                if (strata_dict.ContainsKey(bh.id))
                    geo = strata_dict[bh.id];
                else
                    continue;

                double top = bh.Top;
                foreach (var x in geo)
                {
                    x.Top = top;
                    top = x.Base;
                    bh.Geologies.Add(x);
                }
            }
        }

        // Read strata
        //
        public bool ReadStrata(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            try
            {
                _ReadStrata(objs, tableNameSQL, conditionSQL,
                    orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadStrata(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                if (IsDbNull(row, "ID"))
                    continue;

                Stratum st = new Stratum(row);
                st.name = ReadString(row, "Name");
                st.fullName = ReadString(row, "FullName");
                st.description = ReadString(row, "Description");

                st.id = ReadInt(row, "ID").Value;
                st.GeologyAge = ReadString(row, "GeologicalAge");
                st.FormationType = ReadString(row, "FormationType");
                st.Compaction = ReadString(row, "Compaction");
                st.ElevationRange = ReadString(row, "ElevationOfStratumBottom");
                st.ThicknessRange = ReadString(row, "Thickness");

                st.shape = ReadShape(row);

                objs[st.key] = st;
            }
        }

        // Read Soil properties
        //
        public bool ReadSoilProperties(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            try
            {
                _ReadSoilProperties(objs, tableNameSQL, conditionSQL,
                    orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadSoilProperties(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow reader in table.Rows)
            {
                if (IsDbNull(reader, "ID"))
                    continue;

                SoilProperty soilProp = new SoilProperty(reader);
                soilProp.id = ReadInt(reader, "ID").Value;
                soilProp.name = ReadString(reader, "Name");
                soilProp.StratumID = ReadInt(reader, "StratumID").Value;
                soilProp.StratumSectionID = ReadInt(reader, "StratumSectionID");

                soilProp.StaticProp.w = ReadDouble(reader, "w");
                soilProp.StaticProp.gama = ReadDouble(reader, "gama");
                soilProp.StaticProp.c = ReadDouble(reader, "c");
                soilProp.StaticProp.fai = ReadDouble(reader, "fai");
                soilProp.StaticProp.cuu = ReadDouble(reader, "cuu");
                soilProp.StaticProp.faiuu = ReadDouble(reader, "faiuu");
                soilProp.StaticProp.Cs = ReadDouble(reader, "Cs");
                soilProp.StaticProp.qu = ReadDouble(reader, "qu");
                soilProp.StaticProp.K0 = ReadDouble(reader, "K0");
                soilProp.StaticProp.Kv = ReadDouble(reader, "Kv");
                soilProp.StaticProp.Kh = ReadDouble(reader, "Kh");
                soilProp.StaticProp.e = ReadDouble(reader, "e");
                soilProp.StaticProp.av = ReadDouble(reader, "av");
                soilProp.StaticProp.Cu = ReadDouble(reader, "Cu");

                soilProp.StaticProp.G = ReadDouble(reader, "G");
                soilProp.StaticProp.Sr = ReadDouble(reader, "Sr");
                soilProp.StaticProp.ccq = ReadDouble(reader, "ccq");
                soilProp.StaticProp.faicq = ReadDouble(reader, "faicq");
                soilProp.StaticProp.c_s = ReadDouble(reader, "c_s");
                soilProp.StaticProp.fais = ReadDouble(reader, "fais");
                soilProp.StaticProp.a01_02 = ReadDouble(reader, "a01_02");
                soilProp.StaticProp.Es01_02 = ReadDouble(reader, "Es01_02");
                soilProp.StaticProp.ccu = ReadDouble(reader, "ccu");
                soilProp.StaticProp.faicu = ReadDouble(reader, "faicu");
                soilProp.StaticProp.cprime = ReadDouble(reader, "cprime");
                soilProp.StaticProp.faiprime = ReadDouble(reader, "faiprime");
                soilProp.StaticProp.E015_0025 = ReadDouble(reader, "E015_0025");
                soilProp.StaticProp.E02_0025 = ReadDouble(reader, "E02_0025");
                soilProp.StaticProp.E04_0025 = ReadDouble(reader, "E04_0025");

                SoilDynamicProperty dynamicProp = new SoilDynamicProperty();
                soilProp.DynamicProp.G0 = ReadDouble(reader, "G0");
                soilProp.DynamicProp.ar = ReadDouble(reader, "ar");
                soilProp.DynamicProp.br = ReadDouble(reader, "br");

                objs[soilProp.key] = soilProp;
            }
        }

        // Read StratumSections
        //
        public bool ReadStratumSections(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            try
            {
                _ReadStratumSections(objs, tableNameSQL,
                    conditionSQL, orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        public void _ReadStratumSections(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow reader in table.Rows)
            {
                if (IsDbNull(reader, "ID"))
                    continue;
                StratumSection sec = new StratumSection(reader);
                sec.id = ReadInt(reader, "ID").Value;
                sec.name = ReadString(reader, "Name");
                sec.StartMileage = ReadDouble(reader, "StartMileage");
                sec.EndMileage = ReadDouble(reader, "EndMileage");
                objs[sec.key] = sec;
            }
        }

        // Read River Water
        //
        public bool ReadRiverWaters(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            try
            {
                _ReadRiverWaters(objs, tableNameSQL,
                    conditionSQL, orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadRiverWaters(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow reader in table.Rows)
            {
                if (IsDbNull(reader, "ID"))
                    continue;
                RiverWater rw = new RiverWater(reader);
                rw.id = ReadInt(reader, "ID").Value;
                rw.ObservationLocation = ReadString(reader, "ObservationLocation");
                rw.HighestTidalLevel = ReadDouble(reader, "HighestTidalLevel");
                rw.HighestTidalLevelDate = ReadDateTime(reader, "HighestTidalLevelDate");
                rw.LowestTidalLevel = ReadDouble(reader, "LowestTidalLevel");
                rw.LowestTidalLevelDate = ReadDateTime(reader, "LowestTidalLevelDate");
                rw.AvHighTidalLevel = ReadDouble(reader, "AvHighTidalLevel");
                rw.AvLowTidalLevel = ReadDouble(reader, "AvLowTidalLevel");
                rw.AvTidalRange = ReadDouble(reader, "AvTidalRange");
                rw.DurationOfRise = ReadTimeSpan(reader, "DurationOfRise").ToString();
                rw.DurationOfFall = ReadTimeSpan(reader, "DurationOfFall").ToString();
                objs[rw.key] = rw;
            }
        }

        // Read River Water
        //
        public bool ReadPhreaticWaters(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            try
            {
                _ReadPhreaticWaters(objs, tableNameSQL,
                    conditionSQL, orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadPhreaticWaters(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow reader in table.Rows)
            {
                if (IsDbNull(reader, "ID"))
                    continue;
                PhreaticWater pw = new PhreaticWater(reader);
                pw.id = ReadInt(reader, "ID").Value;
                pw.SiteName = ReadString(reader, "SiteName");
                pw.AvBuriedDepth = ReadDouble(reader, "AvBuriedDepth");
                pw.AvElevation = ReadDouble(reader, "AvElevation");
                objs[pw.key] = pw;
            }
        }

        // Read Confined Water
        //
        public bool ReadConfinedWaters(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            try
            {
                _ReadConfinedWaters(objs, tableNameSQL,
                    conditionSQL, orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadConfinedWaters(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow reader in table.Rows)
            {
                if (IsDbNull(reader, "ID"))
                    continue;
                ConfinedWater cw = new ConfinedWater(reader);
                cw.id = ReadInt(reader, "ID").Value;
                cw.BoreholeName = ReadString(reader, "BoreholeName");
                cw.SiteName = ReadString(reader, "SiteName");
                cw.TopElevation = ReadDouble(reader, "TopElevation");
                cw.ObservationDepth = ReadDouble(reader, "ObservationDepth");
                cw.StratumName = ReadString(reader, "StatumName");
                cw.Layer = ReadInt(reader, "Layer");
                cw.WaterTable = ReadDouble(reader, "WaterTable");
                cw.ObservationDate = ReadDateTime(reader, "ObservationDate");
                objs[cw.key] = cw;
            }
        }

        // Read Water Properties
        //
        public bool ReadWaterProperties(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            try
            {
                _ReadWaterProperties(objs, tableNameSQL,
                    conditionSQL, orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }
        void _ReadWaterProperties(
            DGObjects objs,
            string tableNameSQL,
            string conditionSQL,
            string orderSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow reader in table.Rows)
            {
                if (IsDbNull(reader, "ID"))
                    continue;
                WaterProperty wp = new WaterProperty(reader);
                wp.id = ReadInt(reader, "ID").Value;
                wp.BoreholeName = ReadString(reader, "BoreholeName");
                wp.Cl = ReadDouble(reader, "Cl");
                wp.SO4 = ReadDouble(reader, "SO4");
                wp.Mg = ReadDouble(reader, "Mg");
                wp.NH = ReadDouble(reader, "NH");
                wp.pH = ReadDouble(reader, "pH");
                wp.CO2 = ReadDouble(reader, "CO2");
                wp.Corrosion = ReadString(reader, "Corrosion");
                objs[wp.key] = wp;
            }
        }

    }
}
