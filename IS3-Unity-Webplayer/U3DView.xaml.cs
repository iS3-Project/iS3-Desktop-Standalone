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

using iS3.Core;
using iS3.Unity.Webplayer.UnityCore;

namespace iS3.Unity.Webplayer
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
        protected U3dViewModel _view3D;
        public UnityLayer unityLayer;
        public EventHandler<UnityLayer> UnityLayerHanlder;
        public void UnityLayerListener(object sender, UnityLayer unityLayer)
        {
            if (UnityLayerHanlder != null)
            {
                UnityLayerHanlder(this, unityLayer);
            }
        }
        #region IViewHolder interface
        public void setCoord(string coord)
        {

        }
        public IView view
        {
            get { return _view3D; }
        }
        #endregion
        public U3DView(Project prj, EngineeringMap eMap)
        {
            InitializeComponent();

            _view3D = new U3dViewModel(this, u3dPlayerControl);
            (_view3D as U3dViewModel).UnityLayerHandler += UnityLayerListener;
            _view3D.prj = prj;
            _view3D.eMap = eMap;
        }

    }
}
