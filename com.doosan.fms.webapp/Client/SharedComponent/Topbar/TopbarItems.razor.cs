using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.DataObject;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.SharedComponent.Topbar
{
    public partial class TopbarItems
    {
        private bool _openLangPanel = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override Task OnLoginAsync()
        {
            StateHasChanged();
            return base.OnLoginAsync();
        }

        protected override Task OnLogoutAsync()
        {
            StateHasChanged();
            return base.OnLogoutAsync();
        }

        private bool IsLogin()
        {
            if (string.IsNullOrEmpty(UserProvider.GetToken()))
            {
                return false;
            }
            return true;
        }

        private Color GetLoginIconColor()
        {
            if (IsLogin() == true)
            {
                return Color.Success;
            }
            else
            {
                return Color.Error;
            }
        }

        private string GetLoginId()
        {
            if (IsLogin() == true)
            {
                return UserProvider.GetUserText();
            }
            else
            {
                return "need Login";
            }
        }

        private void OnLoginButtonClick()
        {
            if (IsLogin() == true)
            {
                UserProvider.Logout();
            }
            else
            {
                NavigationManager.NavigateTo("/login");
            }
        }

        private string GetLoginButtonText()
        {
            if (IsLogin() == true)
            {
                return "Logout";
            }
            else
            {
                return "Login";
            }
        }

        private string GetLoginIcon()
        {
            if (IsLogin() == true)
            {
                return Icons.Material.Filled.AccountCircle;
            }
            else
            {
                return Icons.Material.Filled.NoAccounts;
            }
        }

        private void OnLangPanelButtonClick()
        {
            _openLangPanel = !_openLangPanel;
        }

        private void OnLangChangeButtonClick(LangSupData ntnSup)
        {
            ChangeGlobal(ntnSup.LangCode);
            OnLangPanelButtonClick();
        }

        protected override void OnChangedLang()
        {
            StateHasChanged();
            base.OnChangedLang();
        }
    }
}
