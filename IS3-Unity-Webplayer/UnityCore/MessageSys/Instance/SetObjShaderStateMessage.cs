using System.Collections;
using System.Collections.Generic;

namespace iS3.Unity.Webplayer.UnityCore
{
    public class SetObjShaderStateMessage : iS3UnityMessage
    {
        public override MessageType type { get { return MessageType.SetObjShaderState; } }

        public string path { get; set; }
        public bool setToTranslucent { get; set; }

        public override string SerializeObject()
        {
            return string.Format("{0};{1}", path, setToTranslucent.ToString());
        }
        public override void DeSerializeObject(string message)
        {
            string[] values = message.Split(';');
            path = values[0];
            setToTranslucent = bool.Parse(values[1]);
        }
    }
}

