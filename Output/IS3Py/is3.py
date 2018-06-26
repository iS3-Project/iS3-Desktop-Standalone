# Copyright (C) 2015 iS3 Software Foundation
# Author: Xiaojun Li
# Contact: xiaojunli@tongji.edu.cn

import sys
import clr
import System

# Load System.Windows.Media in PresentationCore.dll
sys.path.append('C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5')
prcore = clr.LoadAssemblyFromFile('PresentationCore.dll')
clr.AddReference(prcore)
# Import classes in System
from System import Func,Action
from System.Windows.Media import Colors
from System.Collections.ObjectModel import ObservableCollection
from System.Threading.Tasks import Task

# Load IS3 namespaces
iS3Core = clr.LoadAssemblyFromFile('IS3.Core.dll')
clr.AddReference(iS3Core)
# Import classes in IS3
from IS3.Core import (Globals, Runtime, ErrorReport, ErrorReportTarget,
                      DGObject, DGObjects,
                      ProjectDefinition, Project,
                      EngineeringMap, EngineeringMapType, DrawShapeType,
                      IView, LayerDef, Domain, DomainType, ToolTreeItem)
from IS3.Core.Geometry import *
from IS3.Core.Graphics import *

def output(text):
    print(text)

# Redirect ErrorReport to python cosole
ErrorReport.target = ErrorReportTarget.DelegateConsole
ErrorReport.consoleDelegate = output

# In Windows, UI thread vars and functions are restricted to other threads.
# So, be caution with python calls to functions in UI thread.
# Classes in the main UI thread include: mainframe, view, layer, ...
# Therefore, calling to functions in mainframe, view, layer etc. are restricted.

mainframe = Globals.mainframe            # Global var: mainframe
prj = mainframe.prj                      # Global var: prj
dispatcher = mainframe.Dispatcher        # Global var: dispatcher -> UI thread manager
graphicsEngine = Runtime.graphicEngine   # Global var: graphics Engine
geometryEngine = Runtime.geometryEngine  # Global var: geometry Engine


class MainframeWrapper():
    "Define thread safe calls to mainframe methods"
    @staticmethod
    def addView(emap, canClose = True):
        "A thread safe call to -> mainframe.addView(emap, canclose)"
        if (Globals.isThreadUnsafe()):
            func = Func[EngineeringMap, bool, Task[IView]](mainframe.addView)
            view = dispatcher.Invoke(func, emap, canClose)
        else:
            view = mainframe.addView(emap, canClose)
        viewWrapper = ViewWrapper(view.Result)
        return viewWrapper

    @staticmethod
    def loadDomainPanels():
        "A thread safe call to -> mainframe.loadDomainPanels()"
        if (Globals.isThreadUnsafe()):
            dispatcher.Invoke(mainframe.loadDomainPanels)
        else:
            mainframe.loadDomainPanels()


class ViewWrapper():
    "Define thread safe calls to IS3View methods"
    def __init__(self, view):
        self.view = view

    def addLayer(self, layer):
        "A thread safe call to -> IS3View.addLayer"
        if (Globals.isThreadUnsafe()):
            func = Action[IGraphicsLayer](self.view.addLayer)
            dispatcher.Invoke(func, layer)
        else:
            self.view.addLayer(layer)

    def addLocalTiledLayer(self, file, id):
        "A thread safe call to -> IS3View.addLocalTiledLayer"
        if (Globals.isThreadUnsafe()):
            func = Action[str, str](self.view.addLocalTiledLayer)
            dispatcher.Invoke(func, file, id)
        else:
            self.view.addLocalTiledLayer(file, id)

    def addGdbLayer(self, layerDef, gdbFile, start = 0, maxFeatures = 0):
        "A thread safe call to -> IS3View.addGdbLayer"
        if (Globals.isThreadUnsafe()):
            func = Func[LayerDef, str, int, int, Task[IGraphicsLayer]](self.view.addGdbLayer)
            layer = dispatcher.Invoke(func, layerDef, gdbFile, start, maxFeatures)
        else:
            layer = self.view.addGdbLayer(layerDef, gdbFile, start, maxFeatures)
        layerWrapper = GraphicsLayerWrapper(layer.Result)
        return layerWrapper

    def addShpLayer(self, layerDef, shpFile, start = 0, maxFeatures = 0):
        "A thread safe call to -> IS3View.addShpLayer"
        if (Globals.isThreadUnsafe()):
            func = Func[LayerDef, str, int, int, Task[IGraphicsLayer]](self.view.addShpLayer)
            layer = dispatcher.Invoke(func, layerDef, shpFile, start, maxFeatures)
        else:
            self.view.addShpLayer(layerDef, shpFile, start, maxFeatures)
        layerWrapper = GraphicsLayerWrapper(layer.Result)
        return layerWrapper

    def selectByRect(self):
        "A thread safe call to -> IS3View.selectByRect"
        if (Globals.isThreadUnsafe()):
            dispatcher.Invoke(self.view.selectByRect)
        else:
            self.view.selectByRect()


