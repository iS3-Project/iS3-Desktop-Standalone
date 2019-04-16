using System;
using System.Collections;
using System.Collections.Generic;
namespace iS3.Unity.Webplayer.UnityCore
{

    public static class SerializeMessageHelp
    {

        /// <summary>
        /// 2  -->  (int:2)
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string Serialize(int para)
        {
            return string.Format("({1}:{2})", "int", para.ToString());
        }
        /// <summary>
        /// test   --> (string:test)
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string Serialize(string para)
        {
            return string.Format("({1}:{2}", "string", para);
        }
        /// <summary>
        /// 0,1,2,3   --> (List<int>:0,1,2,3)
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string Serialize(List<int> para)
        {
            return string.Format("({0}:{1})", "List<int>", string.Join(",",
                Array.ConvertAll<int, string>(para.ToArray(), delegate (int i) { return i.ToString(); })));
        }
    }
}