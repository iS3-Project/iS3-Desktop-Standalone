using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

/// ShapeLib for shapes from .MDB.
/// Shape Binary definition is from:
/// 1. ESRI Shapefile Technical Description, An ESRI White Paper - July 1998.
/// 2. ESRI Extended Shapefile Record Fomart, July 28, 2006
/// 3. ESRI Extend Shape Buffer Format, June 20, 2012
/// 4. ESRI File Geodatabase API 1.4 version for Windows (Visual Studio 2013)
/// Written by Lixiaojun, 2015.

namespace IS3.Core.Shape
{
    /// <summary>
    /// Shape type enumeration
    /// </summary>
    /// 
    public enum GeometryType
    {
        Null = 0,
        Point = 1,
        Multipoint = 2,
        Polyline = 3,
        Polygon = 4,
        MultiPatch = 9,
    }
    public enum ShapeType
    {
        Null = 0,
        Point = 1,
        Polyline = 3,
        Polygon = 5,
        Multipoint = 8,
        PointZ = 9,
        PolylineZ = 10,
        PointZM = 11,
        PolylineZM = 13,
        PolygonZM = 15,
        MultipointZM = 18,
        PolygonZ = 19,
        MultipointZ = 20,
        PointM = 21,
        PolylineM = 23,
        PolygonM = 25,
        MultipointM = 28,
        MultiPatchM = 31,
        MultiPatch = 32,
        GeneralPolyline = 50,
        GeneralPolygon = 51,
        GeneralPoint = 52,
        GeneralMultipoint = 53,
        GeneralMultiPatch = 54,
    }

    public enum ShapeModifiers
    {
        /*
        HasZs = -2147483648,
        BasicModifierMask = -1073741824,
        ExtendedModifierMask = -587202560,
        ModifierMask = -16777216,
        BasicTypeMask = 255,
        IsCompressed = 8388608,
        MultiPatchModifierMask = 15728640,
        HasMaterials = 16777216,
        HasPartIDs = 33554432,
        HasTextures = 67108864,
        HasNormals = 134217728,
        HasIDs = 268435456,
        HasCurves = 536870912,
        NonBasicModifierMask = 1056964608,
        HasMs = 1073741824,
        */

        ModifierMask = -16777216,
        ExtendedModifierMask = -587202560,
        BasicModifierMask = -1073741824,
        //ModifierMask =              0xFF000000,
        //ExtendedModifierMask =      0xDD000000,
        //BasicModifierMask =         0xC0000000,
        NonBasicModifierMask =      0x3F000000,
        MultiPatchModifierMask =    0x00F00000,
        BasicTypeMask =             0x000000FF,

        HasZs = -2147483648,
        //HasZs =                     0x80000000,
        HasMs =                     0x40000000,
        HasCurves =                 0x20000000,
        HasIDs =                    0x10000000,
        HasNormals =                0x08000000,
        HasTextures =               0x04000000,
        HasPartIDs =                0x02000000,
        HasMaterials =              0x01000000,
        IsCompressed =              0x00800000,
    }

    public enum SegmentType
    {
        Arc = 1,
        Line = 2,
        Spiral = 3,
        Bezier3Curve = 4,
        EllipticArc = 5,
    }

    public enum SegmentArcModifiers
    {
        IsEmpty = 1,        // arc is undefined
        // (reserved)
        // (reserved)
        IsCCW = 8,          // 1 if arc is in counterclockwise order
        IsMinor = 16,       // 1 if central angle of arc does not exceed pi
        IsLine = 32,        // only SP and EP are defined
        IsPoint = 64,       // CP, SP, EP are identical; angles are stored instead of CP
        DefinedIP = 128,    // IP - interior point;
                            // arcs persisted with 9.2 persist endpoints + 1 interior point,
                            // rather than endpoints and center point so that the
                            // arc shape can be recovered after projecting to another
                            // spatial reference and back again; 
                            // point arcs still replace the center point with SA and CA
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SegmentArc
    {
        // If IsPoint is 0. Also, it is ignored if IsLine is 1
        [FieldOffset(0)]
        public Point CenterPoint;

        /// If IsPoint is 1: start and central angle centerPoint = endPoint
        [FieldOffset(0)]
        public double StartAngle;
        [FieldOffset(8)]
        public double CentralAngle;

        // contains among others bits for IsPoint, IsLine, ...
        // Bits contains the following bits (starting with bit 0)
        // See SegmentArcModifiers
        [FieldOffset(16)]
        public int Bits;
    }

    public struct SegmentBezierCurve
    {
        public Point ControlPoint0;
        public Point ControlPoint1;
    }

