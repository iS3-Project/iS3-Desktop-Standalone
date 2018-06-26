using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Monitoring
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
    //     Monitoring Readering
    public class MonReading
    {
        // Summary:
        //     monitoring point name
        public string monPointName { get; set; }
        // Summary:
        //     reading date and time
        public DateTime time { get; set; }
        // Summary:
        //     reading component
        public string component { get; set; }
        // Summary:
        //    raw reading
        public string reading { get; set; }
        // Summary:
        //    reading value
        public double value { get; set; }
        // Summary:
        //    reading unit
        public string unit { get; set; }

        public override string ToString()
        {
            return string.Format(
                "pnt={0}, time={1}, comp={2}, read={3}, val={4}, unit={5}",
                monPointName, time, component, reading, value, unit);
        }
    }
}
