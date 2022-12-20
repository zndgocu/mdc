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

namespace com.doosan.fms.webapp.Server.Controllers.Fms
{
    [ApiController]
    [Route("fms/ntnlang")]
    public class NtnLangController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDatabaseManger _manager;

        public NtnLangController(IConfiguration config, IDatabaseManger manager)
        {
            _config = config;
            _manager = manager;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public List<NtnLang> Get()
        {
            string message = "";
            List<NtnLang> ntnLangs = new List<NtnLang>();

            try
            {
                string qry = FmsModel.GenInitializeQuery<NtnLang>();

                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);
                var result = _manager.ExcuteQuery(qry);
                foreach (DataRow row in result.Table.Rows)
                {
                    NtnLang v = new NtnLang();
                    v.SetMember(row);
                    ntnLangs.Add(v);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return ntnLangs;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get/{id}")]
        public List<NtnLang> Get(string id)
        {
            string message = "";
            List<NtnLang> ntnLangs = new List<NtnLang>();

            try
            {
                string qry = FmsModel.GenInitializeQuery<NtnLang>();
                qry = qry +
                      $"where ntn_cd = {id.Quot()}";

                if (_manager.Connect(DB_CONNECT_STRING, ref message) == false) throw new Exception(message);
                var result = _manager.ExcuteQuery(qry);
                foreach (DataRow row in result.Table.Rows)
                {
                    NtnLang v = new NtnLang();
                    v.SetMember(row);
                    ntnLangs.Add(v);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return ntnLangs;
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

    }
}
