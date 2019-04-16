using System.Collections;
using System.Collections.Generic;
using System;

namespace iS3.Unity.Webplayer.UnityCore
{
    public class SendUnityLayerMessage : iS3UnityMessage
    {
        public override MessageType type { get { return MessageType.SendUnityLayer; } }

        public UnityLayer MyUnityLayer
        {
            get
            {

                    return myUnityLayer;
                
            }

            set
            {
                myUnityLayer = value;
            }
        }
        //锁住
        UnityLayer myUnityLayer = new UnityLayer();
        private Dictionary<string, string> objWithParent = new Dictionary<string, string>();
        private Dictionary<string, int> objWithLayer = new Dictionary<string, int>();

        public override string SerializeObject()
        {
           
            return null;
        }
        public override void DeSerializeObject(string message)
        {
            //清空objWithParent和objWithLayer
            objWithParent.Clear();
            objWithLayer.Clear();

            //先解析特殊数据和objWithParent和objWithLayer
            String[] strs = message.Split(new String[] {"%"},StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length<3)
            {
                return;
            }
            //特殊处理
            UnityTreeModel treeModel = MyUnityLayer.UnityLayerModel;
            String[] specialStr = strs[0].Split(new String[] {","},StringSplitOptions.RemoveEmptyEntries);
            treeModel.Name = specialStr[0];
            treeModel.layer = int.Parse(specialStr[1]);


            //第一个是父子关系
            String[] parentRealtive = strs[1].Split(new String[] {";"},StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parentRealtive.Length; i++)
            {
                String[] twoParams = parentRealtive[i].Split(new String[] {","},StringSplitOptions.RemoveEmptyEntries);
                if (twoParams.Length>=2)
                {
                    if (!objWithParent.ContainsKey(twoParams[0]))
                    {
                        objWithParent.Add(twoParams[0], twoParams[1]);
                    }
                }
                else
                {
                    return;
                }
            }
            //第二个是节点关系
            String[] layerRealtive = strs[2].Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < layerRealtive.Length; i++)
            {
                if ((layerRealtive[i].StartsWith("(") || (layerRealtive[i].StartsWith(")")))) continue;
                String[] twoParams = layerRealtive[i].Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (twoParams.Length >= 2)
                {
                    if (!objWithLayer.ContainsKey(twoParams[0]))
                    {
                        objWithLayer.Add(twoParams[0], int.Parse(twoParams[1]));
                    }
                }
                else
                {
                    return;
                }
            }

            //开始填充
            SetLayerAndChild(treeModel);
        }

        //反解析设置层和子物体
        public void SetLayerAndChild(UnityTreeModel parent)
        {
            try {
                List<String> strs = FindAllValueByStr(parent.Name);
                if (strs.Count <= 0)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < strs.Count; i++)
                    {
                        UnityTreeModel model = new UnityTreeModel();
                        model.childs = new List<UnityTreeModel>();
                        model.Name = strs[i];
                        if (objWithLayer.ContainsKey(model.Name))
                        {
                            model.layer = objWithLayer[model.Name];
                        }
                        parent.childs.Add(model);
                        model.parent = parent;
                        SetLayerAndChild(model);
                    }
                }
            }
            catch (Exception ex)
            { }
        }


        private List<String> FindAllValueByStr(string value)
        {
            List<String> strs = new List<string>();
            foreach (KeyValuePair<string,string> item in objWithParent)
            {
                if (item.Value == value)
                {
                    strs.Add(item.Key);
                }
            }
            return strs;
        }
        //处理单行obj
        private void AddLayer(string name,int layer)
        {
            if (!objWithLayer.ContainsKey(name))
            {
                objWithLayer.Add(name,layer);
            }
        }
        private void AddParent(string childName,string parentName)
        {
            if (!objWithParent.ContainsKey(childName))
            {
                objWithParent.Add(childName, parentName);
            }
        }
    }
}