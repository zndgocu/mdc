using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class MachineMst : FmsModel
    {
        public MachineMst()
        {

        }
        public MachineMst(MachineMst machineMst)
        {
            _fleet_name = machineMst.Fleet_name;
            _machine_typ = machineMst.Machine_typ;
            _machine_id = machineMst.Machine_id;
            _comm_ip = machineMst.Comm_ip;
            _comm_port_from = machineMst.Comm_port_from;
            _comm_port_to = machineMst.Comm_port_to;
            _comm_port_cur = machineMst.Comm_port_cur;
            _connected_yn = machineMst.Connected_yn;
            _ins_date = machineMst.Ins_date;
            _upd_date = machineMst.Upd_date;
        }

        private string _fleet_name;
        private string _machine_typ;
        private string _machine_id;
        private string _comm_ip;
        private string _comm_port_from;
        private string _comm_port_to;
        private string _comm_port_cur;
        private string _connected_yn;
        private string _ins_date;
        private string _upd_date;

        public string Fleet_name { get => _fleet_name; set => _fleet_name = value; }
        public string Machine_typ { get => _machine_typ; set => _machine_typ = value; }
        public string Machine_id { get => _machine_id; set => _machine_id = value; }
        public string Comm_ip { get => _comm_ip; set => _comm_ip = value; }
        public string Comm_port_from { get => _comm_port_from; set => _comm_port_from = value; }
        public string Comm_port_to { get => _comm_port_to; set => _comm_port_to = value; }
        public string Comm_port_cur { get => _comm_port_cur; set => _comm_port_cur = value; }
        public string Connected_yn { get => _connected_yn; set => _connected_yn = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.machine_mst {CRLF}";
            return qry;
        }

        protected override void SetMemberRuntime(DataRow row)
        {
            _fleet_name = row.TryParse<string>("fleet_name");
            _machine_typ = row.TryParse<string>("machine_typ");
            _machine_id = row.TryParse<string>("machine_id");
            _comm_ip = row.TryParse<string>("comm_ip");
            _comm_port_from = row.TryParse<string>("comm_port_from");
            _comm_port_to = row.TryParse<string>("comm_port_to");
            _comm_port_cur = row.TryParse<string>("comm_port_cur");
            _connected_yn = row.TryParse<string>("connected_yn");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_date = row.TryParse<string>("upd_date");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_fleet_name, _machine_typ, _machine_id);
        }

        public override string LogData()
        {
            return $"fleet_name : {_fleet_name} machine_typ : {_machine_typ} machine_id {_machine_id}";
        }

        protected override FmsModel CloneRuntime()
        {
            return new MachineMst(this);
        }
    }
}
