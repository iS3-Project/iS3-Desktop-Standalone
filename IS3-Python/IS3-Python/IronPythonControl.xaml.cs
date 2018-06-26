using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Xml;
using System.Reflection;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using IronPython.Hosting;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;


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
    public delegate void ConsoleInitializedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaction logic for IronPythonControl.xaml
    /// </summary>
    public partial class IronPythonControl : UserControl
    {
        public event EventHandler ConsoleInitialized;
        ConsoleOptions consoleOptionsProvider;
        string currentFileName;

        public IronPythonControl()
        {
            // Load our custom highlighting definition:
            IHighlightingDefinition pythonHighlighting;
            using (Stream s = typeof(IronPythonControl).Assembly.GetManifestResourceStream("IS3.Python.Resources.Python.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    pythonHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Python Highlighting", new string[] { ".cool" }, pythonHighlighting);

//            LoadHighlightDefinition();
            InitializeComponent();

            textEditor.SyntaxHighlighting = pythonHighlighting;
            textEditor.PreviewKeyDown += new KeyEventHandler(textEditor_PreviewKeyDown);
            consoleOptionsProvider = new ConsoleOptions(console.Pad);
            propertyGridComboBox.SelectedIndex = 0;
            expander.Expanded += new RoutedEventHandler(expander_Expanded);

            console.Host.ConsoleCreated += Host_ConsoleCreated;
            console.Pad.Control.WordWrap = true;
        }

        void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) RunStatements();
        }

        void RunStatements()
        {
            string statementsToRun = "";
            if (textEditor.TextArea.Selection.Length > 0)
                statementsToRun = textEditor.TextArea.Selection.GetText();
            else
                statementsToRun = textEditor.TextArea.Document.Text;
            console.Pad.Console.RunStatements(statementsToRun);
        }

        void expander_Expanded(object sender, RoutedEventArgs e)
        {
            propertyGridComboBoxSelectionChanged(sender, e);
        }
        void propertyGridComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (propertyGrid == null)
                return;
            switch (propertyGridComboBox.SelectedIndex)
            {
                case 0:
                    propertyGrid.SelectedObject = consoleOptionsProvider; // not .Instance
                    break;
                case 1:
                    //propertyGrid.SelectedObject = textEditor.Options; (for WPF native control)
                    propertyGrid.SelectedObject = textEditor.Options;
                    break;
            }
        }


        void LoadHighlightDefinition()
        {
            // Load our custom highlighting definition:
            IHighlightingDefinition pythonHighlighting;
            using (Stream s = typeof(IronPythonControl).Assembly.GetManifestResourceStream("IS3.Python.Resources.Python.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    pythonHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Python Highlighting", new string[] { ".py" }, pythonHighlighting);
        }

        void Host_ConsoleCreated(object sender, EventArgs e)
        {
            console.Console.AllowFullAutocompletion = true;
            console.Console.ConsoleInitialized += Console_ConsoleInitialized;

            ScriptEngine engine = console.Host.Engine;
            addIS3Path(engine);
        }
        void Console_ConsoleInitialized(object sender, EventArgs e)
        {
            if (ConsoleInitialized != null)
                ConsoleInitialized(sender, e);

            string startupScipt = "import IronPythonConsole";
            ScriptSource scriptSource = console.Pad.Console.ScriptScope.Engine.CreateScriptSourceFromString(startupScipt, SourceCodeKind.Statements);
            try
            {
                scriptSource.Execute();
            }
            catch { }
            //double[] test = new double[] { 1.2, 4.6 };
            //console.Pad.Console.ScriptScope.SetVariable("test", test);
        }
        public void addProjectPath(string projectPath)
        {
            ScriptScope scope = IronPython.Hosting.Python.GetSysModule(console.Host.Engine);
            dynamic path = scope.GetVariable("path");

            path.append(projectPath);
        }
        // Summary:
        //     add iS3 paths to python paths
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

        // Summary:
        //     Write a line to console.
        // Remarks:
        //     This function will 'halt' python input.
        //     Call runStatements("") to return to python prompt.
        public void write(string str)
        {
            console.Console.Write(str, Microsoft.Scripting.Hosting.Shell.Style.Out);
        }

        // Summary:
        //     Run statements
        public void runStatements(string statements)
        {
            console.Console.RunStatements(statements);
        }

        void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                textEditor.Load(currentFileName);
                //textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }

        void saveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(currentFileName);
        }

        void runClick(object sender, EventArgs e)
        {
            RunStatements();
        }


    }
}
