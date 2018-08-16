using System.Collections;
using System.Collections.Generic;
namespace IS3.Unity.Webplayer.UnityCore
{
    public class LoadingCompleteMessage :iS3UnityMessage
    {
        public override MessageType type { get { return MessageType.LoadingComplete; } }

        public bool isLoadComplete { get; set; }

        public override string SerializeObject()
        {
            return string.Format("{0}", isLoadComplete.ToString());
        }
        public override void DeSerializeObject(string message)
        {
            isLoadComplete = bool.Parse(message);
        }
    }
}