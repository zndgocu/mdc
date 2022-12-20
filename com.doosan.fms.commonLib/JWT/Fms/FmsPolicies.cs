using System.Collections.Generic;

namespace com.doosan.fms.commonLib.JWT.Fms
{
    public class FmsPolicies
    {
        #region wcs_sha_key sha256
        public const string SecretKey = "BE34A5923224C46D9EF47AFA47049F9C5AD8C5C018266A8EADC57D2CC7B3FF9B";
        public const string Issuer = "Fms";
        public const string Audience = "com.doosan.fms.commonLib";
        #endregion

        #region roles
        public static List<string> Roles = new List<string>() { RoleDeveloper, RoleAdmin, RoleWorker };
        public const string Role = "role";

        public const string RoleDeveloper = "Developer";
        public const string RoleAdmin = "Administrator";
        public const string RoleWorker = "Worker";
        #endregion

        #region claimProp Extension
        //ClaimTypes
        //public const string Expires = "exp";
        #endregion
    }
}
