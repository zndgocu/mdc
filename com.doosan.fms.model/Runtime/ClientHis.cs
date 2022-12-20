using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class ClientHis : FmsModel
    {
        public ClientHis()
        {

        }
        public ClientHis(ClientHis clientHis)
        {
            _fleet_name = clientHis.Fleet_name;
            _ins_date = clientHis.Ins_date;
            _client_ip = clientHis.Client_ip;
            _user_id = clientHis.User_id;
            _pgr_name = clientHis.Pgr_name;
            _task_id = clientHis.Task_id;
            _message = clientHis.Message;
        }

        private string _fleet_name;
        private string _ins_date;
        private string _client_ip;
        private string _user_id;
        private string _pgr_name;
        private string _task_id;
        private string _message;

        public string Fleet_name { get => _fleet_name; set => _fleet_name = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Client_ip { get => _client_ip; set => _client_ip = value; }
        public string User_id { get => _user_id; set => _user_id = value; }
        public string Pgr_name { get => _pgr_name; set => _pgr_name = value; }
        public string Task_id { get => _task_id; set => _task_id = value; }
        public string Message { get => _message; set => _message = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.client_his {CRLF}";
            return qry;
        }
        protected override void SetMemberRuntime(DataRow row)
        {
            _fleet_name = row.TryParse<string>("fleet_name");
            _ins_date = row.TryParse<string>("ins_date");
            _client_ip = row.TryParse<string>("client_ip");
            _user_id = row.TryParse<string>("user_id");
            _pgr_name = row.TryParse<string>("pgr_name");
            _task_id = row.TryParse<string>("task_id");
            _message = row.TryParse<string>("message");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_fleet_name, _pgr_name);
        }

        public override string LogData()
        {
            return $"fleet_name : {_fleet_name} pgr_name : {_pgr_name} ";
        }

        protected override FmsModel CloneRuntime()
        {
            return new ClientHis(this);
        }
    }
}
