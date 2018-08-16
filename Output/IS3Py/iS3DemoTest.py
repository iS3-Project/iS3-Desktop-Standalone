# -*- coding:gb2312 -*-
import is3
is3.mainframe.LoadProject('iS3DemoTest.xml')
is3.prj = is3.mainframe.prj
is3.MainframeWrapper.loadDomainPanels()
for emap in is3.prj.projDef.EngineeringMaps:
    is3.MainframeWrapper.addView(emap)
is3.addView3d('Map3D', 'iS3DemoTest.unity3d')
