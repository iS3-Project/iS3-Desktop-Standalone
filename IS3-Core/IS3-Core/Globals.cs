using System.Threading;
using System.Windows;

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
    //     IS3 global variables
    // Remarks:
    //     project:
    //          project data
    //     mainframe:
    //          the UI main frame
    //     application:
    //          WPF application instance
    //     mainthreadID:
    //          main thread (UI thread) ID
    //          Please note that windows restricts the calls from other
    //          threads to the UI thread.
    //     isThreadUnsafe():
    //          Checks if the current thread is different from UI thread.
    //          This is very useful to diagnose problem such as Python functions
    //          call to functions in IS3 classes failed.
    //
    public static class Globals
    {
        public static Project project { get; set; }
        public static IMainFrame mainframe { get; set; }
        public static Application application { get; set; }
        public static int mainthreadID { get; set; }
        public static bool isThreadUnsafe()
        {
            int threadID = Thread.CurrentThread.ManagedThreadId;
            if (Globals.mainthreadID != threadID)
                return true;
            else
                return false;
        }
    }
}
