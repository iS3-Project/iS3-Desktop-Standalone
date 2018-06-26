# Copyright (C) 2015 iS3 Software Foundation
# Author: Xiaojun Li
# Contact: xiaojunli@tongji.edu.cn

Notes/restrictions/bugs on using python scripts in iS3.

(1) Scripts can be loadded and runned in iS3. In this case, the scripts is runned in the main UI thread. Note that script call to IS3View.addGdbLayer will hang the program (load tt.py will hang the program). This problem doesn't exist in the following case.

(2) Scripts can be inputted and runned immediately in the Python console window. In this case, the scripts is runned in another thread which is different from the main UI thread. In Windows, UI thread vars and functions are restricted to other threads. So, be caution with script calls to functions in UI thread. Classes in the main UI thread include: mainframe, view, layer, etc.

(3) is3.py provides some friendly classes and vars to facilitate use of iS3, such as adding views and layers. Please note that wrapper classes to mainframe, view, and layer are provided to overcome the restrictions of calls to the main UI thread. The wrapper classes are also called thread safe classes.

(4) Scripts located in /IS3/bin/PyPlugins/ will be automatically runned when program starts. It is a nice place to put your frequent use scripts here, such as user-defined toolboxes. See plugin-demo.py for more details.

