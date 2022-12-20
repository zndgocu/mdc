using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class MachineErrHis : FmsModel
    {
        public MachineErrHis()
        {

        }

        public MachineErrHis(MachineErrHis machineErrHis)
        {
            _fleet_name = machineErrHis.Fleet_name;
            _machine_typ = machineErrHis.Machine_typ;
            _machine_id = machineErrHis.Machine_id;
            _err_start_date = machineErrHis.Err_start_date;
            _err_end_date = machineErrHis.Err_end_date;
            _machine_err_cd = machineErrHis.Machine_err_cd;
            _task_id = machineErrHis.Task_id;
        }

        private string _fleet_name;
        private string _machine_typ;
        private string _machine_id;
        private string _err_start_date;
        private string _err_end_date;
        private string _machine_err_cd;
        private string _task_id;

        public string Fleet_name { get => _fleet_name; set => _fleet_name = value; }
        public string Machine_typ { get => _machine_typ; set => _machine_typ = value; }
        public string Machine_id { get => _machine_id; set => _machine_id = value; }
        public string Err_start_date { get => _err_start_date; set => _err_start_date = value; }
        public string Err_end_date { get => _err_end_date; set => _err_end_date = value; }
        public string Machine_err_cd { get => _machine_err_cd; set => _machine_err_cd = value; }
        public string Task_id { get => _task_id; set => _task_id = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.machine_err_his {CRLF}";
            return qry;
        }

        protected override void SetMemberRuntime(DataRow row)
        {
            _fleet_name = row.TryParse<string>("fleet_name");
            _machine_typ = row.TryParse<string>("machine_typ");
            _machine_id = row.TryParse<string>("machine_id");
            _err_start_date = row.TryParse<string>("err_start_date");
            _err_end_date = row.TryParse<string>("err_end_date");
            _machine_err_cd = row.TryParse<string>("machine_err_cd");
            _task_id = row.TryParse<string>("task_id");

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
            return new MachineErrHis(this);
        }
    }
}
