using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.ComponentModel;

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
    public class IS3ViewBase : INotifyPropertyChanged
    {
        protected Project _prj;
        protected EngineeringMap _eMap;
        protected UserControl _parent;
        protected bool _isBusy = false;

        public event PropertyChangedEventHandler PropertyChanged;

        #region properties
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;

                if (PropertyChanged != null)
                {

                    PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
                }
            }
        }

        public Project prj 
        { 
            get { return _prj; }
            set { _prj = value; } 
        }
        public EngineeringMap eMap 
        { 
            get { return _eMap; }
            set { _eMap = value; }
        }
        public string name { get { return _eMap.MapID; } }
        #endregion

    }
}
