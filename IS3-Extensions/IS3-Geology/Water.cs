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
    public class Water : DGObject
    {
        public Water()
        { }

        public Water(DataRow rawData)
            :base(rawData)
        { }
    }

    public class RiverWater : Water
    {
        public string ObservationLocation { get; set; }
        public double? HighestTidalLevel { get; set; }
        public double? LowestTidalLevel { get; set; }
        public double? AvHighTidalLevel { get; set; }
        public double? AvLowTidalLevel { get; set; }
        public double? AvTidalRange { get; set; }
        public DateTime? HighestTidalLevelDate { get; set; }
        public DateTime? LowestTidalLevelDate { get; set; }
        public string DurationOfRise { get; set; }
        public string DurationOfFall { get; set; }

        public RiverWater()
        { }

        public RiverWater(DataRow rawData)
            :base(rawData)
        { }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            GeologyDGObjectLoader loader2 = new GeologyDGObjectLoader(dbContext);
            bool success = loader2.LoadRiverWaters(objs);
            return success;
        }
    }

    public class PhreaticWater : Water
    {
        public string SiteName { get; set; }
        public double? AvBuriedDepth { get; set; }
        public double? AvElevation { get; set; }

        public override string key
        {
            get
            {
                return SiteName;
            }
        }

        public PhreaticWater()
        { }

        public PhreaticWater(DataRow rawData)
            : base(rawData)
        { }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            GeologyDGObjectLoader loader2 = new GeologyDGObjectLoader(dbContext);
            bool success = loader2.LoadPhreaticWaters(objs);
            return success;
        }
    }

    public class ConfinedWater : Water
    {
        public string BoreholeName { get; set; }
        public string SiteName { get; set; }
        public double? TopElevation { get; set; }
        public double? ObservationDepth { get; set; }
        public string StratumName { get; set; }
        public int? Layer { get; set; }
        public double? WaterTable { get; set; }
        public DateTime? ObservationDate { get; set; }

        // Use BoreholeName:ID as the key
        public override string key
        {
            get
            {
                return BoreholeName + ":" + id.ToString();
            }
        }

        public ConfinedWater()
        { }

        public ConfinedWater(DataRow rawData)
            : base(rawData)
        { }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            GeologyDGObjectLoader loader2 = new GeologyDGObjectLoader(dbContext);
            bool success = loader2.LoadConfinedWaters(objs);
            return success;
        }
    }

    public class WaterProperty : DGObject
    {
        public string BoreholeName { get; set; }
        public double? Cl { get; set; }
        public double? SO4 { get; set; }
        public double? Mg { get; set; }
        public double? NH { get; set; }
        public double? pH { get; set; }
        public double? CO2 { get; set; }
        public string Corrosion { get; set; }

        public override string key
        {
            get
            {
                return BoreholeName;
            }
        }

        public WaterProperty()
        { }

        public WaterProperty(DataRow rawData)
            : base(rawData)
        { }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            GeologyDGObjectLoader loader2 = new GeologyDGObjectLoader(dbContext);
            bool success = loader2.LoadWaterProperties(objs);
            return success;
        }
    }
}