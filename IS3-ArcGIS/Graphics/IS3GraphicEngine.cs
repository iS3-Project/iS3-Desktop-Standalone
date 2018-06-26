using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;

using IS3.Core;
using IS3.Core.Graphics;
using IS3.Core.Geometry;
using IS3.ArcGIS.Geometry;

using Esri.ArcGISRuntime.Symbology;

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
    // Graphic engine: generate graphic objects
    public class IS3GraphicEngine : IGraphicEngine
    {
        public IGraphic newGraphic()
        {
            return new IS3Graphic();
        }

        public IGraphic newGraphic(IGeometry geom)
        {
            IS3Graphic g = new IS3Graphic();
            g.Geometry = geom;
            return g;
        }

        public IGraphic newPoint(double x, double y)
        {
            IS3Graphic g = new IS3Graphic();
            g.Geometry = new IS3MapPoint(x, y);
            return g;
        }

        public IGraphic newPoint(double x, double y, ISpatialReference spatialReference)
        {
            IS3Graphic g = new IS3Graphic();
            g.Geometry = new IS3MapPoint(x, y, spatialReference);
            return g;
        }

        public IGraphic newLine(double x1, double y1, double x2, double y2)
        {
            IS3MapPoint p1 = new IS3MapPoint(x1, y1);
            IS3MapPoint p2 = new IS3MapPoint(x2, y2);
            return newLine(p1, p2);
        }

        public IGraphic newLine(double x1, double y1, double x2, double y2, ISpatialReference spatialReference)
        {
            IS3MapPoint p1 = new IS3MapPoint(x1, y1, spatialReference);
            IS3MapPoint p2 = new IS3MapPoint(x2, y2, spatialReference);
            return newLine(p1, p2);
        }

        public IGraphic newLine(IMapPoint p1, IMapPoint p2)
        {
            IS3PointCollection pts = new IS3PointCollection();
            pts.Add(p1 as IS3MapPoint);
            pts.Add(p2 as IS3MapPoint);
            return newPolyline(pts);
        }

        public IGraphic newPolyline(IPointCollection part)
        {
            IS3Polyline pline = new IS3Polyline(part);
            IS3Graphic g = new IS3Graphic();
            g.Geometry = pline;
            return g;
        }

        public IGraphic newPolygon(IPointCollection part)
        {
            IS3Polygon pgon = new IS3Polygon(part);
            IS3Graphic g = new IS3Graphic();
            g.Geometry = pgon;
            return g;
        }

        public IGraphic newTriangle(IMapPoint p1, IMapPoint p2, IMapPoint p3)
        {
            IS3PointCollection pts = new IS3PointCollection();
            pts.Add(p1 as IS3MapPoint);
            pts.Add(p2 as IS3MapPoint);
            pts.Add(p3 as IS3MapPoint);
            pts.Add(p1 as IS3MapPoint);
            return newPolygon(pts);
        }

        public IGraphic newQuadrilateral(IMapPoint p1, IMapPoint p2,
            IMapPoint p3, IMapPoint p4)
        {
            IS3PointCollection pts = new IS3PointCollection();
            pts.Add(p1 as IS3MapPoint);
            pts.Add(p2 as IS3MapPoint);
            pts.Add(p3 as IS3MapPoint);
            pts.Add(p4 as IS3MapPoint);
            return newPolygon(pts);
        }

        public IGraphic newPentagon(IMapPoint p1, IMapPoint p2,
            IMapPoint p3, IMapPoint p4, IMapPoint p5)
        {
            IS3PointCollection pts = new IS3PointCollection();
            pts.Add(p1 as IS3MapPoint);
            pts.Add(p2 as IS3MapPoint);
            pts.Add(p3 as IS3MapPoint);
            pts.Add(p4 as IS3MapPoint);
            pts.Add(p5 as IS3MapPoint);
            return newPolygon(pts);
        }

        public IGraphic newText(string text, IMapPoint p, Color color,
            string fontName, double fontSize)
        {
            IS3TextSymbol textSymbol = new IS3TextSymbol();
            textSymbol.Text = text;
            textSymbol.Color = color;

            IS3SymbolFont font = new IS3SymbolFont(fontName, fontSize);
            textSymbol.Font = font;

            IS3Graphic g = new IS3Graphic();
            
            g.Symbol = textSymbol;
            g.Geometry = p;

            return g;
        }

        public IGraphicCollection newGraphicCollection()
        {
            return new IS3GraphicCollection();
        }
        
        // Summary:
        //     New a graphics layer
        // Remarks:
        //     IS3GraphicsLayer is derived from DispatcherObject,
        //     so it should be placed in the main UI thread.
        //
        public IGraphicsLayer newGraphicsLayer(string id, string displayName)
        {
            if (Globals.isThreadUnsafe())
            {
                var func = new Func<string, string, IGraphicsLayer>(
                    (_id, _displayName) => 
                    {
                        return new IS3GraphicsLayer(_id, _displayName); 
                    }
                    );
                object gLayer = Globals.application.Dispatcher.Invoke(
                    func, id, displayName);
                return gLayer as IS3GraphicsLayer;
            }
            else
                return new IS3GraphicsLayer(id, displayName);
        }

        public ISimpleLineSymbol newSimpleLineSymbol()
        {
            return new IS3SimpleLineSymbol();
        }
        public ISimpleLineSymbol newSimpleLineSymbol(Color color,
            IS3.Core.Graphics.SimpleLineStyle style, double width)
        {
            IS3SimpleLineSymbol symbol = new IS3SimpleLineSymbol();
            symbol.Color = color;
            symbol.Style = style;
            symbol.Width = width;

            return symbol;
        }

        public ISimpleFillSymbol newSimpleFillSymbol()
        {
            return new IS3SimpleFillSymbol();
        }
        public ISimpleFillSymbol newSimpleFillSymbol(Color color,
            IS3.Core.Graphics.SimpleFillStyle style,
            ISimpleLineSymbol outline)
        {
            IS3SimpleFillSymbol symbol = new IS3SimpleFillSymbol();
            symbol.Color = color;
            symbol.Outline = outline as IS3SimpleLineSymbol;
            symbol.Style = style;

            return symbol;
        }

        public ISimpleMarkerSymbol newSimpleMarkerSymbol()
        {
            return new IS3SimpleMarkerSymbol();
        }
        public ISimpleMarkerSymbol newSimpleMarkerSymbol(Color color,
            double size, IS3.Core.Graphics.SimpleMarkerStyle style)
        {
            IS3SimpleMarkerSymbol symbol = new IS3SimpleMarkerSymbol();
            symbol.Color = color;
            symbol.Size = size;
            symbol.Style = style;
            return symbol;
        }
        public ISimpleMarkerSymbol newSimpleMarkerSymbol(Color color,
            double angle, IS3.Core.Graphics.MarkerAngleAlignment angleAlignment,
            double size, IS3.Core.Graphics.SimpleMarkerStyle style,
            ISimpleLineSymbol outline,
            double xOffset, double yOffset)
        {
            IS3SimpleMarkerSymbol symbol = new IS3SimpleMarkerSymbol();
            symbol.Color = color;
            symbol.Angle = angle;
            symbol.AngleAlignment = angleAlignment;
            symbol.Size = size;
            symbol.Style = style;
            symbol.Outline = outline as IS3SimpleLineSymbol;
            symbol.XOffset = xOffset;
            symbol.YOffset = yOffset;

            return symbol;
        }

        public ISimpleRenderer newSimpleRenderer()
        {
            return  new IS3SimpleRenderer();
        }
        public ISimpleRenderer newSimpleRenderer(ISymbol symbol)
        {
            IS3SimpleRenderer renderer = new IS3SimpleRenderer();
            renderer.Symbol = symbol;
            return renderer;
        }

        public IUniqueValueInfo newUniqueValueInfo()
        {
            return new IS3UniqueValueInfo();
        }
        public IUniqueValueInfo newUniqueValueInfo(ISymbol symbol,
            ObservableCollection<object> values)
        {
            IS3UniqueValueInfo info = new IS3UniqueValueInfo();
            info.Symbol = symbol;
            info.Values = values;
            return info;
        }

        public IClassBreakInfo newClassBreakInfo()
        {
            return new IS3ClassBreakInfo();
        }
        public IClassBreakInfo newClassBreakInfo(
            double max, double min, ISymbol symbol)
        {
            IS3ClassBreakInfo info = new IS3ClassBreakInfo();
            info.Maximum = max;
            info.Minimum = min;
            info.Symbol = symbol;
            return info;
        }

        public IUniqueValueRenderer newUniqueValueRenderer()
        {
            return new IS3UniqueValueRenderer();
        }
        public IUniqueValueRenderer newUniqueValueRenderer(
            ISymbol defaultSymbol,
            ObservableCollection<string> fields,
            ObservableCollection<IUniqueValueInfo> infos)
        {
            IS3UniqueValueRenderer renderer = new IS3UniqueValueRenderer();
            renderer.DefaultSymbol = defaultSymbol;
            renderer.Fields = fields;
            renderer.Infos = infos;
            return renderer;
        }

        public IClassBreaksRenderer newClassBreaksRenderer()
        {
            return new IS3ClassBreaksRenderer();
        }
        public IClassBreaksRenderer newClassBreaksRenderer(
            ISymbol defaultSymbol,
            string field,
            ObservableCollection<IClassBreakInfo> infos)
        {
            IS3ClassBreaksRenderer renderer = new IS3ClassBreaksRenderer();
            renderer.DefaultSymbol = defaultSymbol;
            renderer.Field = field;
            renderer.Infos = infos;
            return renderer;
        }

        public ISymbolFont newSymbolFont(SymbolFontDef symbolFontDef)
        {
            IS3SymbolFont font = new IS3SymbolFont();
            font.FontFamily = symbolFontDef.FontFamily;
            font.FontSize = symbolFontDef.FontSize;
            font.FontStyle = symbolFontDef.FontStyle;
            font.FontWeight = symbolFontDef.FontWeight;
            font.TextDecoration = symbolFontDef.TextDecoration;
            return font;
        }

        public ISymbol newSymbol(SymbolDef symbolDef)
        {
            if (symbolDef == null)
                return null;

            Type symbolType = symbolDef.GetType();
            if (symbolType == typeof(SimpleLineSymbolDef))
            {
                SimpleLineSymbolDef sDef = (SimpleLineSymbolDef)symbolDef;
                IS3SimpleLineSymbol symbol = new IS3SimpleLineSymbol();
                symbol.Color = sDef.Color;
                symbol.Style = sDef.Style;
                symbol.Width = sDef.Width;
                return symbol;
            }
            else if (symbolType == typeof(SimpleFillSymbolDef))
            {
                SimpleFillSymbolDef sDef = (SimpleFillSymbolDef)symbolDef;
                IS3SimpleFillSymbol symbol = new IS3SimpleFillSymbol();
                symbol.Color = sDef.Color;
                symbol.Style = sDef.Style;
                symbol.Outline = (ISimpleLineSymbol) newSymbol(sDef.OutlineDef);
                return symbol;
            }
            else if (symbolType == typeof(SimpleMarkerSymbolDef))
            {
                SimpleMarkerSymbolDef sDef = (SimpleMarkerSymbolDef)symbolDef;
                IS3SimpleMarkerSymbol symbol = new IS3SimpleMarkerSymbol();
                symbol.Angle = sDef.Angle;
                symbol.AngleAlignment = sDef.AngleAlignment;
                symbol.Color = sDef.Color;
                symbol.Outline = (ISimpleLineSymbol)newSymbol(sDef.OutlineDef);
                symbol.Size = sDef.Size;
                symbol.Style = sDef.Style;
                symbol.XOffset = sDef.XOffset;
                symbol.YOffset = sDef.YOffset;
                return symbol;
            }
            else if (symbolType == typeof(TextSymbolDef))
            {
                TextSymbolDef sDef = (TextSymbolDef)symbolDef;
                IS3TextSymbol symbol = new IS3TextSymbol();
                symbol.Angle = sDef.Angle;
                symbol.AngleAlignment = sDef.AngleAlignment;
                symbol.BackgroundColor = sDef.BackgroundColor;
                symbol.BorderLineColor = sDef.BorderLineColor;
                symbol.BorderLineSize = sDef.BorderLineSize;
                symbol.Color = sDef.Color;
                symbol.Text = sDef.Text;
                symbol.HorizontalTextAlignment = sDef.HorizontalTextAlignment;
                symbol.VerticalTextAlignment = sDef.VerticalTextAlignment;
                symbol.Font = newSymbolFont(sDef.FontDef);
                symbol.XOffset = sDef.XOffset;
                symbol.YOffset = sDef.YOffset;
                return symbol;
            }

            return null;
        }

        public IRenderer newRenderer(RendererDef rendererDef)
        {
            Type rendererType = rendererDef.GetType();
            if (rendererType == typeof(SimpleRendererDef))
            {
                SimpleRendererDef rDef = (SimpleRendererDef)rendererDef;
                IS3SimpleRenderer renderer = new IS3SimpleRenderer();
                renderer.Symbol = newSymbol(rDef.SymbolDef);
                return renderer;
            }
            else if (rendererType == typeof(UniqueValueRendererDef))
            {
                UniqueValueRendererDef rDef = (UniqueValueRendererDef)rendererDef;
                IS3UniqueValueRenderer renderer = new IS3UniqueValueRenderer();
                renderer.DefaultSymbol = newSymbol(rDef.DefaultSymbolDef);
                renderer.Fields = rDef.Fields;
                renderer.Infos = new ObservableCollection<IUniqueValueInfo>(
                    rDef.InfosDef.Select(x => new IS3UniqueValueInfo 
                    { 
                        Symbol = newSymbol(x.SymbolDef),
                        Values = x.Values
                    })
                    );
                return renderer;
            }
            else if (rendererType == typeof(ClassBreaksRendererDef))
            {
                ClassBreaksRendererDef rDef = (ClassBreaksRendererDef)rendererDef;
                IS3ClassBreaksRenderer renderer = new IS3ClassBreaksRenderer();
                renderer.DefaultSymbol = newSymbol(rDef.DefaultSymbolDef);
                renderer.Field = rDef.Field;
                renderer.Infos = new ObservableCollection<IClassBreakInfo>(
                    rDef.InfosDef.Select(x => new IS3ClassBreakInfo 
                    { 
                        Maximum = x.Maximum,
                        Minimum = x.Maximum,
                        Symbol = newSymbol(x.SymbolDef)
                    })
                    );
                return renderer;
            }
            return null;
        }
    }
}
