using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core.Geometry
{
    // Interface for geometry engine: generate geometry objects
    public interface IGeometryEngine
    {
        // Create object with IMapPoint interface.
        IMapPoint newMapPoint(double x, double y);
        IMapPoint newMapPoint(double x, double y, ISpatialReference spatialReference);
        IMapPoint newMapPoint(double x, double y, double z);
        IMapPoint newMapPoint(double x, double y, double z, ISpatialReference spatialReference);
        IMapPoint newMapPoint(double x, double y, double z, double m);
        IMapPoint newMapPoint(double x, double y, double z, double m, ISpatialReference spatialReference);

        IPointCollection newPointCollection();

        // Create object with IEnvelope interface.
        IEnvelope newEnvelope(double x1, double y1, double x2, double y2);
        IEnvelope newEnvelope(double x1, double y1, double x2, double y2, ISpatialReference spatialReference);

        // Create object with IPolyline interface.
        // Note: the part should contain IEnumerable<IMapPoint> interface.
        IPolyline newPolyline(IPointCollection part);

        // Create object with IPolygon interface.
        // Note: the part should contain IEnumerable<IMapPoint> interface.
        IPolygon newPolygon(IPointCollection part);

        // Summary:
        //     Creates a geometry from an ArcGIS json geometry representation.
        //
        // Parameters:
        //   json:
        //     JSON representation of geometry.
        //
        // Returns:
        //     Geometry converted from a JSON String.
        IGeometry fromJson(string json);
    }
}
