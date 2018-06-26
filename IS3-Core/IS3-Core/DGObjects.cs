using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Data;

using IS3.Core.Serialization;

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
    //     objects definition
    // Remarks:
    //     Object non-graphic infos are usually stored in tables of a database.
    //     Object graphic infos are usually stored in GIS layers.
    //     These infos are defined in objects definition.
    //
    public class DGObjectsDefinition
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public bool HasGeometry { get; set; }
        public string GISLayerName { get; set; }
        public string TableNameSQL { get; set; }
        public string DefNamesSQL { get; set; }
        public string ConditionSQL { get; set; }
        public string OrderSQL { get; set; }
        public bool Has3D { get; set; }
        public string Layer3DName { get; set; }

        public static DGObjectsDefinition LoadDefinition(XElement root)
        {
            DGObjectsDefinition def = new DGObjectsDefinition();
            XAttribute attr;

            def.Type = root.Name.ToString();

            attr = root.Attribute("Name");
            if (attr != null)
                def.Name = (string)attr;
            attr = root.Attribute("HasGeometry");
            if (attr != null)
                def.HasGeometry = (bool)attr;
            attr = root.Attribute("GISLayerName");
            if (attr != null)
                def.GISLayerName = (string)attr;

            attr = root.Attribute("Has3D");
            if (attr != null)
                def.Has3D = (bool)attr;

            attr = root.Attribute("Layer3DName");
            if (attr != null)
                def.Layer3DName = (string)attr;


            attr = root.Attribute("TableNameSQL");
            if (attr != null)
                def.TableNameSQL = (string)attr;
            attr = root.Attribute("DefNamesSQL");
            if (attr != null)
                def.DefNamesSQL = (string)attr;
            attr = root.Attribute("ConditionSQL");
            if (attr != null)
                def.ConditionSQL = (string)attr;
            attr = root.Attribute("OrderSQL");
            if (attr != null)
                def.OrderSQL = (string)attr;

            return def;
        }

        public override string ToString()
        {
            string str = string.Format(
                "Object definition: Type={0}, Name={1}, HasGeometry={2}, GISLayerName={3}," +
                "TableNameSQL={4}, DefNameSQL={5}, ConditionSQL={6}, OrderSQL={7}",
                Type, Name, HasGeometry, GISLayerName,
                TableNameSQL, DefNamesSQL, ConditionSQL, OrderSQL);
            return str;
        }
    }

    // Summary:
    //     DGObjects: a collection of DGObject
    // Remarks:
    //   1.DGObjects is typically a collection of DGObject which is loaded from database.
    //     DGObjects must have a defintion (DGObjectsDefinition) which defines the 
    //     table name in database, GIS layer name, etc.
    //   2.The objects in a DGObjects should be the same type.
    //   3.DGObjects is different from a list of DGObject.
    //   4.The DGObjects has the following index for fast visit:
    //     (1) name of DGObject -> DGObject
    //     (2) id of DGObject -> DGObject
    //     (3) DataRow of DataTable -> DGObject
    //     (4) DGObject -> DataRow of DataTable
    //
    public class DGObjects
    {
        // Summary:
        //     Object dictionay
        // Remarks:
        //     Object name is used as the key
        protected Dictionary<string, DGObject> _objs;
        // Summary:
        //     id index to object
        protected Dictionary<int, DGObject> _id2Obj { get; set; }

        // Parent of the objects
        public Domain parent { get; set; }
        // Objects definition
        public DGObjectsDefinition definition { get; set; }
        // Raw DataSet that is readed from database
        public DataSet rawDataSet { get; set; }

        // Summary:
        //     RowView index to object and vice versa.
        // Remarks:
        //     DataRowVew is used as an index for datagrid selection,
        //     where a selected row can be mapped to object quickly. 
        //     Similarly, object is used as an index to DataRowView.
        public Dictionary<DataRow, DGObject> rowView2Obj { get; set; }
        public Dictionary<DGObject, DataRow> obj2RowView { get; set; }

        // Summary:
        //     Constructors
        public DGObjects(DGObjectsDefinition def)
        {
            _objs = new Dictionary<string, DGObject>();

            definition = def;
        }

        // Summary:
        //     Get object by a key
        public DGObject this[string key]
        {
            get { return _objs[key]; }
            set { _objs[key] = value; value.parent = this; }
        }

        // Summary:
        //     Get object by an id
        public DGObject this[int id]
        {
            get { return _id2Obj[id]; }
        }

        public Dictionary<string, DGObject>.ValueCollection values
        {
            get { return _objs.Values; }
        }

        public bool containsKey(string key)
        {
            return _objs.ContainsKey(key);
        }

        public bool containsKey(int objID)
        {
            return _id2Obj.ContainsKey(objID);
        }

        public int count
        {
            get { return _objs.Count; }
        }

        public override string ToString()
        {
            String str = string.Format("Objs: Type={0}, Count={1}, ",
                definition==null? null : definition.Type, _objs.Count);

            ICollection<string> keys = _objs.Keys;
            string strKeys = "Keys=";
            foreach (string key in keys)
            {
                strKeys += key + ",";
            }

            str += strKeys;
            return str;
        }

        public bool load(DbContext dbContext)
        {
            rawDataSet = new DataSet(definition.Type);

            DGObject objHelper =
                ObjectHelper.CreateDGObjectFromSubclassName(definition.Type);
            bool success = objHelper.LoadObjs(this, dbContext);
            buildIDIndex();
            buildRowViewIndex();

            return success;
        }

        // Summary:
        //     Build ID index to object
        protected void buildIDIndex()
        {
            _id2Obj = new Dictionary<int, DGObject>();
            foreach (DGObject obj in _objs.Values)
            {
                int id = obj.id;
                _id2Obj[id] = obj;
            }
        }

        // Summary:
        //     Build RowView index to object and vice versa.
        protected void buildRowViewIndex()
        {
            if (rawDataSet == null || rawDataSet.Tables.Count == 0)
                return;

            DataTable dt = rawDataSet.Tables[0];
            rowView2Obj = new Dictionary<DataRow, DGObject>();
            foreach (DGObject obj in _objs.Values)
                rowView2Obj[obj.rawData] = obj;

            obj2RowView = new Dictionary<DGObject, DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                if (rowView2Obj.ContainsKey(dr))
                {
                    // get the object using the DataRow as an index
                    //
                    DGObject obj = rowView2Obj[dr];
                    obj2RowView[obj] = dr;
                }
            }
        }
    }
}
