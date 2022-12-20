using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.CustomLanguageProp.Class;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.DataObject;
using System;
using System.Collections.Generic;

namespace com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.Provider
{
    public class CustomLangProvider
    {
        public bool Initialized;
        private CustomLang _lang;
        private string _prevLangCode;

        public CustomLangProvider()
        {
            Initialized = false;
            _lang = new CustomLang();
            _prevLangCode = LangSupData.LANG_DEFAULT_CODE;
        }

        public string GetLang()
        {
            return (string.IsNullOrEmpty(_prevLangCode)) ? LangSupData.LANG_DEFAULT_CODE : _prevLangCode;
        }

        public event Action EventChangedGlobal;


        public void ChangeGlobal(string langCode = "")
        {
            if (string.IsNullOrEmpty(langCode) == false)
            {
                _prevLangCode = langCode;
            }
            EventChangedGlobal?.Invoke();
        }

        public void AddLangWord(string langCode, string langKey, string langWord)
        {
            _lang.AddLangWord(langCode, langKey, langWord);
        }

        public void AddGlobal(string langCode, string langName, int langPrio, string langCss)
        {
            _lang.AddGlobal(langCode, langName, langPrio, langCss);
        }

        public string GetLangWord(string requestWord, string langCode = "")
        {

            return _lang.GetLangWord(requestWord, langCode);
        }

        public string GetLangCss(string langCode)
        {
            if (string.IsNullOrEmpty(langCode)) return _lang.GetLangCss(_prevLangCode);
            return _lang.GetLangCss(langCode);
        }

        public List<LangSupData> GetLangSupData()
        {
            return _lang.GetLangSupData();
        }
    }
}
