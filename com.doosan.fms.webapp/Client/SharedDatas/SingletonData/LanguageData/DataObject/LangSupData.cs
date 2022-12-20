namespace com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.DataObject
{
    public class LangSupData
    {
        public const string LANG_DEFAULT_FLAG = "flag-icon flag-icon-kr";
        public const string LANG_DEFAULT_CODE = "kr";

        public string LangCode;
        public string Name;
        public string Css;
        public int Prio;

        public LangSupData(string langCode, string name, string css, int prio)
        {
            LangCode = langCode;
            Name = name;
            Css = css;
            Prio = prio;
        }

        public string GetCss()
        {
            if (string.IsNullOrEmpty(Css)) return LANG_DEFAULT_FLAG;
            return Css;
        }
    }
}
