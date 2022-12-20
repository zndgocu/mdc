using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class RobotData : FmsModel
    {
        public RobotData()
        {

        }
        public RobotData(RobotData robotData) : base(robotData)
        {
            _robot_id = robotData.Robot_id;
            _task_id = robotData.Task_id;
            _robot_mode = robotData.Robot_mode;
            _robot_type = robotData.Robot_type;
            _goal_node_id = robotData.Goal_node_id;
            _pose_x = robotData.Pose_x;
            _pose_y = robotData.Pose_y;
            _pose_z = robotData.Pose_z;
            _pose_theta = robotData.Pose_theta;
            _target_x = robotData.Target_x;
            _target_y = robotData.Target_y;
            _target_z = robotData.Target_z;
            _progress = robotData.Progress;
            _battery = robotData.Battery;
            _read_date = robotData.Read_date;
            _cmd_state = robotData.Cmd_state;
            _cmd_id = robotData.Cmd_id;
            _error_code = robotData.Error_code;
        }

        private string _robot_id;
        private string _task_id;
        private string _robot_mode;
        private string _robot_type;
        private string _goal_node_id;
        private decimal _pose_x;
        private decimal _pose_y;
        private decimal _pose_z;
        private decimal _pose_theta;
        private decimal _target_x;
        private decimal _target_y;
        private decimal _target_z;
        private decimal _progress;
        private decimal _battery;
        private string _read_date;
        private string _cmd_state;
        private string _cmd_id;
        private string _error_code;

        public string Robot_id { get => _robot_id; set => _robot_id = value; }
        public string Task_id { get => _task_id; set => _task_id = value; }
        public string Robot_mode { get => _robot_mode; set => _robot_mode = value; }
        public string Robot_type { get => _robot_type; set => _robot_type = value; }
        public string Goal_node_id { get => _goal_node_id; set => _goal_node_id = value; }
        public decimal Pose_x { get => _pose_x; set => _pose_x = value; }
        public decimal Pose_y { get => _pose_y; set => _pose_y = value; }
        public decimal Pose_z { get => _pose_z; set => _pose_z = value; }
        public decimal Pose_theta { get => _pose_theta; set => _pose_theta = value; }
        public decimal Target_x { get => _target_x; set => _target_x = value; }
        public decimal Target_y { get => _target_y; set => _target_y = value; }
        public decimal Target_z { get => _target_z; set => _target_z = value; }
        public decimal Progress { get => _progress; set => _progress = value; }
        public decimal Battery { get => _battery; set => _battery = value; }
        public string Read_date { get => _read_date; set => _read_date = value; }
        public string Cmd_state { get => _cmd_state; set => _cmd_state = value; }
        public string Cmd_id { get => _cmd_id; set => _cmd_id = value; }
        public string Error_code { get => _error_code; set => _error_code = value; }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.robot_data {CRLF}";
            return qry;
        }
        protected override void SetMemberRuntime(DataRow row)
        {
            _robot_id = row.TryParse<string>("robot_id");
            _task_id = row.TryParse<string>("task_id");
            _robot_mode = row.TryParse<string>("robot_mode");
            _robot_type = row.TryParse<string>("robot_type");
            _goal_node_id = row.TryParse<string>("goal_node_id");
            _pose_x = row.TryParse<decimal>("pose_x");
            _pose_y = row.TryParse<decimal>("pose_y");
            _pose_z = row.TryParse<decimal>("pose_z");
            _pose_theta = row.TryParse<decimal>("pose_theta");
            _target_x = row.TryParse<decimal>("target_x");
            _target_y = row.TryParse<decimal>("target_y");
            _target_z = row.TryParse<decimal>("target_z");
            _progress = row.TryParse<decimal>("progress");
            _battery = row.TryParse<decimal>("battery");
            _read_date = row.TryParse<string>("read_date");
            _cmd_state = row.TryParse<string>("cmd_state");
            _cmd_id = row.TryParse<string>("cmd_id");
            _error_code = row.TryParse<string>("error_code");
        }


        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_robot_id);
        }

        public override string LogData()
        {
            return $"robot_id : {_robot_id} task_id : {_task_id} robot_mode : {_robot_mode} ";
        }

        protected override FmsModel CloneRuntime()
        {
            return new RobotData(this);
        }
        public override double GetTransformPositionX()
        {
            return Convert.ToDouble(_pose_x);
        }
        public override double GetTransformPositionY()
        {
            return Convert.ToDouble(_pose_y);
        }

        public override double GetTransformRotationZ()
        {
            return Convert.ToDouble(_pose_theta);
        }
    }
}
