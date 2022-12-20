using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace com.doosan.fms.commonLib.Extension
{
    public static class ListExtension
    {
        public static string ToStringLF(this List<string> list)
        {
            string result = "";
            foreach (var str in list)
            {
                result = $"{result}\r\n{str}";
            }
            return result;
        }
        public static string ToJson<T>(this List<T> list)
        {
            try
            {
                return JsonConvert.SerializeObject(list);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
