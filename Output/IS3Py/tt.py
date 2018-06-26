# -*- coding:gb2312 -*-
import is3

from System.Collections.ObjectModel import ObservableCollection
from System.Windows.Media import Colors

def add3dview():
    is3.addView3d('Map3D', 'Test.unity3d')

def addBaseMap():
    is3.mainframe.LoadProject('tt.xml')
    is3.prj = is3.mainframe.prj
    is3.MainframeWrapper.loadDomainPanels()
    
    emap = is3.EngineeringMap('BaseMap',
                              13523000, 3664000, 13525000, 3666000, 0.1)
    emap.LocalTileFileName1 = 'Shanghai_LOD16_CityBlocks.tpk'
    emap.LocalTileFileName2 = 'plan_map_L12.tpk'
    emap.LocalGeoDbFileName = 'plan_map.geodatabase'
    #emap.MapRotation = -24

    viewWP = is3.MainframeWrapper.addView(emap)
    return viewWP

def addLonMap():
    emap = is3.EngineeringMap('LTProfileMap',
                              13521600, 3662850, 13524300, 3663350, 0.1)
    emap.LocalTileFileName1 = 'longitudinal_tunnel_profile_map.tpk'
    emap.LocalGeoDbFileName = 'longitudinal_tunnel_profile_map.geodatabase'
    emap.MapType = is3.EngineeringMapType.GeneralProfileMap;
    viewWP = is3.MainframeWrapper.addView(emap)
    return viewWP

def addBhLayer(viewWP):
    defaultsymbol = is3.SimpleMarkerSymbolDef(Colors.Blue, 12.0, is3.SimpleMarkerStyle.Circle)
    symbol1 = is3.SimpleMarkerSymbolDef(Colors.Green, 12.0, is3.SimpleMarkerStyle.Circle)
    symbol2 = is3.SimpleMarkerSymbolDef(Colors.Black, 12.0, is3.SimpleMarkerStyle.Circle)
    fields = ObservableCollection[str](['BoreholeType'])
    info1 = is3.UniqueValueInfoDef(symbol1, ObservableCollection[object](['取土孔']))
    info2 = is3.UniqueValueInfoDef(symbol2, ObservableCollection[object](['静力触探孔']))
    infos = ObservableCollection[is3.UniqueValueInfoDef]((info1, info2))
    uniquevalue_renderer = is3.UniqueValueRendererDef(defaultsymbol, fields, infos)
    
    layerDef = is3.LayerDef()
    layerDef.Name = 'GEO_BHL'
    layerDef.GeometryType = is3.GeometryType.Point
    #layerDef.Color = Colors.Blue
    #layerDef.MarkerSize = 12
    #layerDef.MarkerStyle = is3.SimpleMarkerStyle.Circle
    layerDef.RendererDef = uniquevalue_renderer
    layerDef.EnableLabel = True
    layerDef.LabelTextExpression = '[Name]'
    bhLayerWrapper = is3.addGdbLayer(viewWP, layerDef)
    return bhLayerWrapper

def addRinLayer(viewWP):
    layerDef = is3.LayerDef()
    layerDef.Name = 'DES_RIN'
    layerDef.GeometryType = is3.GeometryType.Polygon
    layerDef.OutlineColor = Colors.Blue
    layerDef.Color = Colors.LightGray
    layerDef.FillStyle = is3.SimpleFillStyle.Solid
    layerDef.EnableLabel = True
    layerDef.LabelTextExpression = '[Name]'
    layerDef.LabelWhereClause = "[Name] LIKE '%00' OR [Name] LIKE '%50'"
    layerDef.LabelBackgroundColor = Colors.Yellow
    rinLayerWP = is3.addGdbLayer(viewWP, layerDef)
    return rinLayerWP

def addStrLayer(viewWP):
    layerDef = is3.LayerDef()
    layerDef.Name = 'GEO_STR'
    layerDef.GeometryType = is3.GeometryType.Polygon
    layerDef.OutlineColor = Colors.Gray
    layerDef.Color = Colors.LightGray
    layerDef.FillStyle = is3.SimpleFillStyle.Solid
    layerDef.EnableLabel = True
    layerDef.LabelTextExpression = '[Name]'
    layerDef.LabelBackgroundColor = Colors.White
    strLayerWP = is3.addGdbLayer(viewWP, layerDef)
    return strLayerWP

