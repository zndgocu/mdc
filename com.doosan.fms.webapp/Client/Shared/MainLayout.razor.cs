using com.doosan.fms.model.JoinModel;
using com.doosan.fms.webapp.Client.AppSetting;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.Provider;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.Provider;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.Shared
{
    public partial class MainLayout
    {
        [Inject]
        public CustomUserProvider UserProvider { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public CustomLangProvider LangProvider { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public Task<Settings> Settings { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UserProvider.EventOnLogout += OnLogout;
            UserProvider.EventOnLogin += OnLogin;
            if (await InitializeGlobal() == false)
            {
                SnackBar.Add("전역언어 설정에 실패했습니다.");
            }
            await base.OnInitializedAsync();
        }

        private string GetDefaultAddrss()
        {
            return Settings.Result.ApiUrl;
        }

        private async Task<bool> InitializeGlobal()
        {
            try
            {
                Ntn ntn = new Ntn();
                string url = $"{GetDefaultAddrss()}/fms/ntn/get";

                var result = await HttpClient.GetAsync(url);
                if (result.IsSuccessStatusCode == false) throw new Exception($"API CALL FAIL");

                var resultString = await result.Content.ReadAsStringAsync();
                ntn = JsonConvert.DeserializeObject<Ntn>(resultString);

                foreach (var ntnSup in ntn.NtnSup)
                {
                    LangProvider.AddGlobal(ntnSup.Ntn_cd, ntnSup.Ntn_nm, ntnSup.Prio, ntnSup.Ntn_css);
                }

                foreach (var ntnLang in ntn.NtnLang)
                {
                    LangProvider.AddLangWord(ntnLang.Ntn_cd, ntnLang.Ntn_default, ntnLang.Ntn_val);
                }
                LangProvider.Initialized = true;
                LangProvider.ChangeGlobal();
                return true;
            }
            catch (Exception ex)
            {
                LangProvider.Initialized = false;
                SnackBar.Add(ex.Message, Severity.Error);
                return false;
            }
            //LangProvider.Initialized = true;
        }

        public async Task OnLogin()
        {
            NavigationManager.NavigateTo("/");
        }
        public async Task OnLogout()
        {
            NavigationManager.NavigateTo("/login");
        }
    }
}
