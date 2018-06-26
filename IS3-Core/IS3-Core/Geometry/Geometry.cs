using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace IS3.Core.Geometry
{
    public class Point2D
    {
        public Point2D() { }
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Envelope2D
    {
        public Envelope2D() { }
        public Envelope2D(Point2D p1, Point2D p2)
        {
            XMin = p1.X;
            YMin = p1.Y;
            XMax = p2.X;
            YMax = p2.Y;
        }
        public Envelope2D(double x1, double y1, double x2, double y2)
        {
            XMin = x1;
            YMin = y1;
            XMax = x2;
            YMax = y2;
        }

        public double Height { get { return YMax - YMin; } }
        public double Width { get { return XMax - XMin; } }

        public double XMax { get; set; }
        public double XMin { get; set; }
        public double YMax { get; set; }
        public double YMin { get; set; }
    }
}
