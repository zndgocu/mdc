using com.doosan.fms.model.Base;
using com.doosan.fms.model.Runtime;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.JoinModel
{
    public class Ntn : FmsModel
    {
        [JsonInclude]
        public List<NtnSup> NtnSup { get; set; }
        [JsonInclude]
        public List<NtnLang> NtnLang { get; set; }

        public Ntn()
        {
            NtnSup = new List<NtnSup>();
            NtnLang = new List<NtnLang>();
        }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.ntn_lang nl {CRLF}" +
                  $" inner join fms.ntn_sup ns {CRLF}" +
                  $"         on nl.ntn_cd = ns.ntn_cd ";
            return qry;
        }
    }
}
