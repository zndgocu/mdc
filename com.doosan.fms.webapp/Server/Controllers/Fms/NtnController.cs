using com.doosan.fms.commonLib.Helper;
using com.doosan.fms.model.Base;
using com.doosan.fms.model.JoinModel;
using com.doosan.fms.model.Runtime;
using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.webapp.Server.Controllers.Fms
{
    [ApiController]
    //[Route("fms/[controller]")]
    [Route("fms/ntn")]
    public class NtnController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDatabaseManger _manager;

        public NtnController(IConfiguration config, IDatabaseManger manager)
        {
            _config = config;
            _manager = manager;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public Ntn GetNtnJoin()
        {
            string message = "";
            Ntn ntn = new Ntn();

            try
            {

                string qry1 = FmsModel.GenInitializeQuery<NtnLang>();
                string qry2 = FmsModel.GenInitializeQuery<NtnSup>();
                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);
                var results = _manager.ExcuteQuerys(qry1, qry2);

                ntn.NtnLang = ReflectHelper.StaticMethodGeneric<List<FmsModel>>(typeof(NtnLang), true, "GetModels", new object[] { results[0].Table }, new Type[] { typeof(NtnLang) }).Select(x => (NtnLang)x).ToList();
                ntn.NtnSup = ReflectHelper.StaticMethodGeneric<List<FmsModel>>(typeof(NtnSup), true, "GetModels", new object[] { results[1].Table }, new Type[] { typeof(NtnSup) }).Select(x => (NtnSup)x).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return ntn;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("posts")]
        public List<Ntn> Posts([FromBody] List<Ntn> items)
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
        [Route("post")]
        public Ntn Post([FromBody] Ntn item)
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
    }
}
