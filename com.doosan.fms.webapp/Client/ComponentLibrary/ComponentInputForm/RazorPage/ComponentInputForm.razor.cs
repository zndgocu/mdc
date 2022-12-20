using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentInputForm.DataObject;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.Provider;
using Microsoft.AspNetCore.Components;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentInputForm.RazorPage
{
    public partial class ComponentInputForm
    {
        [Parameter]
        public InputFormDataObject InputFormDataObject { get; set; }

        [Parameter]
        public CustomLangProvider LangProvider { get; set; }

        public void OnButtonClick(ButtonData button)
        {
            button.EventButtonClick?.Invoke(button);
        }

        public void Reload()
        {
            StateHasChanged();
        }

        public string GetLangWord(string word)
        {
            if (LangProvider == null) return word;
            return LangProvider.GetLangWord(word);
        }
    }
}
