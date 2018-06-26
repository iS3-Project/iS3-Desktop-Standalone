using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// Credits:
/// Shapefile C Library is (c) 1998 Frank Warmerdam.
/// .NET wrapper provided by David Gancarz, dgancarz@cfl.rr.com or david.gancarz@cityoforlando.net

namespace IS3.Core.Geometry
{
    /// <summary>
    /// Shape type enumeration
    /// </summary>
    public enum ShapeType
    {
        /// <summary>Shape with no geometric data</summary>
        NullShape = 0,
        /// <summary>2D point</summary>
        Point = 1,
        /// <summary>2D polyline</summary>
        PolyLine = 3,
        /// <summary>2D polygon</summary>
        Polygon = 5,
        /// <summary>Set of 2D points</summary>
        MultiPoint = 8,
        /// <summary>3D point</summary>
        PointZ = 11,
        /// <summary>3D polyline</summary>
        PolyLineZ = 13,
        /// <summary>3D polygon</summary>
        PolygonZ = 15,
        /// <summary>Set of 3D points</summary>
        MultiPointZ = 18,
        /// <summary>3D point with measure</summary>
        PointM = 21,
        /// <summary>3D polyline with measure</summary>
        PolyLineM = 23,
        /// <summary>3D polygon with measure</summary>
        PolygonM = 25,
        /// <summary>Set of 3d points with measures</summary>
        MultiPointM = 28,
        /// <summary>Collection of surface patches</summary>
        MultiPatch = 31
    }

    /// <summary>
    /// Part type enumeration - everything but ShapeType.MultiPatch just uses PartType.Ring.
    /// </summary>
    public enum PartType
    {
        /// <summary>
        /// Linked strip of triangles, where every vertex (after the first two) completes a new triangle.
        /// A new triangle is always formed by connecting the new vertex with its two immediate predecessors.
        /// </summary>
        TriangleStrip = 0,
        /// <summary>
        /// A linked fan of triangles, where every vertex (after the first two) completes a new triangle.
        /// A new triangle is always formed by connecting the new vertex with its immediate predecessor 
        /// and the first vertex of the part.
        /// </summary>
        TriangleFan = 1,
        /// <summary>The outer ring of a polygon</summary>
        OuterRing = 2,
        /// <summary>The first ring of a polygon</summary>
        InnerRing = 3,
        /// <summary>The outer ring of a polygon of an unspecified type</summary>
        FirstRing = 4,
        /// <summary>A ring of a polygon of an unspecified type</summary>
        Ring = 5
    }

    /// <summary>
    /// SHPObject - represents on shape (without attributes) read from database.
    /// </summary>
    public class SHPObject
    {
        ///<summary>Shape type as a ShapeType enum</summary>	
        public ShapeType shpType;
        ///<summary>Shape number (-1 is unknown/unassigned)</summary>	
        public int nShapeId;
        ///<summary>Number of parts (0 implies single part with no info)</summary>	
        public int nParts;
        ///<summary>Pointer to int array of part start offsets, of size nParts</summary>	
        public IntPtr paPartStart;
        ///<summary>Pointer to PartType array (PartType.Ring if not ShapeType.MultiPatch) of size nParts</summary>	
        public IntPtr paPartType;
        ///<summary>Number of vertices</summary>	
        public int nVertices;
        ///<summary>Pointer to double array containing X coordinates</summary>	
        public IntPtr padfX;
        ///<summary>Pointer to double array containing Y coordinates</summary>		
        public IntPtr padfY;
        ///<summary>Pointer to double array containing Z coordinates (all zero if not provided)</summary>	
        public IntPtr padfZ;
        ///<summary>Pointer to double array containing Measure coordinates(all zero if not provided)</summary>	
        public IntPtr padfM;
        ///<summary>Bounding rectangle's min X</summary>	
        public double dfXMin;
        ///<summary>Bounding rectangle's min Y</summary>	
        public double dfYMin;
        ///<summary>Bounding rectangle's min Z</summary>	
        public double dfZMin;
        ///<summary>Bounding rectangle's min M</summary>	
        public double dfMMin;
        ///<summary>Bounding rectangle's max X</summary>	
        public double dfXMax;
        ///<summary>Bounding rectangle's max Y</summary>	
        public double dfYMax;
        ///<summary>Bounding rectangle's max Z</summary>	
        public double dfZMax;
        ///<summary>Bounding rectangle's max M</summary>	
        public double dfMMax;
    }

