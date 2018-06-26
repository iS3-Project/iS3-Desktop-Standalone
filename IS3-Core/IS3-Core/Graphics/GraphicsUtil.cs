using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using IS3.Core.Geometry;

namespace IS3.Core.Graphics
{
    public enum ColorRampEnum { Default };

    public class ColorRamp
    {
        static Color[] _default = 
        {
            Color.FromArgb(255,255,255,255),    // White            #FFFFFFFF

            Color.FromArgb(255,173,216,230),    // LightBlue        #FFADD8E6
            Color.FromArgb(255,144,238,144),    // LightGreen       #FF90EE90
            Color.FromArgb(255,240,230,140),    // Khaki            #FFF0E68C
            Color.FromArgb(255,255,252,203),    // Pink             #FFFFC0CB
            Color.FromArgb(255,175,238,238),    // PaleTurquoise    #FFAFEEEE

            Color.FromArgb(255,0,0,205),        // MediumBlue       #FF0000CD
            Color.FromArgb(255,60,179,113),     // MediumSeaGreen   #FF3CB371
            Color.FromArgb(255,255,165,0),      // Orange           #FFFFA500
            Color.FromArgb(255,255,0,255),      // Magenta          #FFFF00FF
            Color.FromArgb(255,72,209,204),     // MediumTurquoise  #FF48D1CC

            Color.FromArgb(255,0,0,255),        // Blue             #FF0000FF
            Color.FromArgb(255,0,255,0),        // Lime             #FF0FF000
            Color.FromArgb(255,255,255,0),      // Yellow           #FFFFFF00
            Color.FromArgb(255,255,0,0),        // Red              #FFFF0000
            Color.FromArgb(255,0,255,255),      // Cyan             #FF00FFFF

            Color.FromArgb(255,135,206,235),    // SkyBlue          #FF87CEEB
            Color.FromArgb(255,124,252,0),      // LawnGreen        #FF7CFC00
            Color.FromArgb(255,218,165,32),     // Goldenrod        #FFDAA520
            Color.FromArgb(255,218,167,214),    // Orchid           #FFDA70D6
            Color.FromArgb(255,64,224,208),     // Turquoise        #FF40E0D0
        };

        public static Color[] GetColorRamp(ColorRampEnum index)
        {
            if (index == ColorRampEnum.Default)
                return _default;
            else
                return null;
        }

    }

    public class GraphicsUtil
    {
        #region default line, mark and fill symbols
        static ISimpleLineSymbol _defaultLineSymbol = null;
        // Get the default line symbol - dark gray and solid line with 1.0 width.
        public static ISimpleLineSymbol GetDefaultLineSymbol()
        {
            if (_defaultLineSymbol == null)
            {
                _defaultLineSymbol = 
                    Runtime.graphicEngine.newSimpleLineSymbol(
                    Colors.DarkGray,
                    Core.Graphics.SimpleLineStyle.Solid, 1.0);
            }
            return _defaultLineSymbol;
        }

        static ISimpleMarkerSymbol _defaultMarkSymbol = null;
        // Get the default mark symbol - blue dot mark with 12 pixels.
        public static ISimpleMarkerSymbol GetDefaultMarkSymbol()
        {
            ISimpleLineSymbol lineSymbol = GetDefaultLineSymbol();
            if (_defaultMarkSymbol == null)
            {
                _defaultMarkSymbol = 
                    Runtime.graphicEngine.newSimpleMarkerSymbol(
                    Colors.Blue, 0, MarkerAngleAlignment.Screen, 12.0,
                    SimpleMarkerStyle.Circle, lineSymbol, 0, 0);
            }
            return _defaultMarkSymbol;
        }

        static ISimpleFillSymbol _defaultFillSymbol = null;
        // Get the default fill symbol - light cyan fill
        public static ISimpleFillSymbol GetDefaultFillSymbol()
        {
            ISimpleLineSymbol lineSymbol = GetDefaultLineSymbol();
            if (_defaultFillSymbol == null)
            {
                _defaultFillSymbol = Runtime.graphicEngine.newSimpleFillSymbol(
                    Colors.LightCyan, SimpleFillStyle.Solid, lineSymbol);
            }
            return _defaultFillSymbol;
        }
        #endregion

        static ISimpleFillSymbol[] _defaultFillSymbols = null;
        // Get a default fill symbol by index.
        // Note: all default fill symbols share a common line symbol: 
        // black and solid line with 1.0 width.
        // You can change the line symbol by specifiying Outline property. 
        public static ISimpleFillSymbol GetDefaultFillSymbols(int index)
        {
            Color[] defaultColors = ColorRamp.GetColorRamp(ColorRampEnum.Default);
            int size = defaultColors.Count();
            ISimpleLineSymbol lineSymbol = GetDefaultLineSymbol();

            if (_defaultFillSymbols == null)
            {
                _defaultFillSymbols = new ISimpleFillSymbol[size];
                for (int i = 0; i < size; ++i)
                {
                    ISimpleFillSymbol symbol = Runtime.graphicEngine.newSimpleFillSymbol(
                        defaultColors[i], 
                        IS3.Core.Graphics.SimpleFillStyle.Solid,
                        lineSymbol);
                    _defaultFillSymbols[i] = symbol;
                }
            }
            return _defaultFillSymbols[index % size];
        }

