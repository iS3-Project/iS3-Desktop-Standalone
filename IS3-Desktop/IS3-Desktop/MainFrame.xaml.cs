using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Collections;
using System.Reflection;

using System.Data;

using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

using IS3.Core;
using IS3.Python;

namespace IS3.Desktop
{
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

    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainFrame : UserControl, IMainFrame
    {
        //string _definitionFileMissing = "Definition file {0} missing.";
        //string _localDbFileMissing = "Local database file {0} missing.";
        //string _configPath = @"..\Conf";
        //string _projFile = "Project.xml";
        string _configFile = @"..\Conf\MainPageLayout.xml";
        string _definitionFile;
        App _app;
        Project _prj;

        List<IView> _views = new List<IView>();
        IView _activeView = null;

        List<IS3Tree> _treePanels = new List<IS3Tree>();
        Tree _lastSelectedTree;


        ProgressBarWindow _pbw;

        #region IMainFrame interfaces

        public event EventHandler<ObjSelectionChangedEventArgs>
            objSelectionChangedTrigger;
        public event EventHandler projectLoaded;
        
        public Project prj 
        { 
            get { return _prj; }
            set { _prj = value; }
        }
        public IEnumerable<IView> views
        {
            get { return _views; }
        }
        public IView activeView
        {
            get { return _activeView; }
            set
            {
                _activeView = value;
                LayoutContent doc = FindLayoutContentByID(value.eMap.MapID);
                if (doc != null && doc.IsSelected == false)
                    doc.IsSelected = true;
            }
        }
        #endregion

        public string DefinitionFile
        {
            get { return _definitionFile; }
        }
        public ProgressBarWindow ProgressBarWindow
        {
            get { return _pbw; }
        }
        public IView GetView(string mapID)
        {
            return _views.Find(i => i.eMap.MapID == mapID);
        }
        public ToolTreeItem toolTreeRoot
        {
            get { return ToolsPanel.toolboxesTree; }
        }

        public MainFrame(string definitionFile)
        {
            // Load project definition and user data at first
            // 
            _definitionFile = definitionFile;
            
            _app = App.Current as App;
            _app.MainFrame = this;
            Globals.mainframe = this;

            InitializeComponent();

            Loaded += MainFrame_Loaded;
            Unloaded += MainFrame_Unloaded;

            _layoutRoot.Manager.Loaded += Manager_Loaded;
            _layoutRoot.Manager.Unloaded += Manager_Unloaded;

            MyDataGrid.DGObjectDataGrid.SelectionChanged += DGObjectDataGrid_SelectionChanged;
            this.objSelectionChangedTrigger += objectView.objSelectionChangedListener;
        }

        // You need to find LayoutContent by ID,
        // because the LayoutElement will be reconstructed after
        // Manager_Loaded() is called.
        public LayoutContent FindLayoutContentByID(string contentID)
        {
            LayoutRoot layoutRoot = DocMan.Layout;
            return FindLayoutContentByID(layoutRoot, contentID);
        }

        LayoutContent FindLayoutContentByID(ILayoutElement layoutElement,
            string contentID)
        {
            if (layoutElement == null)
                return null;

            LayoutContent content = layoutElement as LayoutContent;
            if (content != null)
            {
                if (content.ContentId == contentID)
                    return content;
            }

            IEnumerable<ILayoutElement> children = null;
            LayoutRoot root = layoutElement as LayoutRoot;
            LayoutFloatingWindow fw = layoutElement as LayoutFloatingWindow;
            ILayoutGroup group = layoutElement as ILayoutGroup;
            if (root != null)
                children = root.Children;
            if (fw != null)
                children = fw.Children;
            if (group != null)
                children = group.Children;

            if (children == null)
                return null;

            foreach (ILayoutElement i in children)
            {
                content = FindLayoutContentByID(i, contentID);
                if (content != null)
                    return content;
            }

            return null;
        }

        // Find LayoutDocumentPane where IS3View can be add.
        public LayoutDocumentPane FindViewHolder()
        {
            if (_prj == null)
                return null;

            EngineeringMap baseMap = _prj.projDef.EngineeringMaps.FirstOrDefault();
            if (baseMap == null)
                return null;
            LayoutContent layoutContent = FindLayoutContentByID(baseMap.MapID);
            if (layoutContent == null)
                return null;

            LayoutDocumentPane docPane = layoutContent.Parent as LayoutDocumentPane;
            if (docPane == null)
            {
                LayoutRoot root = layoutContent.Root as LayoutRoot;
                LayoutPanel panel = root.Children.First() as LayoutPanel;
                LayoutDocumentPaneGroup docPaneGroup =
                    panel.Children.ElementAt(1) as LayoutDocumentPaneGroup;
                docPane = docPaneGroup.Children.First()
                    as LayoutDocumentPane;
            }
            return docPane;
        }

        #region load/unload functions
        bool _init = false;
        void MainFrame_Loaded(object sender, RoutedEventArgs e)
        {
            try { init(); }
            catch (Exception ex)
            {
                ipcHost.ConsoleInitialized += (o, args) =>
                {
                    Dispatcher.Invoke(
                        new Action(delegate()
                        {
                            init();
                        }
                            ));
                };

            }
        }

        public void init()
        {
            Thread.Sleep(1000);
            output("\r\n");
            LoadProject("");
            LoadViews();
            loadDomainPanels();
            loadExtensions();
            loadToolboxes();
            loadPyPlugins();
            loadByPython();
            _init = true;
        }
       
        void MainFrame_Unloaded(object sender, RoutedEventArgs e)
        {
            //foreach (IView view in _views)
            //{
            //    if (view.Doc != null)
            //        view.Doc.OnClose();
            //    view.OnClose();
            //}
            //foreach (IS3Tree tree in _treePanels)
            //    tree.OnClose();

            //WriteProject();
        }

        //
        void Manager_Loaded(object sender, RoutedEventArgs e)
        {
            bool fileExist = File.Exists(_configFile);

            if (fileExist)
            {
                // Be careful with the XmlLayoutSerizlizer.Deserialize(DockingManager),
                // the children inside it will be reconstructed.
                // The original children will be disregarded.
                // If a name is put on a child, the child will not work properly any more.
                // You must search inside the reconstructed DockingManager to find the proper one.
                // Call FindLayoutContentByID() to find the LayoutContent.
                DockingManager manager = sender as DockingManager;
                XmlLayoutSerializer instance =
                    new XmlLayoutSerializer(manager);
                instance.Deserialize(_configFile);

                // Because LayoutDocument are regenerated,
                // we need to re-attach the Closed() method to each LayoutDocument.
                foreach (IView view in _views)
                {
                    LayoutContent content = FindLayoutContentByID(view.eMap.MapID);
                    LayoutDocument layoutDoc = content as LayoutDocument;
                    if (layoutDoc != null)
                        layoutDoc.Closed += LayoutDoc_Closed;
                }
            }
        }

        void Manager_Unloaded(object sender, RoutedEventArgs e)
        {
            //if (!Directory.Exists(_configPath))
            //    Directory.CreateDirectory(_configPath);

            //try
            //{
            //    // write window layout
            //    XmlLayoutSerializer instance =
            //        new XmlLayoutSerializer(sender as DockingManager);
            //    instance.Serialize(_configFile);
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show(ex.Message);
            //}
        }
        public void loadByPython()
        {
            AddProjectPathToPython();
            if ((_definitionFile != null)
                && (_definitionFile.Length != 0))
            {
                string projectID = _definitionFile.Substring(0, _definitionFile.IndexOf('.'));
                string statement = string.Format("import {0}", projectID);
                runStatements(statement);
            }
            runStatements("");
        }
        void AddProjectPathToPython()
        {
            string[] folders = Directory.GetDirectories(Runtime.dataPath);
            for (int i = 0; i < folders.Count(); i++)
            {
                ipcHost.addProjectPath(folders[i]);
            }
        }
        public void LoadProject(string definitionFile)
        {
            if (definitionFile == null
                || definitionFile.Length == 0)
                return;

            ////ShowProgressWindow("Loading project data...");
            _prj = Project.load(definitionFile);
            Globals.project = _prj;
            objSelectionChangedTrigger += _prj.objSelectionChangedListener;
            if (projectLoaded != null)
                projectLoaded(this, EventArgs.Empty);
            ////HideProgressWindow();
        }

        public async void LoadViews()
        {
            if (_prj == null)
                return;

            // Initialize predefined docs and views
            //
            foreach (EngineeringMap eMap in _prj.projDef.EngineeringMaps)
            {
                IView view = await addView(eMap, false);
                if (_activeView == null)
                    _activeView = view;
            }
        }

        public void loadDomainPanels()
        {
            if (_prj == null)
                return;

            // Set project title
            ProjectTitle.Text = _prj.projDef.ProjectTitle;

            // Initialize domain tree panels.
            foreach (Domain domain in _prj.domains.Values)
            {
                LayoutAnchorable layoutAnchorable = new LayoutAnchorable();
                layoutAnchorable.ContentId = domain.name;
                layoutAnchorable.Title = domain.name;
                layoutAnchorable.CanClose = false;
                DomainTreeHolder.Children.Add(layoutAnchorable);

                TreePanel treePanel = new TreePanel(domain.root);
                treePanel.OnTreeSelected += treePanel_OnTreeSelected;
                layoutAnchorable.Content = treePanel;

                _treePanels.Add(treePanel.IS3Tree);
            }

            // Initialize tree panels.
            // Real information of each subProject will be loaded.
            // 
            foreach (IS3Tree treePanel in _treePanels)
            {
                treePanel.InitializeTree();
            }
        }

        void treePanel_OnTreeSelected(object sender, Tree tree)
        {
            if (tree == null)
                MyDataGrid.DGObjectDataGrid.ItemsSource = null;
            MyDataGrid.DGObjectDataGrid.ItemsSource = tree.ObjectsView;
            _lastSelectedTree = tree;
        }

        void DGObjectDataGrid_SelectionChanged(object sender, 
            SelectionChangedEventArgs e)
        {
            // Handles selection changed event triggered by user input only.
            // Selection changed event will also be triggered in 
            // situations like DGObjectDataGrid.ItemsSource = IEnumerable<>,
            // but we don't want to handle the event in such conditions.
            // This can be differentiated by the IsKeyboardFocusWithin property.
            if (MyDataGrid.IsKeyboardFocusWithin == false
                || _isEscTriggered)
                return;

            // Trigger a ObjSelectionChangedEvent event
            // 
            DataView dataView = MyDataGrid.DGObjectDataGrid.ItemsSource
                as DataView;
            if (dataView == null)
                return;
            DataTable dataTable = dataView.Table;
            DataSet dataSet = dataTable.DataSet;
            if (dataSet == null)
                return;
            if (!_prj.dataSetIndex.ContainsKey(dataSet))
                return;
            DGObjects objs = _prj.dataSetIndex[dataSet];
            string layerName = objs.definition.GISLayerName;

            IList addedItems = e.AddedItems;
            IList removedItems = e.RemovedItems;
            List<DGObject> addedObjs = new List<DGObject>();
            List<DGObject> removedObjs = new List<DGObject>();

            foreach (DataRowView drv in addedItems)
            {
                DataRow dr = drv.Row;
                if (objs.rowView2Obj.ContainsKey(dr))
                {
                    DGObject obj = objs.rowView2Obj[dr];
                    addedObjs.Add(obj);
                }
            }

            foreach (DataRowView drv in removedItems)
            {
                DataRow dr = drv.Row;
                if (objs.rowView2Obj.ContainsKey(dr))
                {
                    DGObject obj = objs.rowView2Obj[dr];
                    removedObjs.Add(obj);
                }
            }

            if (objSelectionChangedTrigger != null)
            {
                Dictionary<string, IEnumerable<DGObject>> addedObjsDict = null;
                Dictionary<string, IEnumerable<DGObject>> removedObjsDict = null;
                if (addedObjs.Count > 0)
                {
                    addedObjsDict = new Dictionary<string, IEnumerable<DGObject>>();
                    addedObjsDict[layerName] = addedObjs;
                }
                if (removedObjs.Count > 0)
                {
                    removedObjsDict = new Dictionary<string, IEnumerable<DGObject>>();
                    removedObjsDict[layerName] = removedObjs;
                }
                ObjSelectionChangedEventArgs args =
                    new ObjSelectionChangedEventArgs();
                args.addedObjs = addedObjsDict;
                args.removedObjs = removedObjsDict;
                objSelectionChangedListener(sender, args);
            }
        }

        public async Task<IView> addView(EngineeringMap eMap, bool canClose)
        {
            LayoutDocumentPane docPane = FindViewHolder();
            if (docPane == null)
                docPane = ViewPane;

            LayoutDocument LayoutDoc = new LayoutDocument();
            LayoutDoc.ContentId = eMap.MapID;
            LayoutDoc.Title = eMap.MapID;
            LayoutDoc.CanClose = canClose;
            LayoutDoc.Closed += LayoutDoc_Closed;
            docPane.Children.Add(LayoutDoc);
            
            string datafilePath, filePath;
            if (_prj != null)
            {
                datafilePath = _prj.projDef.LocalFilePath;
                filePath = datafilePath + "\\" + eMap.MapID + ".xml";
            }

            IView view = null;
            if (eMap.MapType == EngineeringMapType.FootPrintMap)
            {
                PlanView planView = new PlanView(this, _prj, eMap);
                LayoutDoc.Content = planView;
                view = planView.view;
            }
            else if (eMap.MapType == EngineeringMapType.GeneralProfileMap)
            {
                ProfileView profileView = new ProfileView(this, _prj, eMap);
                LayoutDoc.Content = profileView;
                view = profileView.view;
            }
            else if (eMap.MapType == EngineeringMapType.Map3D)
            {
                U3DView u3dView = new U3DView(this, _prj, eMap);
                LayoutDoc.Content = u3dView;
                view = u3dView.view;
            }

            // view is both a trigger and listener of object selection changed event
            view.objSelectionChangedTrigger += this.objSelectionChangedListener;
            this.objSelectionChangedTrigger += view.objSelectionChangedListener;
            view.initializeView();

            // Load predefined layers
            await view.loadPredefinedLayers();

            // Sync view graphics with data
            view.syncObjects();

            _views.Add(view);
            return view;
        }

        // Summary:
        //     Object selection event listener (function).
        //     It will broadcast the event to views and datagrid.
        public void objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e)
        {
            if (objSelectionChangedTrigger != null)
                objSelectionChangedTrigger(sender, e);
        }

