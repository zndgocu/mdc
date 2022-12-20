using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.CustomUserProp.Interface;

namespace com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.CustomUserProp.Class
{
    public class CustomUser : ICustomUser
    {
        private string _userId;
        private string _userName;
        private string _jwt;


        public CustomUser()
        {
        }

        public string GetToken()
        {
            return _jwt;
        }

        public void Set(string token, string userId, string userName = "uknown")
        {
            _userId = userId;
            _userName = userName;
            _jwt = token;
        }

        public void Clear()
        {
            _userId = "";
            _userName = "";
            _jwt = "";
        }

        public string GetUserText()
        {
            return _userName;
        }

        public CustomUser Get()
        {
            return this;
        }
    }
}