def addLayers(viewWP):
    is3.addGdbLayerLazy(viewWP, 'DES_PIT_WALL', is3.GeometryType.Polyline)
    is3.addGdbLayerLazy(viewWP, 'DES_AXP', is3.GeometryType.Point)
    is3.addGdbLayerLazy(viewWP, 'DES_PAS', is3.GeometryType.Polygon)
    is3.addGdbLayerLazy(viewWP, 'DES_AXL', is3.GeometryType.Polyline)
    is3.addGdbLayerLazy(viewWP, 'DES_TUN', is3.GeometryType.Polygon)
    
    is3.addGdbLayerLazy(viewWP, 'MON_WAT', is3.GeometryType.Point)
    is3.addGdbLayerLazy(viewWP, 'MON_RIN', is3.GeometryType.Point)
    is3.addGdbLayerLazy(viewWP, 'MON_GRO', is3.GeometryType.Point)
    is3.addGdbLayerLazy(viewWP, 'MON_BUI', is3.GeometryType.Point)

def changeRenderer():
    defaultsymbol = is3.graphicsEngine.newSimpleMarkerSymbol(Colors.Red, 12.0, is3.SimpleMarkerStyle.X)
    symbol1 = is3.graphicsEngine.newSimpleMarkerSymbol(Colors.Green, 12.0, is3.SimpleMarkerStyle.X)
    symbol2 = is3.graphicsEngine.newSimpleMarkerSymbol(Colors.Black, 12.0, is3.SimpleMarkerStyle.X)
    fields = ObservableCollection[str](['BoreholeType'])
    info1 = is3.graphicsEngine.newUniqueValueInfo(symbol1, ObservableCollection[object](['取土孔']))
    info2 = is3.graphicsEngine.newUniqueValueInfo(symbol2, ObservableCollection[object](['静力触探孔']))
    infos = ObservableCollection[is3.IUniqueValueInfo]((info1, info2))
    uniquevalue_renderer = is3.graphicsEngine.newUniqueValueRenderer(defaultsymbol, fields, infos)
    bhLayerWP.setRenderer(uniquevalue_renderer)

def addShapefile(name, type, file, start = 0, maxFeatures = 0):
    layerDef = is3.LayerDef()
    layerDef.Name = name
    layerDef.GeometryType = type
    bkgLayerWP = is3.addShpLayer(viewWP, layerDef, file, start, maxFeatures)
    return bkgLayerWP

def test1():
    addShapefile('BKG1', is3.GeometryType.Polyline, 'bkg_lin.shp', 0, 30000)

def test2():
    addShapefile('BKG2', is3.GeometryType.Polyline, 'bkg_lin.shp', 30000, 30000)

def test3():
    addShapefile('BKG3', is3.GeometryType.Polyline, 'bkg_lin.shp', 60000, 30000)

def test():
    global viewWP1, viewWP2, safe_view

    print("--- Add base map ---")
    viewWP1 = addBaseMap()
    rinLayerWP = addRinLayer(viewWP1)
    bhLayerWP = addBhLayer(viewWP1)

    print ("--- Add longitudinal profile map ---")
    viewWP2 = addLonMap()
    addStrLayer(viewWP2)
    addRinLayer(viewWP2)

    print ("--- Add a empty longitudinal profile map ---")
    emap = is3.EngineeringMap('profile1', 0, 0, 100, 100, 0.01)
    emap.MapType = is3.EngineeringMapType.GeneralProfileMap;
    safe_view = is3.MainframeWrapper.addView(emap)
    tilefile = is3.Runtime.tilePath + "\\Empty.tpk"
    safe_view.addLocalTiledLayer(tilefile, 'baselayer')

def newLineLayer():
    global myLayer, sr
    sr = viewWP1.view.spatialReference
    sym_line = is3.graphicsEngine.newSimpleLineSymbol(
        Colors.Red, is3.SimpleLineStyle.Solid, 3.0)
    renderer = is3.graphicsEngine.newSimpleRenderer(sym_line)
    myLayer = is3.newGraphicsLayer('myLayer', 'myLayer')
    myLayer.setRenderer(renderer)
    viewWP1.addLayer(myLayer.layer)

def drawline(x1, y1, x2, y2):
    line = is3.graphicsEngine.newLine(x1, y1, x2, y2, sr)
    myLayer.addGraphic(line)

test()
