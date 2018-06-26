using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IS3.Core;

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
    //     This is the entry point for the extension
    public class EntryPoint : Extensions
    {
        public override string name() { return "iS3.Monitoring"; }
        public override string provider() { return "Tongji iS3 team"; }
        public override string version() { return "1.0"; }
        public override string init()
        {
            Globals.mainframe.projectLoaded += mainframe_projectLoaded;
            return base.init();
        }

        // Summary:
        //     Project loaded call back function
        // Remarks:
        //     When project data is loaded, the relationship between MonGroup 
        //     and MonPoint need to be established.
        void mainframe_projectLoaded(object sender, EventArgs e)
        {
            if (Globals.project == null)
                return;
            Domain domainMon = Globals.project.getDomain(DomainType.Monitoring);
            if (domainMon == null)
                return;

            foreach (var def in domainMon.objsDefinitions.Values)
            {
                if (def.Type == "MonGroup")
                {
                    DGObjects objs = domainMon.objsContainer[def.Name];
                    foreach (var obj in objs.values)
                    {
                        MonGroup group = obj as MonGroup;
                        if (group == null)
                            continue;
                        resumeGroup2PointRelationship(group, domainMon);
                    }
                }
            }
        }

        // Summary:
        //     Resume the relationship betwwen MonGroup and MonPoint.
        void resumeGroup2PointRelationship(MonGroup group, Domain domainMon)
        {
            if (group.monPntNames == null)
                return;

            foreach (string monPntName in group.monPntNames)
            {
                MonPoint monPnt = findMonPoint(monPntName, domainMon);
                if (monPnt == null)
                    continue;
                group.monPntDict[monPnt.name] = monPnt;
            }
        }

        // Summary:
        //     Find MonPoint by the given name.
        MonPoint findMonPoint(string monPntName, Domain domainMon)
        {
            foreach (DGObjects objs in domainMon.objsContainer.Values)
            {
                if (objs.definition.Type == "MonPoint")
                {
                    if (objs.containsKey(monPntName))
                        return objs[monPntName] as MonPoint;
                }
            }
            return null;
        }
    }
}
