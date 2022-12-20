using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using com.doosan.fms.model.Runtime;
using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

/// <summary>
//C - Post { List<T>, T } 2
//R - Get { List<T> } 1 
//U - Put { int } 1 
//D - Delete { int } 1 

namespace com.doosan.fms.webapp.Server.Controllers.Fms
{
    [ApiController]
    //[Route("fms/[controller]")]
    [Route("fms/usermst")]
    public class UserMstController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDatabaseManger _manager;

        public UserMstController(IConfiguration config, IDatabaseManger manager)
        {
            _config = config;
            _manager = manager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("posts")] //htt~/fms/usermst/post
        public List<UserMst> Posts([FromBody] List<UserMst> items)
        {
            string message = "";
            string qry;

            try
            {
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);

                _manager.TryBeginTrans(ref message);

                foreach (var item in items)
                {
                    qry = item.GetCreateQuery();
                    var result = _manager.ExcuteNonQuery(qry);
                    if (result.Success == false) throw new Exception(result.Message);
                }

                _manager.CommitTrans(ref message);
            }
            catch (Exception ex)
            {
                _manager.RollbackTrans(ref message);
                Console.WriteLine(ex.Message);
                return null;
            }
            return items;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("post")] //htt~/fms/usermst/post
        public UserMst Post([FromBody] UserMst item)
        {
            string message = "";
            string qry;

            try
            {
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);

                _manager.TryBeginTrans(ref message);

                qry = item.GetCreateQuery();
                var result = _manager.ExcuteNonQuery(qry);
                if (result.Success == false) throw new Exception(result.Message);

                _manager.CommitTrans(ref message);
            }
            catch (Exception ex)
            {
                _manager.RollbackTrans(ref message);
                Console.WriteLine(ex.Message);
                return null;
            }
            return item;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get/{id?}")] //htt~/fms/vertex/get
        public List<UserMst> Get(string? id)
        {
            string message = "";
            List<UserMst> users = new List<UserMst>();

            try
            {
                string qry = FmsModel.GenInitializeQuery<UserMst>();
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);
                var result = _manager.ExcuteQuery(qry);
                foreach (DataRow row in result.Table.Rows)
                {
                    UserMst v = new UserMst();
                    v.SetMember(row);
                    users.Add(v);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return users;
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("puts")] //htt~/fms/vertex/put
        public int Put([FromBody] List<UserMst> items)
        {
            //db result 제네릭 형태로 만들기
            string message = "";
            string qry;
            try
            {
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);

                _manager.TryBeginTrans(ref message);

                foreach (var item in items)
                {
                    qry = item.GetUpdateQuery();
                    var result = _manager.ExcuteNonQuery(qry);
                    if (result.Success == false) throw new Exception(result.Message);
                }

                _manager.CommitTrans(ref message);
            }
            catch (Exception ex)
            {
                _manager.RollbackTrans(ref message);
                Console.WriteLine(ex.Message);
                return -1;
            }
            return items.Count;
        }

        [HttpDelete]
        [AllowAnonymous]
        [Route("delete/{user_id?}")] //htt~/fms/vertex/delete
        public int Delete(string? user_ids)
        {
            string message = "";
            string qry;
            List<string> user_idList = user_ids.UnBoxSeperator();

            try
            {
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);

                _manager.TryBeginTrans(ref message);

                foreach (var user_id in user_ids)
                {
                    //qry = FmsModel.GetDeleteQuery<UserMst>();
                    //var result = _manager.ExcuteNonQuery(qry);
                    //if (result.Success == false) throw new Exception(result.Message);
                }

                _manager.CommitTrans(ref message);
            }
            catch (Exception ex)
            {
                _manager.RollbackTrans(ref message);
                Console.WriteLine(ex.Message);
                return -1;
            }

            return user_idList.Count;
        }
    }
}