    public enum SegmentEllipticArcModifiers
    {
        IsEmpty = 1,        // 1 if the arc is undefined
        // (reserved)
        // (reserved)
        // (reserved)       // These 5 bits are used only at run-time.
        // (reserved)       // Their values are ignored when reading from disk.
        // (reserved)
        IsLine = 64,        // 1 if the MinorMajor Ratio is 0.
        IsPoint = 128,      // 1 if the SemiMajor axis is 0.
        IsCircular = 256,   // 1 if the MinorMajorRatio is 1.
        CenterTo = 512,     // 1 if the Center and the ToPoint are identical.
        CenterFrom = 1024,  // 1 if the Center and the FromPoint are identical.
        IsCCW = 2048,       // 1 if the arc is in counterclockwise order.
        IsMinor = 4096,     // 1 if the DeltaV does not exceed pi (or go below -pi).
        IsComplete = 8192,  // 1 if DeltaV is plus or minus 2*pi
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SegmentEllipticArc
    {
        // If CenterTo and CenterFrom are both 0
        [FieldOffset(0)]
        public Point Center;

        // If CenterTo or CenterFrom is 1: from and delta Vs.
        // Center = fromPoint or toPoint, depending on values.
        // Vs are similar to angles.
        // If you think of an ellipse as a stretched circle,
        // then Vs are angles on the circle being stretched.
        [FieldOffset(0)]
        public double Vs1;
        [FieldOffset(8)]
        public double Vs2;

        // If CenterFrom is 1 or CenterTo is 1 or IsLine is 0.
        // Rotation of the semimajor axis relative to the x-axis, in radians.
        [FieldOffset(16)]
        public double Rotation;
        // If CenterTo is 0, CenterFrom is 0, and IsLine is 1.
        [FieldOffset(16)]
        public double FromV;

        // On the embedded ellipse, this is the distance 
        // from the center to the point furthest from the center.
        [FieldOffset(24)]
        public double SemiMajor;

        // If CenterFrom is 1 or CenterTo is 1 or IsLine is 0.
        // The ratio between the semiminor and semimajor axes.
        [FieldOffset(32)]
        public double MinorMajorRatio;
        // If CenterFrom is 0 and CenterTo is 0 and IsLine is 1.
        [FieldOffset(32)]
        public double DeltaV;

        [FieldOffset(40)]
        public int Bits;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct _SegmentParams
    {
        [FieldOffset(0)]
        public SegmentArc Arc;
        [FieldOffset(0)]
        public SegmentBezierCurve BezierCurve;
        [FieldOffset(0)]
        public SegmentEllipticArc EllipticArc;
    };

    public struct SegmentModifier
    {
        public int StartPointIndex;
        public int SegmentType;
        public _SegmentParams SegmentParams;
    }

    /// <summary>
    /// Part type enumeration - for ShapeType.GeneralMultiPatch
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
        Ring = 5,
        Triangles = 6,
    }
    
    public enum PartTypeModifiers
    {
        // The values used for encoding part type is defined in enum PartType.
        //
        // LevelOfDetail value is a hint reserved for future use.
        //
        // Priority is an integer value in the range of [-32, 31]
        // indicating the relative priority between the coincident surface
        // patches. The default zero as the most typical value.
        //
        // Material value is the index into MaterialParts[] array,
        // see MultiPatchZM struct.
        //
        PartType        = 0x0000000F,       // int: 4
        LevelOfDetail   = 0x000003F0,       // unsigned: 6
        Priority        = 0x0000FC00,       // int: 6
        Material        = -65536,           // int: 16
        //        Material        = 0xFFFF0000,       // int: 16
    }

    public struct Normal
    {
        public float f0;
        public float f1;
        public float f2;
    }

    public enum TexCompressionType
    {
        TextureCompressionNever     = 1,
        TextureCompressionNone      = 2,
        TextureCompressionJPEG      = 3,
        TextureCompressionJPEGPlus  = 4, // with transparency mask
    }

    public enum MaterialType
    {
        MaterialColor           = 1,
        MaterialTextureMap      = 2,
        MaterialTransparency    = 3,
        MaterialShininess       = 4,
        MaterialSharedTexture   = 5,
        MaterialCullBackFaces   = 6,
        MaterialEdgeColor       = 9,
        MaterialEdgeWidth       = 10,
        MaterialLast            = 11,
    }

    public struct Material
    {
        public Byte Type;
        public object Data;
    }

    public struct MaterialColor
    {
        public Byte Red, Green, Blue;  // [0, 255]
    }

    public struct MaterialTextureMap
    {
        public Byte Bpp;           // Bytes per pixel
        public short Width;        // texture width
        public short Height;       // texture height
        public int Size;           // texture buffer size
        public int TCType;         // TexCompressionType
        public Byte[] TexBuff;     // texBuff[Height][Width][Bpp] if not compressed
        /*
         * Note that texBuff is stored row-wise, where:
         *      texBuff[0][anyCol] corresponds to texture coordinates (s, t=0.0);
         *      texBuff[height-1][anyCol] corresponds to texture coordinates (s, t=1.0);
         *      texBuff[anyRow][0] corresponds to texture coordinates (s=0.0, t);
         *      texBuff[anyRow][width-1] corresponds to texture coordinates (s=1.0, t);
         * Color components, if not compressed, are stored in RGBA order.
         * For example, in the case of 3 bytes per pixel (Bpp=3), the components are stored as:
         *      texBuff[row][col][0] representing Red value;
         *      texBuff[row][col][1] representing Green value;
         *      texBuff[row][col][2] representing Blue value.
         * If texture is compressed, texBuff contains the compressed stream,
         * and size is the compressed size in bytes.
         */
    }

    public struct MaterialTransparency
    {
        public Byte Transparency;  // percent transparency [0, 100]
    }

