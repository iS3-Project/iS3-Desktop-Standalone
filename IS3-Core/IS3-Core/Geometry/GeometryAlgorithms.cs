using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core.Geometry
{
    public enum ExtendOption { None, This, Other, Both }
    public enum AngleDirection { Clockwise, CounterClockwise }
    public enum Direction { Left, Right }

    public static class GeometryAlgorithms
    {
        public const double Zero = 1e-6;

        // point 1: (x0, y0), point 2: (x1, y1)
        //
        public static double PointDistanceToPoint(double x0, double y0, double x1, double y1)
        {
            return Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
        }

        // point:(px,py), line:(x0,y0)-(x1,y1)
        //
        public static double PointDistanceToLine(double px, double py,
            double x0, double y0, double x1, double y1)
        {
            //d=|y0-k*x0-b|/sqrt(1+k*k) 点到直线的距离
            double k, b;
            double dDistance;

            if (x0 == x1)
                dDistance = Math.Abs(x0 - px);
            else
            {
                k = (y0 - y1) / (x0 - x1);
                b = y0 - k * x0;
                dDistance = Math.Abs(py - k * px - b) / Math.Sqrt(1.0 + k * k);
            }
            return dDistance;
        }

        // line 1: (x0,y0)-(x1,y1), line 2: (x2,y2)-(x3,y3)
        // intersection point: (outx, outy)
        // return value: false (no intersection point), true (1 intersection point)
        //
        public static bool LineIntersectWithLine(
            double x0, double y0, double x1, double y1,
            double x2, double y2, double x3, double y3,
            ref double outx, ref double outy,
            ExtendOption option)
        {
            double a11 = x3 - x2;
            double a12 = x0 - x1;
            double a21 = y3 - y2;
            double a22 = y0 - y1;
            double b1 = x0 - x2;
            double b2 = y0 - y2;
            double djacob = a11 * a22 - a21 * a12;

            if (Math.Abs(djacob) < Zero)
                return false;//line parrel, no cross point

            double t1 = (a22 * b1 - a12 * b2) / djacob;
            double t2 = (-a21 * b1 + a11 * b2) / djacob;

            //calculate cross point
            outx = x2 + (x3 - x2) * t1;
            outy = y2 + (y3 - y2) * t1;

            if (option == ExtendOption.None)
            {
                if (t1 > -Zero && t1 < (1 + Zero) && t2 > -Zero && t2 < (1 + Zero))
                    return true;
            }
            else if (option == ExtendOption.This)
            {
                if (t1 > -Zero && t1 < (1 + Zero))
                    return true;
            }
            else if (option == ExtendOption.Other)
            {
                if (t2 > -Zero && t2 < (1 + Zero))
                    return true;
            }
            else if (option == ExtendOption.Both)
                return true;

            return false;
        }

        // project point to line
        // point: (px, py), line: (x0,y0)-(x1,y1)
        // return value: false (no projection point), true (1 projection point)
        //
        public static bool ProjectPointToLine(double px, double py,
            double x0, double y0, double x1, double y1,
            ref double outx, ref double outy)
        {
            if (x0 == x1)
            {
                outx = x0;
                outy = py;

                double ymin = y0 > y1 ? y1 : y0;
                double ymax = y0 > y1 ? y0 : y1;
                if (py > ymin && py < ymax)
                    return true;
                else
                    return false;
            }
            if (y0 == y1)
            {
                outx = px;
                outy = y0;

                double xmin = x0 > x1 ? x1 : x0;
                double xmax = x0 > x1 ? x0 : x1;
                if (px > xmin && py < xmax)
                    return true;
                else
                    return false;
            }


            double k0, k1, b0, b1;
            k0 = (y1 - y0) / (x1 - x0);
            b0 = y0 - k0 * x0;
            k1 = -1 / k0;
            b1 = py - k1 * px;

            double x, y;
            if (px != 0) { x = 0; y = b1; }
            else { x = 100; y = k1 * 100 + b1; }
            return LineIntersectWithLine(px, py, x, y, x0, y0, x1, y1, ref outx, ref outy, ExtendOption.This);
        }

        // produce points on a circle
        // circle: (x, y, r)
        // number of points: num
        // return value: px, py
        //
        public static void CircleToPoints(double x, double y, double r,
            int num, double[] px, double[] py, AngleDirection dir)
        {
            double a = 0;
            for (int i = 0; i < num; ++i)
            {
                if (dir == AngleDirection.CounterClockwise)
                    a = 2.0 * Math.PI * i / num;
                else
                    a = 2.0 * Math.PI * (1.0 - (double)i / (double)num);
                px[i] = x + r * Math.Cos(a);
                py[i] = y + r * Math.Sin(a);
            }
        }

        // orientation of the line : (x0,y0)-(x1,y1)
        //
        public static double LineOrientation(double x0, double y0, double x1, double y1)
        {
            return Math.Atan2(y1 - y0, x1 - x0);
        }

        // angle between two lines 
        // line 1: (x0,y0)-(x1,y1), line 2: (x2,y2)-(x3,y3)
        // return value > 0 means line2 turns anti-clockwise relative to line1
        // return value < 0 means line2 turns clockwise relative to line1
        //
        public static double AngleBetweenLines(
            double x0, double y0, double x1, double y1,
            double x2, double y2, double x3, double y3)
        {
            double orientation1 = LineOrientation(x0, y0, x1, y1);
            double orientation2 = LineOrientation(x2, y2, x3, y3);
            return orientation1 - orientation2;
        }

        // point direction to line
        // point: (x,y)
        // line: (x0,y0)-(x1,y1)
        //
        public static Direction PointDirectionToLine(double x, double y,
            double x0, double y0, double x1, double y1)
        {
            double angle = AngleBetweenLines(x0, y0, x, y,
                x0, y0, x1, y1);
            if (angle > 0)      // anti-clockwise
                return Direction.Left;
            else
                return Direction.Right;
        }

        // Interpolate (or Extrapolate) a point on a line
        // line: (x0,y0)-(x1,y1)
        // scale: 0 if the interpolated point is coincide with (x0,y0),
        //        1 if the interpolated point is coincide with (x1,y1),
        //        and 0-1 if the interpolated point is within the line segment
        //
        public static void InterpolatePointOnLine(double x0, double y0, double x1, double y1,
            double scale, ref double outx, ref double outy)
        {
            outx = x0 + scale * (x1 - x0);
            outy = y0 + scale * (y1 - y0);
        }
        public static void InterpolatePointOnLine(double x0, double y0, double z0,
            double x1, double y1, double z1, double scale,
            ref double outx, ref double outy, ref double outz)
        {
            outx = x0 + scale * (x1 - x0);
            outy = y0 + scale * (y1 - y0);
            outz = z0 + scale * (z1 - z0);
        }

        // Area of polygon
        // return value maybe negative, which means the points are clockwise.
        //
        public static double AreaOfPolygon(double[] x, double[] y, int pts)
        {
            if (pts < 3)
                return 0.0;

            double sum = 0.0;
            double x1 = x[0];
            double y1 = y[0];
            double x2, y2, x3, y3;
            for (int i = 1; i < pts - 1; ++i)
            {
                x2 = x[i];
                y2 = y[i];
                x3 = x[i + 1];
                y3 = y[i + 1];
                double a = ((x1 - x3) * (y2 - y3) - (x2 - x3) * (y1 - y3)) / 2.0;
                sum += a;
            }

            return sum;
        }

    }
}
