using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class UserMst : FmsModel
    {
        public UserMst()
        {

        }
        public UserMst(UserMst userMst)
        {
            _user_id = userMst.User_id;
            _user_pw = userMst.User_pw;
            _user_nm = userMst.User_nm;
            _grp_cd = userMst.Grp_cd;
            _remarks = userMst.Remarks;
            _ins_user_id = userMst.Ins_user_id;
            _ins_date = userMst.Ins_date;
            _upd_user_id = userMst.Upd_user_id;
            _upd_date = userMst._upd_date;
        }

        private string _user_id;
        private string _user_pw;
        private string _user_nm;
        private string _grp_cd;
        private string _remarks;
        private string _ins_user_id;
        private string _ins_date;
        private string _upd_user_id;
        private string _upd_date;

        public string User_id { get => _user_id; set => _user_id = value; }
        public string User_pw { get => _user_pw; set => _user_pw = value; }
        public string User_nm { get => _user_nm; set => _user_nm = value; }
        public string Grp_cd { get => _grp_cd; set => _grp_cd = value; }
        public string Remarks { get => _remarks; set => _remarks = value; }
        public string Ins_user_id { get => _ins_user_id; set => _ins_user_id = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_user_id { get => _upd_user_id; set => _upd_user_id = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.user_mst {CRLF}";
            return qry;
        }
        protected override string GetCreateQueryRuntime()
        {
            string qry = default(string);
            qry = $" insert into fms.user_mst {CRLF}" +
                  $" (user_id, user_pw, user_nm, grp_cd, remarks, ins_user_id, ins_date, upd_user_id, upd_date) {CRLF}" +
                  $" values ({_user_id.Quot()},{_user_pw.Quot()},{_user_nm.Quot()},{_grp_cd.Quot()},{_remarks.Quot()},{_ins_user_id.Quot()}, now(),{_upd_user_id.Quot()}, now())";
            return qry;
        }
        protected override string GetUpdateQueryRuntime()
        {
            string qry = default(string);
            qry = $" update fms.user_mst {CRLF}" +
                  $"    set user_pw = {_user_pw.Quot()}, user_nm = {_user_nm.Quot()}, grp_cd = {_grp_cd.Quot()}, remarks = {_remarks.Quot()}, upd_user_id = {_upd_user_id.Quot()}, upd_date = now() {CRLF}" +
                  $"  where user_id = {_user_id.Quot()} ";
            return qry;
        }
        protected override string GetDeleteQueryRuntime()
        {
            string qry = default(string);
            qry = $" delete from fms.user_mst {CRLF}";
            return qry;
        }
        protected override void SetMemberRuntime(DataRow row)
        {
            _user_id = row.TryParse<string>("user_id");
            _user_pw = row.TryParse<string>("user_pw");
            _user_nm = row.TryParse<string>("user_nm");
            _grp_cd = row.TryParse<string>("grp_cd");
            _remarks = row.TryParse<string>("remarks");
            _ins_user_id = row.TryParse<string>("ins_user_id");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_user_id = row.TryParse<string>("upd_user_id");
            _upd_date = row.TryParse<string>("upd_date");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_user_id);
        }

        public override string LogData()
        {
            return $"user_id : {_user_id} ";
        }

        protected override FmsModel CloneRuntime()
        {
            return new UserMst(this);
        }
    }
}
