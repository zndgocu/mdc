using com.doosan.fms.model.Auth;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.Provider;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.DataObject;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.Provider;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.RazorLibrary.Abstract
{
    /// <summary>
    /// how to use
    /// *.razor
    /// @using com.doosan.fms.webapp.Client.RazorLibrary.Abstract
    /// @inherits FmsRazorAbstract 
    /// </summary>
    public abstract class FmsRazorAbstract : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public CustomUserProvider UserProvider { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public CustomLangProvider LangProvider { get; set; }

        public string GetLang()
        {
            return LangProvider.GetLang();
        }
        public List<LangSupData> GetLangSupData()
        {
            return LangProvider.GetLangSupData();
        }
        public string GetLangCss(string langCode = "")
        {
            return LangProvider.GetLangCss(langCode);
        }

        public void ChangeGlobal(string langCode)
        {
            LangProvider.ChangeGlobal(langCode);
        }
        public void AddLangWord(string langCode, string langKey, string langWord)
        {
            LangProvider.AddLangWord(langCode, langKey, langWord);
        }
        public void AddGlobal(string langCode, string langName, int langPrio, string langCss)
        {
            LangProvider.AddGlobal(langCode, langName, langPrio, langCss);
        }
        public string GetLangWord(string requestWord, string langCode = "")
        {
            return LangProvider.GetLangWord(requestWord, langCode);
        }

        /// <summary>
        /// url 암호화된 되이터 js로 할지 고민할 것
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        public async Task<AuthData> TryLogin(string id, string pw)
        {
            AuthData authData = new AuthData();
            authData.Message = "LOGIN FAIL";

            string encryptId = id;
            string encryptPw = pw;

            string url = $"https://localhost:44304/identity/get/{id}/{pw}";

            //api call
            var result = await HttpClient.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                var resultString = await result.Content.ReadAsStringAsync();
                authData = JsonConvert.DeserializeObject<AuthData>(resultString);
            }

            return authData;
        }

        public async ValueTask DisposeAsync()
        {
            UserProvider.EventOnLogin -= OnLoginAsync;
            UserProvider.EventOnLogout -= OnLogoutAsync;
            LangProvider.EventChangedGlobal -= OnChangedLang;
            await DisposeAsyncChild();
        }

        protected virtual async ValueTask DisposeAsyncChild()
        {

        }

        protected override async Task OnInitializedAsync()
        {
            UserProvider.EventOnLogin += OnLoginAsync;
            UserProvider.EventOnLogout += OnLogoutAsync;
            LangProvider.EventChangedGlobal += OnChangedLang;
            await base.OnInitializedAsync();
        }

        protected virtual void OnChangedLang()
        {

        }

        protected virtual async Task OnLoginAsync()
        {
            await Task.Delay(0);
        }

        protected virtual async Task OnLogoutAsync()
        {
            await Task.Delay(0);
        }

    }
}
