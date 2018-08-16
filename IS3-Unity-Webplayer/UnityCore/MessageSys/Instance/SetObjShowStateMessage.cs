using System.Collections;
using System.Collections.Generic;
namespace IS3.Unity.Webplayer.UnityCore
{
    public class SetObjShowStateMessage : iS3UnityMessage
    {

        public override MessageType type { get { return MessageType.SetObjShowState; } }
        /// <summary>
        /// /iS3Project/Geology/Borehole/1010101010
        /// </summary>
        public string path { get; set; }
        public bool iSShow { get; set; }
        public override string SerializeObject()
        {
            return string.Format("{0};{1}",path,iSShow.ToString());
        }
        public override void DeSerializeObject(string message)
        {
            string[] values = message.Split(';');
            path = values[0];
            iSShow = bool.Parse(values[1]);
        }
        
    }
}