        #region default line, mark and fill symbols for drawing objects
        static ISimpleLineSymbol _defaultDrawingLineSymbol = null;
        // Get the default drawing line symbol - blue and solid line with 3.0 width.
        public static ISimpleLineSymbol GetDefaultDrawingLineSymbol()
        {
            if (_defaultDrawingLineSymbol == null)
            {
                _defaultDrawingLineSymbol = 
                    Runtime.graphicEngine.newSimpleLineSymbol(
                    Colors.Blue,
                    Core.Graphics.SimpleLineStyle.Solid, 3.0);
            }
            return _defaultDrawingLineSymbol;
        }

        static ISimpleMarkerSymbol _defaultDrawingMarkSymbol = null;
        // Get the default drawing mark symbol - LightGreen dot mark with 12 pixels.
        public static ISimpleMarkerSymbol GetDefaultDrawingMarkSymbol()
        {
            ISimpleLineSymbol lineSymbol = GetDefaultLineSymbol();
            if (_defaultMarkSymbol == null)
            {
                _defaultMarkSymbol =
                    Runtime.graphicEngine.newSimpleMarkerSymbol(
                    Colors.LightGreen, 0, MarkerAngleAlignment.Screen, 12.0,
                    SimpleMarkerStyle.Circle, lineSymbol, 0, 0);
            }
            return _defaultMarkSymbol;
        }

        static ISimpleFillSymbol _defaultDrawingFillSymbol = null;
        // Get the default fill symbol - LightGreen fill
        public static ISimpleFillSymbol GetDefaultDrawingFillSymbol()
        {
            ISimpleLineSymbol lineSymbol = GetDefaultLineSymbol();
            if (_defaultFillSymbol == null)
            {
                _defaultFillSymbol = Runtime.graphicEngine.newSimpleFillSymbol(
                    Colors.LightGreen, SimpleFillStyle.Solid, lineSymbol);
            }
            return _defaultFillSymbol;
        }
        #endregion

        // Assign a default symbol to the graphic
        public static void AssignDefaultSymbol(IGraphic g)
        {
            switch(g.Geometry.GeometryType)
            {
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    g.Symbol = GetDefaultMarkSymbol();
                    break;
                case GeometryType.Polyline:
                    g.Symbol = GetDefaultLineSymbol();
                    break;
                case GeometryType.Polygon:
                case GeometryType.Envelope:
                    g.Symbol = GetDefaultFillSymbol();
                    break;
            }
        }

        // Assign a default drawing symbol to the graphic
        public static void AssignDefaultDrawingSymbol(IGraphic g)
        {
            switch (g.Geometry.GeometryType)
            {
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    g.Symbol = GetDefaultDrawingMarkSymbol();
                    break;
                case GeometryType.Polyline:
                    g.Symbol = GetDefaultDrawingLineSymbol();
                    break;
                case GeometryType.Polygon:
                case GeometryType.Envelope:
                    g.Symbol = GetDefaultDrawingFillSymbol();
                    break;
            }
        }

        // Get the extent of a bunch of graphics
        public static IEnvelope GetGraphicsEnvelope(IGraphicCollection gc)
        {
            if (gc == null || gc.Count == 0)
                return null;

            IEnvelope env = gc[0].Geometry.Extent;
            for (int i = 1; i < gc.Count; ++i)
            {
                env = env.Union(gc[i].Geometry.Extent);
            }
            return env;
        }

        public static ISymbol GetDefaultSymbol(GeometryType geometryType)
        {
            if (geometryType == GeometryType.Point
                || geometryType == GeometryType.Multipoint)
            {
                return _defaultMarkSymbol;
            }
            else if (geometryType == Core.Geometry.GeometryType.Polyline)
            {
                return _defaultLineSymbol;
            }
            else if (geometryType == Core.Geometry.GeometryType.Polygon)
            {
                return _defaultFillSymbol;
            }

            return null;
        }

