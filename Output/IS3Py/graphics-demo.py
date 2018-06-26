# -*- coding:gb2312 -*-
import is3
import System

from System.Windows.Media import Colors

def demo():
    emap = is3.EngineeringMap('demo', 0, 0, 100, 100, 0.01)
    safe_view = is3.MainframeWrapper.addView(emap)
    tilefile = is3.Runtime.tilePath + "\\Empty.tpk"
    safe_view.addLocalTiledLayer(tilefile, 'baselayer')

    sym_point = is3.graphicsEngine.newSimpleMarkerSymbol(
        Colors.Red, 12.0, is3.SimpleMarkerStyle.X)
    renderer1 = is3.graphicsEngine.newSimpleRenderer(sym_point)
    p1 = is3.graphicsEngine.newPoint(50, 50)
    layer1WP = is3.newGraphicsLayer('layer1', 'layer1')
    layer1WP.setRenderer(renderer1)
    layer1WP.layer.Graphics.Add(p1)
    safe_view.addLayer(layer1WP.layer)

    sym_line = is3.graphicsEngine.newSimpleLineSymbol(
        Colors.Blue, is3.SimpleLineStyle.Solid, 1.0)
    renderer2 = is3.graphicsEngine.newSimpleRenderer(sym_line)
    line1 = is3.graphicsEngine.newLine(20, 20, 80, 20)
    line2 = is3.graphicsEngine.newLine(80, 20, 80, 80)
    line3 = is3.graphicsEngine.newLine(80, 80, 20, 80)
    line4 = is3.graphicsEngine.newLine(20, 80, 20, 20)
    layer2WP = is3.newGraphicsLayer('layer2', 'layer2')
    layer2WP.setRenderer(renderer2)
    layer2WP.layer.Graphics.Add(line1)
    layer2WP.layer.Graphics.Add(line2)
    layer2WP.layer.Graphics.Add(line3)
    layer2WP.layer.Graphics.Add(line4)
    safe_view.addLayer(layer2WP.layer)

    sym_fill = is3.graphicsEngine.newSimpleFillSymbol(
        Colors.Red, is3.SimpleFillStyle.Solid, sym_line)
    renderer3 = is3.graphicsEngine.newSimpleRenderer(sym_fill)
    p1 = is3.geometryEngine.newMapPoint(30, 30)
    p2 = is3.geometryEngine.newMapPoint(40, 30)
    p3 = is3.geometryEngine.newMapPoint(30, 40)
    triangle = is3.graphicsEngine.newTriangle(p1, p2, p3)
    layer3WP = is3.newGraphicsLayer('layer3', 'layer3')
    layer3WP.setRenderer(renderer3)
    layer3WP.layer.Graphics.Add(triangle)
    safe_view.addLayer(layer3WP.layer)

demo()