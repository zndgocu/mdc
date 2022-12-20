using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.commonLib.JWT.Fms;
using com.doosan.fms.commonLib.JWT.SignalR;
using com.doosan.fms.model.Auth;
using com.doosan.fms.model.Runtime;
using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.webapp.Server.Controllers.Auth
{
    [ApiController]
    //[Route("fms/[controller]")]
    [Route("identity")]
    public class IdentityController
    {
        private readonly IConfiguration _config;
        private readonly IDatabaseManger _manager;

        public IdentityController(IConfiguration config, IDatabaseManger manager)
        {
            _config = config;
            _manager = manager;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public string Get()
        {
            return SignalRJWTHelper.GenerateJWTToken(Guid.NewGuid().ToString(), SignalRPolicies.RoleReceive);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get/{id}/{pw}")]  //identity/get/id/pw
        public AuthData Get(string id, string pw)
        {
            AuthData auth = new AuthData();
            auth.Id = id;
            string message = "";
            try
            {
                string qry = qry = $" select * {CRLF}" +
                                   $"   from fms.user_mst {CRLF}" +
                                   $"  where user_id = {id.Quot()}" +
                                   $"   limit 1";
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);
                var result = _manager.ExcuteQuery(qry);
                if (result.Result_count < 1)
                {
                    auth.Message = "ID NOT FOUND";
                    return auth;
                }

                UserMst user = new UserMst();
                user.SetMember(result.Table.Rows[0]);

                if (user.User_pw != pw)
                {
                    auth.Message = "different Password";
                    return auth;
                }

                auth.Name = user.User_nm;
                auth.Jwt = FmsJWTHelper.GenerateJWTToken(auth.Name, FmsPolicies.RoleWorker);
                return auth;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
