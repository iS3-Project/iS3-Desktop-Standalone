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

namespace IS3.Monitoring.Serialization
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

    // Summary:
    //    Monitoring Db data loader
    class MonitoringDbDataLoader : DbDataLoader
    {
        public MonitoringDbDataLoader(DbContext dbContext)
            : base(dbContext)
        { }

        // Summary:
        //     Read monitoring points
        public bool ReadMonPoints(DGObjects objs, string tableNameSQL,
            List<int> objsIDs)
        {
            string conditionSQL = WhereSQL(objsIDs);

            return ReadMonPoints(objs, tableNameSQL, conditionSQL, null);
        }

        // Summary:
        //    Read monitoring points from the specified tables, using condition and order
        public bool ReadMonPoints(DGObjects objs, string tableNameSQL,
            string conditionSQL, string orderSQL)
        {
            try
            {
                ReadRawData_Partial(objs, tableNameSQL, orderSQL, conditionSQL);
                _ReadMonPoints(objs, tableNameSQL, conditionSQL, orderSQL);
                _ReadMonReadings(objs);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }

        // Summary:
        //    Re-read monitoring point from the specified tables.
        //
        public bool RereadMonPoints(DGObjects objs, string tableNameSQL,
            string conditionSQL, string orderSQL)
        {
            try
            {
                // read raw data ingoring condition
                ReadRawData_Partial(objs, tableNameSQL, orderSQL, conditionSQL);

                // fill readings (note: objs are already exist)
                _ReadMonReadings(objs);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }

        void _ReadMonPoints(DGObjects objs, string tableNameSQL,
            string conditionSQL, string orderSQL)
        {
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                if (IsDbNull(row, "ID") || IsDbNull(row, "Name"))
                    continue;

                MonPoint obj = new MonPoint(row);
                obj.id = ReadInt(row, "ID").Value;
                obj.name = ReadString(row, "Name");
                obj.fullName = ReadString(row, "FullName");
                obj.description = ReadString(row, "Description");
                obj.shape = ReadShape(row);

                obj.refPointName = ReadString(row, "refPointName");
                obj.distanceX = ReadDouble(row, "distanceX");
                obj.distanceY = ReadDouble(row, "distanceY");
                obj.distanceZ = ReadDouble(row, "distanceZ");
                obj.time = ReadDateTime(row, "time");
                obj.instrumentDetail = ReadString(row, "instrumentDetail");
                obj.bearingA = ReadDouble(row, "bearingA");
                obj.bearingB = ReadDouble(row, "bearingB");
                obj.bearingC = ReadDouble(row, "bearingC");
                obj.inclinationA = ReadDouble(row, "inclinationA");
                obj.inclinationB = ReadDouble(row, "inclinationB");
                obj.inclinationC = ReadDouble(row, "inclinationC");
                obj.readingSignA = ReadString(row, "readingSignA");
                obj.readingSignB = ReadString(row, "readingSignB");
                obj.readingSignC = ReadString(row, "readingSignC");
                obj.componentCount = ReadInt(row, "componentCount").Value;
                obj.componentNames = ReadString(row, "componentNames");
                obj.remarks = ReadString(row, "remarks");
                obj.contractor = ReadString(row, "contractor");
                obj.fileName = ReadString(row, "fileName");

                objs[obj.key] = obj;
            }
        }

        void _ReadMonReadings(DGObjects objs)
        {
            if (objs.rawDataSet.Tables.Count <= 1)
                return;

            DataTable dt = objs.rawDataSet.Tables[1];
            foreach (DataRow row in dt.Rows)
            {
                if (IsDbNull(row, "monPointName") || IsDbNull(row, "time"))
                    continue;
                // if both the reading and the value are null, skip it
                if (IsDbNull(row, "reading") && IsDbNull(row, "value"))
                    continue;

                MonReading reading = new MonReading();
                reading.monPointName = ReadString(row, "monPointName");
                reading.time = ReadDateTime(row, "time").Value;
                reading.component = ReadString(row, "component");
                reading.reading = ReadString(row, "reading");
                reading.unit = ReadString(row, "unit");

                double? value = ReadDouble(row, "value");
                if (value == null)
                {
                    try
                    {
                        reading.value = Double.Parse(reading.reading);
                    }
                    catch(Exception)
                    {
                        // do nothing
                    }
                }
                else
                    reading.value = value.Value;

                MonPoint monPnt;
                if (objs.containsKey(reading.monPointName))
                    monPnt = objs[reading.monPointName] as MonPoint;
                else
                    continue;

                _AddReading(monPnt, reading);
            }
        }

        void _AddReading(MonPoint monPnt, MonReading reading)
        {
            Dictionary<string, List<MonReading>> readingsDict =
                monPnt.readingsDict;
            List<MonReading> readings;
            if (readingsDict.ContainsKey(reading.component))
                readings = readingsDict[reading.component];
            else
            {
                readings = new List<MonReading>();
                readingsDict[reading.component] = readings;
            }
            readings.Add(reading);
        }

        // Summary:
        //     Read monitoring groups
        public bool ReadMonGroups(DGObjects objs, string tableNameSQL,
            string conditionSQL, string orderSQL)
        {
            try
            {
                ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);
                _ReadMonGroups(objs, tableNameSQL, conditionSQL, orderSQL);
            }
            catch (DbException ex)
            {
                string str = ex.ToString();
                ErrorReport.Report(str);
                return false;
            }
            return true;
        }

        void _ReadMonGroups(DGObjects objs, string tableNameSQL,
            string conditionSQL, string orderSQL)
        {
            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                if (IsDbNull(row, "ID") || IsDbNull(row, "Name"))
                    continue;

                MonGroup obj = new MonGroup(row);
                obj.id = ReadInt(row, "ID").Value;
                obj.name = ReadString(row, "Name");
                obj.fullName = ReadString(row, "FullName");
                obj.description = ReadString(row, "Description");
                obj.shape = ReadShape(row);

                obj.groupShape = ReadString(row, "groupShape");
                string str = ReadString(row, "monPntNames");
                if (str != null)
                    obj.monPntNames = str.Split(_separator);

                objs[obj.key] = obj;
            }
        }
    }

}
