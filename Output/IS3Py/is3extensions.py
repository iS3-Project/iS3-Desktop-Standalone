# Copyright (C) 2016 iS3 Software Foundation
# Author: Xiaojun Li
# Contact: xiaojunli@tongji.edu.cn

import sys
import clr
sys.path.append('extensions')
monlib = clr.LoadAssemblyFromFile('IS3.Monitoring.dll')
clr.AddReference(monlib)
from IS3.Monitoring import (MonitoringHelper, MonPoint,
                            MonGroup, MonReading)
