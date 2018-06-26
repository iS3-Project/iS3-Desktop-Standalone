using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core.Geometry
{
    // Summary:
    //    Spatial Reference
    public interface ISpatialReference
    {
        //
        // Summary:
        //     Returns true if this is a geographic coordinate system.
        bool IsGeographic { get; }
        //
        // Summary:
        //     Returns true if this coordinate system supports wrap around.
        bool IsPannable { get; }
        //
        // Summary:
        //     Returns true if this is a projected coordinate system.
        bool IsProjected { get; }
        //
        // Summary:
        //     Gets the vertical coordinate system Well-known ID.
        int VerticalWkid { get; }
        //
        // Summary:
        //     Gets the Well-known ID for this instance.
        int Wkid { get; }
        //
        // Summary:
        //     Gets the Well-known Text for this instance.
        string WkText { get; }
    }

    // Summary:
    //     Geometry Type Enumeration
    public enum GeometryType
    {
        // Summary:
        //     Unknown geometry type
        Unknown = 0,
        //
        // Summary:
        //     Point
        Point = 1,
        //
        // Summary:
        //     Multipoint
        Multipoint = 2,
        //
        // Summary:
        //     Polyline
        Polyline = 3,
        //
        // Summary:
        //     Polygon
        Polygon = 4,
        //
        // Summary:
        //     Envelope
        Envelope = 5,
    }

    // interface for IGeometry
    public interface IGeometry
    {
        // Gets the dimension of the geometry.
        int Dimension { get; }
        // Gets the minimum enclosing Envelope of the instance
        IEnvelope Extent { get; }
        // Gets the geometry type.
        GeometryType GeometryType { get; }
        // Gets a value indicating if the geometry has M
        bool HasM { get; }
        // Gets a value indicating if the geometry has Z.
        bool HasZ { get; }
        // Gets a value indicating whether or not the geometry is empty.
        bool IsEmpty { get; }

        // Geometry represented as a JSON String.
        string ToJson();
    }

    // interface for IMapPoint
    public interface IMapPoint : IGeometry
    {
        // Gets the measure value.
        double M { get; }

        // Gets the X coordinate.
        double X { get; }

        // Gets the Y coordinate.
        double Y { get; }

        // Gets the Z coordinate.
        double Z { get; }
    }

    // interface for IPointCollection
    public interface IPointCollection : IEnumerable<IMapPoint>
    {
        // Adds an object to the end.
        void Add(IMapPoint item);

        // Gets the number of elements actually contained.
        int Count { get; }

        // Gets or sets the element at the specified index.
        IMapPoint this[int index] { get; set; }
    }

    // interface for IEnvelop
    public interface IEnvelope : IGeometry
    {
        // Gets the height of the Envelope.
        double Height { get; }
        // Gets maxium M value (measure).
        double MMax { get; }
        // Gets minimum M value (measure).
        double MMin { get; }
        // Gets the width of the Envelope.
        double Width { get; }
        // Gets maximum X
        double XMax { get; }
        // Gets minimum X
        double XMin { get; }
        // Gets maximum Y
        double YMax { get; }
        // Gets minimum Y value
        double YMin { get; }
        // Gets maximum Z value
        double ZMax { get; }
        // Gets minimum Z value
        double ZMin { get; }

        // Gets the center of the Envelope.
        IMapPoint GetCenter();

        // Calculates the intersection between this instance and the specified envelope.
        // Returns:
        //     The intersecting envelope or null if they don't intersect.
        IEnvelope Intersection(IEnvelope other);

        // Returns true if this instance intersects an envelope.
        bool Intersects(IEnvelope other);

        // Returns the union of this instance and the specified envelope.
        // Returns:
        //     Envelope containing both envelope
        IEnvelope Union(IEnvelope other);

    }

    // Interface for IPolyline
    public interface IPolyline : IGeometry
    {
        IPointCollection GetPoints();
    }

    // Interface for IPolygon
    public interface IPolygon : IGeometry
    {
        IPointCollection GetPoints();
    }
}
