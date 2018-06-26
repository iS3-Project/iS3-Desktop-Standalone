using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using IS3.Core;
using IS3.Core.Serialization;
using IS3.Geology.Serialization;

namespace IS3.Geology
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

    public class Stratum : DGObject
    {
        public string GeologyAge { get; set; }
        public string FormationType { get; set; }
        public string Compaction { get; set; }
        public string ElevationRange { get; set; }
        public string ThicknessRange { get; set; }

        public Stratum()
        { }

        public Stratum(DataRow rawData)
            :base(rawData)
        { }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            GeologyDGObjectLoader loader2 = new GeologyDGObjectLoader(dbContext);
            bool success = loader2.LoadStrata(objs);
            return success;
        }

        public override string ToString()
        {
            string str = base.ToString();

            string str1 = string.Format(
                ", GeoAge={0}, Formation={1}, Compaction={2}, ElevationRange={3}, ThicknessRange={4}",
                GeologyAge, FormationType, Compaction, ElevationRange, ThicknessRange);
            str += str1;

            return str;
        }

    }
}