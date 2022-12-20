using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.CustomUserProp.Class;
using System;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.Provider
{
    public class CustomUserProvider
    {
        private CustomUser _user;
        public string LastMessage { get; set; } = "";
        public event Func<Task> EventOnLogin;
        public event Func<Task> EventOnLogout;

        public CustomUserProvider()
        {
            _user = new CustomUser();
        }
        public CustomUser Get()
        {
            return _user;
        }

        public bool IsLogin()
        {
            if (string.IsNullOrEmpty(_user.GetToken()))
            {
                return false;
            }
            return true;
        }

        public bool Login(string token, string userId, string userNm)
        {
            try
            {
                if (IsLogin())
                {
                    LastMessage = "already login";
                    return false;
                }
                _user.Set(token, userId, userNm);
                EventOnLogin?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Logout()
        {
            try
            {
                _user.Clear();
                EventOnLogout?.Invoke();
            }
            catch (Exception ex)
            {
            }
        }


        public string GetToken()
        {
            return _user.GetToken();
        }

        public string GetUserText()
        {
            return _user.GetUserText();
        }

        public void SetToken(string token)
        {
            _user.Set(token, "", "");
        }

    }
}
