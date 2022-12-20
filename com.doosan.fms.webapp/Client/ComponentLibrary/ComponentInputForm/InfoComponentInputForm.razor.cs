using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentInputForm.DataObject;
using MudBlazor;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentInputForm
{
    public partial class InfoComponentInputForm
    {
        RazorPage.ComponentInputForm _ref;
        InputFormDataObject _dataObject;
        protected override async Task OnInitializedAsync()
        {
            _dataObject = new InputFormDataObject();
            _dataObject.AddInputData(new InputData("ID", Icons.Filled.SupervisorAccount, InputType.Text), new InputData("PW", Icons.Filled.SupervisorAccount, InputType.Password));
            ButtonData loginButton = new ButtonData("Login");
            loginButton.EventButtonClick += OnButtonClick;
            _dataObject.AddButtonData(loginButton);
            await base.OnInitializedAsync();
        }

        public void OnButtonClick(ButtonData buttonData)
        {
            if (buttonData.ButtonText == "Login")
            {
                var a = 0;
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
