using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Collections;
using System.IO;

using IS3.Core;
using IS3.Core.Geometry;
using IS3.Core.Graphics;
using UnityCore.MessageSys;

namespace IS3.Desktop
{
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************
    public class IS3View3D : IS3ViewBase, IView
    {
        #region IView interface
        public ViewType type { get { return ViewType.General3DView; } }
        public IEnumerable<IGraphicsLayer> layers { get { return null; } }
        public IGraphicsLayer drawingLayer { get { return null; } }
        public ISpatialReference spatialReference { get { return null; } }
        public void initializeView(){}

        public void onClose(){}

        public void highlightObject(DGObject obj, bool on = true){
            if (obj == null || obj.parent == null || (obj.parent.definition.Has3D == false))
                return;

            SetObjSelectStateMessage message = new SetObjSelectStateMessage();
            message.path = obj.parent.definition.Layer3DName + "/" + obj.fullName;
            message.iSSelected = on;
            ExcuteCommand(message);
        }
        public void highlightObjects(IEnumerable<DGObject> objs, bool on = true){
            if (objs == null)
                return;
            foreach (DGObject obj in objs)
                highlightObject(obj, on);
        }
        public void highlightObjects(IEnumerable<DGObject> objs,
            string layerID, bool on = true){}
        public void highlightAll(bool on = true){}

        public IMapPoint screenToLocation(System.Windows.Point screenPoint)
        { return null; }
        public System.Windows.Point locationToScreen(IMapPoint mapPoint)
        { return new System.Windows.Point(); }

        public void addSeletableLayer(string layerID) { }
        public void removeSelectableLayer(string layerID) { }

        public void zoomTo(IGeometry geom) { }

        public void addLayer(IGraphicsLayer layer){}
        public IGraphicsLayer getLayer(string layerID)
        {
            return null;
        }
        public IGraphicsLayer removeLayer(string layerID)
        {
            return null;
        }

        public void addLocalTiledLayer(string filePath, string id) { }
        public Task<IGraphicsLayer> addGdbLayer(LayerDef layerDef,
            string dbFile, int start = 0, int maxFeatures = 0)
        {
            return null;
        }

        public Task<IGraphicsLayer> addShpLayer(LayerDef layerDef,
            string shpFile, int start = 0, int maxFeatures = 0) 
        {
            return null;
        }


        public int syncObjects()
        {
            return 0;
        }
        public async Task loadPredefinedLayers()
        {
            Load3DScene();
        }
        public void objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e) {
            if (sender == this)
                return;

            if (e.addedObjs != null)
            {
                foreach (string layerID in e.addedObjs.Keys)
                    highlightObjects(e.addedObjs[layerID], true);

            }
            if (e.removedObjs != null)
            {
                foreach (string layerID in e.removedObjs.Keys)
                    highlightObjects(e.removedObjs[layerID], false);
            }
        }
        public event EventHandler<ObjSelectionChangedEventArgs>
            objSelectionChangedTrigger;
        public event EventHandler<DrawingGraphicsChangedEventArgs>
            drawingGraphicsChangedTrigger;
        #endregion

        U3DPlayerAxLib.U3DPlayerControl _u3dPlayerControl;

        public IS3View3D(UserControl parent,
            U3DPlayerAxLib.U3DPlayerControl u3dPlayerControl)
        {
            _parent = parent;
            _u3dPlayerControl = u3dPlayerControl;
        }
        public bool IsValidFileName(string filename)
        {

            if (filename == null || filename.Count() == 0)
                return false;
            else
                return true;
        }
        public EventHandler<IS3ToUnityArgs> sendMessageEventHandler;
        public EventHandler<UnityToIS3Args> receiveMessageHandler;

