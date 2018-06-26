using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    //     A collection of DGObjects
    // Remarks:
    //     The DGObjects in the collection should be the same type.
    //
    public class DGObjectsCollection : List<DGObjects>
    {
        // Summary:
        //     Get object by an object ID
        public new DGObject this [int objID]
        {
            get
            {
                foreach (DGObjects objs in this)
                {
                    if (objs.containsKey(objID))
                        return objs[objID];
                }
                return null;
            }
        }

        // Summary:
        //     Get object by a object key
        public DGObject this [string key]
        {
            get
            {
                foreach (DGObjects objs in this)
                {
                    if (objs.containsKey(key))
                        return objs[key];
                }
                return null;
            }
        }

        // Summary:
        //     Merge the collection to a list of DGObject
        public List<DGObject> merge()
        {
            List<DGObject> result = new List<DGObject>();
            foreach (DGObjects objs in this)
            {
                foreach (DGObject obj in objs.values)
                {
                    result.Add(obj);
                }
            }
            return result;
        }
    }
}
