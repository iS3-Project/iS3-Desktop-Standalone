//************************  Notice  **********************************
//** This file is part of iS3
//**
//** Copyright (c) 2018 Tongji University iS3 Team. All rights reserved.
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
//**
//** The class in this file is used to configure the ProjectList.xml
//**
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
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Windows.Markup;
using System.Xml.Serialization;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ProjInfoWindow.xaml
    /// </summary>
    public partial class ProjectsWindow : Window
    {
        public string ProjID = "";
        public double ProjLocX = 0;
        public double ProjLocY = 0;

        public ProjectList ProjectList {
            get { return _projList; }
        }

        ProjectList _projList;
        GraphicsLayer _gLayer;
        PictureMarkerSymbol _pinMarkerSymbol = new PictureMarkerSymbol();

        public ProjectsWindow(ProjectList projList)
        {
            InitializeComponent();
            _projList = projList;

            InitializePictureMarkerSymbol();
            MyMapView.Loaded += MyMapView_Loaded;
            MyMapView.MouseDown += MyMapView_MouseDown;

            foreach (ProjectLocation loc in _projList.Locations)
            {
                ProjectListLB.Items.Add(loc);
            }
        }

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            _gLayer = Map.Layers["ProjectGraphicsLayer"] as GraphicsLayer;
            if (_projList == null)
                return;
            foreach (ProjectLocation loc in _projList.Locations)
            {
                AddProjectLocationOnMap(loc);
            }
        }

        private void ProjectListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
            {
                ProjectDescTB.Visibility = Visibility.Hidden;
                _gLayer.Graphics.Clear();
                return;
            }

            ProjectDescTB.Visibility = Visibility.Visible;
            ProjectDescTB.Text = loc.Description;
            ProjectDescTB.IsReadOnly = true;

            _gLayer.Graphics.Clear();
            AddProjectLocationOnMap(loc);

            PromptTB.Text = "Select operations from the configuration.";
            StepLB.SelectedIndex = -1;
        }

        private void StepLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            int step = StepLB.SelectedIndex;
            if (step == 0)
            {
                PromptTB.Text = "Input project description in the text box.";
                ProjectDescTB.IsReadOnly = false;
                ProjectDescTB.Focus();
                ProjectDescTB.SelectAll();
                ProjectDescTB.Visibility = System.Windows.Visibility.Visible;
                _gLayer.Graphics.Clear();
            }
            else if (step == 1)
            {
                PromptTB.Text = "Drop the project location on the map.";
                ProjectDescTB.Visibility = System.Windows.Visibility.Hidden;
                AddProjectLocationOnMap(loc);
            }
        }

        private void ProjectTitleTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            loc.Description = ProjectDescTB.Text;
        }

        void MyMapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            int step = StepLB.SelectedIndex;
            if (step != 1)
                return;

            Point screenPoint = e.GetPosition(MyMapView);
            MapPoint coord = MyMapView.ScreenToLocation(screenPoint);
            _gLayer.Graphics.Clear();

            loc.X = coord.X;
            loc.Y = coord.Y;
            AddProjectLocationOnMap(loc);
        }

        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            AddProjWindow addProjWnd = new AddProjWindow();
            addProjWnd.Owner = this;
            addProjWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = addProjWnd.ShowDialog();
            if (ok!=null && ok.Value==true)
            {
                ProjectLocation loc = new ProjectLocation();
                loc.ID = addProjWnd.IdTB.Text;
                loc.Description = addProjWnd.DescTB.Text;
                loc.DefinitionFile = loc.ID + ".xml";
                loc.X = 0;
                loc.Y = 0;
                _projList.Locations.Add(loc);

                ProjectListLB.Items.Add(loc);
            }
        }

        private void RemoveProject_Click(object sender, RoutedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            _projList.Locations.Remove(loc);
            ProjectListLB.Items.Remove(loc);

            ProjectListLB.SelectedIndex = -1;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
            {
                PromptTB.Text = "Please select a project before going to next step.";
                return;
            }

            ProjID = loc.ID;
            ProjLocX = loc.X;
            ProjLocY = loc.Y;
            // finish 
            DialogResult = true;
            Close();
        }

        // Load picture 'pin_red.png' to _pinMarkerSymbol
        //
        private async void InitializePictureMarkerSymbol()
        {
            try
            {
                await _pinMarkerSymbol.SetSourceAsync(
                    new Uri("pack://application:,,,/IS3.Config;component/Images/pin_red.png"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Add a pin marker to the map for the specfied project location
        //
        void AddProjectLocationOnMap(ProjectLocation loc)
        {
            Graphic g = new Graphic()
            {
                Geometry = new MapPoint(loc.X, loc.Y),
                Symbol = _pinMarkerSymbol,
            };
            g.Attributes["ID"] = loc.ID;
            g.Attributes["DefinitionFile"] = loc.DefinitionFile;
            g.Attributes["Description"] = loc.Description;

            _gLayer.Graphics.Add(g);
        }
    }
}
