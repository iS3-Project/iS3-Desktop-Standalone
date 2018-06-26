using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Windows.Markup;

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
    //     Project location definition.
    // Remarks:
    //     It includes (X, Y), ID, definition file, descrition.
    //     If the Default is set to true, it will be load automatically.
    //
    public class ProjectLocation
    {
        public ProjectLocation() { Default = false; }

        public double X { get; set; }
        public double Y { get; set; }
        public string ID { get; set; }
        public string DefinitionFile { get; set; }
        public string Description { get; set; }
        public bool Default { get; set; }
    }

    // Summary:
    //     A list of project location defintions.
    //
    public class ProjectList
    {
        public ProjectList()
        {
            Locations = new List<ProjectLocation>();
        }

        public double XMax { get; set; }
        public double XMin { get; set; }
        public double YMax { get; set; }
        public double YMin { get; set; }
        public List<ProjectLocation> Locations { get; set; }
        public bool UseGeographicMap { get; set; }

    }

    public enum ProjectType { DeepExcavation, ShieldTunnel, NATMTunnel }

    // Summary:
    //     Project basic information: ID and project type
    //
    [DataContract]
    public class ProjectInformation
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public ProjectType ProjectType { get; set; }
    }

    // Summary:
    //     Deep excavation project information
    //
    [DataContract]
    public class DeepExcavationProjectInformation : ProjectInformation
    {
        [DataMember]
        public double ExcavationDepth { get; set; }
    }

    // Summary:
    //     Shield tunnel project information
    //
    [DataContract]
    public class ShieldTunnelProjectInformation : ProjectInformation
    {
        [DataMember]
        public double Length { get; set; }
        [DataMember]
        public double OuterDiameter { get; set; }
        [DataMember]
        public double InnerDiamter { get; set; }
    }

    // Summary:
    //      Project definition
    // Remarks:
    //      ID:
    //           id of the project
    //      ProjectTitle:
    //           project title, shown as the head of the main frame
    //      DefaultMapID:
    //           default map id
    //      LocalFilePath:
    //           local database file path
    //      LocalTilePath:
    //           local ArcGIS tiled package (.TPK) file path
    //      LocalDatabaseName:
    //           local database name
    //      DataServiceUrl:
    //           data service Url
    //      GeometryServiceUrl:
    //           geometry service Url
    //      SubProjectInfos:
    //           a list of sub-project infos.
    //      EngineeringMaps:
    //           a list of engineering map definition
    //
    [DataContract]
    [KnownType(typeof(DeepExcavationProjectInformation))]
    [KnownType(typeof(ShieldTunnelProjectInformation))]
    public class ProjectDefinition
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string ProjectTitle { get; set; }
        [DataMember]
        public string DefaultMapID { get; set; }
        [DataMember]
        public string LocalFilePath { get; set; }
        [DataMember]
        public string LocalTilePath { get; set; }
        [DataMember]
        public string LocalDatabaseName { get; set; }
        [DataMember]
        public string DataServiceUrl { get; set; }
        [DataMember]
        public string GeometryServiceUrl { get; set; }

        [DataMember]
        public List<ProjectInformation> SubProjectInfos { get; set; }
        [DataMember]
        public List<EngineeringMap> EngineeringMaps { get; set; }

        public ProjectDefinition()
        {
            SubProjectInfos = new List<ProjectInformation>();
            EngineeringMaps = new List<EngineeringMap>();
        }

        public ProjectInformation GetSubProjectInformation(string subProjectID)
        {
            foreach (ProjectInformation pi in SubProjectInfos)
            {
                if (pi.ID == subProjectID)
                    return pi;
            }
            return null;
        }
        public EngineeringMap GetEMap(string mapID)
        {
            foreach (EngineeringMap eMap in EngineeringMaps)
            {
                if (eMap.MapID == mapID)
                    return eMap;
            }
            return null;
        }

        public static ProjectDefinition Load(XElement root)
        {
            XNamespace is3 = "clr-namespace:IS3.Core;assembly=IS3.Core";
            XElement node = root.Element(is3 + "ProjectDefinition");
            object obj = null;
            if (node != null)
                obj = XamlReader.Parse(node.ToString());

            ProjectDefinition prjDef = (ProjectDefinition)obj;

            return (ProjectDefinition)obj;
        }

        public override string ToString()
        {
            string str = string.Format(
                "Project definition: ID={0}, ProjectTitle={1}, DefaultMapID={2}, LocalDatabaseName={3}",
                ID, ProjectTitle, DefaultMapID, LocalDatabaseName);
            return str;
        }
    }
}
