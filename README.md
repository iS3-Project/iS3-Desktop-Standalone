# iS3-Desktop-Standalone

The standalone version of iS3 application for Desktop

## 1. Runtime environment
--------
   1.The Microsoft .NET Framework 4.5 or higher version
   
   2.Win7 or higher version


## 2. Installer
-------------
No Chinese characters in the installer path!!
After downloading the stand-alone version from GitHub, we need to finish some preparations .

#### 2.1 Install 3 necessary plug-ins
There are installations for the necessary plug-ins in this folder: Plug-ins\
    ① IronPython-2.7.5.msi    
    ② UnityWebPlayer.exe      
    ③ UnityWebPlayerFull.exe 

Install them all.

#### 2.2 Generate the execute program(Can be skipped if not for development)

The executable program will be generated into the target folder: \Output. Open iS3-Desktop\iS3-Desktop\IS3-Desktop.sln.
Choose Project named ”IS3-Desktop”, Build it, then you will generate “IS3-Desktop.exe” in the target folder.

#### 2.3 Run the execute program
Run the execute file ”\Output\IS3-Desktop.exe”, then you need a username and a password to sign in. The username:”iS3Guest”.
The password:”iS3Guest”.

![image](https://github.com/iS3-Project/iS3-Desktop-Standalone/blob/master/images/Login.jpg)

#### 2.4 Some Problem during Install and Using
Read the file"Read Me For Installation.pdf"

## 3. Simple use manual
----------------
### 3.1 ProjectList View
After successfully User-Login, you can have a look of the list of project that be managered. click one of tip on the GisMap, you can view the detail information of that project.
![image](https://github.com/iS3-Project/iS3-Desktop-Standalone/blob/master/images/ProjectList.PNG)

### 3.2 Digital Object List in iS3
Here comes the mainframe for project managerment. you can view the GIS, 3D and data in this page. By Click the node on tree such as "监测点", data will show in the field of "DataList".
![image](https://github.com/iS3-Project/iS3-Desktop-Standalone/blob/master/images/DataList.PNG)

### 3.3 Property View
You can choose one enginneer object by click on GIS, 3D or DataList, the Property will show.
![image](https://github.com/iS3-Project/iS3-Desktop-Standalone/blob/master/images/PropertyShow.PNG)