        public void Load3DScene()
        {
            // check file exists
            string filePath = _prj.projDef.LocalFilePath + "\\"
                + _eMap.LocalMapFileName;
            if (File.Exists(filePath))
            {
                _u3dPlayerControl.LoadScence(filePath);
            }
            _u3dPlayerControl.UnityCall += new U3DPlayerAxLib.U3DPlayerControl.ExternalCallHandler(_u3dPlayerControl_UnityCall);
            receiveMessageHandler += new EventHandler<UnityToIS3Args>(ReceiveMessageListener);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _u3dPlayerControl_UnityCall(object sender, AxUnityWebPlayerAXLib._DUnityWebPlayerAXEvents_OnExternalCallEvent e)
        {
            try
            {
                string message = e.value;
                string[] list = message.Split('"');
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].StartsWith("@"))
                    {
                        iS3UnityMessage myMessage = MessageConverter.DeSerializeMessage(list[i]);
                        switch (myMessage.type)
                        {
                            case MessageType.SendUnityLayer:
                                break;
                            case MessageType.SetObjSelectState:
                                SetObjSelectStateMessage _message = myMessage as SetObjSelectStateMessage;
                                string _path = _message.path;
                                int id = int.Parse(_path.Split('/')[_path.Split('/').Length - 1]);
                                bool isSelected = _message.iSSelected;
                                DGObject obj = null;
                                foreach (string key in prj.objsLayerIndex.Keys)
                                {
                                    DGObjects objs = prj.objsLayerIndex[key];
                                    if ((objs.definition.Has3D) && (_path.StartsWith(objs.definition.Layer3DName)))
                                    {
                                        foreach (DGObject _obj in objs.values)
                                        {
                                            if (_obj.fullName == id.ToString())
                                            {
                                                obj = _obj;
                                                break;
                                            }
                                        }

                                    }
                                }
                                if (obj != null && objSelectionChangedTrigger != null)
                                {
                                    ObjSelectionChangedEventArgs args = new ObjSelectionChangedEventArgs();
                                    if (isSelected)
                                    {
                                        args.addedObjs = new Dictionary<string, IEnumerable<DGObject>>();
                                        List<DGObject> objs = new List<DGObject>() { obj };
                                        args.addedObjs.Add(obj.parent.definition.GISLayerName, objs);
                                    }
                                    else
                                    {
                                        args.removedObjs = new Dictionary<string, IEnumerable<DGObject>>();
                                        List<DGObject> objs = new List<DGObject>() { obj };
                                        args.removedObjs.Add(obj.parent.definition.GISLayerName, objs);
                                    }
                                    objSelectionChangedTrigger(this, args);
                                }
                                break;
                            case MessageType.SetObjShowState:
                                break;
                            default: break;
                        }
                    }
                }
            }
            catch { }

        }
        private void ReceiveMessageListener(object sender, UnityToIS3Args args)
        {
            //switch (args.methodType)
            //{
            //    case UnityToIS3Method.LoadComplete: break;
            //    case UnityToIS3Method.Select:
            //        SelectObjByName(args.info);
            //        break;
            //    default: break;
            //}
        }
        public void ExcuteCommand(iS3UnityMessage message)
        {
            ExcuteCommand(MessageConverter.SerializeMessage(message));
        }
        public void ExcuteCommand(string command)
        {
            _u3dPlayerControl.SendMessage("Main Camera", "ReceiveMessage", command);
        }
        #region  receive function
        public void SelectObjByName(string message)
        {
            try
            {
                string nameInfo = message.Split(',')[0];
                bool _state = (message.Split(',')[1].ToUpper()) == "TRUE" ? true : false;
                DGObject obj = TurnNameToObj(nameInfo);
                if (obj != null && objSelectionChangedTrigger != null)
                {
                    ObjSelectionChangedEventArgs args = new ObjSelectionChangedEventArgs();
                    if (_state)
                    {
                        args.addedObjs = new Dictionary<string, IEnumerable<DGObject>>();
                        List<DGObject> objs = new List<DGObject>() { obj };
                        args.addedObjs.Add(obj.parent.definition.GISLayerName, objs);
                    }
                    else
                    {
                        args.removedObjs = new Dictionary<string, IEnumerable<DGObject>>();
                        List<DGObject> objs = new List<DGObject>() { obj };
                        args.removedObjs.Add(obj.parent.definition.GISLayerName, objs);
                    }
                    objSelectionChangedTrigger(this, args);
                }
            }
            catch { }

        }
        public DGObject TurnNameToObj(string nameInfo)
        {
            try
            {
                string[] nameList = nameInfo.Split('+');
                string projectName = nameList[0];
                string domainName = nameList[1];
                string objDefName = nameList[2];
                string objName = nameList[3];
                DGObject obj = Globals.project.domains[domainName].objsContainer[objDefName][objName];
                return obj;
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region send function
        public string TurnObjToName(DGObject obj)
        {
            string result = obj.name;
            result = obj.parent.definition.Name + "+" + result;
            result = obj.parent.parent.name + "+" + result;
            result = Globals.project.projDef.ID + "+" + result;
            return result;
        }


        #endregion
    }
    #region 方法枚举
    public enum IS3ToUnityMethod
    {
        SetObjShowByName,
        SetObjShowByType,
        SetObjSelectByName,
        SetObjSelectByType,
        SetAllObjSelectState,
        SetObjPosByName,
        SetObjPosByType,
        MoveObjPosByName,
        MoveObjPosByType,
        QueryPosByName
    }
    public enum UnityToIS3Method
    {
        LoadComplete,
        Select,
    }
    #endregion
    #region 事件定义
    /// <summary>
    /// Reveive Message From Unity Event
    /// </summary>
    public class UnityToIS3Args : EventArgs
    {
        public UnityToIS3Method methodType;
        public string info;
    }
    /// <summary>
    /// unity success load
    /// </summary>
    public class IS3ToUnityArgs : EventArgs
    {
        public string obj { get; set; }
        public IS3ToUnityMethod method { get; set; }
        public string para { get; set; }
    }
    #endregion
}