class GraphicsLayerWrapper():
    "Define thread safe calls to IS3GraphicsLayer methods"
    def __init__(self, glayer):
        self.layer = glayer

    def setRenderer(self, renderer):
        "A thread safe call to -> IS3GraphicsLayer.setRenderer"
        if (Globals.isThreadUnsafe()):
            func = Action[IRenderer](self.layer.setRenderer)
            dispatcher.Invoke(func, renderer)
        else:
            self.layer.setRenderer(renderer)

    def addGraphic(self, graphic):
        "A thread safe call to -> IS3GraphicsLayer.addGraphic"
        if (Globals.isThreadUnsafe()):
            func = Action[IGraphic](self.layer.addGraphic)
            dispatcher.Invoke(func, graphic)
        else:
            self.layer.addGraphic(graphic)


def newGraphicsLayer(id, displayName):
    layer = graphicsEngine.newGraphicsLayer(id, displayName)
    layerWrapper = GraphicsLayerWrapper(layer)
    return layerWrapper

def addView3d(id, file):
    map3d = EngineeringMap()
    map3d.MapID = id
    map3d.MapType = EngineeringMapType.Map3D
    map3d.LocalMapFileName = file
    view3d = MainframeWrapper.addView(map3d, True)
    return view3d

def addGdbLayer(viewWrapper, layerDef, gdbFile = None, start = 0, maxFeatures = 0):
    prj = Globals.project
    layerWrapper = viewWrapper.addGdbLayer(layerDef, gdbFile, start, maxFeatures)
    if (layerWrapper.layer == None):
        print('addGdbFileELayer failed: ' + layerDef.Name)
        return None
    else:
        print('addGdbFileELayer succeeded: ' + layerDef.Name)

    objs = prj.findObjects(layerDef.Name)
    if (objs == None):
        print('Layer ' + layerDef.Name + ' has no corresponding objects in the project.')
    else:
        count = layerWrapper.layer.syncObjects(objs)
        print('Sync with ' + str(count) + ' objects for layer ' + layerDef.Name)

    return layerWrapper

def addGdbLayerLazy(view, name, type, gdbFile = None, start = 0, maxFeatures = 0):
    layerDef = LayerDef()
    layerDef.Name = name
    layerDef.GeometryType = type
    layerWrapper = addGdbLayer(view, layerDef, gdbFile, start, maxFeatures)
    return layerWrapper

def addShpLayer(viewWrapper, layerDef, shpfile, start = 0, maxFeatures = 0):
    prj = Globals.project
    layerWrapper = viewWrapper.addShpLayer(layerDef, shpfile, start, maxFeatures)
    if (layerWrapper.layer == None):
        print('addShpFileELayer failed: ' + layerDef.Name)
        return None
    else:
        print('addShpFileELayer succeeded: ' + layerDef.Name)
    objs = prj.findObjects(layerDef.Name)
    if (objs == None):
        print('Layer ' + layerDef.Name + ' has no corresponding objects in the project.')
    else:
        count = layerWrapper.layer.syncObjects(objs)
        print('Sync with ' + str(count) + ' objects for layer ' + layerDef.Name)
    return layerWrapper
