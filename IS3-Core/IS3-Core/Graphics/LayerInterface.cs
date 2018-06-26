using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Media;

using IS3.Core.Geometry;

namespace IS3.Core.Graphics
{
    // interface for graphics layer
    public interface IGraphicsLayer
    {
        #region Windows naming style vars
        // Summary:
        //     Gets or sets the ID for the GraphicsLayer.
        string ID { get; set; }
        //
        // Summary:
        //     Gets or sets the layer visibility.
        bool IsVisible { get; set; }
        //
        // Summary:
        //     Gets or sets the maximum scale to display this layer at.  A small number
        //     allows the map to display the layer when zooming further in.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     MaxScale
        //
        // Remarks:
        //      A scale is usually referred to as 1:X, where X is the scale specified here.
        //      This value is the relative scale to the real world, where on inch on the
        //     screen is X inches in the real world. Note that this is only an approximation
        //     and is dependent on the map's projection that can add some distortion, as
        //     well as the system's reported DPI setting which doesn't necessarily match
        //     the actual DPI of the screen.
        //     The default value of this property is System.Double.NaN which makes the layer
        //     unbounded by any scale.
        double MaxScale { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum scale to render this layer at.  A large number allows
        //     the map to display the layer when zooming further out.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     MinScale
        //
        // Remarks:
        //      A scale is usually referred to as 1:X, where X is the scale specified here.
        //      This value is the relative scale to the real world, where on inch on the
        //     screen is X inches in the real world. Note that this is only an approximation
        //     and is dependent on the map's projection that can add some distortion, as
        //     well as the system's reported DPI setting which doesn't necessarily match
        //     the actual DPI of the screen.
        //     The default value of this property is System.Double.NaN which makes the layer
        //     unbounded by any scale.
        double MinScale { get; set; }
        //
        // Summary:
        //     Gets or sets the opacity.
        double Opacity { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether this layer should show in a legend
        bool ShowLegend { get; set; }
        //
        // Summary:
        //     Gets or sets the color for selected graphics.
        //
        // Remarks:
        //     The color is used to highlight graphics that have Graphic.IsSelected
        //     property set to true.
        Color SelectionColor { get; set; }
        #endregion

        // Summary:
        //     Gets the interface of underlying graphics collection.
        IList graphics { get; }

        // Summary:
        //     Gets or sets the renderer used for generating symbols.
        //
        // Remarks:
        //     Symbols set on the individual graphics will override the renderer.
        IRenderer renderer { get; set; }

        // Summary:
        //     Gets the geometry type of the features in the layer.
        GeometryType geometryType { get; }

        // Summary:
        //      Sync graphic with objects based on the following condition:
        //          graphic.Attribute["Name"] = obj.Name
        int syncObjects(IEnumerable<DGObject> objs);
        int syncObjects(DGObjects objs);

        // Summary:
        //      Get graphics that belong to a object
        IGraphicCollection getGraphics(DGObject obj);

        // Summary:
        //      Get object with the given graphic name
        DGObject getObject(string graphicName);

        // Summary:
        //      Get object with the given graphic
        DGObject getObject(IGraphic graphic);

        // Summary:
        //      Get selected objects
        List<DGObject> getHighlightedObjects();

        // Summary:
        //      Get Selected graphics
        IEnumerable selectedGraphics { get; }

        // Summary:
        //      Select objects according to the geometry
        // Remarks:
        //      Selected objects will be highlighted.
        List<DGObject> selectObjectsByRect(IGeometry geom);

        // Summary:
        //      Highlight/Unhighlight objects 
        int highlightObject(DGObject obj, bool on = true);
        int highlightObject(IEnumerable<DGObject> objs, bool on = true);
        int highlightAll(bool on = true);

        void setRenderer(IRenderer renderer);
        void addGraphic(IGraphic graphic);
        void addGraphics(IGraphicCollection gc);
    }
}
