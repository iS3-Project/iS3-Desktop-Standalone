using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using IS3.Core;

namespace IS3.Desktop
{
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

    /// <summary>
    /// Interaction logic for U3DView.xaml
    /// </summary>
    public partial class U3DView : UserControl, IViewHolder
    {
        protected MainFrame _mainFrame;
        protected IS3View3D _view3D;

        #region IViewHolder interface
        public void setCoord(string coord)
        {

        }
        public IView view
        {
            get { return _view3D; }
        }
        #endregion

        public U3DView(MainFrame mainFrame, Project prj, EngineeringMap eMap)
        {
            InitializeComponent();

            _mainFrame = mainFrame;
            _view3D = new IS3View3D(this, u3dPlayerControl);
            _view3D.prj = prj;
            _view3D.eMap = eMap;
        }

    }
}
