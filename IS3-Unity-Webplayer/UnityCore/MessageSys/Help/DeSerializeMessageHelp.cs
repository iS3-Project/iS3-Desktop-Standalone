using System;
using System.Collections;
using System.Collections.Generic;
namespace IS3.Unity.Webplayer.UnityCore
{

    public static class DeSerializeMessageHelp
    {

        public static object DeSerialize(string rawString)
        {
            try
            {
                int typeSplitIndex = rawString.IndexOf(':');
                string type = rawString.Substring(1, typeSplitIndex);
                string StringContent = rawString.Substring(typeSplitIndex + 1, rawString.Length - typeSplitIndex);
                switch (type)
                {
                    case "int":
                        return DeSerialize_int(StringContent);
                    case "string":
                        return Deserialize_string(StringContent);
                    case "List<int>":
                        return Deserialize_List_int(StringContent);
                    default: return "";

                }
            }
            catch { return ""; }
        }
        public static int DeSerialize_int(string StringContent)
        {
            return int.Parse(StringContent);
        }
        public static string Deserialize_string(string StringContent)
        {
            return StringContent;
        }
        public static List<int> Deserialize_List_int(string StringContent)
        {
            return new List<int>(
                Array.ConvertAll<string, int>(StringContent.Split(','), delegate (string s) { return int.Parse(s); }));
        }
    }
}

