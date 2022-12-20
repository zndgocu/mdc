using com.doosan.fms.commonLib.JWT.SignalR;
using com.doosan.fms.signalRHub.Extension.Provider.User;
using com.doosan.fms.signalRHub.HubServer.BaseServer;
using com.doosan.fms.signalRHub.SignalR.DataSet;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace com.doosan.fms.signalRHub.HubServer
{
    public class FmsDataHubServer<T> : HubServerBase<T> where T : FmsData, new()
    {
        private static T _hubSrvFuncMdcInitData = null;
        public FmsDataHubServer(ICustomUserProvider provider) : base(provider)
        {
        }

        /// [Authorize(Roles = SignalRPolicies.RoleReceive)]
        public async Task HubSrvFuncReqeustMdcInitData(string message)
        {
            if (_hubSrvFuncMdcInitData != null)
            {
                await Clients.Caller.SendCoreAsync(FmsData.HUB_CLI_FUNC_REQUEST_MDCINIT_DATA, new object[] { _hubSrvFuncMdcInitData });
            }
        }

        [Authorize(Roles = SignalRPolicies.RoleRequest)]
        public async void HubSrvFuncMdcInit(T data)
        {
            _hubSrvFuncMdcInitData = data;
            await Clients.Caller.SendCoreAsync(FmsData.HUB_CLI_FUNC_MDCINIT, new object[] { });
        }

        [Authorize(Roles = SignalRPolicies.RoleRequest)]
        public async Task HubSrvFuncBroadCastMdc(T data)
        {
            string message = "";
            var broadCastLists = _userProv.GetUserIdentityListExcept(ref message, FmsData.HUB_REQUEST_IDENTITY_NAME);
            if (broadCastLists == null || broadCastLists.Count < 1) return;
            await SendUsersMessage(broadCastLists, FmsData.HUB_CLI_FUNC_BROADCAST_MDC, data);
        }
    }
}
