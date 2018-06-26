using System;
using System.Windows;
using System.Windows.Controls;

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
    /// Interaction logic for TreePanel.xaml
    /// </summary>
    public partial class TreePanel : UserControl
    {
        protected IS3Tree _is3Tree;
        public IS3Tree IS3Tree
        {
            get { return _is3Tree; }
        }

        public TreePanel(Tree rootTree)
        {
            InitializeComponent();
            _is3Tree = new IS3Tree(this, DomainTreeView, rootTree);
        }

        public event EventHandler<Tree> OnTreeSelected;
        private void DomainTreeView_SelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            if (OnTreeSelected != null)
                OnTreeSelected(this, e.NewValue as Tree);
        }
    }
}
