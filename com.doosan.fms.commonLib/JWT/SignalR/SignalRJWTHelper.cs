using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace com.doosan.fms.commonLib.JWT.SignalR
{
    public static class SignalRJWTHelper
    {
        private const int UNLIMITED_ADD_YEAR = 1000;
        #region Generate Token
        public static string GenerateJWTToken(string claimName, string role)
        {
            return GenerateJWTToken(SignalRPolicies.SecretKey, claimName, role, SignalRPolicies.Issuer, SignalRPolicies.Audience, DateTime.Now.AddYears(UNLIMITED_ADD_YEAR));
        }

        public static string GenerateJWTToken(string key, string claimName, string parmRole, string parmIssuser, string parmAudience, DateTime parmExpires)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (string.IsNullOrEmpty(claimName)) return string.Empty;

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, (string.IsNullOrEmpty(claimName) ? "" : claimName)),
                new Claim(SignalRPolicies.Role, (string.IsNullOrEmpty(parmRole)) ? SignalRPolicies.RoleNone : parmRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: parmIssuser,
                audience: parmAudience,
                claims: claims,
                expires: parmExpires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region Get item in JWT
        public static string GetRole(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return string.Empty;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
                return jwtSecurityToken.Claims.Where(x => x.Type == SignalRPolicies.Role).FirstOrDefault()?.Value;
            }
            catch (Exception exAll)
            {
                return "";
            }
        }
        public static string GetClaimName(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return string.Empty;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
                return jwtSecurityToken.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
            }
            catch (Exception exAll)
            {
                return "";
            }
        }

        public static bool GetValidToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return false;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64((jwtSecurityToken.Payload.Exp)));
                TimeSpan diff = (DateTime.Now) - (dateTimeOffset.LocalDateTime);
                return (diff.TotalMilliseconds > 0);
            }
            catch (Exception exAll)
            {
                return false;
            }
        }
        #endregion
    }
}
