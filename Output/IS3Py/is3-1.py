import sys
import clr


IS3Core = clr.LoadAssemblyFromFile('IS3.Core.dll')
clr.AddReference(IS3Core)
from IS3.Core import (IS3Global, IS3Runtime, ErrorReport, ErrorReportTarget,
                      DGObject, DGObjectType, DGObjectState, DGObjects,
                      ProjectDefinition, Project,
                      Domain, DomainType)

runtime=IS3Runtime
runtime.Initialize()

def output(text):
    print text

ErrorReport.target = ErrorReportTarget.DelegateConsole
ErrorReport.consoleDelegate = output

def LoadGeoData():
    dbContext.Open()
    geo.LoadAllObjects(dbContext)
    dbContext.Close()

def LoadStrData():
    dbContext.Open()
    struct.LoadAllObjects(dbContext)
    dbContext.Close()


prj = Project()
prj.LoadDefinition("SH_MetroL13.xml")
print prj

dbContext = prj.GetDbContext()
geo = prj.GetDomain(DomainType.Geology)
struct = prj.GetDomain(DomainType.Structure)

LoadGeoData()
objs = geo.ObjsContainer['AllWaterProperties']
boreholes = geo.ObjsContainer['Allboreholes']
strata = geo.ObjsContainer['AllStrata']
soilprops = geo.ObjsContainer['AllSoilProperties']

LoadStrData()
tunnels = struct.ObjsContainer['AllTunnelAxes']
SLs = struct.ObjsContainer['AllSLs']


