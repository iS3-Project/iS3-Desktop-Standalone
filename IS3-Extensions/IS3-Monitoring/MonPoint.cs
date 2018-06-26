using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;

using IS3.Core;
using IS3.Core.Serialization;
using IS3.Monitoring.Serialization;

namespace IS3.Monitoring
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
    //    Monitoring Point
    public class MonPoint : DGObject
    {
        // Summary:
        //    reference point name
        public string refPointName { get; set; }
        // Summary:
        //    distance to the reference point
        public double? distanceX { get; set; }
        public double? distanceY { get; set; }
        public double? distanceZ { get; set; }
        // Summary:
        //    Installation date and time
        public DateTime? time { get; set; }
        // Summary:
        //    Instrument detail
        public string instrumentDetail { get; set; }
        // Summary:
        //    Bearing of monitoring axis (A, B, C): in degree
        public double? bearingA { get; set; }
        public double? bearingB { get; set; }
        public double? bearingC { get; set; }
        // Summary:
        //    Inclination of instrument axis (A, B, C): in degree
        public double? inclinationA { get; set; }
        public double? inclinationB { get; set; }
        public double? inclinationC { get; set; }
        // Summary:
        //    Reading sign convention in direction (A, B, C)
        public string readingSignA { get; set; }
        public string readingSignB { get; set; }
        public string readingSignC { get; set; }
        // Summary:
        //    componennt count
        public int componentCount { get; set; }
        // Summary:
        //    Component names
        public string componentNames { get; set; }
        // Summary:
        //    Remarks
        public string remarks { get; set; }
        // Summary:
        //    Contractor who installed monitoring instrument
        public string contractor { get; set; }
        // Summary:
        //    Associated file reference
        public string fileName { get; set; }

        // Summary:
        //    readings dictionary - reading component indexed 
        public Dictionary<string, List<MonReading>> readingsDict;

        public List<MonReading> this[string key]
        {
            get { return readingsDict[key]; }
        }

        public MonPoint()
        {
            readingsDict = new Dictionary<string, List<MonReading>>();
        }
        public MonPoint(DataRow rawData)
            : base(rawData)
        {
            readingsDict = new Dictionary<string, List<MonReading>>();
        }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            MonitoringDGObjectLoader loader =
                new MonitoringDGObjectLoader(dbContext);
            bool success = loader.LoadMonPoints(objs);
            return success;
        }

        public override string ToString()
        {
            string str = base.ToString();

            ICollection<string> keys = readingsDict.Keys;
            string strKeys = ", Keys=";
            foreach (string key in keys)
            {
                strKeys += key + ",";
            }
            str += strKeys;

            return str;
        }

        public override List<DataView> tableViews(IEnumerable<DGObject> objs)
        {
            List<DataView> dataViews = base.tableViews(objs);

            for (int i = 1; i < parent.rawDataSet.Tables.Count; ++i)
            {
                DataTable table = parent.rawDataSet.Tables[i];
                string filter = nameFilter(objs);
                DataView view = new DataView(table, filter, "[monPointName]",
                    DataViewRowState.CurrentRows);
                dataViews.Add(view);
            }

            return dataViews;
        }

        string nameFilter(IEnumerable<DGObject> objs)
        {
            string sql = "monPointName in (";
            foreach (var obj in objs)
            {
                sql += '\'' + obj.name + '\'';
                sql += ",";
            }
            sql += ")";
            return sql;
        }

        public override List<FrameworkElement> chartViews(
            IEnumerable<DGObject> objs, double width, double height)
        {
            List<FrameworkElement> charts = new List<FrameworkElement>();
            FrameworkElement chart = 
                FormsCharting.getMonPointChart(objs, width, height);
            charts.Add(chart);
            return charts;
        }
    }
}
