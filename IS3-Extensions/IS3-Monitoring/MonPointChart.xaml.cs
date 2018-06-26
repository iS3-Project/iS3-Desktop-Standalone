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
    /// <summary>
    /// Interaction logic for MonPointChart.xaml
    /// </summary>
    public partial class MonPointChart : UserControl
    {
        IEnumerable<DGObject> _objs;
        string _component = "ALL";
        public MonPointChart(double width, double height)
        {
            InitializeComponent();
            chartHost.Width = width - 12;
        }

        public void setObjs(IEnumerable<DGObject> objs)
        {
            _objs = objs;

            if (objs == null || objs.Count() == 0)
                return;
            MonPoint firstMonPoint = objs.First() as MonPoint;
            if (firstMonPoint == null)
                return;
            string unit = ChartHelper.getMonPointUnit(firstMonPoint);
            
            if (CBComponents.Items.Count == 1)
            {
                // initialize the display list when the control first show
                // add componenet name 
                foreach (string key in firstMonPoint.readingsDict.Keys)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = key;
                    CBComponents.Items.Add(item);
                }
            }

            Chart chart1 = new Chart();
            chart1.Name = "Chart1";
            chart1.Text = "Chart1";

            ChartArea chartArea1 = new ChartArea("ChartArea1");
            ChartHelper.setChartAreaStyle(chartArea1);
            chartArea1.AxisX.LabelAutoFitStyle =
                LabelAutoFitStyles.IncreaseFont |
                LabelAutoFitStyles.DecreaseFont |
                LabelAutoFitStyles.WordWrap;
            chartArea1.AxisX.Title = "Date";
            chartArea1.AxisX.ScrollBar.LineColor = Color.Black;
            chartArea1.AxisX.ScrollBar.Size = 10;
            chartArea1.AxisY.Title = "Value (" + unit + ")";
            chartArea1.AxisY.ScrollBar.LineColor = Color.Black;
            chartArea1.AxisY.ScrollBar.Size = 10;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas.Add(chartArea1);

            Legend legend1 = new Legend("Legend1");
            legend1.DockedToChartArea = "ChartArea1";
            chart1.Legends.Add(legend1);

            foreach (DGObject obj in objs)
            {
                MonPoint monPnt = obj as MonPoint;
                if (monPnt == null)
                    continue;
                foreach (string key in monPnt.readingsDict.Keys)
                {
                    if (_component != "ALL" && _component != key)
                        continue;

                    List<MonReading> readings = monPnt.readingsDict[key];
                    if (readings.Count == 0)
                        continue;

                    Series series1 = new Series();
                    series1.Name = monPnt.name + ":" + key;
                    series1.ChartType = SeriesChartType.FastLine;
                    series1.ChartArea = "ChartArea1";
                    series1.Points.DataBind(readings, "time", "value", null);
                    series1.BorderWidth = 2;

                    chart1.Series.Add(series1);
                }
            }

            chartHost.Child = chart1;
        }

        private void CBComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = CBComponents.SelectedItem as ComboBoxItem;
            _component = item.Content.ToString();

            setObjs(_objs);
        }
    }
}
