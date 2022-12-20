using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class UserGrp : FmsModel
    {
        public UserGrp()
        {

        }
        public UserGrp(UserGrp userGrp)
        {
            _grp_cd = userGrp.Grp_cd;
            _grp_name = userGrp.Grp_name;
            _grp_level = userGrp.Grp_level;
            _ins_user_id = userGrp.Ins_user_id;
            _ins_date = userGrp.Ins_date;
            _upd_user_id = userGrp.Upd_user_id;
            _upd_date = userGrp.Upd_date;
        }

        private string _grp_cd;
        private string _grp_name;
        private string _grp_level;
        private string _ins_user_id;
        private string _ins_date;
        private string _upd_user_id;
        private string _upd_date;

        public string Grp_cd { get => _grp_cd; set => _grp_cd = value; }
        public string Grp_name { get => _grp_name; set => _grp_name = value; }
        public string Grp_level { get => _grp_level; set => _grp_level = value; }
        public string Ins_user_id { get => _ins_user_id; set => _ins_user_id = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_user_id { get => _upd_user_id; set => _upd_user_id = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.user_grp {CRLF}";
            return qry;
        }
        protected override void SetMemberRuntime(DataRow row)
        {
            _grp_cd = row.TryParse<string>("grp_cd");
            _grp_name = row.TryParse<string>("grp_name");
            _grp_level = row.TryParse<string>("grp_level");
            _ins_user_id = row.TryParse<string>("ins_user_id ");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_user_id = row.TryParse<string>("upd_user_id");
            _upd_date = row.TryParse<string>("upd_date");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_grp_cd, _grp_name);
        }

        public override string LogData()
        {
            return $"grp_cd : {_grp_cd} grp_name : {_grp_name} ";
        }

        protected override FmsModel CloneRuntime()
        {
            return new UserGrp(this);
        }
    }
}