    public struct MaterialShininess
    {
        public Byte Shininess;     // percent shininess [0, 100]
    }

    public struct MaterialSharedTexture
    {
        public int MaterialIndex;  // material with the full MaterialTextureMap
    }

    public struct MaterialCullBackFaces
    {
    }

    public struct MaterialEdgeColor
    {
        public Byte Red, Green, Blue;  // [0, 255]
    }

    public struct MaterialEdgeWidth
    {
        public Byte Width;         // <= 255
    }

    public struct ShapeObject
    {
        public GeometryType Type;
        public object Data;

        public ShapeObject(GeometryType geometryType, object data)
        {
            Type = geometryType;
            Data = data;
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

    public struct Envelope
    {
        public double XMin, YMin, XMax, YMax;
        public Envelope(double xMin, double yMin, double xMax, double yMax)
        {
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
        }
    }

    public struct Multipoint
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumPoints;   // Total Number of Points
        public Point[] Points;  // The Points in the Set
        public Multipoint(Envelope box, int numPoints, Point[] points)
        {
            BoundingBox = box;
            NumPoints = numPoints;
            Points = points;
        }
    }

    public struct Polyline
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public Polyline(Envelope box, int numParts, int numPoints,
            int[] parts, Point[] points)
        {
            BoundingBox = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            Points = points;
        }
    }

    public struct Polygon
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public Polygon(Envelope box, int numParts, int numPoints,
            int[] parts, Point[] points)
        {
            BoundingBox = box;
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

    public struct MultipointM
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumPoints;   // Total Number of Points
        public Point[] Points;  // The Points in the Set
        public double MMin; // Bounding Measure Range
        public double MMax;
        public double[] Ms; // Measures
        public MultipointM(Envelope box, int numPoints, Point[] points,
            double mMin, double mMax, double[] mArray)
        {
            BoundingBox = box;
            NumPoints = numPoints;
            Points = points;
            MMin = mMin;
            MMax = mMax;
            Ms = mArray;
        }
    }

    public struct PolylineM
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double MMin; // Bounding Measure Range
        public double MMax;
        public double[] Ms; // Measures
        public PolylineM(Envelope box, int numParts, int numPoints,
            int[] parts, Point[] points,
            double mMin, double mMax, double[] mArray)
        {
            BoundingBox = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            Points = points;
            MMin = mMin;
            MMax = mMax;
            Ms = mArray;
        }
    }

    public struct PolygonM
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double MMin; // Bounding Measure Range
        public double MMax;
        public double[] Ms; // Measures
        public PolygonM(Envelope box, int numParts, int numPoints,
            int[] parts, Point[] points,
            double mMin, double mMax, double[] mArray)
        {
            BoundingBox = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            Points = points;
            MMin = mMin;
            MMax = mMax;
            Ms = mArray;
        }
    }

    public struct PointZM
    {
        public double X;        // X coordinate
        public double Y;        // Y coordinate
        public double Z;        // Z coordinate
        public double M;        // Measure
        public int ID;
        public PointZM(double x, double y, double z, double m, int id)
        {
            X = x;
            Y = y;
            Z = z;
            M = m;
            ID = id;
        }
    }

