# -*- coding:gb2312 -*-
import is3

from System.Collections.ObjectModel import ObservableCollection
from System.Windows.Media import Colors

##  加载工程
def LoadPrj():
    print("---Load Project---")
    is3.mainframe.LoadProject('iS3DemoTest.xml')     #--->注：加载工程配置XML文件，替换Example为对应的工程名称
    is3.prj = is3.mainframe.prj
    is3.MainframeWrapper.loadDomainPanels()
    return

##  加载工程三维   
def add3dview():
    print ("--- Add 3D map ---")
    #--->注：加载三维发布的.unity3d文件，替换成对应的工程ID
    is3.addView3d('Map3D', 'iS3DemoTest.unity3d')    
    return

##  加载工程二维底图
def addBaseMap():
    print("--- Add base map ---")
    #--->注：设置底图名称，底图的显示范围（XMin，YMin，XMax，YMax）
    emap = is3.EngineeringMap('BaseMap',12661378,-82283,12867753,13760, 0.1)  
    #--->注：设置底图类型为平面图，类型可选FootPrintMap（平面），GeneralProfileMap（剖面）  
    emap.MapType = is3.EngineeringMapType.FootPrintMap                             
    #--->注：设置底图文件，替换Example为对应的工程名称
    emap.LocalTileFileName1 = 'iS3DemoTest.tpk'                                        
    #--->注：设置底图数据库文件，替换Example为对应的工程名称
    emap.LocalGeoDbFileName = 'iS3DemoTest.geodatabase'                                
    #--->注：添加底图到iS3内
    viewWP = is3.MainframeWrapper.addView(emap)                                    
    #--->注：将底图数据库内容以要素形式添加到底图上
    addMapLayer(viewWP)                                                            
    return

##  加载二维数据库元素到工程底图上
def addMapLayer(viewWP): 
    layerDef = is3.LayerDef()
    layerDef.Name = 'MonPoint'                                                     
    #--->注：原先ArcMap内打包时对应的图层名称
    layerDef.GeometryType = is3.GeometryType.Point                               
    #--->注：图层要素的表现形式，Point(点),Polyline(线),Polygon(面)
    layerDef.Color = Colors.Green                                                  
    #--->注：图层要素的颜色
    layerDef.FillStyle = is3.SimpleFillStyle.Solid
    layerDef.EnableLabel = True
    layerDef.LabelTextExpression = '[Name]'
    layerWrapper = is3.addGdbLayer(viewWP, layerDef)
    return

def Load():
    LoadPrj()                                                                     #--->注：加载工程
    addBaseMap()                                                                  #--->注：加载工程二维
    add3dview()                                                                   #--->注：加载工程三维


##程序入口
Load()
