using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class MachineEcdMst : FmsModel
    {
        public MachineEcdMst()
        {

        }
        public MachineEcdMst(MachineEcdMst machineEcdMst)
        {
            _machine_typ = machineEcdMst.Machine_typ;
            _machine_err_cd = machineEcdMst.Machine_err_cd;
            _err_msg = machineEcdMst.Err_msg;
            _action = machineEcdMst.Action;
            _ins_date = machineEcdMst.Ins_date;
            _upd_date = machineEcdMst.Upd_date;
        }

        private string _machine_typ;
        private string _machine_err_cd;
        private string _err_msg;
        private string _action;
        private string _ins_date;
        private string _upd_date;

        public string Machine_typ { get => _machine_typ; set => _machine_typ = value; }
        public string Machine_err_cd { get => _machine_err_cd; set => _machine_err_cd = value; }
        public string Err_msg { get => _err_msg; set => _err_msg = value; }
        public string Action { get => _action; set => _action = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.machine_ecd_mst {CRLF}";
            return qry;
        }

        protected override void SetMemberRuntime(DataRow row)
        {
            _machine_typ = row.TryParse<string>("machine_typ");
            _machine_err_cd = row.TryParse<string>("machine_err_cd");
            _err_msg = row.TryParse<string>("err_msg");
            _action = row.TryParse<string>("action");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_date = row.TryParse<string>("upd_date");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_machine_typ, _machine_err_cd);
        }

        public override string LogData()
        {
            return $"machine_typ : {_machine_typ} machine_err_cd : {_machine_err_cd} ";
        }

        protected override FmsModel CloneRuntime()
        {
            return new MachineEcdMst(this);
        }
    }
}
