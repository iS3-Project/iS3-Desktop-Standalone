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
using System.IO;
using System.Threading;

using IS3.Core;
using IS3.Core.Serialization;

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

    public class IS3Tree
    {
        protected MainFrame _mainFrame;
        protected Tree _treeRoot;
        protected TreeView _treeView;
        protected UserControl _parent;

        protected bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
        }

        public IS3Tree(UserControl parent, TreeView treeView,
            Tree tree)
        {
            App app = App.Current as App;
            _mainFrame = app.MainFrame;

            _parent = parent;
            _treeView = treeView;
            _treeRoot = tree;
            _treeView.ItemsSource = _treeRoot.Children;

            _isBusy = false;
        }

        public virtual void InitializeTree()
        {
        }

        public virtual void OnClose()
        {
        }
    }
}
