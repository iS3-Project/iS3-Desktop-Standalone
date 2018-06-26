using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using System.Diagnostics;

using System.Reflection;

using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;

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

    public partial class ProjectListPage : UserControl
    {
        static ProjectList Projects;
        string file = "ProjectList.xml";
        PictureMarkerSymbol _pinMarkerSymbol;
        private bool _isHitTesting;

        public ProjectListPage()
        {
            InitializeComponent();

            ProjectTitle.Text = "";

            MyMapView.Loaded += MyMapView_Loaded;

            MyMapView.MouseMove += MyMapView_MouseMove;
            MyMapView.MouseDown += MyMapView_MouseDown;
            MyMapView.NavigationCompleted += MyMapView_NavigationCompleted;
            _isHitTesting = true;

            InitializePictureMarkerSymbol();
        }

        private async void InitializePictureMarkerSymbol()
        {
            _pinMarkerSymbol = LayoutRoot.Resources["DefaultMarkerSymbol"]
                as PictureMarkerSymbol;

            try
            {
                await _pinMarkerSymbol.SetSourceAsync(
                    new Uri("pack://application:,,,/IS3.Desktop;component/Images/pin_red.png"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        void MyMapView_NavigationCompleted(object sender, EventArgs e)
        {
            MyMapView.NavigationCompleted -= MyMapView_NavigationCompleted;
            _isHitTesting = false;
        }

        async void MyMapView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isHitTesting)
                return;

            try
            {
                _isHitTesting = true;

                Point screenPoint = e.GetPosition(MyMapView);
                Graphic graphic = await ProjectGraphicsLayer.HitTestAsync(MyMapView, screenPoint);
                if (graphic != null)
                {
                    mapTip.DataContext = graphic;
                    mapTip.Visibility = System.Windows.Visibility.Visible;

                    ProjectTitle.Text = (string)graphic.Attributes["Description"];
                }
                else
                {
                    mapTip.Visibility = System.Windows.Visibility.Collapsed;
                    ProjectTitle.Text = "";
                }
            }
            catch
            {
                mapTip.Visibility = System.Windows.Visibility.Collapsed;
                ProjectTitle.Text = "";
            }
            finally
            {
                _isHitTesting = false;
            }
        }

        async void MyMapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _isHitTesting = true;

                Point screenPoint = e.GetPosition(MyMapView);
                Graphic graphic = await ProjectGraphicsLayer.HitTestAsync(MyMapView, screenPoint);
                if (graphic != null)
                {
                    string definitionFile = graphic.Attributes["DefinitionFile"] as string;
                    App app = Application.Current as App;

                    IS3MainWindow mw = (IS3MainWindow)app.MainWindow;
                    mw.SwitchToMainFrame(definitionFile);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _isHitTesting = false;
            }
        }

        void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Projects == null)
            {
                LoadProjectList();

                // switch to the default project
                if (Projects != null)
                {
                    ProjectLocation loc =
                        Projects.Locations.Find(i => i.Default == true);
                    if (loc != null)
                    {
                        App app = Application.Current as App;
                        IS3MainWindow mw = (IS3MainWindow)app.MainWindow;
                        mw.SwitchToMainFrame(loc.DefinitionFile);
                    }
                }
            }

            if (Projects != null)
            {
                Envelope projectExtent = new Envelope(Projects.XMin, Projects.YMin,
                    Projects.XMax, Projects.YMax);

                AddProjectsToMap();
                //Map.ZoomTo(ProjectExtent);
            }
        }

        public void LoadProjectList()
        {
            if (Projects != null)
                return;

            try
            {
                string exeLocation = Assembly.GetExecutingAssembly().Location;
                string exePath = System.IO.Path.GetDirectoryName(exeLocation);
                DirectoryInfo di = System.IO.Directory.GetParent(exePath);
                string rootPath = di.FullName+"\\Output";
                string dataPath = rootPath + "\\Data";
                string filePath = dataPath + "\\" + file;

                StreamReader reader = new StreamReader(filePath);
                object obj = XamlReader.Load(reader.BaseStream);
                Projects = obj as ProjectList;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void AddOnlineBaseMap()
        {
            if (Projects.UseGeographicMap)
            {
                ArcGISTiledMapServiceLayer baseLayer =
                    new ArcGISTiledMapServiceLayer();
                baseLayer.ID = "BaseLayer";
                baseLayer.ServiceUri = "http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer";
                baseLayer.IsVisible = true;

                Map.Layers.Insert(0, baseLayer);
            }
        }

        private void AddProjectsToMap()
        {
            GraphicsLayer gLayer = 
                Map.Layers["ProjectGraphicsLayer"] as GraphicsLayer;

            foreach (ProjectLocation loc in Projects.Locations)
            {
                Graphic g = new Graphic()
                {
                    Geometry = new MapPoint(loc.X, loc.Y),
                    Symbol = _pinMarkerSymbol,
                };
                g.Attributes["ID"] = loc.ID;
                g.Attributes["DefinitionFile"] = loc.DefinitionFile;
                g.Attributes["Description"] = loc.Description;

                gLayer.Graphics.Add(g);
            }
        }
    }
}
