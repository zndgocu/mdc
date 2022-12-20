namespace com.doosan.fms.signalRHub.SignalR.DataSet
{
    public class FmsData
    {
        public const string HUB_NAME = "FmsData";

        public const string HUB_REQUEST_IDENTITY_NAME = "MdcEnginePolling";

        public const string HUB_SRV_FUNC_MDCINIT = "HubSrvFuncMdcInit";
        public const string HUB_CLI_FUNC_MDCINIT = "HubCliFuncMdcInit";

        public const string HUB_SRV_FUNC_BROADCAST_MDCINIT = "HubSrvFuncBroadCastMdcInit";
        public const string HUB_CLI_FUNC_BROADCAST_MDCINIT = "HubCliFuncBroadCastMdcInit";

        public const string HUB_SRV_FUNC_REQUEST_MDCINIT_DATA = "HubSrvFuncReqeustMdcInitData";
        public const string HUB_CLI_FUNC_REQUEST_MDCINIT_DATA = "HubCliFuncReqeustMdcInitData";

        public const string HUB_SRV_FUNC_BROADCAST_MDC = "HubSrvFuncBroadCastMdc";
        public const string HUB_CLI_FUNC_BROADCAST_MDC = "HubCliFuncBroadCastMdc";

        private string _message = "test입니다";

        public FmsData()
        {
        }

        public FmsData(FmsData data)
        {
            _message = data.Message;
        }

        public string Message { get => _message; set => _message = value; }

        public void SetMessage(string message)
        {
            _message = message;
        }

    }
}
