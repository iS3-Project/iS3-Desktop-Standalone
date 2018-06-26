using System.Reflection;
using System.IO;

using IS3.Core.Graphics;
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

    // Summary:
    //     iS3 runtime
    // Remarks:
    //     rootPath: path to executables
    //     dataPath: path to data files
    //     tilePath: path to tiled packages (.TPK)
    //     projPath: path to current project
    //     graphicEngine: graphics engine
    //     geometryEngine: gemoetry engine
    //
    public static class Runtime
    {
        public static string rootPath { get; set; }
        public static string dataPath { get; set; }
        public static string tilePath { get; set; }
        public static string projPath { get; set; }
        public static string configurationPath { get; set; }

        static IGraphicEngine _graphicEngine;
        static IGeometryEngine _geometryEngine;

        public static void initialize(string rootPath = null)
        {
            if (rootPath == null || rootPath.Length == 0)
            {
                string exeLocation = Assembly.GetExecutingAssembly().Location;
                string exePath = System.IO.Path.GetDirectoryName(exeLocation);
                DirectoryInfo di = System.IO.Directory.GetParent(exePath);
                rootPath = di.FullName;
            }
            string dataPath = rootPath + "\\Data";
            string tilePath = dataPath + "\\TPKs";
            Runtime.rootPath = rootPath;
            Runtime.dataPath = dataPath;
            Runtime.tilePath = tilePath;
            Runtime.configurationPath = rootPath + "\\IS3-Configuration\\DBconfig.xml";
        }

        public static void initializeEngines(IGraphicEngine graphicEngine,
            IGeometryEngine geometryEngine)
        {
            _graphicEngine = graphicEngine;
            _geometryEngine = geometryEngine;
        }
        public static IGraphicEngine graphicEngine 
        {
            get { return _graphicEngine; }
        }

        public static IGeometryEngine geometryEngine
        {
            get { return _geometryEngine; }
        }

        public static string status
        {
            get 
            {
                string str = string.Format(
                    "Runtime status: RootPath={0}, DataPath={1}, TilePath={2}, ProjPath={3}",
                    rootPath, dataPath, tilePath, projPath);
                return str;
            }
        }
    }
}
