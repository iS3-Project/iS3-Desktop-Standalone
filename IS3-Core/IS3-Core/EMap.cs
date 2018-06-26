using System.Collections.Generic;
using System.Runtime.Serialization;

using IS3.Core.Geometry;

namespace IS3.Core
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

    public enum EngineeringMapType { FootPrintMap, GeneralProfileMap, Map3D }

    public enum ProjectionType { Auto, CrossSection, LongitudinalSection }

    // Summary:
    //     GIS map definition.
    // Remarks:
    //     MapID:
    //          Id of the map.
    //     LocalTileFileName1:
    //          ArcGIS local tiled package file #1 (.TPK)
    //     LocalTileFileName2:
    //          ArcGIS local tiled package file #2 (.TPK)
    //     LocalMapFileName:
    //          ArcGIS local map package file (.MPK)
    //     LocalGeoDbFileName:
    //          ArcGIS local geodatabase file (.geodatabase)
    //     MapUrl:
    //          url of the map service
    //     XMin,YMin,XMax,YMax:
    //          initial extent of the map
    //     MinimumResolution:
    //          minimum resolution of the map
    //
    [DataContract]
    public class GISMap
    {
        [DataMember]
        public string MapID { get; set; }
        [DataMember]
        public string LocalTileFileName1 { get; set; }
        [DataMember]
        public string LocalTileFileName2 { get; set; }
        [DataMember]
        public string LocalMapFileName { get; set; }
        [DataMember]
        public string LocalGeoDbFileName { get; set; }
        [DataMember]
        public string MapUrl { get; set; }
        [DataMember]
        public double XMax { get; set; }
        [DataMember]
        public double XMin { get; set; }
        [DataMember]
        public double YMax { get; set; }
        [DataMember]
        public double YMin { get; set; }
        [DataMember]
        public double MinimumResolution { get; set; }
        [DataMember]
        public double MapRotation { get; set; }

        public GISMap() {}
        public GISMap(string mapID)
        {
            MapID = mapID;
        }
        public GISMap(string mapID, double xmin, double ymin,
            double xmax, double ymax, double minRes)
        {
            MapID = mapID;
            XMin = xmin;
            YMin = ymin;
            XMax = xmax;
            YMax = ymax;
            MinimumResolution = minRes;
        }

        public void CopyFrom(GISMap src)
        {
            MapID = src.MapID;
            LocalTileFileName1 = src.LocalTileFileName1;
            LocalTileFileName2 = src.LocalTileFileName2;
            LocalMapFileName = src.LocalMapFileName;
            LocalGeoDbFileName = src.LocalGeoDbFileName;
            MapUrl = src.MapUrl;
            XMax = src.XMax;
            XMin = src.XMin;
            YMax = src.YMax;
            YMin = src.YMin;
            MinimumResolution = src.MinimumResolution;
            MapRotation = src.MapRotation;
        }
    }


    // Summary:
    //     Engineering map definition.
    // Remarks:
    //      MapType:
    //               type of map, such as plan map, profile map or 3D map
    //      Calibrated, Scale, ScaleX, ScaleY, ScaleZ:
    //               When the engineering map is put onto a world map,
    //               there will be some distortions, these five variables
    //               are used to try to mitigate the scale distortions.
    //               Note that this is not a very good solution. Don't 
    //               reply on it.
    //      LocalGDbLayersDef: 
    //               local geodatabase layer definition,
    //               see LayerDef for more info.
    //
    [DataContract]
    public class EngineeringMap : GISMap
    {
        [DataMember]
        public EngineeringMapType MapType { get; set; }

        // Scale factor when the EngineeringMap is merged with world map
        [DataMember]
        public bool Calibrated { get; set; }
        [DataMember]
        public double Scale { get; set; }
        [DataMember]
        public double ScaleX { get; set; }
        [DataMember]
        public double ScaleY { get; set; }

        // ScaleZ for maps that are not footprint map.
        [DataMember]
        public double ScaleZ { get; set; }

        // Summary:
        //     Local Layers in a geodatabase
        public List<LayerDef> LocalGdbLayersDef { get; set; }

        // The profile line for EngineeringMapType.GeneralProfileMap
        public IPolyline profileLine;

        public EngineeringMap()
        {
            LocalGdbLayersDef = new List<LayerDef>();
            Calibrated = false;
            Scale = 1.0;
            ScaleX = 1.0;
            ScaleY = 1.0;
            ScaleZ = 1.0;
        }
        public EngineeringMap(string mapID)
            : base(mapID)
        {
            LocalGdbLayersDef = new List<LayerDef>();
            Calibrated = false;
            Scale = 1.0;
            ScaleX = 1.0;
            ScaleY = 1.0;
            ScaleZ = 1.0;
        }
        public EngineeringMap(string mapID, double xmin, double ymin,
            double xmax, double ymax, double minRes)
            : base(mapID, xmin, ymin, xmax, ymax, minRes)
        {
            LocalGdbLayersDef = new List<LayerDef>();
            Calibrated = false;
            Scale = 1.0;
            ScaleX = 1.0;
            ScaleY = 1.0;
            ScaleZ = 1.0;
        }
    }


}
