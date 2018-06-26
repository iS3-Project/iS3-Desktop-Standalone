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
using System.Windows.Forms.Integration;
using System.Windows.Forms.DataVisualization.Charting;

namespace IS3.Monitoring
{
    public static class ChartHelper
    {
        public static string getMonPointUnit(MonPoint monPoint)
        {
            if (monPoint == null || monPoint.readingsDict == null ||
                monPoint.readingsDict.Count == 0)
                return null;
            List<MonReading> readings = monPoint.readingsDict.Values.First();
            if (readings == null || readings.Count == 0)
                return null;
            string unit = readings.First().unit;
            return unit;
        }

        public static string getMonGroupUnit(MonGroup group)
        {
            if (group == null || group.monPntDict == null
                || group.monPntDict.Count == 0)
                return null;
            MonPoint monPoint = group.monPntDict.Values.First();
            string unit = getMonPointUnit(monPoint);
            return unit;
        }

        public static void setChartAreaStyle(ChartArea chartArea)
        {
            chartArea.AxisX.LabelStyle.Font =
                new Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.LabelStyle.Font =
                new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.BackColor = Color.Gainsboro;
            chartArea.BackGradientStyle = GradientStyle.TopBottom;
            chartArea.BackSecondaryColor = Color.White;
            chartArea.BorderColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.BorderDashStyle = ChartDashStyle.Solid;
            chartArea.ShadowColor = System.Drawing.Color.Transparent;
        }
    }
}
