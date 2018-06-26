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
    //    Monitoring Group
    public class MonGroup: DGObject
    {
        // Summary:
        //    Shape of the group: line, circle
        public string groupShape { get; set; }
        // Summary:
        //    Monitoring points names in the group
        public string[] monPntNames { get; set; }
        // Summary:
        //   Monitoring points
        public Dictionary<string, MonPoint> monPntDict { get; set; }

        public MonPoint this[string key]
        {
            get { return monPntDict[key]; }
        }

        public MonGroup()
        {
            monPntDict = new Dictionary<string, MonPoint>();
        }

        public MonGroup(DataRow row)
            : base(row)
        {
            monPntDict = new Dictionary<string, MonPoint>();
        }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            MonitoringDGObjectLoader loader =
                new MonitoringDGObjectLoader(dbContext);
            bool success = loader.LoadMonGroups(objs);
            return success;
        }

        public override string ToString()
        {
            string str = base.ToString();

            ICollection<string> keys = monPntDict.Keys;
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

            DataSet dataSet = getMonGroupDataSet(objs);
            if (dataSet == null)
                return null;
            for (int i = 1; i < dataSet.Tables.Count; ++i)
            {
                DataTable table = dataSet.Tables[i];
                string filter = nameFilter(objs);
                DataView view = new DataView(table, filter, "[monPointName]",
                    DataViewRowState.CurrentRows);
                dataViews.Add(view);
            }

            return dataViews;
        }

        // Returns the MonPoint's parent (DGObjects) rawDataSet
        // because MonGroup doesn't stores readings.
        DataSet getMonGroupDataSet(IEnumerable<DGObject> objs)
        {
            if (objs.Count() > 0)
            {
                MonGroup group = objs.First() as MonGroup;
                if (group.monPntDict.Count > 0)
                {
                    MonPoint point = group.monPntDict.Values.First();
                    return point.parent.rawDataSet;
                }
            }
            return null;
        }

        string nameFilter(IEnumerable<DGObject> objs)
        {
            string sql = "monPointName in (";
            foreach (var obj in objs)
            {
                MonGroup group = obj as MonGroup;
                if (group == null)
                    continue;
                foreach (string name in group.monPntNames)
                {
                    sql += '\'' + name + '\'';
                    sql += ",";
                }
            }
            sql += ")";
            return sql;
        }

        public override List<FrameworkElement> chartViews(
            IEnumerable<DGObject> objs, double width, double height)
        {
            List<FrameworkElement> charts = new List<FrameworkElement>();

            // point curve
            List<MonPoint> allPoints = new List<MonPoint>();
            foreach (var obj in objs)
            {
                MonGroup group = obj as MonGroup;
                if (group == null)
                    continue;
                allPoints.AddRange(group.monPntDict.Values);
            }
            FrameworkElement chart =
                FormsCharting.getMonPointChart(allPoints, width, height);
            if (chart != null)
                charts.Add(chart);

            // group curve
            chart = FormsCharting.getMonGroupChart(objs, width, height);
            if (chart != null)
                charts.Add(chart);

            return charts;
        }

    }


}
