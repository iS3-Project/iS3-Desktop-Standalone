using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.ObjectModel;

using IS3.Core.Geometry;

namespace IS3.Core.Graphics
{
    public interface IGraphicEngine
    {
        // New an instance that contain the IGraphic interface.
        IGraphic newGraphic();
        IGraphic newGraphic(IGeometry geom);

        // New a point(x,y)
        IGraphic newPoint(double x, double y);
        IGraphic newPoint(double x, double y, ISpatialReference spatialReference);
        
        // New a line (x1,y1)-(x2,y2)
        IGraphic newLine(double x1, double y1, double x2, double y2);
        IGraphic newLine(double x1, double y1, double x2, double y2, ISpatialReference spatialReference);

        // New a line (p1-p2)
        IGraphic newLine(IMapPoint p1, IMapPoint p2);

        // New a polyline
        IGraphic newPolyline(IPointCollection part);

        // New a polygon
        IGraphic newPolygon(IPointCollection part);

        // New a triangle
        IGraphic newTriangle(IMapPoint p1, IMapPoint p2, IMapPoint p3);

        // New a quadrilateral
        IGraphic newQuadrilateral(IMapPoint p1, IMapPoint p2,
            IMapPoint p3, IMapPoint p4);

        // New a pentagon
        IGraphic newPentagon(IMapPoint p1, IMapPoint p2,
            IMapPoint p3, IMapPoint p4, IMapPoint p5);

        // New a text
        IGraphic newText(string text, IMapPoint p, Color color,
            string fontName, double fontSize);

        // New a graphic collection
        IGraphicCollection newGraphicCollection();

        // New a graphics layer
        IGraphicsLayer newGraphicsLayer(string id, string displayName);

        // New a simple line symbol
        ISimpleLineSymbol newSimpleLineSymbol();
        ISimpleLineSymbol newSimpleLineSymbol(Color color,
            SimpleLineStyle style, double width);

        // New a simple fill symbol
        ISimpleFillSymbol newSimpleFillSymbol();
        ISimpleFillSymbol newSimpleFillSymbol(Color color,
            SimpleFillStyle style, ISimpleLineSymbol outline);

        // New a simple marker symbol
        ISimpleMarkerSymbol newSimpleMarkerSymbol();
        ISimpleMarkerSymbol newSimpleMarkerSymbol(Color color,
            double size, SimpleMarkerStyle style);
        ISimpleMarkerSymbol newSimpleMarkerSymbol(Color color,
            double angle, MarkerAngleAlignment angleAlignment,
            double size, SimpleMarkerStyle style,
            ISimpleLineSymbol outline,
            double xOffset, double yOffset);

        // Create a SimpleRenderer
        ISimpleRenderer newSimpleRenderer();
        ISimpleRenderer newSimpleRenderer(ISymbol symbol);

        // Create a UniqueValueInfo
        IUniqueValueInfo newUniqueValueInfo();
        IUniqueValueInfo newUniqueValueInfo(ISymbol symbol,
            ObservableCollection<object> values);

        // Create a ClassBreakInfo
        IClassBreakInfo newClassBreakInfo();
        IClassBreakInfo newClassBreakInfo(double max, double min, ISymbol symbol);

        // Create a UniqueValueRenderer
        IUniqueValueRenderer newUniqueValueRenderer();
        IUniqueValueRenderer newUniqueValueRenderer(
            ISymbol defaultSymbol,
            ObservableCollection<string> fields,
            ObservableCollection<IUniqueValueInfo> infos);

        // Create a ClassBreaksRenderer
        IClassBreaksRenderer newClassBreaksRenderer();
        IClassBreaksRenderer newClassBreaksRenderer(
            ISymbol defaultSymbol, string field,
            ObservableCollection<IClassBreakInfo> infos);

        // Create a SymbolFont from the definition
        ISymbolFont newSymbolFont(SymbolFontDef symbolFontDef);

        // Create a Symbol from the definition
        ISymbol newSymbol(SymbolDef symbolDef);

        // Create a renderer from the definition
        IRenderer newRenderer(RendererDef rendererDef);

    }
}