        void LayoutDoc_Closed(object sender, EventArgs e)
        {
            LayoutDocument layoutDoc = sender as LayoutDocument;
            IViewHolder viewHolder = layoutDoc.Content as IViewHolder;
            IView view = viewHolder.view;
            RemoveView(view);
        }

        void RemoveView(IView view)
        {
            _views.Remove(view);
        }


        #endregion

        public void ShowProgressWindow(string message)
        {
            _pbw = new ProgressBarWindow();
            _pbw.Message.Text = message;
            _pbw.Owner = _app.MainWindow;
            _pbw.ShowDialog();
        }

        public void HideProgressWindow()
        {
            _pbw.Close();
        }

        bool _isEscTriggered = false;
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_prj != null)
                {
                    // trigger object selection changed event
                    Dictionary<string, IEnumerable<DGObject>> selectedObjs =
                        _prj.getSelectedObjs();
                    if (objSelectionChangedTrigger != null)
                    {
                        ObjSelectionChangedEventArgs args
                            = new ObjSelectionChangedEventArgs();
                        args.removedObjs = selectedObjs;
                        _isEscTriggered = true;
                        objSelectionChangedTrigger(this, args);
                        _isEscTriggered = false;
                    }
                }

                // clear selections in all views in case graphics all not cleared.
                foreach (IView view in views)
                {
                    if (view.layers != null)
                    {
                        foreach (var layer in view.layers)
                            layer.highlightAll(false);
                    }
                }
            }
            else if (e.Key == Key.Delete)
            {
                //RemoveDrawingObjects();
            }
            else if (e.Key == Key.N && 
                System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Ctrl+N
            }
            base.OnKeyUp(e);
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Application app = App.Current;
            IS3MainWindow mw = app.MainWindow as IS3MainWindow;
            mw.SwitchToProjectListPage();
        }

        private void Python_Click(object sender, RoutedEventArgs e)
        {
            string pyPath = Runtime.rootPath;
            pyPath = pyPath + "\\IS3Py";

            System.Windows.Forms.OpenFileDialog dlg = 
                new System.Windows.Forms.OpenFileDialog();
            dlg.InitialDirectory = pyPath;
            dlg.Filter = "Python scripts (*.py)|*.py";
            dlg.DefaultExt = "py";

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                runPythonScripts(dlg.FileName);
            }
        }

        // Summary:
        //     Run python scripts
        // Remarks:
        //     The scripts runned here is in the main UI thread, 
        //     which is different from scripts inputted in the Python
        //     Console window. In the case of Python Console window,
        //     the script is runned in another thread. Therefore,
        //     there is no restriction on calls to main UI thread here.
        // Known bugs:
        //     Script call to IS3View.addGdbLayer will hang the program.
        //     This problem doesn't exist in scripts runned from console window.
        public void runPythonScripts(string file)
        {
            PyManager pyMan = new PyManager();
            pyMan.run(file);
        }

        static List<Assembly> _loadedExtensions = new List<Assembly>();
        // Summary:
        //     Load extensions which are located in the bin\extensions\ directory.
        public void loadExtensions()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly exeAssembly = Assembly.GetExecutingAssembly();
            string exeLocation = exeAssembly.Location;
            string exePath = Path.GetDirectoryName(exeLocation);
            string extensionsPath = exePath + "\\extensions";

            if (!Directory.Exists(extensionsPath))
                return;

            // try to load *.dll in bin\extensions\
            var files = Directory.EnumerateFiles(extensionsPath, "*.dll",
                SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                // skip the assembly that has been loaded
                string shortName = Path.GetFileName(file);
                if (allAssemblies.Any(x => x.ManifestModule.Name == shortName))
                    continue;

                // Assembly.LoadFile doesn't resolve dependencies, 
                // so don't use Assembly.LoadFile
                Assembly assembly = Assembly.LoadFrom(file);
                if (assembly != null)
                    _loadedExtensions.Add(assembly);
            }

            // call init() in extensions
            foreach (Assembly assembly in _loadedExtensions)
            {
                // call init() function in the loaded assembly
                var types = from type in assembly.GetTypes()
                            where type.IsSubclassOf(typeof(IS3.Core.Extensions))
                            select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    IS3.Core.Extensions extension = obj as IS3.Core.Extensions;
                    if (extension == null)
                        continue;
                    string msg = extension.init();
                    output(msg);
                }
            }
        }

        static List<Assembly> _loadedToolboxes = new List<Assembly>();
        // Summary:
        //     Load tools which are located in the bin\tools\ directory.
        public void loadToolboxes()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly exeAssembly = Assembly.GetExecutingAssembly();
            string exeLocation = exeAssembly.Location;
            string exePath = Path.GetDirectoryName(exeLocation);
            string toolsPath = exePath + "\\tools";

            if (!Directory.Exists(toolsPath))
                return;

            // try to load *.dll in bin\tools\
            var files = Directory.EnumerateFiles(toolsPath, "*.dll",
                SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                string shortName = Path.GetFileName(file);
                if (allAssemblies.Any(x => x.ManifestModule.Name == shortName))
                    continue;

                // Assembly.LoadFile doesn't resolve dependencies, 
                // so don't use Assembly.LoadFile
                Assembly assembly = Assembly.LoadFrom(file);
                if (assembly != null)
                    _loadedToolboxes.Add(assembly);
            }

            foreach (Assembly assembly in _loadedToolboxes)
            {
                // call init() function in the loaded assembly
                var types = from type in assembly.GetTypes()
                            where type.IsSubclassOf(typeof(IS3.Core.Extensions))
                            select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    IS3.Core.Extensions extension = obj as IS3.Core.Extensions;
                    if (extension == null)
                        continue;
                    string msg = extension.init();
                    output(msg);
                }

                // call tools.treeItems() can add it to ToolsPanel
                types = from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(Tools))
                        select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    Tools tools = obj as Tools;
                    IEnumerable<ToolTreeItem> treeitems = tools.treeItems();
                    if (treeitems == null)
                        continue;
                    foreach (var item in treeitems)
                        ToolsPanel.toolboxesTree.add(item);
                }
            }
        }

        // Summary:
        //     load python plugins
        // Remarks:
        //     Python scripts located in bin\PyPlugins will be executed automatically.
        public void loadPyPlugins()
        {
            Assembly exeAssembly = Assembly.GetExecutingAssembly();
            string exeLocation = exeAssembly.Location;
            string exePath = Path.GetDirectoryName(exeLocation);
            string pyPluginsPath = exePath + "\\PyPlugins";

            if (!Directory.Exists(pyPluginsPath))
                return;

            PyManager pyMan = new PyManager();
            pyMan.loadPlugins(pyPluginsPath);
        }

        // Summary:
        //     Write a string to console.
        // Remarks:
        //     This function will 'halt' python input.
        //     Call RunStatements("") to return to python prompt.
        public void output(string str)
        {
            ipcHost.write(str);
        }

        // Summary:
        //     Run statements in python console.
        // Remarks:
        //     A run statements helps return to python prompt.
        public void runStatements(string statements)
        {
            ipcHost.runStatements(statements);
        }
    }

}
