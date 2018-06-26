using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ToolsPanel.xaml
    /// </summary>
    public partial class ToolsPanel : UserControl
    {
        public ToolTreeItem toolboxesTree { get; set; }

        public ToolsPanel()
        {
            InitializeComponent();

            ToolTreeItem root = new ToolTreeItem(null, "Root");
            toolboxesTree = new ToolTreeItem(null, "Toolboxes");
            root.add(toolboxesTree);

            //ToolTreeItem test1 = new ToolTreeItem("Structure|Tunnel", "Test1");
            //toolboxesTree.add(test1);
            //ToolTreeItem tree = root.find("Toolboxes/Structure/Tunnel/Test1");

            ToolsTreeView.ItemsSource = root.items;
        }

        private void ToolsTreeView_MouseDoubleClick(object sender,
            MouseButtonEventArgs e)
        {
            ToolTreeItem tree = ToolsTreeView.SelectedItem as ToolTreeItem;

            try
            {
                if (tree != null && tree.func != null)
                    tree.func();
            }
            catch (Exception ex)
            {
                string format = "Error running plugin tool: {0}.";
                string msg = String.Format(format, tree.displayName);
                ErrorReport.Report(msg);
                ErrorReport.Report(ex.Message);
            }
        }
    }
}
