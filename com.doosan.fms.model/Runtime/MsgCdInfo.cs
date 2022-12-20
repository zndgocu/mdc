using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class MsgCdInfo : FmsModel
    {
        public MsgCdInfo()
        {

        }
        public MsgCdInfo(MsgCdInfo msgCd)
        {
            _msg_nm = msgCd.Msg_nm;
            _cmn_cd = msgCd.Cmn_cd;
            _cmn_val = msgCd.Cmn_val;
            _remark = msgCd.Remark;
            _ins_date = msgCd.Ins_date;
            _upd_date = msgCd.Upd_date;
        }

        private string _msg_nm;
        private string _cmn_cd;
        private string _cmn_val;
        private string _remark;
        private string _ins_date;
        private string _upd_date;

        public string Msg_nm { get => _msg_nm; set => _msg_nm = value; }
        public string Cmn_cd { get => _cmn_cd; set => _cmn_cd = value; }
        public string Cmn_val { get => _cmn_val; set => _cmn_val = value; }
        public string Remark { get => _remark; set => _remark = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.msg_cd_info {CRLF}";
            return qry;
        }

        protected override void SetMemberRuntime(DataRow row)
        {
            _msg_nm = row.TryParse<string>("msg_nm");
            _cmn_cd = row.TryParse<string>("cmn_cd");
            _cmn_val = row.TryParse<string>("cmn_val");
            _remark = row.TryParse<string>("remark");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_date = row.TryParse<string>("upd_date");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_msg_nm, _cmn_cd);
        }

        public override string LogData()
        {
            return $"msg_nm : {_msg_nm} cmn_cd : {_cmn_cd} ";
        }

        protected override FmsModel CloneRuntime()
        {
            return new MsgCdInfo(this);
        }
    }
}
