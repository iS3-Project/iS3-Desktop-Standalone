using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Forms.DataVisualization.Charting;

using IS3.Core;

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
    //     Charting using System.Windows.Forms.DataVisualization.Charting
    // Remarks:
    //     There are two versions of charting from Microsoft:
    //     System.Windows.Forms.DataVisualization.Charting in .Net 4.0+
    //     System.Windows.Controls.DataVisualization.Charting in WPFToolkit
    //
    public class FormsCharting
    {
        public static FrameworkElement getMonPointChart(
            IEnumerable<DGObject> objs, double width, double height)
        {
            MonPointChart chart = new MonPointChart(width, height);
            chart.setObjs(objs);
            return chart;
        }

        public static FrameworkElement getMonGroupChart(
            IEnumerable<DGObject> objs, double width, double height)
        {
            if (objs == null || objs.Count() == 0)
                return null;
            MonGroup firstMonGroup = objs.First() as MonGroup;
            if (firstMonGroup == null || 
                firstMonGroup.monPntDict == null ||
                firstMonGroup.monPntDict.Count == 0)
                return null;
            MonPoint lastMonPoint = firstMonGroup.monPntDict.Values.Last();
            if (lastMonPoint == null)
                return null;

            string shape = firstMonGroup.groupShape.ToLower();
            if (shape == "line" || shape == "linear")
            {
                if (lastMonPoint.distanceZ != null &&
                    lastMonPoint.distanceZ.Value != 0)
                {
                    MonGroupChartLinearZ chart = new MonGroupChartLinearZ(width, height);
                    chart.setObjs(objs);
                    return chart;
                }
                else
                {
                    MonGroupChartLinearX chart = new MonGroupChartLinearX(width, height);
                    chart.setObjs(objs);
                    return chart;
                }
            }
            else if (shape == "circle" || shape == "circular")
            {
                return getMonGroupChart_Circular(objs, width, height);
            }
            return null;
        }

        static FrameworkElement getMonGroupChart_Circular(
            IEnumerable<DGObject> objs, double width, double height)
        {
            // Not implemented yet.
            // 
            // Use Polar Chart -> SeriesChartType.Polar
            // and use PrePaint and PostPaint Events.
            return null;
        }
    }
}
