using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class TaskMst : FmsModel
    {
        public TaskMst()
        {

        }

        public TaskMst(TaskMst taskMst)
        {
            _task_id = taskMst.Task_id;
            _fleet_name = taskMst.Fleet_name;
            _robot_id = taskMst.Robot_id;
            _status = taskMst.Status;
            _start_time = taskMst.Start_time;
            _end_time = taskMst.End_time;
            _state = taskMst.State;
            _submission_time = taskMst.Submission_time;
            _priority = taskMst.Priority;
            _task_type = taskMst.Task_type;
            _start_node = taskMst._start_node;
            _goal_node = taskMst.Goal_node;
            _ins_date = taskMst.Ins_date;
            _upd_date = taskMst.Upd_date;
            _comp_robot_id = taskMst.Comp_robot_id;
        }

        private string _task_id;
        private string _fleet_name;
        private string _robot_id;
        private string _status;
        private string _start_time;
        private string _end_time;
        private decimal _state;
        private string _submission_time;
        private decimal _priority;
        private decimal _task_type;
        private string _start_node;
        private string _goal_node;
        private string _ins_date;
        private string _upd_date;
        private string _comp_robot_id;

        public string Task_id { get => _task_id; set => _task_id = value; }
        public string Fleet_name { get => _fleet_name; set => _fleet_name = value; }
        public string Robot_id { get => _robot_id; set => _robot_id = value; }
        public string Status { get => _status; set => _status = value; }
        public string Start_time { get => _start_time; set => _start_time = value; }
        public string End_time { get => _end_time; set => _end_time = value; }
        public decimal State { get => _state; set => _state = value; }
        public string Submission_time { get => _submission_time; set => _submission_time = value; }
        public decimal Priority { get => _priority; set => _priority = value; }
        public decimal Task_type { get => _task_type; set => _task_type = value; }
        public string Start_node { get => _start_node; set => _start_node = value; }
        public string Goal_node { get => _goal_node; set => _goal_node = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }
        public string Comp_robot_id { get => _comp_robot_id; set => _comp_robot_id = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.task_mst {CRLF}";
            return qry;
        }
        protected override void SetMemberRuntime(DataRow row)
        {
            _task_id = row.TryParse<string>("task_id");
            _fleet_name = row.TryParse<string>("fleet_name");
            _robot_id = row.TryParse<string>("robot_id");
            _status = row.TryParse<string>("status");
            _start_time = row.TryParse<string>("start_time");
            _end_time = row.TryParse<string>("end_time");
            _state = row.TryParse<decimal>("state");
            _submission_time = row.TryParse<string>("submission_time");
            _priority = row.TryParse<decimal>("priority");
            _task_type = row.TryParse<decimal>("task_type");
            _start_node = row.TryParse<string>("start_node");
            _goal_node = row.TryParse<string>("goal_node");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_date = row.TryParse<string>("upd_date");
            _comp_robot_id = row.TryParse<string>("comp_robot_id");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_task_id, _fleet_name);
        }

        public override string LogData()
        {
            return $"task_id : {_task_id} ";
        }

        protected override FmsModel CloneRuntime()
        {
            return new TaskMst(this);
        }
    }
}
