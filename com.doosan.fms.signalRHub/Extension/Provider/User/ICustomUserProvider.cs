using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace com.doosan.fms.signalRHub.Extension.Provider.User
{
    public interface ICustomUserProvider
    {
        public bool AddUser(HubCallerContext connection, ref string message);
        public bool Removeuser(HubCallerContext connection, ref string message);
        public string GetUserIdentityFirstOrDefault(string claimName, ref string message);
        public List<string> GetUserIdentityList(string claimName, ref string message);
        public List<string> GetUserIdentityListExcept(ref string message, params string[] exceptClaimNames);
    }
}