    public struct MultipointZM
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumPoints;   // Total Number of Points
        public Point[] Points;  // The Points in the Set
        public double ZMin; // Bounding Z Range
        public double ZMax;
        public double[] Zs; // Z Values
        public double MMin; // Bounding Measure Range
        public double MMax;
        public double[] Ms; // Measures
        public int[] IDs;
        public MultipointZM(Envelope box, int numPoints, Point[] points,
            double zMin, double zMax, double[] zArray,
            double mMin, double mMax, double[] mArray,
            int[] ids)
        {
            BoundingBox = box;
            NumPoints = numPoints;
            Points = points;
            ZMin = zMin;
            ZMax = zMax;
            Zs = zArray;
            MMin = mMin;
            MMax = mMax;
            Ms = mArray;
            IDs = ids;
        }
    }

    public struct PolylineZM
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double ZMin; // Bounding Z Range
        public double ZMax;
        public double[] Zs; // Z Values
        public double MMin; // Bounding Measure Range
        public double MMax;
        public double[] Ms; // Measures
        public int NumCurves;
        public SegmentModifier[] SegmentModifiers;
        public int[] IDs;
        
        public PolylineZM(Envelope box, int numParts, int numPoints,
            int[] parts, Point[] points,
            double zMin, double zMax, double[] zArray,
            double mMin, double mMax, double[] mArray,
            int numCurves, SegmentModifier[] segmentModifiers,
            int[] ids)
        {
            BoundingBox = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            Points = points;
            ZMin = zMin;
            ZMax = zMax;
            Zs = zArray;
            MMin = mMin;
            MMax = mMax;
            Ms = mArray;
            NumCurves = numCurves;
            SegmentModifiers = segmentModifiers;
            IDs = ids;
        }
    }

    public struct PolygonZM
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public Point[] Points;  // Points for All Parts
        public double ZMin; // Bounding Z Range
        public double ZMax;
        public double[] Zs; // Z Values
        public double MMin; // Bounding Measure Range
        public double MMax;
        public double[] Ms; // Measures
        public int NumCurves;
        public SegmentModifier[] SegmentModifiers;
        public int[] IDs;

        public PolygonZM(Envelope box, int numParts, int numPoints,
            int[] parts, Point[] points,
            double zMin, double zMax, double[] zArray,
            double mMin, double mMax, double[] mArray,
            int numCurves, SegmentModifier[] segmentModifiers,
            int[] ids)
        {
            BoundingBox = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            Points = points;
            ZMin = zMin;
            ZMax = zMax;
            Zs = zArray;
            MMin = mMin;
            MMax = mMax;
            Ms = mArray;
            NumCurves = numCurves;
            SegmentModifiers = segmentModifiers;
            IDs = ids;
        }
    }

    public struct MultiPatchZM
    {
        public Envelope BoundingBox;    // Bounding Box
        public int NumParts;    // Number of Parts
        public int NumPoints;   // Total Number of Points
        public int[] Parts;     // Index to First Point in Part
        public int[] PartTypes; // Part Type
        public Point[] Points;  // Points for All Parts
        public double ZMin; // Bounding Z Range
        public double ZMax;
        public double[] Zs; // Z Values
        public double MMin; // Bounding Measure Range
        public double MMax;
        public double[] Ms; // Measures
        public int NumIDs;
        public int[] IDs;
        public int NumNormals;
        public Normal[] Normals;
        public int NumTex;
        public int TexDim;
        public int[] TexParts;
        public float[,] TexCoords;
        public int NumMaterials;
        public int TexCompType;
        public int[] MaterialParts;
        public Material[] Materials;
        public MultiPatchZM(Envelope box, int numParts, int numPoints,
            int[] parts, int[] partTypes, Point[] points,
            double zMin, double zMax, double[] zArray,
            double mMin, double mMax, double[] mArray,
            int numIDs, int[] ids,
            int numNormals, Normal[] normals,
            int numTex, int texDim, int[] texParts, float[,] texCoords,
            int numMaterial, int texCompType, int[] materialParts, Material[] materials)
        {
            BoundingBox = box;
            NumParts = numParts;
            NumPoints = numPoints;
            Parts = parts;
            PartTypes = partTypes;
            Points = points;
            ZMin = zMin;
            ZMax = zMax;
            Zs = zArray;
            MMin = mMin;
            MMax = mMax;
            Ms = mArray;
            NumIDs = numIDs;
            IDs = ids;
            NumNormals = numNormals;
            Normals = normals;
            NumTex = numTex;
            TexDim = texDim;
            TexParts = texParts;
            TexCoords = texCoords;
            NumMaterials = numMaterial;
            TexCompType = texCompType;
            MaterialParts = materialParts;
            Materials = materials;
        }
    }

    public class ShapeBuilder
    {
        static Point BuildPoint(byte[] bytes, int startIndex)
        {
            Point p;
            p.X = BitConverter.ToDouble(bytes, startIndex);
            p.Y = BitConverter.ToDouble(bytes, startIndex + 8);
            return p;
        }
        static Envelope BuildEnvelope(byte[] bytes, int startIndex)
        {
            Envelope boundingBox;
            boundingBox.XMin = BitConverter.ToDouble(bytes, startIndex);
            boundingBox.YMin = BitConverter.ToDouble(bytes, startIndex + 8);
            boundingBox.XMax = BitConverter.ToDouble(bytes, startIndex + 16);
            boundingBox.YMax = BitConverter.ToDouble(bytes, startIndex + 24);
            return boundingBox;
        }
        static Point[] BuildPoints(byte[] bytes, int startIndex, int numPoints)
        {
            Point[] points = new Point[numPoints];
            for (int i = 0; i < numPoints; ++i)
                points[i] = BuildPoint(bytes, startIndex + i * 16);
            return points;
        }

        static int[] BuildInt32Array(byte[] bytes, int startIndex, int size)
        {
            int[] int32Array = new int[size];
            for (int i = 0; i < size; ++i)
                int32Array[i] = BitConverter.ToInt32(bytes, startIndex + i * 4);
            return int32Array;
        }

        static double[] BuildDoubleArray(byte[] bytes, int startIndex, int size)
        {
            double[] doubleArray = new double[size];
            for (int i = 0; i < size; ++i)
                doubleArray[i] = BitConverter.ToDouble(bytes, startIndex + i * 8);
            return doubleArray;
        }

        static int SegmentLength(int segmentType)
        {
            if (segmentType == (int)SegmentType.Arc)
                return 28;
            else if (segmentType == (int)SegmentType.Bezier3Curve)
                return 40;
            else if (segmentType == (int)SegmentType.EllipticArc)
                return 52;
            else
                return 0;
        }

        static SegmentModifier[] BuildSegmentModifiers(byte[] bytes, int startIndex, int numCurves, out int totalLen)
        {
            SegmentModifier[] segmentModifiers = new SegmentModifier[numCurves];
            int len = 0;
            for (int i = 0; i < numCurves; ++i)
            {
                segmentModifiers[i].StartPointIndex = BitConverter.ToInt32(bytes, startIndex + len);
                int segmentType = BitConverter.ToInt32(bytes, startIndex + len + 4);
                segmentModifiers[i].SegmentType = segmentType;
                segmentModifiers[i].SegmentParams = BuildSegmentParams(bytes, startIndex + len + 8, segmentType);
                len += SegmentLength(segmentType);
            }
            totalLen = len;
            return segmentModifiers;
        }

        static _SegmentParams BuildSegmentParams(byte[] bytes, int startIndex, int segmentType)
        {
            _SegmentParams segmentParams = new _SegmentParams();
            int segmentLength = SegmentLength(segmentType);
            if (segmentType == (int)SegmentType.Arc)
            {
                segmentParams.Arc.CenterPoint = BuildPoint(bytes, startIndex);
                segmentParams.Arc.Bits = BitConverter.ToInt32(bytes, startIndex + 16);
            }
            else if (segmentType == (int)SegmentType.Bezier3Curve)
            {
                segmentParams.BezierCurve.ControlPoint0 = BuildPoint(bytes, startIndex);
                segmentParams.BezierCurve.ControlPoint1 = BuildPoint(bytes, startIndex + 16);
            }
            else if (segmentType == (int)SegmentType.EllipticArc)
            {
                segmentParams.EllipticArc.Center = BuildPoint(bytes, startIndex);
                segmentParams.EllipticArc.Rotation = BitConverter.ToDouble(bytes, startIndex + 16);
                segmentParams.EllipticArc.SemiMajor = BitConverter.ToDouble(bytes, startIndex + 24);
                segmentParams.EllipticArc.MinorMajorRatio = BitConverter.ToDouble(bytes, startIndex + 32);
                segmentParams.EllipticArc.Bits = BitConverter.ToInt32(bytes, startIndex + 40);
            }
            return segmentParams;
        }

        static Normal[] BuildNormals(byte[] bytes, int startIndex, int numNormals)
        {
            Normal[] normals = new Normal[numNormals];
            for (int i = 0; i < numNormals; ++i)
            {
                normals[i].f0 = BitConverter.ToSingle(bytes, startIndex + i * 12);
                normals[i].f1 = BitConverter.ToSingle(bytes, startIndex + i * 12 + 4);
                normals[i].f2 = BitConverter.ToSingle(bytes, startIndex + i * 12 + 8);
            }
            return normals;
        }

        static float[,] BuildTexCoords(byte[] bytes, int startIndex, int texDim, int numTex)
        {
            float[,] texCoords = new float[texDim, numTex];
            for (int i = 0; i < numTex; ++i)
            {
                for (int j = 0; i < texDim; ++j)
                {
                    texCoords[i, j] = BitConverter.ToSingle(bytes, startIndex + (i * texDim * 4) + j * 4);
                }
            }
            return texCoords;
        }

        static byte[] BuildBytes(byte[] bytes, int startIndex, int size)
        {
            byte[] result = new byte[size];
            for (int i = 0; i < size; ++i)
                result[i] = bytes[i];
            return result;
        }

        static Material BuildMaterial(byte[] bytes, int startIndex)
        {
            Material mat = new Material();
            byte matType = bytes[startIndex];
            mat.Type = matType;

            if (matType == (byte)MaterialType.MaterialColor)
            {
                MaterialColor mColor = new MaterialColor();
                mColor.Red = bytes[startIndex + 1];
                mColor.Green = bytes[startIndex + 2];
                mColor.Blue = bytes[startIndex + 3];
                mat.Data = mColor;
            }
            else if (matType == (byte)MaterialType.MaterialTextureMap)
            {
                MaterialTextureMap mTexMap = new MaterialTextureMap();
                mTexMap.Bpp = bytes[startIndex + 1];
                mTexMap.Width = BitConverter.ToInt16(bytes, startIndex + 2);
                mTexMap.Height = BitConverter.ToInt16(bytes, startIndex + 4);
                mTexMap.Size = BitConverter.ToInt32(bytes, startIndex + 6);
                mTexMap.TCType = BitConverter.ToInt32(bytes, startIndex + 10);
                mTexMap.TexBuff = BuildBytes(bytes, startIndex + 14, mTexMap.Size);
                mat.Data = mTexMap;
            }
            else if (matType == (byte)MaterialType.MaterialTransparency)
            {
                MaterialTransparency mTrans = new MaterialTransparency();
                mTrans.Transparency = bytes[startIndex + 1];
                mat.Data = mTrans;
            }
            else if (matType == (byte)MaterialType.MaterialShininess)
            {
                MaterialShininess mShin = new MaterialShininess();
                mShin.Shininess = bytes[startIndex + 1];
                mat.Data = mShin;
            }
            else if (matType == (byte)MaterialType.MaterialSharedTexture)
            {
                MaterialSharedTexture mSharedTex = new MaterialSharedTexture();
                mSharedTex.MaterialIndex = BitConverter.ToInt32(bytes, startIndex + 1);
                mat.Data = mSharedTex;
            }
            else if (matType == (byte)MaterialType.MaterialCullBackFaces)
            {
                MaterialCullBackFaces mCullBackFaces = new MaterialCullBackFaces();
                mat.Data = mCullBackFaces;
            }
            else if (matType == (byte)MaterialType.MaterialEdgeColor)
            {
                MaterialEdgeColor mEdgeColor = new MaterialEdgeColor();
                mEdgeColor.Red = bytes[startIndex + 1];
                mEdgeColor.Green = bytes[startIndex + 2];
                mEdgeColor.Blue = bytes[startIndex + 3];
                mat.Data = mEdgeColor;
            }
            else if (matType == (byte)MaterialType.MaterialEdgeWidth)
            {
                MaterialEdgeWidth mEdgeWidth = new MaterialEdgeWidth();
                mEdgeWidth.Width = bytes[startIndex + 1];
                mat.Data = mEdgeWidth;
            }
            else if (matType == (byte)MaterialType.MaterialLast)
            {
            }
            return mat;
        }

        static Material[] BuildMaterials(byte[] bytes, int startIndex, int[] materialParts, int numMaterial)
        {
            Material[] materials = new Material[numMaterial];
            for (int i = 0; i < numMaterial; ++i)
            {
                materials[i] = BuildMaterial(bytes, startIndex + materialParts[i]);
            }
            return materials;
        }

        static bool IsPoint(int iType)
        {
            if (iType == (int)ShapeType.Point ||
                iType == (int)ShapeType.PointZ ||
                iType == (int)ShapeType.PointM ||
                iType == (int)ShapeType.PointZM ||
                (iType & (int)ShapeModifiers.BasicTypeMask) == (int)ShapeType.GeneralPoint)
                return true;
            return false;
        }

        static bool IsMultipoint(int iType)
        {
            if (iType == (int)ShapeType.Multipoint ||
                iType == (int)ShapeType.MultipointZ ||
                iType == (int)ShapeType.MultipointM ||
                iType == (int)ShapeType.MultipointZM ||
                (iType & (int)ShapeModifiers.BasicTypeMask) == (int)ShapeType.GeneralMultipoint)
                return true;
            return false;
        }

        static bool IsPolyline(int iType)
        {
            if (iType == (int)ShapeType.Polyline ||
                iType == (int)ShapeType.PolylineZ ||
                iType == (int)ShapeType.PolylineM ||
                iType == (int)ShapeType.PolylineZM ||
                (iType & (int)ShapeModifiers.BasicTypeMask) == (int)ShapeType.GeneralPolyline)
                return true;
            return false;
        }

        static bool IsPolygon(int iType)
        {
            if (iType == (int)ShapeType.Polygon ||
                iType == (int)ShapeType.PolygonZ ||
                iType == (int)ShapeType.PolygonM ||
                iType == (int)ShapeType.PolygonZM ||
                (iType & (int)ShapeModifiers.BasicTypeMask) == (int)ShapeType.GeneralPolygon)
                return true;
            return false;
        }

        static bool IsMultiPatch(int iType)
        {
            if (iType == (int)ShapeType.MultiPatch ||
                iType == (int)ShapeType.MultiPatchM ||
                (iType & (int)ShapeModifiers.BasicTypeMask) == (int)ShapeType.GeneralMultiPatch)
                return true;
            return false;
        }

        static bool HasZ(int iType)
        {
            if ((iType & (int)ShapeModifiers.HasZs) != 0)
                return true;
            else
                return false;
        }

        static bool HasM(int iType)
        {
            if ((iType & (int)ShapeModifiers.HasMs) != 0)
                return true;
            else
                return false;
        }

        static bool HasCurve(int iType)
        {
            if ((iType & (int)ShapeModifiers.HasCurves) != 0)
                return true;
            else
                return false;
        }

        static bool HasID(int iType)
        {
            if ((iType & (int)ShapeModifiers.HasIDs) != 0)
                return true;
            else
                return false;
        }

        static bool HasNormal(int iType)
        {
            if ((iType & (int)ShapeModifiers.HasNormals) != 0)
                return true;
            else
                return false;
        }

        static bool HasTexture(int iType)
        {
            if ((iType & (int)ShapeModifiers.HasTextures) != 0)
                return true;
            else
                return false;
        }

        static bool HasMaterail(int iType)
        {
            if ((iType & (int)ShapeModifiers.HasMaterials) != 0)
                return true;
            else
                return false;
        }

        public static PointZM BuildPoint(byte[] bytes)
        {
            int iType = BitConverter.ToInt32(bytes, 0);

            double x, y, z = double.NaN, m = double.NaN;
            int id = 0;
            x = BitConverter.ToDouble(bytes, 4);
            y = BitConverter.ToDouble(bytes, 12);

            int A = 20 + (!HasZ(iType) ? 0 : 8);
            int B = A + (!HasM(iType) ? 0 : 8);

            if (HasZ(iType))
                z = BitConverter.ToDouble(bytes, 20);
            if (HasM(iType))
                m = BitConverter.ToDouble(bytes, A);
            if (HasID(iType))
                id = BitConverter.ToInt32(bytes, B);

            return new PointZM(x, y, z, m, id);
        }

        public static MultipointZM BuildMultipoint(byte[] bytes)
        {
            int iType = BitConverter.ToInt32(bytes, 0);

            int numPoints;
            Envelope boundingBox;
            double zMin = double.NaN, zMax = double.NaN;
            double mMin = double.NaN, mMax = double.NaN;
            double[] ZArray = null;
            double[] MArray = null;
            Point[] points;
            int[] ids = null;

            boundingBox = BuildEnvelope(bytes, 4);
            numPoints = BitConverter.ToInt32(bytes, 36);
            points = BuildPoints(bytes, 40, numPoints);

            int C = 40 + (16 * numPoints);
            int D = C + (!HasZ(iType) ? 0 : 16 + (8 * numPoints));
            int E = D + (!HasM(iType) ? 0 : 16 + (8 * numPoints));
            
            if (HasZ(iType))
            {
                zMin = BitConverter.ToDouble(bytes, C);
                zMax = BitConverter.ToDouble(bytes, C + 8);
                ZArray = BuildDoubleArray(bytes, C + 16, numPoints);
            }
            if (HasM(iType))
            {
                mMin = BitConverter.ToDouble(bytes, D);
                mMax = BitConverter.ToDouble(bytes, D + 8);
                MArray = BuildDoubleArray(bytes, D + 16, numPoints);
            }
            if (HasID(iType))
            {
                ids = BuildInt32Array(bytes, E, numPoints);
            }

            return new MultipointZM(boundingBox, numPoints, points,
                zMin, zMax, ZArray, mMin, mMax, MArray, ids);
        }

        public static PolylineZM BuildPolyline(byte[] bytes)
        {
            int iType = BitConverter.ToInt32(bytes, 0);
            int numPoints, numParts;
            Envelope boundingBox;
            double zMin = double.NaN, zMax = double.NaN;
            double mMin = double.NaN, mMax = double.NaN;
            double[] ZArray = null;
            double[] MArray = null;
            Point[] points;
            int[] parts;
            int numCurves = 0;
            SegmentModifier[] segmentModifiers = null;
            int totalSegmentLength = 0;
            int[] ids = null;

            boundingBox = BuildEnvelope(bytes, 4);
            numParts = BitConverter.ToInt32(bytes, 36);
            numPoints = BitConverter.ToInt32(bytes, 40);
            parts = BuildInt32Array(bytes, 44, numParts);

            int F = 44 + (4 * numParts);
            int G = F + (16 * numPoints);
            int H = G + (!HasZ(iType) ? 0 : 16 + (8 * numPoints));
            int I = H + (!HasM(iType) ? 0 : 16 + (8 * numPoints));

            points = BuildPoints(bytes, F, numPoints);
            if (HasZ(iType))
            {
                zMin = BitConverter.ToDouble(bytes, G);
                zMax = BitConverter.ToDouble(bytes, G + 8);
                ZArray = BuildDoubleArray(bytes, G + 16, numPoints);
            }
            if (HasM(iType))
            {
                mMin = BitConverter.ToDouble(bytes, H);
                mMax = BitConverter.ToDouble(bytes, H + 8);
                MArray = BuildDoubleArray(bytes, H + 16, numPoints);
            }
            if (HasCurve(iType))
            {
                numCurves = BitConverter.ToInt32(bytes, I);
                segmentModifiers = BuildSegmentModifiers(bytes, I + 4, numCurves, out totalSegmentLength);
            }
            if (HasID(iType))
            {
                int J = I + (!HasCurve(iType) ? 0 : totalSegmentLength);
                ids = BuildInt32Array(bytes, J, numPoints);
            }
            return new PolylineZM(boundingBox, numParts, numPoints, parts, points,
                zMin, zMax, ZArray, mMin, mMax, MArray, numCurves, segmentModifiers, ids);
        }

        public static PolygonZM BuildPolygon(byte[] bytes)
        {
            int iType = BitConverter.ToInt32(bytes, 0);
            int numPoints, numParts;
            Envelope boundingBox;
            double zMin = double.NaN, zMax = double.NaN;
            double mMin = double.NaN, mMax = double.NaN;
            double[] ZArray = null;
            double[] MArray = null;
            Point[] points;
            int[] parts;
            int numCurves = 0;
            SegmentModifier[] segmentModifiers = null;
            int totalSegmentLength = 0;
            int[] ids = null;

            boundingBox = BuildEnvelope(bytes, 4);
            numParts = BitConverter.ToInt32(bytes, 36);
            numPoints = BitConverter.ToInt32(bytes, 40);
            parts = BuildInt32Array(bytes, 44, numParts);

            int F = 44 + (4 * numParts);
            int G = F + (16 * numPoints);
            int H = G + (!HasZ(iType) ? 0 : 16 + (8 * numPoints));
            int I = H + (!HasM(iType) ? 0 : 16 + (8 * numPoints));

            points = BuildPoints(bytes, F, numPoints);
            if (HasZ(iType))
            {
                zMin = BitConverter.ToDouble(bytes, G);
                zMax = BitConverter.ToDouble(bytes, G + 8);
                ZArray = BuildDoubleArray(bytes, G + 16, numPoints);
            }
            if (HasM(iType))
            {
                mMin = BitConverter.ToDouble(bytes, H);
                mMax = BitConverter.ToDouble(bytes, H + 8);
                MArray = BuildDoubleArray(bytes, H + 16, numPoints);
            }
            if (HasCurve(iType))
            {
                numCurves = BitConverter.ToInt32(bytes, I);
                segmentModifiers = BuildSegmentModifiers(bytes, I + 4, numCurves, out totalSegmentLength);
            }
            if (HasID(iType))
            {
                int J = I + (!HasCurve(iType) ? 0 : totalSegmentLength);
                ids = BuildInt32Array(bytes, J, numPoints);
            }
            return new PolygonZM(boundingBox, numParts, numPoints, parts, points,
                zMin, zMax, ZArray, mMin, mMax, MArray, numCurves, segmentModifiers, ids);
        }

        public static MultiPatchZM BuildMultiPatch(byte[] bytes)
        {
            int iType = BitConverter.ToInt32(bytes, 0);
            int numPoints, numParts;
            Envelope boundingBox;
            double zMin = double.NaN, zMax = double.NaN;
            double mMin = double.NaN, mMax = double.NaN;
            double[] ZArray = null;
            double[] MArray = null;
            Point[] points;
            int[] parts;
            int[] partTypes;
            int numMs = 0;
            int[] ids = null;
            int numIds = 0;
            int numNormals = 0;
            Normal[] normals = null;
            int numTex = 0;
            int texDim = 0;
            int[] texParts = null;
            float[,] texCoords = null;
            int numMaterials = 0;
            int texCompType = 0;
            int[] materialParts = null;
            Material[] materials = null;

            boundingBox = BuildEnvelope(bytes, 4);
            numParts = BitConverter.ToInt32(bytes, 36);
            numPoints = BitConverter.ToInt32(bytes, 40);
            parts = BuildInt32Array(bytes, 44, numParts);

            int K = 44 + (4 * numParts);
            int L = K + (4 * numParts);
            int M = L + (16 * numPoints);
            int N = M + 16 + (8 * numPoints);

            partTypes = BuildInt32Array(bytes, K, numParts);
            points = BuildPoints(bytes, L, numPoints);
            zMin = BitConverter.ToDouble(bytes, M);
            zMax = BitConverter.ToDouble(bytes, M + 8);
            ZArray = BuildDoubleArray(bytes, M + 16, numPoints);
            numMs = BitConverter.ToInt32(bytes, N);
            if (HasM(iType))
            {
                mMin = BitConverter.ToDouble(bytes, N + 4);
                mMax = BitConverter.ToDouble(bytes, N + 12);
                MArray = BuildDoubleArray(bytes, N + 20, numMs);
            }

            int O = N + ((!HasM(iType)) ? 4 : 20 + (8 * numMs));
            numIds = BitConverter.ToInt32(bytes, O);
            if (HasID(iType))
            {
                ids = BuildInt32Array(bytes, O + 4, numIds);
            }

            int P = O + ((!HasID(iType)) ? 4 : (4 + 4 * numIds));
            numNormals = BitConverter.ToInt32(bytes, P);
            if (HasNormal(iType))
            {
                normals = BuildNormals(bytes, P + 4, numNormals);
            }

            int Q = P + ((!HasNormal(iType)) ? 4 : (4 + 12 * numNormals));
            numTex = BitConverter.ToInt32(bytes, Q);
            if (HasTexture(iType))
            {
                texDim = BitConverter.ToInt32(bytes, Q + 4);
                texParts = BuildInt32Array(bytes, Q + 8, numParts);
                texCoords = BuildTexCoords(bytes, Q + 8 + (4 * numParts), texDim, numTex);
            }

            int R = Q + ((!HasTexture(iType)) ? 4 : (8 + (4 * numParts) + (4 * texDim + numTex)));
            numMaterials = BitConverter.ToInt32(bytes, R);
            if (HasMaterail(iType))
            {
                texCompType = BitConverter.ToInt32(bytes, R + 4);
                materialParts = BuildInt32Array(bytes, R + 8, numMaterials + 1);
                int S = R + (12 + 4*numMaterials);
                materials = BuildMaterials(bytes, S, materialParts, numMaterials);
            }

            return new MultiPatchZM(boundingBox, numParts, numPoints, parts, partTypes, points,
                zMin, zMax, ZArray, mMin, mMax, MArray, numIds, ids, numNormals, normals,
                numTex, texDim, texParts, texCoords,
                numMaterials, texCompType, materialParts, materials);
        }

        public static ShapeObject BuildObject(byte[] bytes)
        {
            ShapeObject shp = new ShapeObject();
            if (bytes == null)
                return shp;

            int iType = BitConverter.ToInt32(bytes, 0);
            if (IsPoint(iType))
            {
                shp.Type = GeometryType.Point;
                shp.Data = BuildPoint(bytes);
            }
            else if (IsMultipoint(iType))
            {
                shp.Type = GeometryType.Multipoint;
                shp.Data = BuildMultipoint(bytes);
            }
            else if (IsPolyline(iType))
            {
                shp.Type = GeometryType.Polyline;
                shp.Data = BuildPolyline(bytes);
            }
            else if (IsPolygon(iType))
            {
                shp.Type = GeometryType.Polygon;
                shp.Data = BuildPolygon(bytes);
            }
            else if (IsMultiPatch(iType))
            {
                shp.Type = GeometryType.MultiPatch;
                shp.Data = BuildMultiPatch(bytes);
            }
            return shp;
        }
    }
}
