using System;
using System.Collections;
using System.Collections.Generic;
namespace IS3.Unity.Webplayer.UnityCore
{
    public static class MessageConverter
    {
        //@type@params
        public static string SerializeMessage(iS3UnityMessage message)
        {
            return (string.Format("@{0}@{1}",message.type,message.SerializeObject()));
        }
        //@type@params
        public static iS3UnityMessage  DeSerializeMessage(string message)
        {
            try
            {
                string[] list = message.Split('@');
                MessageType type = (MessageType)Enum.Parse(typeof(MessageType), list[1]);
                switch (type)
                {
                    case MessageType.LoadingComplete:
                        LoadingCompleteMessage _message1 = new LoadingCompleteMessage();
                        _message1.DeSerializeObject(list[2]);
                        return _message1;
                    case MessageType.SendUnityLayer:
                        SendUnityLayerMessage _message2 = new SendUnityLayerMessage();
                        _message2.DeSerializeObject(list[2]);
                        return _message2;
                    case MessageType.SetObjSelectState:
  
                        SetObjSelectStateMessage _message3 = new SetObjSelectStateMessage();
                        _message3.DeSerializeObject(list[2]);
                        return _message3;
                    case MessageType.SetObjShowState:
                        SetObjShowStateMessage _message4 = new SetObjShowStateMessage();
                        _message4.DeSerializeObject(list[2]);
                        return _message4;
                    default: return null;
                }
            }
            catch { return null; }
        }
    }
}