    public class SHPObj
    {
        public ShapeType ShapeType { get; set; }
        public object Shape { get; set; }

        public SHPObj(ShapeType shapeType)
        {
            ShapeType = shapeType;
        }
    }
    public struct Point
    {
        public double X;        // X coordinate
        public double Y;        // Y coordinate
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public struct MultiPoint
    {
        public double[] Box;    // Bounding Box
        public int NumPoints;   // Total Number of Points
        public Point[] Points;  // The Points in the Set
        public MultiPoint(int numPoints)
        {
            Box = new double[4];
            NumPoints = numPoints;
            Points = new Point[numPoints];
        }
        public MultiPoint(double[] box, int numPoints, Point[] points)
        {
            Box = box;
            NumPoints = numPoints;
            Points = points;
        }
    }

    public struct PolyLine
    {
        public double[] Box;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public PolyLine(int numParts, int numPoints)
        {
            Box = new double[4];
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = new int[numParts];
            Points = new Point[numPoints];
        }
        public PolyLine(double[] box, int numParts, int numPoints, int[] parts, Point[] points)
        {
            Box = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            Points = points;
        }
    }

    public struct Polygon
    {
        public double[] Box;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public Polygon(int numParts, int numPoints)
        {
            Box = new double[4];
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = new int[numParts];
            Points = new Point[numPoints];
        }
        public Polygon(double[] box, int numParts, int numPoints, int[] parts, Point[] points)
        {
            Box = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            Points = points;
        }
    }

    public struct PointM
    {
        public double X;        // X coordinate
        public double Y;        // Y coordinate
        public double M;        // Measure
        public PointM(double x, double y, double m)
        {
            X = x;
            Y = y;
            M = m;
        }
    }

    public struct MultiPointM
    {
        public double[] Box;    // Bounding Box
        public int NumPoints;   // Total Number of Points
        public Point[] Points;  // The Points in the Set
        public double[] MRange; // Bounding Measure Range
        public double[] MArray; // Measures
        public MultiPointM(int numPoints)
        {
            Box = new double[4];
            NumPoints = numPoints;
            Points = new Point[numPoints];
            MRange = new double[2];
            MArray = new double[numPoints];
        }
    }

    public struct PolyLineM
    {
        public double[] Box;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double[] MRange; // Bounding Measure Range
        public double[] MArray; // Measures
        public PolyLineM(int numParts, int numPoints)
        {
            Box = new double[4];
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = new int[numParts];
            Points = new Point[numPoints];
            MRange = new double[2];
            MArray = new double[numPoints];
        }
    }

    public struct PolygonM
    {
        public double[] Box;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double[] MRange; // Bounding Measure Range
        public double[] MArray; // Measures
        public PolygonM(int numParts, int numPoints)
        {
            Box = new double[4];
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = new int[numParts];
            Points = new Point[numPoints];
            MRange = new double[2];
            MArray = new double[numPoints];
        }
    }

    public struct PointZ
    {
        public double X;        // X coordinate
        public double Y;        // Y coordinate
        public double Z;        // Z coordinate
        public double M;        // Measure
        public PointZ(double x, double y, double z, double m)
        {
            X = x;
            Y = y;
            Z = z;
            M = m;
        }
    }

    public struct MultiPointZ
    {
        public double[] Box;    // Bounding Box
        public int NumPoints;   // Total Number of Points
        public Point[] Points;  // The Points in the Set
        public double[] ZRange; // Bounding Z Range
        public double[] ZArray; // Z Values
        public double[] MRange; // Bounding Measure Range
        public double[] MArray; // Measures
        public MultiPointZ(int numPoints)
        {
            Box = new double[4];
            NumPoints = numPoints;
            Points = new Point[numPoints];
            ZRange = new double[2];
            ZArray = new double[numPoints];
            MRange = new double[2];
            MArray = new double[numPoints];
        }
    }

    public struct PolyLineZ
    {
        public double[] Box;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double[] ZRange; // Bounding Z Range
        public double[] ZArray; // Z Values
        public double[] MRange; // Bounding Measure Range
        public double[] MArray; // Measures
        public PolyLineZ(int numParts, int numPoints)
        {
            Box = new double[4];
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = new int[numParts];
            Points = new Point[numPoints];
            ZRange = new double[2];
            ZArray = new double[numPoints];
            MRange = new double[2];
            MArray = new double[numPoints];
        }
    }

