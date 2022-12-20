using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class NtnSup : FmsModel
    {
        public NtnSup()
        {
        }

        public NtnSup(NtnSup ntnSup) : base(ntnSup)
        {
            _ntn_cd = ntnSup.Ntn_cd;
            _ntn_nm = ntnSup.Ntn_nm;
            _ntn_css = ntnSup.Ntn_css;
            _prio = ntnSup.Prio;
        }
        public NtnSup(string ntn_cd, string ntn_nm, string ntn_css, int prio)
        {
            _ntn_cd = ntn_cd;
            _ntn_nm = ntn_nm;
            _ntn_css = ntn_css;
            _prio = prio;
        }

        private string _ntn_cd;
        private string _ntn_nm;
        private string _ntn_css;
        private int _prio;

        public string Ntn_cd { get => _ntn_cd; set => _ntn_cd = value; }
        public string Ntn_nm { get => _ntn_nm; set => _ntn_nm = value; }
        public string Ntn_css { get => _ntn_css; set => _ntn_css = value; }
        public int Prio { get => _prio; set => _prio = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.ntn_sup {CRLF}";
            return qry;
        }

        protected override void SetMemberRuntime(DataRow row)
        {
            _ntn_cd = row.TryParse<string>("ntn_cd");
            _ntn_nm = row.TryParse<string>("ntn_nm");
            _ntn_css = row.TryParse<string>("ntn_css");
            _prio = row.TryParse<int>("prio");
        }

        protected override string GetKey()
        {
            return $"ntn_cd : {_ntn_cd}";
        }
    }
}
