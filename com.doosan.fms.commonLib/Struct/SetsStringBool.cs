using System.Text.Json.Serialization;

namespace com.doosan.fms.commonLib.Struct
{
    public class SetsStringBool
    {
        public void Sets(string str, bool b)
        {
            this.Str = str;
            this.B = b;
        }

        public SetsStringBool()
        {
        }

        public SetsStringBool(bool b)
        {
            B = b;
        }

        public SetsStringBool(string str)
        {
            Str = str;
        }

        public SetsStringBool(string str, bool b)
        {
            Str = str;
            B = b;
        }

        [JsonInclude]
        public string Str { get; set; } = "";
        [JsonInclude]
        public bool B { get; set; }
    }
}
