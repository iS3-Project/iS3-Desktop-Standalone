using System;
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

    public delegate void ConsoleDelegate(string str);

    public enum ErrorReportTarget {DebugConsole, MessageBox, DelegateConsole};

    // Summary:
    //     Error report class
    // Remarks:
    //     Available error report target include:
    //          DebugConsole
    //          MessageBox
    //          DelegateConsole: user defined function
    //
    public static class ErrorReport
    {
        public static ErrorReportTarget target = ErrorReportTarget.MessageBox;
        public static ConsoleDelegate consoleDelegate = null;

        public static void Report(string error)
        {
            if (target == ErrorReportTarget.DebugConsole)
                Console.Write(error);
            else if (target == ErrorReportTarget.MessageBox)
                MessageBox.Show(error, "Error");
            else if (target == ErrorReportTarget.DelegateConsole)
            {
                if (consoleDelegate != null)
                    consoleDelegate(error);
            }
        }
    }
}
