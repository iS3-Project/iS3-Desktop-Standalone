using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using IS3.Core;
using IS3.Core.Graphics;
using IS3.ArcGIS.Graphics;
using IS3.ArcGIS.Geometry;

namespace iS3.Config
{
    public static class GdbHelper
    {
        static IS3GraphicEngine _graphicEngine = new IS3GraphicEngine();
        static IS3GeometryEngine _geometryEngine = new IS3GeometryEngine();

        public static void Initialize()
        {
            Runtime.initializeEngines(_graphicEngine, _geometryEngine);
        }

        // Summary:
        //     Add a layer in a geodatabase (aka. local layer)
        //     The name of the layer and display options are specified in the LocalELayer
        //
        public static async Task<IGraphicsLayer> addGeodatabaseLayer(
            Map map, LayerDef layerDef, GeodatabaseFeatureTable table,
            int start = 0, int maxFeatures = 0)
        {
            if (layerDef == null || table == null)
                return null;

            IS3GraphicsLayer gLayer = await featureTable2GraphicsLayer(
                map, table, start, maxFeatures);
            if (gLayer == null)
                return null;

            gLayer.ID = table.Name;
            gLayer.MinScale = table.ServiceInfo.MinScale;
            gLayer.MaxScale = table.ServiceInfo.MaxScale;
            setGraphicLayerDisplayOptions(layerDef, gLayer);

            map.Layers.Add(gLayer);
            return gLayer;
        }

        // Summary:
        //     Load features in a FeatureTable into a GraphicsLayer
        //
        static async Task<IS3GraphicsLayer> featureTable2GraphicsLayer(
            Map map, FeatureTable table, int start = 0, int maxFeatures = 0)
        {
            if (table == null)
                return null;

            // The spatial reference in the first table is used as the project spatial reference.
            // All features on other layers will be projected to the spatial reference.
            map.SpatialReference = table.SpatialReference;

            //// We cannot use the feature layer class because it typically 
            //// has a different SpatialReferece object (coordinate system)
            //// other than the tiled layer (WKID = 3857 or 102100),
            //// and there is no easy way to reproject feature layer
            //// to another coordinate system.
            //// We can only use the feature layer when there is no tiled layer defined,
            //// which is not a usual case.
            //FeatureLayer fLayer = new FeatureLayer(table);
            //fLayer.ID = table.Name;
            //fLayer.DisplayName = table.Name;
            //_map.Layers.Add(fLayer);

            QueryFilter qf = new QueryFilter();
            qf.WhereClause = "1=1";
            IEnumerable<Feature> features = await table.QueryAsync(qf);
            IS3GraphicCollection graphics = new IS3GraphicCollection();

            int index = 0, count = 0;
            foreach (Feature f in features)
            {
                // jump to start position
                if (index++ < start)
                    continue;

                // Note:
                //     In ArcGIS Runtime SDK: User-defined coordinate system
                //     is not allowed when using ShapefileTable.OpenAsync().
                // Workaround:
                //     (1) Do not assign user-defined CS in shape file;
                //     (2) Assign CS dynamically here to _srEMap.
                //
                Esri.ArcGISRuntime.Geometry.Geometry geometry = f.Geometry;

                // import the attributes
                IS3Graphic g = new IS3Graphic(geometry);
                foreach (KeyValuePair<string, object> item in f.Attributes.AsEnumerable())
                    g.Attributes.Add(item);
                graphics.Add(g);

                // Load max featuers
                if (maxFeatures != 0 && count++ == maxFeatures)
                    break;
            }

            IS3GraphicsLayer gLayer = new IS3GraphicsLayer();
            gLayer.DisplayName = table.Name;
            gLayer.GraphicsSource = graphics;
            gLayer.geometryType = (IS3.Core.Geometry.GeometryType)(int)table.GeometryType;
            //gLayer.FullExtent = table.Extent;

            return gLayer;
        }

        // Summary:
        //     Set the GraphicsLayer display options according to the definition
        //     which is specified in the LocalELayer.
        //     The display options include selection color, renderer, and labelling
        //
        static void setGraphicLayerDisplayOptions(LayerDef layerDef, IS3GraphicsLayer gLayer)
        {
            if (layerDef == null || gLayer == null)
                return;

            gLayer.IsVisible = layerDef.IsVisible;

            gLayer.SelectionColor = layerDef.SelectionColor;
            if (layerDef.RendererDef == null)
            {
                ISymbol symbol = GraphicsUtil.GenerateLayerSymbol(layerDef, gLayer.geometryType);
                gLayer.renderer = Runtime.graphicEngine.newSimpleRenderer(symbol);
            }
            else
            {
                gLayer.renderer = Runtime.graphicEngine.newRenderer(layerDef.RendererDef);
            }

            if (layerDef.EnableLabel == true)
            {
                AttributeLabelClass labelClass = generateLayerAttributeLable(layerDef, gLayer.geometryType);
                gLayer.Labeling.LabelClasses.Add(labelClass);
            }
        }

