using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.doosan.fms.signalRHub.Extension.Provider.User
{
    public class CustomUserProvider : ICustomUserProvider
    {
        /// <summary>
        /// identity, claimName
        /// </summary>
        private Dictionary<string, string> _mappers;

        private object _lock;

        public CustomUserProvider()
        {
            _mappers = new Dictionary<string, string>();
            _lock = new object();
        }

        #region try catch locking block template
        /*
        try
        {
            lock (_lock)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
         */
        #endregion


        public List<string> GetUserIdentityListExcept(ref string message, params string[] exceptClaimNames)
        {
            try
            {
                lock (_lock)
                {
                    if (exceptClaimNames == null || exceptClaimNames.Length < 1) return default(List<string>);
                    return _mappers.Where(x => (exceptClaimNames.Contains(x.Value) == false))?.Select(x => x.Key)?.ToList();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return default(List<string>);
            }
        }

        public string GetUserIdentityFirstOrDefault(string claimName, ref string message)
        {
            try
            {
                lock (_lock)
                {
                    return _mappers.Where(x => x.Value == claimName)?.Select(x => x.Value).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return string.Empty;
            }
        }

        public List<string> GetUserIdentityList(string claimName, ref string message)
        {
            try
            {
                lock (_lock)
                {
                    return _mappers.Where(x => x.Value == claimName)?.Select(x => x.Value)?.ToList();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return default(List<string>);
            }
        }


        public bool AddUser(HubCallerContext connection, ref string message)
        {
            try
            {
                lock (_lock)
                {
                    if (_mappers.ContainsKey(connection.ConnectionId) == true)
                    {
                        message = $"already exist user id : {connection.ConnectionId}";
                        return false;
                    }
                    _mappers.Add(connection.ConnectionId, connection.User.Identity.Name);
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool Removeuser(HubCallerContext connection, ref string message)
        {
            try
            {
                lock (_lock)
                {
                    if (_mappers.ContainsKey(connection.ConnectionId) == false)
                    {
                        message = $"uncontains user id : {connection.ConnectionId}";
                        return false;
                    }
                    _mappers.Remove(connection.ConnectionId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

    }
}
