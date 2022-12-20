using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.DataObject;
using System.Collections.Generic;
using System.Linq;

namespace com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.CustomLanguageProp.Class
{
    public class CustomLang
    {
        /// <summary>
        /// T1 : 국가코드 : EN, T2 : Dictonary<key : 안녕하세요, HI  
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> _langGlobalDatas;
        private Dictionary<string, LangSupData> _langSupDatas;

        public CustomLang()
        {
            _langGlobalDatas = new Dictionary<string, Dictionary<string, string>>();
            _langSupDatas = new Dictionary<string, LangSupData>();
        }

        public void AddGlobal(string langCode, string langName, int langPrio, string langCss)
        {
            if (ContainsGlobal(langCode)) return;
            _langGlobalDatas.Add(langCode, new Dictionary<string, string>());
            _langSupDatas.Add(langCode, new LangSupData(langCode, langName, langCss, langPrio));
        }

        public string GetGlobalIcon(string langCode)
        {
            if (ContainsGlobal(langCode) == false) return LangSupData.LANG_DEFAULT_FLAG;
            return _langSupDatas[langCode].GetCss();
        }

        private bool ContainsGlobal(string langCode)
        {
            if (_langGlobalDatas.ContainsKey(langCode)) return true;
            return false;
        }

        public string GetLangCss(string langCode)
        {
            if (ContainsGlobal(langCode) == false) return LangSupData.LANG_DEFAULT_FLAG;
            return _langSupDatas[langCode].GetCss();
        }

        public List<LangSupData> GetLangSupData()
        {
            if (_langSupDatas.Values.Count < 1) return new List<LangSupData>();
            return _langSupDatas.Values.ToList();
        }

        private Dictionary<string, string> GetGlobal(string langCode)
        {
            if (ContainsGlobal(langCode) == false) return null;
            return _langGlobalDatas[langCode];
        }

        public void AddLangWord(string langCode, string langKey, string langWord)
        {
            Dictionary<string, string> global = GetGlobal(langCode);
            if (global == null) return;
            if (global.ContainsKey(langKey))
            {
                global[langKey] = langWord;
            }
            else
            {
                global.Add(langKey, langWord);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang">langcode : KR</param>
        /// <param name="requestLang">request lang : 안녕하세요</param>
        /// <returns></returns>
        public string GetLangWord(string requestWord, string langCode)
        {
            Dictionary<string, string> langCodeDatas;
            if (_langGlobalDatas.TryGetValue(langCode, out langCodeDatas) == false) return requestWord;

            string convertWordData;
            if (langCodeDatas.TryGetValue(requestWord, out convertWordData) == false) return requestWord;

            return convertWordData;
        }

    }
}
