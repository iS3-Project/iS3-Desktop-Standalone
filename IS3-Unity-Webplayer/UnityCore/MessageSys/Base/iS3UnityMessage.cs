using System;
using System.Collections;
using System.Collections.Generic;

namespace iS3.Unity.Webplayer.UnityCore
{
    public class iS3UnityMessage : IMessage
    {
        private MessageType _type;
        public virtual MessageType type
        {
            get
            {
                return _type;
            }
        }
        public virtual string SerializeObject()
        {
            return "";
        }
        public virtual void DeSerializeObject(string message)
        {

        }
    }

}
