using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args;
using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Enum;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCard.DataObject;
using com.doosan.fms.webapp.Client.FmsMonitoringCore.Manager;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.Pages.Fms
{
    public partial class FmsMonitoring
    {
        #region 메뉴
        private const int MENU_SIZE = 3;

        private const string MENU_USER = "User";
        private const string MENU_COMMUNICATION = "Communication";
        private const string MENU_MONITORING = "Monitoring";
        private const string MENU_VEHICLE = "Vehicle";
        private const string MENU_COMMAND = "Command";
        private const string MENU_PORT = "Port";
        private const string MENU_ALARM = "Alarm";
        private const string MENU_REPORT = "Report";
        private const string MENU_LOG = "Log";

        private Dictionary<string, CardDataObject> _cardMenus = new Dictionary<string, CardDataObject>();

        private Dictionary<string, string> _menuContents = new Dictionary<string, string>()
        {
            {MENU_USER,          Icons.Filled.AccountCircle},
            {MENU_COMMUNICATION, Icons.Filled.CompareArrows},
            {MENU_MONITORING,    Icons.Filled.MonitorHeart},
            {MENU_VEHICLE,       Icons.Filled.DirectionsCar},
            {MENU_COMMAND,       Icons.Filled.LocalOffer},
            {MENU_PORT,          Icons.Filled.NetworkWifi},
            {MENU_ALARM,         Icons.Filled.AccessAlarms},
            {MENU_REPORT,        Icons.Filled.ReportGmailerrorred},
            {MENU_LOG,           Icons.Filled.FactCheck},
        };

        private Dictionary<string, List<HrefArgs>> _menuContentInner = new Dictionary<string, List<HrefArgs>>()
        {
            { MENU_USER,  new List<HrefArgs>(){
                            new HrefArgs("Login", "Login"),
                            new HrefArgs("Logout", "Logout"),
                            new HrefArgs("UserManage", "Manage"),
                          }
            },
            { MENU_COMMUNICATION,  new List<HrefArgs>(){
                            new HrefArgs("MESSETTING", "Mes ACS Setting"),
                            new HrefArgs("MESSETTING", "Mes EIF Setting"),
                          }
            },
            { MENU_MONITORING,  new List<HrefArgs>(){
                            new HrefArgs("VEHICLEMONITORING", "Vehicle Monitoring"),
                            new HrefArgs("SAFETYMONITORING", "Safety Monitoring"),
                            new HrefArgs("MESSAGEMONITORING", "Message Monitoring"),
                          }
            },
            { MENU_VEHICLE,  new List<HrefArgs>(){
                            new HrefArgs("VEHICLEMANAGE", "Vehicle Manage"),
                            new HrefArgs("STOPMODESETTING", "StopMode Manage"),
                            new HrefArgs("STANDBYMODESETTING", "Standby Manage"),
                          }
            },
            { MENU_COMMAND,  new List<HrefArgs>(){
                            new HrefArgs("COMMANDMANAGE", "Command Manage"),
                            new HrefArgs("MANUALTRANSFERCOMMAND", "Manual Transfer"),
                            new HrefArgs("MANUALMOVECOMMAND", "Manual Move"),
                            new HrefArgs("COMMANDHISTORY", "Command History"),
                          }
            },
            { MENU_PORT,  new List<HrefArgs>(){
                            new HrefArgs("PORTENABLE", "Port Enable Disable"),
                            new HrefArgs("PORTPRIORITYSETTING", "Port Priority Setting"),
                          }
            },
            { MENU_ALARM,  new List<HrefArgs>(){
                            new HrefArgs("ALARMAMSTER", "Alarm Master"),
                            new HrefArgs("ALARMHISTORY", "Alarm History"),
                          }
            },
            { MENU_REPORT,  new List<HrefArgs>(){
                            new HrefArgs("OPERATIONREPORT", "Operation Report"),
                          }
            },
            { MENU_LOG,  new List<HrefArgs>(){
                            new HrefArgs("SETTINGLOG", "Setting Log"),
                          }
            },
        };
        #endregion


        public const string CANVAS_ID = "canvas-body";

        [Inject]
        public HttpClient _httpClient { get; set; }

        [Inject]
        public ISnackbar _snackbar { get; set; }

        [Inject]
        public NavigationManager _navigationManager { get; set; }

        [Inject]
        public IJSRuntime _jsr { get; set; }

        private FmsMonitoringManager _manager;

        protected override Task OnInitializedAsync()
        {
            foreach (var content in _menuContents)
            {
                ImageArgs cardDataObjectImageArgs = new ImageArgs();
                cardDataObjectImageArgs.ImgType = ImageType.Icon;
                cardDataObjectImageArgs.ImgResource = content.Value;
                List<HrefArgs> cardDataObjectHerfArgs = new List<HrefArgs>();
                List<HrefArgs> hrefArgs;
                if (_menuContentInner.TryGetValue(content.Key, out hrefArgs) == false) continue;
                foreach (var contentMenu in hrefArgs)
                {
                    cardDataObjectHerfArgs.Add(contentMenu);
                }
                _cardMenus.Add(content.Key, new CardDataObject(cardDataObjectImageArgs, cardDataObjectHerfArgs));
                _cardMenus[content.Key].EventClick += OnMenuClick;
            }
            return base.OnInitializedAsync();
        }
        public void OnMenuClick(HrefArgs args)
        {
            _snackbar.Add($"menu click {args.GetKey()}");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender == true)
                {
                    var result = await _httpClient.GetAsync("https://localhost:44304/identity/get");
                    string auth = await result.Content.ReadAsStringAsync();
                    _manager = new FmsMonitoringManager(_jsr);
                    if (await _manager.Initialize() == false) throw new Exception();
                    if (await _manager.InitializeCore(DotNetObjectReference.Create(this), CANVAS_ID) == false) throw new Exception();
                    if (await _manager.StartWorker(_navigationManager.BaseUri, auth) == false) throw new Exception();
                }
            }
            catch (Exception exception)
            {
                _snackbar.Add(exception.Message);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
