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
    /// Interaction logic for MonGroupChart.xaml
    /// </summary>
    public partial class MonGroupChart : UserControl
    {
        IEnumerable<DGObject> _objs;
        protected int _sign = 1;
        protected bool _showName = true;
        public MonGroupChart(double width, double height)
        {
            InitializeComponent();
            chartHost.Width = width - 12;
        }

        public virtual void setObjs(IEnumerable<DGObject> objs)
        {
            _objs = objs;
        }
        private void DataCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setObjs(_objs);
        }

        private void TBSign_Clicked(object sender, RoutedEventArgs e)
        {
            _sign = TBSign.IsChecked == true ? -1 : 1;
            setObjs(_objs);
        }

        private void TBName_Clicked(object sender, RoutedEventArgs e)
        {
            _showName = TBName.IsChecked == true ? true : false;
            setObjs(_objs);
        }

    }
}
