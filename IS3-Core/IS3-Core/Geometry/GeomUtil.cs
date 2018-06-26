using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IS3.Core;

namespace IS3.Core.Geometry
{
    public class GeomUtil
    {
        // Project a point to polyline.
        // point: (p), polyline: (pts)
        // output distance relative to the start point of the polyline: (distance)
        // output projection point: (outPnt)
        // return value:
        //      true: the point is projected on the polyline without extending the polyline
        //      false: the point is projected on the polyline through extending the polyline
        //
        public static bool ProjectPointToPolyline(IMapPoint p,
            IPointCollection pts,ref double distance, ref IMapPoint outPnt)
        {
            distance = 0.0;

            double outx = 0.0, outy = 0.0;
            IMapPoint p0, p1;
            for (int i = 0; i < pts.Count - 1; ++i)
            {
                p0 = pts[i];
                p1 = pts[i + 1];

                bool canProject = GeometryAlgorithms.ProjectPointToLine(p.X, p.Y,
                    p0.X, p0.Y, p1.X, p1.Y, ref outx, ref outy);

                if (canProject == true)
                {
                    distance += GeometryAlgorithms.PointDistanceToPoint(outx, outy, p0.X, p0.Y);
                    outPnt = Runtime.geometryEngine.newMapPoint(outx, outy);
                    return true;
                }
                distance += GeometryAlgorithms.PointDistanceToPoint(p0.X, p0.Y, p1.X, p1.Y);
            }

            // Project the point by extending the polyline
            p0 = pts[0];
            p1 = pts[pts.Count - 1];
            double d0p = GeometryAlgorithms.PointDistanceToPoint(p.X, p.Y, p0.X, p0.Y);
            double d1p = GeometryAlgorithms.PointDistanceToPoint(p.X, p.Y, p1.X, p1.Y);
            if (d0p < d1p)
            {
                // the map point is closer to the beginning of the polyline,
                // then extend the beginning segment.
                p1 = pts[1];
                GeometryAlgorithms.ProjectPointToLine(p.X, p.Y,
                    p0.X, p0.Y, p1.X, p1.Y, ref outx, ref outy);
                distance = GeometryAlgorithms.PointDistanceToPoint(outx, outy, p0.X, p0.Y);
                distance *= -1.0;
                outPnt = Runtime.geometryEngine.newMapPoint(outx, outy);
            }
            else
            {
                // the map point is closer to the endding of the polyline,
                // since the loop is ended on the last segment, just use the result is OK.
                distance += GeometryAlgorithms.PointDistanceToPoint(outx, outy, p1.X, p1.Y);
                outPnt = Runtime.geometryEngine.newMapPoint(outx, outy);
            }
            return false;
        }

        #region from IS3.Core ArcGISMappingUtility.cs
        public static IMapPoint Center(IPolygon polygon)
        {
            IEnumerable<IMapPoint> pc = polygon.GetPoints();
            double x = 0;
            double y = 0;

            foreach (IMapPoint p in pc)
            {
                x += p.X;
                y += p.Y;
            }
            x /= pc.Count();
            y /= pc.Count();
            IMapPoint center = Runtime.geometryEngine.newMapPoint(x, y);
            return center;
        }

        public static double Length(IPointCollection pts)
        {
            double len = 0;
            IMapPoint p1, p2;
            for (int i = 0; i < pts.Count - 1; ++i)
            {
                p1 = pts[i];
                p2 = pts[i + 1];
                len += Distance(p1, p2);
            }
            return len;
        }

        public static double Distance(IMapPoint p1, IMapPoint p2)
        {
            return GeometryAlgorithms.PointDistanceToPoint(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static IMapPoint MidPoint(IMapPoint p1, IMapPoint p2)
        {
            double x = (p1.X + p2.X) / 2.0;
            double y = (p1.Y + p2.Y) / 2.0;
            IMapPoint p = Runtime.geometryEngine.newMapPoint(x, y);
            return p;
        }

        public static IMapPoint InterpolatePointOnLine(IMapPoint p1, IMapPoint p2, double scale)
        {
            double x = 0.0, y = 0.0;
            GeometryAlgorithms.InterpolatePointOnLine(p1.X, p1.Y, p2.X, p2.Y, scale, ref x, ref y);
            IMapPoint p = Runtime.geometryEngine.newMapPoint(x, y);
            return p;
        }

        public static void Reverse(IPointCollection pts)
        {
            IEnumerable<IMapPoint> reversedPts = pts.Reverse();

            IPointCollection newPts = Runtime.geometryEngine.newPointCollection();
            foreach (IMapPoint pt in reversedPts)
                newPts.Add(pt);

            pts = Runtime.geometryEngine.newPointCollection();
            foreach (IMapPoint pt in newPts)
                pts.Add(pt);
        }

        /*
         * When you draw a arrow, you need to calculate the points of arrows.
         * The function do this job.
         */
        public static IPointCollection VectorArrowPoints(IMapPoint beginPnt, IMapPoint endPnt)
        {
            double x = beginPnt.X - endPnt.X;
            double y = beginPnt.Y - endPnt.Y;
            double angle = Math.Atan2(y, x);

            double alpha = Math.PI / 6;                         // arrow is 30 degree by each side of original line
            double length = Math.Sqrt(x * x + y * y) * 0.25;      // arrow is a quarter of original length 

            double x1 = endPnt.X + length * Math.Cos(angle + alpha);
            double y1 = endPnt.Y + length * Math.Sin(angle + alpha);
            double x2 = endPnt.X + length * Math.Cos(angle - alpha);
            double y2 = endPnt.Y + length * Math.Sin(angle - alpha);

            IMapPoint p1 = Runtime.geometryEngine.newMapPoint(x1, y1);
            IMapPoint p2 = Runtime.geometryEngine.newMapPoint(x2, y2);

            IPointCollection pc = Runtime.geometryEngine.newPointCollection();
            pc.Add(p1);
            pc.Add(endPnt);
            pc.Add(p2);

            return pc;
        }

        public static IPointCollection VectorArrowPoints(double beginPntX, double beginPntY, IMapPoint endPnt)
        {
            double x = beginPntX - endPnt.X;
            double y = beginPntY - endPnt.Y;
            double angle = Math.Atan2(y, x);

            double alpha = Math.PI / 6;                         // arrow is 30 degree by each side of original line
            double length = Math.Sqrt(x * x + y * y) * 0.25;      // arrow is a quarter of original length 

            double x1 = endPnt.X + length * Math.Cos(angle + alpha);
            double y1 = endPnt.Y + length * Math.Sin(angle + alpha);
            double x2 = endPnt.X + length * Math.Cos(angle - alpha);
            double y2 = endPnt.Y + length * Math.Sin(angle - alpha);

            IMapPoint p1 = Runtime.geometryEngine.newMapPoint(x1, y1);
            IMapPoint p2 = Runtime.geometryEngine.newMapPoint(x2, y2);

            IPointCollection pc = Runtime.geometryEngine.newPointCollection();
            pc.Add(p1);
            pc.Add(endPnt);
            pc.Add(p2);

            return pc;
        }
        #endregion
    }
}
