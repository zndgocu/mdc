using System.Net.Http;
using System.Text;

namespace com.doosan.fms.commonLib.Extension
{
    public static class HttpExtension
    {
        public const string MEDIATYPE_JSON = "application/json";
        public static StringContent ToStringContent(this string str, Encoding encode, string? mediaType)
        {
            if (str == null) return null;
            return new StringContent(str, encode, mediaType);
        }
    }
}