        public static ISymbol GenerateLayerSymbol(LayerDef layerDef,
            GeometryType geometryType)
        {
            if (layerDef == null)
                return GetDefaultSymbol(geometryType);

            if (layerDef.GeometryType == GeometryType.Point
                || layerDef.GeometryType == GeometryType.Multipoint)
            {
                ISimpleLineSymbol outline = 
                    Runtime.graphicEngine.newSimpleLineSymbol();
                outline.Color = layerDef.OutlineColor;
                outline.Style = layerDef.LineStyle;
                outline.Width = layerDef.LineWidth;

                ISimpleMarkerSymbol symbol = 
                    Runtime.graphicEngine.newSimpleMarkerSymbol();
                symbol.Color = layerDef.Color;
                symbol.Size = layerDef.MarkerSize;
                symbol.Style = layerDef.MarkerStyle;
                symbol.Outline = outline;

                return symbol;
            }
            else if (layerDef.GeometryType == GeometryType.Polyline)
            {
                ISimpleLineSymbol symbol = 
                    Runtime.graphicEngine.newSimpleLineSymbol();
                symbol.Color = layerDef.Color;
                symbol.Style = layerDef.LineStyle;
                symbol.Width = layerDef.LineWidth;

                return symbol;
            }
            else if (layerDef.GeometryType == GeometryType.Polygon)
            {
                ISimpleLineSymbol outline =
                    Runtime.graphicEngine.newSimpleLineSymbol();
                outline.Color = layerDef.OutlineColor;
                outline.Style = layerDef.LineStyle;
                outline.Width = layerDef.LineWidth;

                ISimpleFillSymbol symbol =
                    Runtime.graphicEngine.newSimpleFillSymbol();
                symbol.Color = layerDef.Color;
                symbol.Style = layerDef.FillStyle;
                symbol.Outline = outline;

                return symbol;
            }

            return null;
        }
        #region from IS3.Core ArcGISMappingUtility.cs
        // Draw vertical distributed load
        // Note: y1<y2, (x1,y1)-(x1,y2) is a vertical line, (x3,y1)-(x2,y2) is a oblique line
        //
        public static IGraphicCollection DistributedLoad_Vertical(double x1, double x2, double x3, double y1, double y2,
            ISymbol backgroundFillSymbol, ISymbol arrowFillSymbol, ISymbol lineSymbol)
        {
            IGraphicCollection gc = Runtime.graphicEngine.newGraphicCollection();

            IMapPoint p1 = Runtime.geometryEngine.newMapPoint(x1, y1);
            IMapPoint p2 = Runtime.geometryEngine.newMapPoint(x1, y2);
            IMapPoint p3 = Runtime.geometryEngine.newMapPoint(x2, y2);
            IMapPoint p4 = Runtime.geometryEngine.newMapPoint(x3, y1);
            IGraphic g = Runtime.graphicEngine.newQuadrilateral(p1, p2, p3, p4);
            g.Symbol = backgroundFillSymbol;
            gc.Add(g);

            IPointCollection pc = Runtime.geometryEngine.newPointCollection();
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            pc.Add(p4);
            pc.Add(p1);
            g = Runtime.graphicEngine.newPolyline(pc);
            g.Symbol = lineSymbol;
            gc.Add(g);

            double x00, x01, y00;
            for (int i = 0; i <= 10; ++i)
            {
                x00 = x1;
                x01 = x2 + i * (x3 - x2) / 10.0;
                y00 = y2 - i * (y2 - y1) / 10.0;
                IMapPoint p00 = Runtime.geometryEngine.newMapPoint(x00, y00);
                IMapPoint p01 = Runtime.geometryEngine.newMapPoint(x01, y00);
                g = Runtime.graphicEngine.newLine(p00, p01);
                g.Symbol = lineSymbol;
                gc.Add(g);

                pc = GeomUtil.VectorArrowPoints(x2, y00, p00);
                g = Runtime.graphicEngine.newPolygon(pc);
                g.Symbol = arrowFillSymbol;
                gc.Add(g);
            }
            return gc;
        }

        // Draw horizontal distributed load
        // Note: x1<x2, (x1,y1)-(x2,y1) is a horizontal line, (x1,y2)-(x2,y3) is a oblique line
        //
        public static IGraphicCollection DistributedLoad_Horizontal(double x1, double x2, double y1, double y2, double y3,
            ISymbol backgroundFillSymbol, ISymbol arrowFillSymbol, ISymbol lineSymbol)
        {
            IGraphicCollection gc = Runtime.graphicEngine.newGraphicCollection();

            IMapPoint p1 = Runtime.geometryEngine.newMapPoint(x1, y1);
            IMapPoint p2 = Runtime.geometryEngine.newMapPoint(x1, y2);
            IMapPoint p3 = Runtime.geometryEngine.newMapPoint(x2, y3);
            IMapPoint p4 = Runtime.geometryEngine.newMapPoint(x2, y1);
            IGraphic g = Runtime.graphicEngine.newQuadrilateral(p1, p2, p3, p4);
            g.Symbol = backgroundFillSymbol;
            gc.Add(g);

            IPointCollection pc = Runtime.geometryEngine.newPointCollection();
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            pc.Add(p4);
            pc.Add(p1);
            g = Runtime.graphicEngine.newPolyline(pc);
            g.Symbol = lineSymbol;
            gc.Add(g);

            double x00, y00, y01;
            for (int i = 0; i <= 10; ++i)
            {
                x00 = x1 + i * (x2 - x1) / 10.0;
                y00 = y1;
                y01 = y2 + i * (y3 - y2) / 10.0;
                IMapPoint p00 = Runtime.geometryEngine.newMapPoint(x00, y00);
                IMapPoint p01 = Runtime.geometryEngine.newMapPoint(x00, y01);
                g = Runtime.graphicEngine.newLine(p00, p01);
                g.Symbol = lineSymbol;
                gc.Add(g);

                pc = GeomUtil.VectorArrowPoints(x00, y2, p00);
                g = Runtime.graphicEngine.newPolygon(pc);
                g.Symbol = arrowFillSymbol;
                gc.Add(g);
            }
            return gc;
        }
        #endregion
    }
}
