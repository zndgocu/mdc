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
/// </summary>
namespace com.doosan.fms.webapp.Server.Controllers.Fms
{
    [ApiController]
    //[Route("fms/[controller]")]
    [Route("fms/vertex")]
    public class VertexController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDatabaseManger _manager;

        public VertexController(IConfiguration config, IDatabaseManger manager)
        {
            _config = config;
            _manager = manager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("posts")] //htt~/fms/vertex/post
        public List<VertexInfo> Posts([FromBody] List<VertexInfo> items)
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
        [Route("post")] //htt~/fms/vertex/post
        public VertexInfo Post([FromBody] VertexInfo item)
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
                return default(VertexInfo);
            }
            return item;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get/{id?}")] //htt~/fms/vertex/get
        public List<VertexInfo> Get(string? id)
        {
            string message = "";
            List<VertexInfo> vertices = new List<VertexInfo>();

            try
            {
                string qry = FmsModel.GenInitializeQuery<VertexInfo>();
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);
                var result = _manager.ExcuteQuery(qry);
                foreach (DataRow row in result.Table.Rows)
                {
                    VertexInfo v = new VertexInfo();
                    v.SetMember(row);
                    vertices.Add(v);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return vertices;
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("puts")] //htt~/fms/vertex/put
        public int Put([FromBody] List<VertexInfo> items)
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
        [Route("delete/{fleet_name?}/{pose_x?}/{pose_y?}")] //htt~/fms/vertex/delete
        public int Delete(string? fleet_names, string? pose_xs, string? pose_ys)
        {
            string message = "";
            string qry;
            string fleet_name = "";
            string pose_x = "";
            string pose_y = "";
            List<string> fleet_nameList = fleet_names.UnBoxSeperator();
            List<string> pose_xsList = pose_xs.UnBoxSeperator();
            List<string> pose_ysList = pose_ys.UnBoxSeperator();

            try
            {
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);

                _manager.TryBeginTrans(ref message);

                for (int i = 0; i < pose_ysList.Count; i++)
                {
                    fleet_name = fleet_nameList[i].ToString();
                    pose_x = pose_xsList[i].ToString();
                    pose_y = pose_ysList[i].ToString();

                    //qry = FmsModel.GenDeleteQuery<VertexInfo>(fleet_name, pose_x, pose_y); 
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

            return pose_ysList.Count;
        }
    }
}
