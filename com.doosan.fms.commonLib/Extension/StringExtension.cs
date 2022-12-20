using System.Collections.Generic;
using System.Linq;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;


namespace com.doosan.fms.commonLib.Extension
{
    public static class StringExtension
    {
        public static string BoxSeperator(string seprator = KEY_SEPERATOR, params string[] strs)
        {
            string result = "";
            foreach (var str in strs)
            {
                result.Add(str, KEY_SEPERATOR);
            }
            return result;
        }

        public static List<string> UnBoxSeperator(this string str, string seprator = KEY_SEPERATOR)
        {
            return str.Split(seprator).ToList();
        }

        public static string Add(this string str, params string[] strs)
        {
            foreach (string strstr in strs)
            {
                str = str + strstr;
            }
            return str;
        }
        public static string Quot(this string str)
        {
            return $"\'{str}\'";
        }

        public static string GenerateKey(params string[] strs)
        {
            string result = "";
            foreach (string str in strs)
            {
                result = result.Add(result, str, KEY_SEPERATOR);
            }
            return result;
        }
    }
}
