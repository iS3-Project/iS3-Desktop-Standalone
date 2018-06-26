using System;
using System.Collections.Generic;
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
    //     IMainframe: interfaces for main UI frame class
    // Remarks:
    //     (1) IMainframe is in the core of the MVC design pattern.
    //         It is the controller, so it knows both the model
    //         (Project) and the view (IView).
    //     (2) IMainframe consists of a collection of IView.
    //
    public interface IMainFrame
    {
        // Summary:
        //     prj: the model
        Project prj { get; }

        // Summary:
        //     a collection of views
        IEnumerable<IView> views { get; }

        // Summary
        //     active view
        //
        IView activeView { get; set; }

        // Summary:
        //     Add a view to the mainframe
        Task<IView> addView(EngineeringMap eMap, bool canClose);

        // Summary:
        //     Write a string to console.
        // Remarks:
        //     This function will 'halt' python input.
        //     Call RunStatements("") to return to python prompt.
        void output(string str);

        // Summary:
        //     Run statements in python console.
        // Remarks:
        //     A run statements helps return to python prompt.
        //     Note the statements are runned not in the current thread.
        void runStatements(string statements);

        // Summary:
        //     Run python scripts.
        // Remarks:
        //     The python scripts are runned in the current thread.
        void runPythonScripts(string file);

        // Summary:
        //     Object selection changed event trigger
        // Remarks:
        //     When user selects objects in the views, data grid, etc.,
        //     this event will be triggered.
        event EventHandler<ObjSelectionChangedEventArgs>
            objSelectionChangedTrigger;

        // Summary:
        //     Project loaded event
        // Remarks:
        //     When project data is loaded, this event will be triggered.
        event EventHandler projectLoaded;
    }

}
