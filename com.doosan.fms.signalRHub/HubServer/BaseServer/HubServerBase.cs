using com.doosan.fms.signalRHub.Extension.Provider.User;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.doosan.fms.signalRHub.HubServer.BaseServer
{
    public class HubServerBase<T> : Hub
    {
        protected ICustomUserProvider _userProv;
        public HubServerBase(ICustomUserProvider provider)
        {
            _userProv = provider;
        }

        public override Task OnConnectedAsync()
        {
            string message = string.Empty;
            _userProv.AddUser(Context, ref message);
            if (message != string.Empty) Console.WriteLine(message);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string message = "";
            _userProv.Removeuser(Context, ref message);
            if (message != string.Empty) Console.WriteLine(message);
            return base.OnDisconnectedAsync(exception);
        }

        protected async Task SendUserMessage(string userName, string funcName, T message)
        {

        }
        protected async Task SendUsersMessage(List<string> userNames, string funcName, T message)
        {
            await Clients.Clients(userNames).SendAsync(funcName, message);
        }

        protected async Task SendAllMessage(string funcName, T message)
        {
            try
            {
                await Clients.All.SendAsync(funcName, message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"여기 에러나면 안됨 1{exception.Message}");
            }
        }

    }
}
