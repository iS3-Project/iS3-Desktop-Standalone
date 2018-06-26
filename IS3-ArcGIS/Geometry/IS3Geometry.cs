using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using Esri.ArcGISRuntime.Geometry;

using IS3.Core.Geometry;

namespace IS3.ArcGIS.Geometry
{
    #region Copyright Notice
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************
    #endregion
    public class IS3SpatialReference : SpatialReference, ISpatialReference
    {
        // Constructors
        public IS3SpatialReference(int wkid)
            : base(wkid)
        { }
        public IS3SpatialReference(string wktext)
            : base(wktext)
        { }
        public IS3SpatialReference(int wkid, int verticalWkid)
            : base(wkid, verticalWkid)
        { }
    }

    public class IS3MapPoint : MapPoint, IMapPoint
    {
        // Constructors
        public IS3MapPoint(double x, double y)
            : base(x, y)
        { }
        public IS3MapPoint(double x, double y, ISpatialReference spatialReference)
            : base(x, y, spatialReference as SpatialReference)
        { }
        public IS3MapPoint(double x, double y, double z)
            : base(x, y, z)
        { }
        public IS3MapPoint(double x, double y, double z, ISpatialReference spatialReference)
            : base(x, y, z, spatialReference as SpatialReference)
        { }
        public IS3MapPoint(double x, double y, double z, double m)
            : base(x, y, z, m)
        { }
        public IS3MapPoint(double x, double y, double z, double m, ISpatialReference spatialReference)
            : base(x, y, z, m, spatialReference as SpatialReference)
        { }
        public IS3MapPoint(MapPoint other)
            : base(other.X, other.Y, other.Z, other.M, other.SpatialReference)
        { }

        // Implementation for IGeometry interfaces
        public new IEnvelope Extent
        {
            get { return new IS3Envelope(base.Extent); }
        }

        public new Core.Geometry.GeometryType GeometryType
        {
            get { return (Core.Geometry.GeometryType)base.GeometryType; }
        }
    }

    public class IS3PointCollection: List<IS3MapPoint>, IPointCollection
    {
        public new IEnumerator<IMapPoint> GetEnumerator()
        {
            return base.GetEnumerator();
        }
        public void Add(IMapPoint item)
        {
            base.Add(item as IS3MapPoint);
        }

        public new IMapPoint this[int index]
        {
            get { return base[index]; }
            set { base[index] = value as IS3MapPoint; }
        }
    }

    public class IS3Envelope : Envelope, IEnvelope
    {
        // Constructors
        public IS3Envelope(double x1, double y1, double x2, double y2)
            : base(x1, y1, x2, y2)
        { }
        public IS3Envelope(double x1, double y1, double x2, double y2, ISpatialReference spatialReference)
            : base(x1, y1, x2, y2, spatialReference as SpatialReference)
        { }
        public IS3Envelope(Envelope other)
            : base(other.XMin, other.YMin, other.XMax, other.YMax, other.SpatialReference)
        { }

        // Implementation for IGeometry interfaces
        public new IEnvelope Extent
        {
            get { return new IS3Envelope(base.Extent); }
        }

        public new Core.Geometry.GeometryType GeometryType
        {
            get { return (Core.Geometry.GeometryType)base.GeometryType; }
        }

        public new IMapPoint GetCenter()
        {
            MapPoint p = base.GetCenter();
            return new IS3MapPoint(p.X, p.Y, p.Z);
        }

        public IEnvelope Intersection(IEnvelope other)
        {
            Envelope e = base.Intersection(other as Envelope);
            return new IS3Envelope(e);
        }

        public bool Intersects(IEnvelope other)
        {
            return base.Intersects(other as Envelope);
        }

        public IEnvelope Union(IEnvelope other)
        {
            Envelope e = base.Union(other as Envelope);
            return new IS3Envelope(e);
        }
    }

    public class IS3Polyline: Polyline, IPolyline
    {
        // Constructor
        // Note: The parameter part should be List<IS3MapPoint>
        //       because it contains both IEnumerable<IMapPoint> 
        //       and IEnumerable<MapPoint> interfaces.
        //       If you see runtime errors, see the following test code.
        //
        // Test code #1 (Pass)
        //   List<IS3MapPoint> list1 = new List<IS3MapPoint>();
        //   IEnumerable<IMapPoint> iter11 = list1;    // compiler pass and run OK
        //   IEnumerable<MapPoint> iter12 = list1;     // compiler pass and run OK
        //   iter12 = (IEnumerable<MapPoint>)iter11;   // compiler pass and run OK
        //   iter11 = (IEnumerable<IMapPoint>)iter12;  // compiler pass and run OK
        //
        // Test code #2 (Fail)
        //   List<MapPoint> list2 = new List<MapPoint>();
        //   IEnumerable<IMapPoint> iter2 = list2;    // compiler not pass
        //   iter2 = (IEnumerable<IMapPoint>)list2;   // compiler pass but run fail
        //
        // Test code #3 (Fail)
        //   List<IMapPoint> list3 = new List<IMapPoint>();
        //   IEnumerable<IMapPoint> iter31 = list3;     // compiler pass and run OK
        //   IEnumerable<MapPoint> iter32 = list3;      // compiler fail
        //   iter32 = (IEnumerable<MapPoint>)iter31;    // compiler pass but run fail
        //   
        public IS3Polyline(IPointCollection part)
            : base((IEnumerable<MapPoint>)part)
        { }
        public IS3Polyline(Polyline other)
            : base(other.Parts, other.SpatialReference)
        { }

        // Implementation for IGeometry interfaces
        public new IEnvelope Extent
        {
            get { return new IS3Envelope(base.Extent); }
        }

        public new Core.Geometry.GeometryType GeometryType
        {
            get { return (Core.Geometry.GeometryType)base.GeometryType; }
        }
    
        public IPointCollection GetPoints()
        {
            if (IsEmpty)
                return null;
            else
            {
                IEnumerable<MapPoint> part = Parts[0].GetPoints();
                IS3PointCollection pts = new IS3PointCollection();
                foreach (MapPoint p in part)
                {
                    pts.Add(new IS3MapPoint(p));
                }
                return pts;
            }
        }
    }

    public class IS3Polygon : Polygon, IPolygon
    {
        // Constructor
        // Note: Please refer to IS3Polyline constructor for more instructions.
        public IS3Polygon(IPointCollection part)
            : base ((IEnumerable<MapPoint>)part)
        { }
        public IS3Polygon(Polygon other)
            : base(other.Parts, other.SpatialReference)
        { }

        // Implementation for IGeometry interfaces
        public new IEnvelope Extent
        {
            get { return new IS3Envelope(base.Extent); }
        }

        public new Core.Geometry.GeometryType GeometryType
        {
            get { return (Core.Geometry.GeometryType)base.GeometryType; }
        }

        public IPointCollection GetPoints()
        {
            if (IsEmpty)
                return null;
            else
            {
                IEnumerable<MapPoint> part = Parts[0].GetPoints();
                IS3PointCollection pts = new IS3PointCollection();
                foreach (MapPoint p in part)
                {
                    pts.Add(new IS3MapPoint(p));
                }
                return pts;
            }
        }

    }
}
