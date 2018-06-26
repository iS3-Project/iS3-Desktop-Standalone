using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

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
    //     Object helper:
    //     (1) create object from class name
    //     (2) object data to string
    //     (3) collection data to string
    //
    public static class ObjectHelper
    {
        static IEnumerable<Type> subclasses = null;

        // create DGObject from give subclass name
        //
        public static DGObject CreateDGObjectFromSubclassName(string subclassName)
        {
            if (subclassName == "DGObject")
                return new DGObject();

            if (subclasses == null)
            {
                subclasses =
                        from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(DGObject))
                        select type;
            }

            // match the subclassName with full name at first
            Type t = subclasses.FirstOrDefault(x => x.FullName == subclassName);

            // if not found, match the subclassName with name
            if (t == null)
                t = subclasses.FirstOrDefault(x => x.Name == subclassName);

            if (t != null)
                return (DGObject)Activator.CreateInstance(t);
            else
                return null;
        }

        // get object value string, for example:
        //
        //      public class Foo
        //      {
        //          public int ID {get; set;}
        //          public string Name{get;set;}
        //      }
        //      Foo foo = new Foo() { ID = 1, Name = "Foo1" };
        //      
        // returned value of ObjectToString(obj) is:
        //      {ID=1, Name="Foo1"}
        //
        public static string ObjectToString(object obj, bool displayName)
        {
            string str = "";
            if (obj == null)
                return "null";

            // If the object type is one of the primitive types, string,
            // or DateTime, return the value immediately.
            Type objType = obj.GetType();
            if (objType.IsPrimitive || objType.IsEnum)
            {
                return obj.ToString();
            }
            else if (objType == typeof(string))
            {
                str += "\"";
                str += obj.ToString();
                str += "\"";
                return str;
            }
            else if (objType == typeof(DateTime))
            {
                return obj.ToString();
            }
            else if (objType.FullName.Contains("System.Windows"))
            {
                return obj.ToString();
            }

            // If the object type is a collection,
            // we need to display each of them.
            ICollection coll = obj as ICollection;
            if (coll != null)
            {
                str += "{";
                str += CollectionToString(obj, displayName);
                str += "}";
                return str;
            }

            // If the object is a user-defined class,
            // display the members.
            PropertyInfo[] propInfos = obj.GetType().GetProperties();
            if (propInfos != null && propInfos.Count() > 0)
            {
                str += "{";
                for (int i = 0; i < propInfos.Count(); ++i)
                {
                    // Property name
                    PropertyInfo info = propInfos[i];

                    // Do not show read-only property
                    if (info.CanWrite == false)
                        continue;

                    if (displayName)
                    {
                        str += info.Name;
                        str += "=";
                    }

                    // Property value
                    object value = info.GetValue(obj);
                    if (value == null)
                        str += "null";
                    else
                        str += ObjectToString(value, displayName);

                    if (i != propInfos.Count() - 1)
                        str += ",";
                }
                str += "}";
            }

            FieldInfo[] fldInfos = obj.GetType().GetFields();
            if (fldInfos != null && fldInfos.Count() > 0)
            {
                str += "{";
                for (int i = 0; i < fldInfos.Count(); ++i)
                {
                    // Field name
                    FieldInfo info = fldInfos[i];
                    if (displayName)
                    {
                        str += info.Name;
                        str += "=";
                    }

                    // Field value
                    object value = info.GetValue(obj);
                    if (value == null)
                        str += "null";
                    else
                        str += ObjectToString(value, displayName);

                    if (i != fldInfos.Count() - 1)
                        str += ",";
                }
                str += "}";
            }
            return str;
        }
        static string CollectionToString(object obj, bool displayName)
        {
            string str = "";
            ICollection coll = obj as ICollection;
            if (coll == null || coll.Count == 0)
                return str;

            IEnumerator iter = coll.GetEnumerator();
            while (iter.MoveNext())
            {
                object curr = iter.Current;
                str += ObjectToString(curr, displayName);
                str += ",";
            }

            str = str.Substring(0, str.Count() - 1);
            return str;
        }
    }

    // Attachment: attached file(s) for an entity
    public class Attachment
    {
        protected string _name;
        protected string _url;

        public string Name 
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Url 
        {
            get { return _url; }
            set { _url = value; }
        }
    }

    // SubCategory: definition for subclasses in a category
    // The digital object category can be classified into various sub-categories, e.g., 
    // Segment lining category can be divided according to their overburden soil depth: deep, medium, and shallow one.
    public class SubCategory : DGObject
    {
    }

    // TreeData: Tree, which is defined in DigitalGeotec.Common namespce, is a class for manipulate digital object category.
    // See [TreeDefinition] data table in the database for more information.
    // Note this class is mainly used for network transferring purposes.
    public class TreeData
    {
        protected string _xml;

        public string Xml 
        {
            get { return _xml; }
            set { _xml = value; }
        }
    }

}
