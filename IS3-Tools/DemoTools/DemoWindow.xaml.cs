using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using IS3.Core;
using IS3.Core.Geometry;
using IS3.Core.Graphics;
using IS3.ShieldTunnel;

namespace DemoTools
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

    /// <summary>
    /// Interaction logic of DemoWindow.xaml
    /// </summary>
    public partial class DemoWindow : Window
    {
        //定义全局变量
        Project _prj;
        Domain _structureDomain;
        IMainFrame _mainFrame;
        IView _inputView;

        //DGObject members
        DGObjectsCollection _allSLs; //所有衬砌对象
        List<string> _slLayerIDs; //衬砌图层ID
        Dictionary<string, IEnumerable<DGObject>> _selectedSLsDict; //选中的衬砌

        //graphics members
        ISpatialReference _spatialRef; //视图坐标系

        //result
        Dictionary<int, int> _slsGrade; //用来存储衬砌评估等级
        Dictionary<int, IGraphicCollection> _slsGraphics; //图形结果存储
        
        public DemoWindow()
        {
            InitializeComponent();

            //初始化全局变量
            _selectedSLsDict = new Dictionary<string, IEnumerable<DGObject>>();
            _slsGrade = new Dictionary<int, int>();
            _slsGraphics = new Dictionary<int, IGraphicCollection>();

            _mainFrame = Globals.mainframe;
            _prj = Globals.project;
            _structureDomain = _prj.getDomain(DomainType.Structure);
            _allSLs = _structureDomain.getObjects("SegmentLining");
            _slLayerIDs = new List<string>();
            foreach (DGObjects objs in _allSLs)
                _slLayerIDs.Add(objs.definition.GISLayerName);

            //窗口加载、关闭事件
            Loaded += DemoWindow_Loaded;
            Unloaded += DemoWindow_Unloaded;
        }

        //窗口加载事件
        void DemoWindow_Loaded(object sender,
            RoutedEventArgs e)
        {
            //设置窗口在右下角弹出
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Owner = mainWindow;
            this.Left = mainWindow.Left +
                (mainWindow.Width - this.ActualWidth - 10);
            this.Top = mainWindow.Top +
                (mainWindow.Height - this.ActualHeight - 10);

            //设置input view combobox数据源
            List<IView> planViews = new List<IView>();
            foreach (IView view in _mainFrame.views)
            {
                if (view.eMap.MapType == EngineeringMapType.FootPrintMap)
                    planViews.Add(view);
            }
            InputCB.ItemsSource = planViews;
            if (planViews.Count > 0)
            {
                _inputView = planViews[0];
                InputCB.SelectedIndex = 0;
            }
            else
            {
                return;
            }

            //设置segmenglining listbox数据源
            _inputView_objSelectionChangedListener(null, null);
        }

        //窗口关闭事件
        void DemoWindow_Unloaded(object sender,
            RoutedEventArgs e)
        {
            _inputView.addSeletableLayer("_ALL");
            _inputView.objSelectionChangedTrigger -=
                _inputView_objSelectionChangedListener;
        }

        //视图对象选择监听事件
        void _inputView_objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e)
        {
            //设置segmenglining listbox数据源
            _selectedSLsDict = _prj.getSelectedObjs(_structureDomain, "SegmentLining");
            List<DGObject> _sls = new List<DGObject>();
            foreach (var item in _selectedSLsDict.Values)
            {
                foreach (var obj in item)
                {
                    _sls.Add(obj);
                }
            }
            if (_sls != null && _sls.Count() > 0)
                SLLB.ItemsSource = _sls;
        }

        //input view cobombox选择事件
        private void InputCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //上一次选择的view
            _inputView.addSeletableLayer("_ALL");
            _inputView.objSelectionChangedTrigger -=
                    _inputView_objSelectionChangedListener;
            
            //新选择的view
            _inputView = InputCB.SelectedItem as IView;
            _inputView.removeSelectableLayer("_ALL");
            foreach (string layerID in _slLayerIDs)
                _inputView.addSeletableLayer(layerID);

            //为新的view添加对象选择监听事件
            _inputView.objSelectionChangedTrigger +=
                _inputView_objSelectionChangedListener;
        }

        //开始按钮事件
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            StartAnalysis();
            SyncToView();
            Close();
        }

        //关闭按钮事件
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void StartAnalysis()
        {
            //获取输入的view和复制坐标系
            IView view = InputCB.SelectedItem as IView;
            _spatialRef = view.spatialReference;

            //开始分析
            foreach (string SLLayerID in _selectedSLsDict.Keys)
            {
                //获取衬砌选中列表
                IEnumerable<DGObject> sls = _selectedSLsDict[SLLayerID];
                List<DGObject> slList = sls.ToList();
                IGraphicsLayer gLayer = _inputView.getLayer(SLLayerID);             
                foreach(DGObject dg in slList)
                {
                    //获取单个衬砌对象，计算评估等级
                    SegmentLining sl = dg as SegmentLining;
                    SLConvergenceRecordType slConvergenceRecordType = sl.ConstructionRecord.SLConvergenceRecords;
                    if (slConvergenceRecordType.SLConvergenceItems.Count == 0)
                        continue;

                    SLConvergenceItem slConvergenceItem = slConvergenceRecordType.SLConvergenceItems[0];
                    if(slConvergenceItem.HorizontalDev == double.NaN)
                        continue;
                    double raduis = (double)slConvergenceItem.HorizontalRad;
                    double deviation = (double)slConvergenceItem.HorizontalDev;
                    double ratio = deviation / (raduis - deviation) * 1000;

                    int grade;
                    if (ratio <= 3)
                        grade = 5;
                    else if (ratio <= 5)
                        grade = 4;
                    else if (ratio <= 8)
                        grade = 3;
                    else if (ratio <= 10)
                        grade = 2;
                    else
                        grade = 1;

                    //根据评估等级获取图形样式
                    ISymbol symbol = GetSymbol(grade);

                    //为了演示，采用了较复杂的方法
                    //<简便方法 可替换下面代码>
                    //IGraphicCollection gcollection = gLayer.getGraphics(sl);
                    //IGraphic g = gcollection[0];
                    //g.Symbol = symbol;
                    //IGraphicCollection gc = Runtime.graphicEngine.newGraphicCollection();
                    //gc.Add(g);
                    //_slsGraphics[sl.id] = gc;
                    //</简便方法>

                    //获取衬砌图形
                    IGraphicCollection gcollection = gLayer.getGraphics(sl);
                    IGraphic g = gcollection[0];
                    IPolygon polygon = g.Geometry as IPolygon;
                    IPointCollection pointCollection = polygon.GetPoints(); //获取端点
                    //衬砌为长方形，故有四个点
                    IMapPoint p1_temp = pointCollection[0];
                    IMapPoint p2_temp = pointCollection[1];
                    IMapPoint p3_temp = pointCollection[2];
                    IMapPoint p4_temp = pointCollection[3];
                    //新建新的点，注意复制坐标系
                    IMapPoint p1 = Runtime.geometryEngine.newMapPoint(p1_temp.X, p1_temp.Y, _spatialRef);
                    IMapPoint p2 = Runtime.geometryEngine.newMapPoint(p2_temp.X, p2_temp.Y, _spatialRef);
                    IMapPoint p3 = Runtime.geometryEngine.newMapPoint(p3_temp.X, p3_temp.Y, _spatialRef);
                    IMapPoint p4 = Runtime.geometryEngine.newMapPoint(p4_temp.X, p4_temp.Y, _spatialRef);
                    //生成新的图形
                    g = Runtime.graphicEngine.newQuadrilateral(p1, p2, p3, p4);
                    g.Symbol = symbol;
                    IGraphicCollection gc = Runtime.graphicEngine.newGraphicCollection();
                    gc.Add(g);
                    _slsGraphics[sl.id] = gc; //保存结果
                }
            }
        }
        
        //在view中加载图形，和同步图形
        void SyncToView()
        {
            IView view = InputCB.SelectedItem as IView;

            //为图形赋值“Name”属性，以便图形和数据关联
            foreach (int slID in _slsGraphics.Keys)
            {
                SegmentLining sl = _allSLs[slID] as SegmentLining;
                IGraphicCollection gc = _slsGraphics[slID];
                foreach (IGraphic g in gc)
                    g.Attributes["Name"] = sl.name;
            }

            //将图形添加到view中
            string layerID = "DemoLayer"; //图层ID
            IGraphicsLayer gLayer = getDemoLayer(view, layerID); //获取图层函数
            foreach (int id in _slsGraphics.Keys)
            {
                IGraphicCollection gc = _slsGraphics[id];            
                gLayer.addGraphics(gc);
            }

            //使数据与图形关联
            List<DGObject> sls = _allSLs.merge();
            gLayer.syncObjects(sls);

            //计算新建图形范围，并在地图中显示该范围
            IEnvelope ext = null;
            foreach (IGraphicCollection gc in _slsGraphics.Values)
            {
                IEnvelope itemExt = GraphicsUtil.GetGraphicsEnvelope(gc);
                if (ext == null)
                    ext = itemExt;
                else
                    ext = ext.Union(itemExt);
            }
            _mainFrame.activeView = view;
            view.zoomTo(ext);
        }

        //根据评估等级获取样式
        ISymbol GetSymbol(int grade)
        {
            ISimpleLineSymbol linesymbol = Runtime.graphicEngine.newSimpleLineSymbol(
                                Colors.Black, SimpleLineStyle.Solid, 1.0);
            Color color = Colors.Green;
            if (grade == 5)
                color = Colors.LightGreen;
            if (grade == 4)
                color = Colors.LightYellow;
            if (grade == 3)
                color = Colors.LightSkyBlue;
            if (grade == 2)
                color = Colors.LightSalmon;
            if (grade == 1)
                color = Colors.LightPink;
            return Runtime.graphicEngine.newSimpleFillSymbol(color, SimpleFillStyle.Solid, linesymbol);
        }

        //获取新建图层
        IGraphicsLayer getDemoLayer(IView view, string layerID)
        {
            IGraphicsLayer gLayer = view.getLayer(layerID);
            if (gLayer == null)
            {
                gLayer = Runtime.graphicEngine.newGraphicsLayer(
                    layerID, layerID);
                var sym_fill = GraphicsUtil.GetDefaultFillSymbol();
                var renderer = Runtime.graphicEngine.newSimpleRenderer(sym_fill);
                gLayer.setRenderer(renderer);
                gLayer.Opacity = 0.9;
                view.addLayer(gLayer);
            }
            return gLayer;
        }
    }
}
