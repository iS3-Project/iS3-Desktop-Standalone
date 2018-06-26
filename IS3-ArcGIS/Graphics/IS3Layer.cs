using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;

using IS3.Core;
using IS3.Core.Graphics;
using IS3.Core.Geometry;

namespace IS3.ArcGIS.Graphics
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
    // Instances of this class represent a graphics layer.
    //      It delegates operations to ArcGIS Runtime.
    public class IS3GraphicsLayer : GraphicsLayer, IGraphicsLayer
    {
        // index: obj -> graphic collection
        public Dictionary<DGObject, IGraphicCollection> _obj2Graphics { get; set; }
        // index: graphic name -> obj
        public Dictionary<string, DGObject> _graphicName2Objs { get; set; }
        // index: graphic -> obj
        public Dictionary<IGraphic, DGObject> _graphic2Objs { get; set; }

        // Summary:
        //     Constructors
        public IS3GraphicsLayer() { }
        public IS3GraphicsLayer(string id, string displayName)
        {
            ID = id;
            DisplayName = displayName;
        }

        // Summary:
        //     Gets the graphics collection.
        public IList graphics
        {
            get { return base.Graphics; }
        }
        public IEnumerable selectedGraphics
        {
            get { return base.SelectedGraphics; }
        }

        public IRenderer renderer
        {
            get { return base.Renderer as IRenderer; }
            set { base.Renderer = value as Renderer; }
        }

        // Summary:
        //     Gets the geometry type of the features in the layer.
        public IS3.Core.Geometry.GeometryType geometryType { get; set; }

        // Summary:
        //      Sync graphic with objects based on the following condition:
        //          graphic.Attribute["Name"] = obj.Name
        public int syncObjects(DGObjects objs)
        {
            return syncObjects(objs.values);
        }
        public int syncObjects(IEnumerable<DGObject> objs)
        {
            if (objs == null)
                return 0;

            // Build (graphic name)-(graphics) index.
            // Graphics may share a same name if they belong to a object.
            //
            Dictionary<string, IGraphicCollection> graphicIndex =
                new Dictionary<string, IGraphicCollection>();
            foreach (IGraphic g in graphics)
            {
                if (g.Attributes.ContainsKey("Name"))
                {
                    string name = g.Attributes["Name"] as string;
                    if (name == null)
                        continue;
                    IGraphicCollection gc = null;
                    if (graphicIndex.ContainsKey(name))
                        gc = graphicIndex[name];
                    else
                    {
                        gc = new IS3GraphicCollection();
                        graphicIndex[name] = gc;
                    }
                    gc.Add(g);
                }
            }

            // Sync objects with graphics
            //
            _obj2Graphics = new Dictionary<DGObject, IGraphicCollection>();
            _graphicName2Objs = new Dictionary<string, DGObject>();
            int count = 0;
            foreach (DGObject obj in objs)
            {
                string name = obj.name;
                if (graphicIndex.ContainsKey(name))
                {
                    IGraphicCollection gc = graphicIndex[name];
                    _obj2Graphics[obj] = gc;
                    _graphicName2Objs[name] = obj;
                    count++;
                }
            }
            _graphic2Objs = new Dictionary<IGraphic, DGObject>();
            foreach (IGraphic g in graphics)
            {
                if (g.Attributes.ContainsKey("Name"))
                {
                    string name = g.Attributes["Name"] as string;
                    if (name == null)
                        continue;
                    if (_graphicName2Objs.ContainsKey(name))
                    {
                        _graphic2Objs[g] = _graphicName2Objs[name];
                    }
                }
            }

            return count;
        }

        public IGraphicCollection getGraphics(DGObject obj)
        {
            if (_obj2Graphics == null)
                return null;
            if (_obj2Graphics.ContainsKey(obj))
                return _obj2Graphics[obj];
            else
                return null;
        }

        public DGObject getObject(string graphicName)
        {
            if (_graphicName2Objs == null)
                return null;
            if (_graphicName2Objs.ContainsKey(graphicName))
                return _graphicName2Objs[graphicName];
            else
                return null;
        }

        public DGObject getObject(IGraphic graphic)
        {
            if (_graphic2Objs == null || graphic == null)
                return null;
            if (_graphic2Objs.ContainsKey(graphic))
                return _graphic2Objs[graphic];
            else
                return null;
        }

        // Summary:
        //     Get selected objects
        // Remarks:
        //     The function checke IsSelected property of graphics at first,
        //     and then return the corresponding DGObject.
        //     The graphic is ignored if it has no related DGObjects.
        public List<DGObject> getHighlightedObjects()
        {
            HashSet<DGObject> objs = new HashSet<DGObject>();
            foreach (IGraphic g in selectedGraphics)
            {
                if (_graphic2Objs != null
                    && _graphic2Objs.ContainsKey(g))
                {
                    DGObject obj = _graphic2Objs[g];
                    objs.Add(obj);
                }
            }
            return objs.ToList();
        }

        // Summary:
        //     Select objects by point
        // Remarks:
        //     The function selects graphics at first,
        //     then it returns the corresponding DGObject.
        //     If a graphic has no corresponding DGObject, it will
        //     still be in a selected state.
        //     The selected object will be highlighted.
        public async Task<DGObject> selectObjectByPoint(
            System.Windows.Point screenPoint,
            Esri.ArcGISRuntime.Controls.MapView mapView)
        {
            Graphic g = await HitTestAsync(mapView, screenPoint);
            if (g != null)
                g.IsSelected = true;
            DGObject obj = getObject(g as IGraphic);
            if (obj != null)
                highlightObject(obj, true);
            return obj;
        }

        // Summary:
        //     Hit test by screen point
        public async Task<DGObject> hitTestAsync(
            System.Windows.Point screenPoint,
            Esri.ArcGISRuntime.Controls.MapView mapView)
        {
            Graphic g = await HitTestAsync(mapView, screenPoint);
            DGObject obj = getObject(g as IGraphic);
            return obj;
        }

        // Summary:
        //     Select objects by geometry
        // Remarks:
        //     The function selects graphics at first,
        //     then it returns the corresponding DGObjects as a list.
        //     If a graphic has no corresponding DGObject, it will
        //     still be in a selected state.
        public List<DGObject> selectObjectsByRect(IGeometry geom)
        {
            Esri.ArcGISRuntime.Geometry.Geometry rect = geom
                as Esri.ArcGISRuntime.Geometry.Geometry;

            foreach (Graphic g in graphics)
            {
                if (!GeometryEngine.Contains(rect, g.Geometry))
                    continue;

                IGraphic ig = g as IGraphic;
                // make sure all graphics with the name is selected.
                if (_graphic2Objs != null &&
                    _graphic2Objs.ContainsKey(ig))
                {
                    DGObject obj = _graphic2Objs[ig];
                    IGraphicCollection gc = _obj2Graphics[obj];
                    foreach (IGraphic item in gc)
                    {
                        item.IsSelected = true;
                    }
                }
                else
                    g.IsSelected = true;
            }
            List<DGObject> objs = getHighlightedObjects();
            return objs;
        }

        public int highlightObject(DGObject obj, bool on = true)
        {
            int count = 0;
            if (_obj2Graphics != null &&
                _obj2Graphics.ContainsKey(obj))
            {
                IGraphicCollection gc = _obj2Graphics[obj];
                foreach (IGraphic g in gc)
                {
                    if (g.IsSelected != on)
                        g.IsSelected = on;
                }
            }
            return count;
        }

        public int highlightObject(IEnumerable<DGObject> objs,
            bool on = true)
        {
            int count = 0;
            foreach (DGObject obj in objs)
            {
                count += highlightObject(obj, on);
            }
            return count;
        }

        public int highlightAll(bool on = true)
        {
            int count = 0;
            foreach (IGraphic g in graphics)
            {
                if (g.IsSelected != on)
                {
                    g.IsSelected = on;
                    count++;
                }
            }
            return count;
        }

        public void setRenderer(IRenderer renderer)
        {
            base.Renderer = renderer as Renderer;
        }
        public void addGraphic(IGraphic graphic)
        {
            base.Graphics.Add(graphic as Graphic);
        }
        public void addGraphics(IGraphicCollection gc)
        {
            foreach (IGraphic g in gc)
                addGraphic(g);
        }
    }
}
