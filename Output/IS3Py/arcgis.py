# Copyright (C) 2015 IS3 Software Foundation
# Author: Xiaojun Li
# Contact: xiaojunli@tongji.edu.cn

import clr

arcgis = clr.LoadAssemblyFromFile('Esri.ArcGISRuntime.dll')
clr.AddReference(arcgis)

from Esri.ArcGISRuntime.Data import Geodatabase

def gdbFileInfo(file):
    # get information about a geodatabase file
    gdb = Geodatabase.OpenAsync(file)
    gdb = gdb.Result
    print("--- layers, geometryType, rowCount ---")
    for table in gdb.FeatureTables:
        print(table.Name, table.GeometryType, table.RowCount)
