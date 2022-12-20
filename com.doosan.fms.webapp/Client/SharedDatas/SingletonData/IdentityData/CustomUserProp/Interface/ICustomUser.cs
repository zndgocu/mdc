using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.CustomUserProp.Class;

namespace com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.CustomUserProp.Interface
{
    public interface ICustomUser
    {
        public void Set(string token, string userId, string userName);
        public CustomUser Get();
        public string GetToken();
    }
}
