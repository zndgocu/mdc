using com.doosan.fms.model.Auth;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentInputForm.DataObject;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentInputForm.RazorPage;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.Pages.Common
{
    public partial class Login
    {
        private const string ID = "ID";
        private const string PW = "PW";

        private ComponentInputForm _ref { get; set; }
        private InputFormDataObject _inputFormDataObject;

        protected override Task OnInitializedAsync()
        {
            _inputFormDataObject = new InputFormDataObject();
            _inputFormDataObject.AddInputData(new InputData(ID, Icons.Filled.SupervisorAccount, InputType.Text)
                                            , new InputData(PW, Icons.Filled.SupervisorAccount, InputType.Password));
            ButtonData loginButton = new ButtonData("Login");
            loginButton.EventButtonClick += OnClickLogin;
            _inputFormDataObject.AddButtonData(loginButton);
            return base.OnInitializedAsync();
        }
        public async void OnClickLogin(ButtonData buttonData)
        {
            if (buttonData.ButtonText == "Login")
            {
                if (UserProvider.IsLogin())
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    AuthData auth = new AuthData();
                    try
                    {
                        string id = _inputFormDataObject.InputDatas.Where(x => x.InputDescText == ID).FirstOrDefault().InputText;
                        string pw = _inputFormDataObject.InputDatas.Where(x => x.InputDescText == PW).FirstOrDefault().InputText;
                        auth = await TryLogin(id, pw);
                        if (auth == null) throw new System.Exception("auth null");
                        if (string.IsNullOrEmpty(auth.Jwt)) throw new System.Exception($"jwt null {auth.Message}");
                        if (UserProvider.Login(auth.Jwt, id, auth.Name) == false)
                        {
                            throw new System.Exception(UserProvider.LastMessage);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        SnackBar.Add(ex.Message);
                    }
                }
            }
        }

        protected override void OnChangedLang()
        {
            _ref.Reload();
            base.OnChangedLang();
        }


        protected override async ValueTask DisposeAsyncChild()
        {
            await base.DisposeAsyncChild();
        }
    }
}
