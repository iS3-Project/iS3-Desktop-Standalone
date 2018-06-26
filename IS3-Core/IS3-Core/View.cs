using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IS3.Core.Geometry;
using IS3.Core.Graphics;

namespace IS3.Core
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
    //     iS3 uses MVC (Model/View/Controll) Design Pattern
    //     Model: Project
    //     View: IView
    //     Controller: IMainFrame
    //

    // Summary:
    //     view type
    public enum ViewType
    {
        None = 0,
        General2DView,
        PlanarView,
        ProfileView,
        CrossSectionView,
        General3DView = 11,
    };

    // Summary:
    //     The drawing mode
    public enum DrawShapeType
    {
        // Summary:
        //     Uses a single click to create a MapPoint.
        Point = 0,
        //
        // Summary:
        //     Draw a shape that returns a Polyline.
        Polyline = 1,
        //
        // Summary:
        //     Draw a shape that returns a Polygon.
        Polygon = 2,
        //
        // Summary:
        //     Draw a shape that returns a map-aligned Envelope.
        Envelope = 3,
        //
        // Summary:
        //     Draw a shape that returns a screen-aligned Polygon.
        Rectangle = 4,
        //
        // Summary:
        //     Draw a free hand Polyline.
        Freehand = 5,
        //
        // Summary:
        //     Draw an arrow shape as a Polygon.
        Arrow = 6,
        //
        // Summary:
        //     Draw a triangle shape as a Polygon.
        Triangle = 7,
        //
        // Summary:
        //     Draw an ellipse as a Polygon.
        Ellipse = 8,
        //
        // Summary:
        //     Draw a circle as a Polygon.
        Circle = 9,
        //
        // Summary:
        //     Draw a single line segment that returns a Polyline.
        LineSegment = 10,
    }

    // Summary:
    //     Object selection changed event args
    public class ObjSelectionChangedEventArgs : EventArgs
    {
        // Summary:
        //     Added objects are stored in a dictionary whose key is the layer ID.
        public Dictionary<string, IEnumerable<DGObject>> addedObjs;
        public Dictionary<string, IEnumerable<DGObject>> removedObjs;
    }

    // Summary:
    //     User drawing graphics changed event args
    public class DrawingGraphicsChangedEventArgs : EventArgs
    {
        public IEnumerable<IGraphic> addedItems;
        public IEnumerable<IGraphic> removedItems;
    }

    // Summary:
    //     IView: interfaces for view class
    // Remarks:
    //     (1) IView is in the core of the MVC design pattern.
    //         View knows the existence of Project (the model), but it
    //         doesn't know IMainframe (the controller). So it uses the
    //         events to notify IMainframe (the controller).
    //     (2) IView consists of a collection of IGraphicLayer,
    //         see IGraphicLayer for more info.
    //     (3) IView has a special layer called drawingLayer, which is
    //         intended to host user drawing graphics.
    //     (4) IView can also host ArcGIS local tiled package layer,
    //         which is not IGraphicsLayer obviously.
    //
    public interface IView
    {
        // Properties
        //
        Project prj { get; }
        ViewType type { get; }
        string name { get; }
        EngineeringMap eMap { get; }
        IEnumerable<IGraphicsLayer> layers { get; }
        ISpatialReference spatialReference { get; }
        // Summary:
        //     Each view has a drawing layer
        IGraphicsLayer drawingLayer { get; }

        // Init and Close
        void initializeView();
        void onClose();

        // Highlight/Unhighlight an object or objects on a layer.
        //
        void highlightObject(DGObject obj, bool on = true);
        void highlightObjects(IEnumerable<DGObject> objs, bool on = true);
        void highlightObjects(IEnumerable<DGObject> objs, string layerID,
            bool on = true);
        void highlightAll(bool on = true);

        // Summary:
        //     Screen point to map point conversions
        //
        IMapPoint screenToLocation(System.Windows.Point screenPoint);
        System.Windows.Point locationToScreen(IMapPoint mapPoint);

        // Summary:
        //     Controls whether graphics are selectable on a layer
        // Remarks:
        //     If layerID is '_ALL', then all layers are selectable.
        void addSeletableLayer(string layerID);
        // Summary:
        //     Controls whether graphics are selectable on a layer
        // Remarks:
        //     If layerID is '_ALL', then all layers are unselectable.
        void removeSelectableLayer(string layerID);

        // View control
        //
        void zoomTo(IGeometry geom);

        // Layers
        //
        void addLayer(IGraphicsLayer layer);
        IGraphicsLayer getLayer(string layerID);
        IGraphicsLayer removeLayer(string layerID);

        // Summary:
        //     Add a local tiled layer (.TPK file)
        // Parameter:
        //     filePath: full file name to .TPK
        //     id: layer id
        void addLocalTiledLayer(string filePath, string layerID);
        // Summary:
        //     Add a layer from local geodatabase dynamically.
        // Parameters:
        //     eLayer: specify the layer name and display options
        //     dbFile: geodatabase file name
        //     start: start index of the feature in the layer,
        //            default to zero.
        //     maxFeatures: maximum features allowed in loading the layer,
        //            default to zero (no limit).
        // Return value:
        //     a graphics layer 
        //
        Task<IGraphicsLayer> addGdbLayer(LayerDef layerDef,
            string dbFile, int start = 0, int maxFeatures = 0);
        // Summary:
        //     Add a layer from a shape file dynamically.
        // Parameters:
        //     eLayer: specify the layer name and display options
        //     shpFile: shape file name
        //     start: start index of the feature in the layer,
        //            default to zero.
        //     maxFeatures: maximum features allowed in loading the layer,
        //            default to zero (no limit).
        // Return value:
        //     a graphics layer 
        //
        Task<IGraphicsLayer> addShpLayer(LayerDef layerDef,
            string shpFile, int start = 0, int maxFeatures = 0);

        // Graphics-Objects Sync
        //
        int syncObjects();

        // Load predefined layers
        //
        Task loadPredefinedLayers();

        // Summary:
        //     A view is both a listener and a trigger of object selection
        //     changed event. When objects are selected in this view,
        //     it will trigger a object selection changed envent.
        //     The view also listens to the objec selection changed
        //     event which is triggered from other views, and datagrid, etc.
        //
        void objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e);
        event EventHandler<ObjSelectionChangedEventArgs>
            objSelectionChangedTrigger;

        // Summary:
        //     The drawing graphics added/removed event trigger
        event EventHandler<DrawingGraphicsChangedEventArgs>
            drawingGraphicsChangedTrigger;
    }

    // Summary:
    //     View holder class: for plan view and profile view.
    //
    public interface IViewHolder
    {
        void setCoord(string coord);
        IView view { get; }
    }

}
