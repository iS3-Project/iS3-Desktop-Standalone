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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;
using System.Windows.Forms.DataVisualization.Charting;

using IS3.Core;

namespace IS3.Monitoring
{
    public partial class MonGroupChartLinearZ : MonGroupChart
    {
        public MonGroupChartLinearZ(double width, double height)
            : base(width, height)
        { }

        public override void setObjs(IEnumerable<DGObject> objs)
        {
            base.setObjs(objs);

            if (objs == null || objs.Count() == 0)
                return;
            MonGroup firstMonGroup = objs.First() as MonGroup;
            string unit = ChartHelper.getMonGroupUnit(firstMonGroup);

            Chart chart1 = new Chart();
            chart1.Name = "Chart1";
            chart1.Text = "Chart1";

            ChartArea chartArea1 = new ChartArea("ChartArea1");
            ChartHelper.setChartAreaStyle(chartArea1);
            chartArea1.AxisX.Title = "Value (" + unit + ")";
            chartArea1.AxisY.Title = "Distance (m)";
            chart1.ChartAreas.Add(chartArea1);

            Legend legend1 = new Legend("Legend1");
            legend1.DockedToChartArea = "ChartArea1";
            chart1.Legends.Add(legend1);

            foreach (DGObject obj in objs)
            {
                MonGroup monGroup = obj as MonGroup;
                if (monGroup == null)
                    continue;
                if (monGroup.monPntDict.Count == 0)
                    continue;

                AddMonGroup(chart1, monGroup);

                chartHost.Child = chart1;
            }
        }

        void AddMonGroup(Chart chart, MonGroup monGroup)
        {
            ComboBoxItem selectedItem = DataCount.SelectedItem as ComboBoxItem;
            int dataCount = int.Parse(selectedItem.Content.ToString());
            int markStyle = 1;

            MonPoint firstPnt = monGroup.monPntDict.Values.First();
            foreach (string key in firstPnt.readingsDict.Keys)
            {
                List<MonReading> firstPnt_readings = firstPnt.readingsDict[key];
                int numReadings = firstPnt_readings.Count;
                if (numReadings < dataCount)
                    dataCount = numReadings;

                for (int i = 0; i < dataCount; ++i)
                {
                    int index = numReadings - i * numReadings / dataCount - 1;
                    MonReading firstPnt_reading = firstPnt_readings[index];
                    DateTime time = firstPnt_reading.time;

                    Series series1 = new Series();
                    series1.ChartType = SeriesChartType.Line;
                    series1.ChartArea = "ChartArea1";
                    series1.Name = string.Format("{0}: {1} ({2:d})",
                        monGroup.name, key, time);
                    series1.BorderWidth = 2;
                    series1.MarkerStyle = (MarkerStyle)(markStyle++ % 9);
                    series1.MarkerSize = 8;

                    foreach (MonPoint monPoint in monGroup.monPntDict.Values)
                    {
                        if (monPoint.readingsDict.ContainsKey(key) == false)
                            continue;
                        List<MonReading> readings = monPoint.readingsDict[key];
                        int readIndex = index;
                        if (readIndex > readings.Count - 1)
                            readIndex = readings.Count - 1;
                        MonReading reading = readings[readIndex];
                        double x = reading.value * _sign;
                        double y = monPoint.distanceZ.Value;
                        DataPoint dataPoint = new DataPoint(x, y);
                        if (_showName)
                            dataPoint.Label = monPoint.name;
                        dataPoint.ToolTip = "#VALX";
                        series1.Points.Add(dataPoint);
                    }

                    chart.Series.Add(series1);
                }
            }
        }
    }
}
