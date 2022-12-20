using System.Collections.Generic;

namespace com.doosan.fms.commonLib.JWT.SignalR
{
    public static class SignalRPolicies
    {
        #region wcs_sha_key sha256
        public const string SecretKey = "BE34A5923224C46D9EF47AFA47049F9C5AD8C5C018266A8EADC57D2CC7B3FF9B";
        public const string Issuer = "SignalR";
        public const string Audience = "com.doosan.fms.commonLib";
        #endregion

        #region roles
        public static List<string> Roles = new List<string>() { RoleRequest, RoleReceive, RoleNone };
        public const string Role = "role";

        public const string RoleRequest = "Reqest";
        public const string RoleReceive = "Receive";
        public const string RoleNone = "None";
        #endregion

        #region claimProp Extension
        //ClaimTypes
        //public const string Expires = "exp";
        //public const string UserId = "UserId";
        //public const string UserName = "UserName";
        #endregion
    }
}