        // Summary:
        //     Generate a label attributes according to the definition
        //     which is specified in the LayerDef
        //
        static AttributeLabelClass generateLayerAttributeLable(LayerDef layerDef,
            IS3.Core.Geometry.GeometryType geometryType)
        {
            if (layerDef == null)
                return generateDefaultLayerAttributeLable(geometryType);

            IS3SymbolFont font = new IS3SymbolFont(
                layerDef.LabelFontFamily, layerDef.LabelFontSize);
            font.FontStyle = layerDef.LabelFontStyle;
            font.FontWeight = layerDef.LabelFontWeight;
            font.TextDecoration = layerDef.LabelTextDecoration;

            IS3TextSymbol textSymbol = new IS3TextSymbol();
            textSymbol.Color = layerDef.LabelColor;
            textSymbol.Font = font;
            textSymbol.BorderLineColor = layerDef.LabelBorderLineColor;
            textSymbol.BorderLineSize = layerDef.LabelBorderLineWidth;
            textSymbol.BackgroundColor = layerDef.LabelBackgroundColor;

            AttributeLabelClass labelClass = new AttributeLabelClass();
            labelClass.IsVisible = true;
            labelClass.TextExpression = layerDef.LabelTextExpression;
            labelClass.WhereClause = layerDef.LabelWhereClause;
            labelClass.Symbol = textSymbol;

            if (geometryType == IS3.Core.Geometry.GeometryType.Polygon)
                labelClass.LabelPlacement = LabelPlacement.PolygonAlwaysHorizontal;

            return labelClass;
        }


        // Summary:
        //     Generate a default label attributes according to the GeometryType
        //     The default labelling property of a feature is [Name]
        //
        static AttributeLabelClass generateDefaultLayerAttributeLable(IS3.Core.Geometry.GeometryType geometryType)
        {
            IS3TextSymbol textSymbol = new IS3TextSymbol();
            textSymbol.Color = System.Windows.Media.Colors.Black;

            AttributeLabelClass labelClass = new AttributeLabelClass();
            labelClass.IsVisible = true;
            labelClass.TextExpression = "[Name]";
            labelClass.Symbol = textSymbol;

            if (geometryType == IS3.Core.Geometry.GeometryType.Polygon)
                labelClass.LabelPlacement = LabelPlacement.PolygonAlwaysHorizontal;

            return labelClass;
        }

        public static LayerDef GenerateDefaultLayerDef(string name, IS3.Core.Geometry.GeometryType geomType)
        {
            LayerDef lyrDef = new LayerDef();
            lyrDef.Name = name;
            lyrDef.IsVisible = true;
            if (geomType == IS3.Core.Geometry.GeometryType.Point)
            {
                lyrDef.GeometryType = IS3.Core.Geometry.GeometryType.Point;
                lyrDef.Color = Colors.Red;
                lyrDef.FillStyle = SimpleFillStyle.Solid;
                lyrDef.EnableLabel = true;
                lyrDef.LabelTextExpression = "[Name]";
            }
            else if (geomType == IS3.Core.Geometry.GeometryType.Polyline)
            {
                lyrDef.GeometryType = IS3.Core.Geometry.GeometryType.Polyline;
                lyrDef.OutlineColor = Colors.Green;
                lyrDef.Color = Colors.Green;
                lyrDef.FillStyle = SimpleFillStyle.Solid;
            }
            else if (geomType == IS3.Core.Geometry.GeometryType.Polygon)
            {
                lyrDef.GeometryType = IS3.Core.Geometry.GeometryType.Polygon;
                lyrDef.OutlineColor = Colors.Blue;
                lyrDef.Color = Colors.Blue;
                lyrDef.FillStyle = SimpleFillStyle.Solid;
            }

            return lyrDef;
        }

        public static LayerDef GenerateDefaultLayerDef(GeodatabaseFeatureTable table)
        {
            IS3.Core.Geometry.GeometryType geomType = IS3.Core.Geometry.GeometryType.Point;
            if (table.GeometryType == GeometryType.Point)
            {
                geomType = IS3.Core.Geometry.GeometryType.Point;
            }
            else if (table.GeometryType == GeometryType.Polyline)
            {
                geomType = IS3.Core.Geometry.GeometryType.Polyline;
            }
            else if (table.GeometryType == GeometryType.Polygon)
            {
                geomType = IS3.Core.Geometry.GeometryType.Polygon;
            }
            return GenerateDefaultLayerDef(table.Name, geomType);
        }

    }
}
