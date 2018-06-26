 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;

namespace IS3.Python
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

    public class PyManager
    {
        ScriptEngine _engine;

        public PyManager()
        {
            _engine = IronPython.Hosting.Python.CreateEngine();
            addIS3Path(_engine);
        }

        void addIS3Path(ScriptEngine engine)
        {
            ScriptScope scope = IronPython.Hosting.Python.GetSysModule(engine);
            dynamic path = scope.GetVariable("path");

            string exeLocation = Assembly.GetExecutingAssembly().Location;
            string exePath = System.IO.Path.GetDirectoryName(exeLocation);
            DirectoryInfo di = System.IO.Directory.GetParent(exePath);
            string rootPath = di.FullName;
            string is3PyPath = rootPath + "\\IS3Py";

            path.append(rootPath);
            path.append(exePath);
            path.append(is3PyPath);
        }

        public void loadPlugins(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.py",
                SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                run(file);
            }
        }

        public void run(string file)
        {
            try
            {
                ScriptSource script = _engine.CreateScriptSourceFromFile(file);
                CompiledCode code = script.Compile();
                script.Execute();
            }
            catch (SyntaxWarningException e)
            {
                string msg = "Syntax warning in \"{0}\"";
                showError(msg, Path.GetFileName(file), e);
            }
            catch (SystemExitException e)
            {
                string msg = "SystemExit in \"{0}\"";
                showError(msg, Path.GetFileName(file), e);
            }
            catch (Exception e)
            {
                string msg = "Error loading plugin \"{0}\"";
                showError(msg, Path.GetFileName(file), e);
            }
        }

        public void showError(string title, string name, Exception e)
        {
            string caption = String.Format(title, name);
            ExceptionOperations eo = _engine.GetService<ExceptionOperations>();
            string error = eo.FormatException(e);
            MessageBox.Show(error, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
