using System;
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
    //     This class is intended for defining user-defined extensions.
    // Remarks:
    //     (1) User defined extensions must inherit from Extensions.
    //     (2) The extension *.dll file should be placed in \bin\extensionss\.
    //         When the dll is loaded, the init() function 
    //         be called immediately.
    //
    public class Extensions
    {
        // Summary:
        //     Name, version and provide of the extension
        public virtual string name() { return "Unknown extension"; }
        public virtual string version() { return "Unknown"; }
        public virtual string provider() { return "Unknown provider"; }

        // Summary:
        //     Initialize the extension, called immediately after loaded.
        // Return value:
        //     A string that will be printed in output window.
        public virtual string init() 
        {
            string msg = String.Format("Loaded {0} by {1}, version {2}.\n",
                name(), provider(), version());
            return msg; 
        }
    }
}