    public struct PolygonZ
    {
        public double[] Box;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double[] ZRange; // Bounding Z Range
        public double[] ZArray; // Z Values
        public double[] MRange; // Bounding Measure Range
        public double[] MArray; // Measures
        public PolygonZ(int numParts, int numPoints)
        {
            Box = new double[4];
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = new int[numParts];
            Points = new Point[numPoints];
            ZRange = new double[2];
            ZArray = new double[numPoints];
            MRange = new double[2];
            MArray = new double[numPoints];
        }
    }

    public struct MultiPatch
    {
        public double[] Box;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public int[] PartTypes; // Part Type
        public Point[] Points;  // Points for All Parts
        public double[] ZRange; // Bounding Z Range
        public double[] ZArray; // Z Values
        public double[] MRange; // Bounding Measure Range
        public double[] MArray; // Measures
        public MultiPatch(int numParts, int numPoints)
        {
            Box = new double[4];
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = new int[numParts];
            PartTypes = new int[numParts];
            Points = new Point[numPoints];
            ZRange = new double[2];
            ZArray = new double[numPoints];
            MRange = new double[2];
            MArray = new double[numPoints];
        }
    }

    public class ShapeLib
    {
        static Point BuildPoint(byte[] bytes, int startIndex)
        {
            Point p = new Point();
            p.X = BitConverter.ToDouble(bytes, startIndex);
            p.Y = BitConverter.ToDouble(bytes, startIndex + 8);
            return p;
        }
        static double[] BuildBox(byte[] bytes, int startIndex)
        {
            double[] box = new double[4];
            for (int i = 0; i < 4; ++i)
                box[i] = BitConverter.ToDouble(bytes, startIndex + i * 8);
            return box;
        }
        static Point[] BuildPoints(byte[] bytes, int startIndex, int numPoints)
        {
            Point[] points = new Point[numPoints];
            for (int i = 0; i < numPoints; ++i)
                points[i] = BuildPoint(bytes, startIndex + i * 16);
            return points;
        }

        static int[] BuildParts(byte[] bytes, int startIndex, int numParts)
        {
            int[] parts = new int[numParts];
            for (int i = 0; i < numParts; ++i)
                parts[i] = BitConverter.ToInt32(bytes, startIndex + i * 4);
            return parts;
        }

        public static SHPObj BuildObject(byte[] bytes)
        {
            int iType = BitConverter.ToInt32(bytes, 0);
            ShapeType shapeType = (ShapeType)Enum.ToObject(typeof(ShapeType), iType);
            SHPObj obj = new SHPObj(shapeType);

            int numPoints, numParts;
            double[] box;
            Point[] points;
            int[] parts;

            switch (shapeType)
            {
                case ShapeType.Point:
                    obj.Shape = BuildPoint(bytes, 4);
                    break;
                case ShapeType.MultiPoint:
                    box = BuildBox(bytes, 4);
                    numPoints = BitConverter.ToInt32(bytes, 36);
                    points = BuildPoints(bytes, 40, numPoints);
                    obj.Shape = new MultiPoint(box, numPoints, points);
                    break;
                case ShapeType.PolyLine:
                    box = BuildBox(bytes, 4);
                    numParts = BitConverter.ToInt32(bytes, 36);
                    numPoints = BitConverter.ToInt32(bytes, 40);
                    parts = BuildParts(bytes, 44, numParts);
                    points = BuildPoints(bytes, 44 + 4 * numParts, numPoints);
                    obj.Shape = new PolyLine(box, numParts, numPoints, parts, points);
                    break;
                case ShapeType.Polygon:
                    box = BuildBox(bytes, 4);
                    numParts = BitConverter.ToInt32(bytes, 36);
                    numPoints = BitConverter.ToInt32(bytes, 40);
                    parts = BuildParts(bytes, 44, numParts);
                    points = BuildPoints(bytes, 44 + 4 * numParts, numPoints);
                    obj.Shape = new Polygon(box, numParts, numPoints, parts, points);
                    break;
            }

            return obj;
        }
    }

}
