using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using IS3.Core.Serialization;
using IS3.Core.Shape;

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

    // Engineering domain type
    public enum DomainType
    {
        Unknown,
        Geology,
        Surroundings,
        Structure,
        Design,
        Construction,
        Monitoring,
        Maintenance,
        Tdisease,
    };

    // Summary:
    //     Domain: stores engineering domain data.
    // Remarks:
    //     (1) Domain typically corresponds an engineering discipline,
    //         such as geology, structure, monitoring, etc.
    //     (2) Objects are first defined by DGObjectsDefinition, which
    //         gives the info about object table name in the database, etc.
    //     (3) Objects are then stored in objsContainer, which
    //         are indexed by object definition name.
    //
    public class Domain
    {
        // Domain parent
        public Project parent { get; set; }
        // Domain type
        public DomainType type { get; set; }
        // Domain name
        public string name { get; set; }
        // Root tree of the domain
        public Tree root { get; set; }

        // Summary:
        //     Objects definitions class
        // Remarks:
        //     Object definition name is used as the key.
        public Dictionary<string, DGObjectsDefinition> 
            objsDefinitions { get; set; }

        // Summary:
        //      Objects container class
        // Remarks:
        //   (1) Object definition name is used as the key.
        //   (2) To access objects with a layer ID, use Project.ObjsLayerIndex
        //   (3) To access object with object type, use the function 
        //       DGObjects getObjects(string objType)
        //   (4) To access objects through the displayed name of a tree,
        //       use Tree.ObjectsView (which is a DataView)
        public Dictionary<string, DGObjects> objsContainer { get; set; }

        public DGObjects this[string key]
        {
            get { return objsContainer[key]; }
        }

        public Domain(string domainName, DomainType domainType)
        {
            name = domainName;
            type = domainType;

            root = new Tree();
            root.Name = name;

            objsDefinitions = new Dictionary<string, DGObjectsDefinition>();
            objsContainer = new Dictionary<string, DGObjects>();
        }

        public override string ToString()
        {
            string str = string.Format(
                "Domain: Name={0}, Type={1}, ",
                name, type);

            ICollection<string> keys = objsContainer.Keys;
            string strKeys = "Keys=";
            foreach (string key in keys)
            {
                strKeys += key + ",";
            }
            str += strKeys;

            return str;
        }

        // Summary:
        //     Load a Domain from XML element
        public static Domain loadDefinition(XElement root)
        {
            string name = root.Attribute("Name").Value;
            string type = root.Attribute("Type").Value;
            DomainType domainType = (DomainType)Enum.Parse(typeof(DomainType), type);

            Domain domain = new Domain(name, domainType);

            // Load tree definition
            //
            XElement treeDefNode = root.Element("TreeDefinition");
            if (treeDefNode != null)
            {
                // only first node is loaded, other nodes are ignored.
                IEnumerable<XElement> nodes = treeDefNode.Elements();
                if (nodes.Count() > 0)
                {
                    XElement node = nodes.First();
                    domain.root = Tree.Element2Tree(node);
                }
            }

            // Load DGObjects definition
            //
            XElement objsDefNode = root.Element("ObjsDefinition");
            if (objsDefNode != null)
            {
                IEnumerable<XElement> nodes = objsDefNode.Elements();
                foreach (XElement node in nodes)
                {
                    DGObjectsDefinition def = DGObjectsDefinition.LoadDefinition(node);
                    domain.objsDefinitions.Add(def.Name, def);
                }
            }

            return domain;
        }

        // Summary:
        //     Load objects from database
        public bool loadObjects(string objDefName, DbContext dbContext)
        {
            if (parent == null)
                return false;

            DGObjectsDefinition def = objsDefinitions[objDefName];
            if (def == null)
                return false;

            DGObjects objs = new DGObjects(def);
            bool success = objs.load(dbContext);
            objs.parent = this;

            // Old objs will be replaced recently loaded objects.
            if (success)
                objsContainer[def.Name] = objs;

            return success;
        }

        // Summary:
        //     Load all objects from database
        public bool loadAllObjects(DbContext dbContext)
        {
            if (parent == null)
                return false;

            bool success = true;

            foreach (DGObjectsDefinition def in objsDefinitions.Values)
            {
                bool loaded = loadObjects(def.Name, dbContext);
                if (!loaded)
                    success = false;
            }

            return success;
        }

        // Summary:
        //     Get objects according to the specified object type
        // Remarks:
        //     If there is only a DGObjects with the specified object type,
        //     it will be returned directly.
        //
        //     If there are multiple DGObjects with the specified object type,
        //     then a new DGObjects is returned which will merged all the objects.
        //     In this situation, the index of the DGObjects are lost.
        public DGObjectsCollection getObjects(string objType)
        {
            IEnumerable<DGObjectsDefinition> defs = 
                objsDefinitions.Values.Where(x => x.Type == objType);
            if (defs == null || defs.Count() == 0)
                return null;

            DGObjectsCollection result = new DGObjectsCollection();
            foreach (DGObjectsDefinition def in defs)
            {
                if (objsContainer.ContainsKey(def.Name))
                {
                    DGObjects objs = objsContainer[def.Name];
                    result.Add(objs);
                }
            }
            return result;
        }

        // test code
        public void test()
        {
            var objs = objsContainer["AllTunnels"];
            var obj = objs.values.FirstOrDefault();
            var row = obj.rawData;
            var shape = row["Shape"];
            
            byte[] bytes = (byte[])shape;

            //int i = BitConverter.ToInt32(bytes, 0);
            //double x1 = BitConverter.ToDouble(bytes, 4);
            //double y1 = BitConverter.ToDouble(bytes, 12);
            //double x2 = BitConverter.ToDouble(bytes, 20);
            //double y2 = BitConverter.ToDouble(bytes, 28);

            ShapeObject shp = ShapeBuilder.BuildObject(bytes);

        }
    }
}
