using System.Collections;
using System.Collections.Generic;
namespace iS3.Unity.Webplayer.UnityCore
{
    /// <summary>
    /// path "/iS3Project/Geology"
    /// iSSelected  true
    /// 
    /// @SetObjSelectState@       ;       ;       {[]};
    /// </summary>
    public class SetObjSelectStateMessage : iS3UnityMessage
    {

        public override  MessageType type { get { return MessageType.SetObjSelectState; } }
        /// <summary>
        /// /iS3Project/Geology/Borehole/1010101010
        /// </summary>
        public string path { get; set; }
        public bool iSSelected { get; set; }


        ///  iS3Project/Geology；true
        public override string SerializeObject()
        {
            return string.Format("{0};{1}",path,iSSelected.ToString());
        }

        public override void DeSerializeObject(string message)
        {
            string[] list = message.Split(';');
            path = list[0];
            iSSelected = bool.Parse(list[1]);
        }
    }
}