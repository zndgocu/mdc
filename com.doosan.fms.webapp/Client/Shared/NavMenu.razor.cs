using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.Provider;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.Shared
{
    public partial class NavMenu
    {
        [Inject]
        public CustomLangProvider LangProvider { get; set; }

        protected override Task OnInitializedAsync()
        {
            LangProvider.EventChangedGlobal += OnChangedLang;
            return base.OnInitializedAsync();
        }

        private void OnChangedLang()
        {
            StateHasChanged();
        }
    }
}
