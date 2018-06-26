using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    // Summary:
    //     Geometry engine: generate geometry objects
    public class IS3GeometryEngine : IGeometryEngine
    {
        // Summary:
        //     Create object with IMapPoint interface
        public IMapPoint newMapPoint(double x, double y)
        {
            return new IS3MapPoint(x, y);
        }
        public IMapPoint newMapPoint(double x, double y, ISpatialReference spatialReference)
        {
            return new IS3MapPoint(x, y, spatialReference);
        }
        public IMapPoint newMapPoint(double x, double y, double z)
        {
            return new IS3MapPoint(x, y, z);
        }
        public IMapPoint newMapPoint(double x, double y, double z, ISpatialReference spatialReference)
        {
            return new IS3MapPoint(x, y, z, spatialReference);
        }
        public IMapPoint newMapPoint(double x, double y, double z, double m)
        {
            return new IS3MapPoint(x, y, z, m);
        }
        public IMapPoint newMapPoint(double x, double y, double z, double m, ISpatialReference spatialReference)
        {
            return new IS3MapPoint(x, y, z, m, spatialReference);
        }

        // Summary:
        //     Create object with IPointCollection interface
        public IPointCollection newPointCollection()
        {
            return new IS3PointCollection();
        }

        // Summary:
        //     Create object with IEnvelope interface
        public IEnvelope newEnvelope(double x1, double y1, double x2, double y2)
        {
            return new IS3Envelope(x1, y1, x2, y2);
        }
        public IEnvelope newEnvelope(double x1, double y1, double x2, double y2, ISpatialReference spatialReference)
        {
            return new IS3Envelope(x1, y1, x2, y2, spatialReference);
        }

        // Create object with IPolyline interface
        // Note: Please refer to IS3Polyline constructor for more instructions.
        public IPolyline newPolyline(IPointCollection part)
        {
            return new IS3Polyline(part);
        }

        // Create object with IPolygon interface
        // Note: Please refer to IS3Polygon constructor for more instructions.
        public IPolygon newPolygon(IPointCollection part)
        {
            return new IS3Polygon(part);
        }

        public IGeometry fromJson(string json)
        {
            Esri.ArcGISRuntime.Geometry.Geometry g =
                Esri.ArcGISRuntime.Geometry.Geometry.FromJson(json);

            switch (g.GeometryType)
            {
                case Esri.ArcGISRuntime.Geometry.GeometryType.Point:
                    return new IS3MapPoint(g as MapPoint);
                case Esri.ArcGISRuntime.Geometry.GeometryType.Polyline:
                    return new IS3Polyline(g as Polyline);
                case Esri.ArcGISRuntime.Geometry.GeometryType.Polygon:
                    return new IS3Polygon(g as Polygon);
                case Esri.ArcGISRuntime.Geometry.GeometryType.Envelope:
                    return new IS3Envelope(g as Envelope);
                default:
                    throw new System.Exception("Not implemented geometry type.");
            }
        }

        // High level method: can't be used through IGeometryEngine interface.
        // Generate geometry from Esri.ArcGISRuntime.Geometry.Geometry.
        public static IGeometry fromGeometry(Esri.ArcGISRuntime.Geometry.Geometry g)
        {
            switch (g.GeometryType)
            {
                case Esri.ArcGISRuntime.Geometry.GeometryType.Point:
                    return new IS3MapPoint(g as MapPoint);
                case Esri.ArcGISRuntime.Geometry.GeometryType.Polyline:
                    return new IS3Polyline(g as Polyline);
                case Esri.ArcGISRuntime.Geometry.GeometryType.Polygon:
                    return new IS3Polygon(g as Polygon);
                case Esri.ArcGISRuntime.Geometry.GeometryType.Envelope:
                    return new IS3Envelope(g as Envelope);
                default:
                    throw new System.Exception("Not implemented geometry type.");
            }
        }
    }

}
