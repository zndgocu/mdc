using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class NtnLang : FmsModel
    {

        public NtnLang()
        {
        }

        public NtnLang(NtnLang ntnLang) : base(ntnLang)
        {
            _ntn_cd = ntnLang.Ntn_cd;
            _ntn_default = ntnLang.Ntn_default;
            _ntn_val = ntnLang.Ntn_val;
            _ntn_css = ntnLang.Ntn_css;
        }

        public NtnLang(string ntn_cd, string ntn_default, string ntn_val, string ntn_css)
        {
            _ntn_cd = ntn_cd;
            _ntn_default = ntn_default;
            _ntn_val = ntn_val;
            _ntn_css = ntn_css;
        }

        private string _ntn_cd;
        private string _ntn_default;
        private string _ntn_val;
        private string _ntn_css;

        public string Ntn_cd { get => _ntn_cd; set => _ntn_cd = value; }
        public string Ntn_default { get => _ntn_default; set => _ntn_default = value; }
        public string Ntn_val { get => _ntn_val; set => _ntn_val = value; }
        public string Ntn_css { get => _ntn_css; set => _ntn_css = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.ntn_lang {CRLF}";
            return qry;
        }

        protected override void SetMemberRuntime(DataRow row)
        {
            _ntn_cd = row.TryParse<string>("ntn_cd");
            _ntn_default = row.TryParse<string>("ntn_default");
            _ntn_val = row.TryParse<string>("ntn_val");
            _ntn_css = row.TryParse<string>("ntn_css");
        }

        protected override string GetKey()
        {
            return $"ntn_cd : {_ntn_cd} ntn_default : {_ntn_default} ";
        }
    }
}
