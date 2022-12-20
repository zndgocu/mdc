using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class VertexInfo : FmsModel
    {
        public VertexInfo()
        {
        }

        public VertexInfo(VertexInfo vertexInfo) : base(vertexInfo)
        {
            _fleet_name = vertexInfo.Fleet_name;
            _pose_x = vertexInfo.Pose_x;
            _pose_y = vertexInfo.Pose_y;
            _name = vertexInfo.Name;
            _zone = vertexInfo.Zone;
        }

        public VertexInfo(string fleetName, decimal poseX, decimal poseY, string name, int zone)
        {
            _fleet_name = fleetName;
            _pose_x = poseX;
            _pose_y = poseY;
            _name = name;
            _zone = zone;
        }

        private string _fleet_name;
        private decimal _pose_x;
        private decimal _pose_y;
        private string _name;
        private int _zone;
        private string _ins_date;
        private string _upd_date;


        public string Fleet_name { get => _fleet_name; set => _fleet_name = value; }
        public decimal Pose_x { get => _pose_x; set => _pose_x = value; }
        public decimal Pose_y { get => _pose_y; set => _pose_y = value; }
        public string Name { get => _name; set => _name = value; }
        public int Zone { get => _zone; set => _zone = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }

        public static string GetRemoveAllQuery()
        {
            string qry;
            qry = $" delete from fms.vertex_info ";
            return qry;
        }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.vertex_info {CRLF}";
            return qry;
        }
        protected override string GetCreateQueryRuntime()
        {
            string qry = default(string);
            qry = $" insert into fms.vertex_info {CRLF}" +
                  $" (fleet_name, pose_x, pose_y, name, zone, ins_date) {CRLF}" +
                  $" values ({_fleet_name.Quot()},{_pose_x},{_pose_y},{_name.Quot()},{_zone},now())";
            return qry;
        }
        protected override string GetUpdateQueryRuntime()
        {
            string qry = default(string);
            qry = $" update fms.vertex_info {CRLF}" +
                  $"    set name = {_name.Quot()}, zone = {_zone}, upd_date = {_upd_date.Quot()} {CRLF}" +
                  $"  where fleet_name = {_fleet_name.Quot()} and pose_x = '{_pose_x}' and pose_y = '{_pose_y}' ";
            return qry;
        }

        protected override void SetMemberRuntime(DataRow row)
        {
            _fleet_name = row.TryParse<string>("fleet_name");
            _pose_x = row.TryParse<decimal>("pose_x");
            _pose_y = row.TryParse<decimal>("pose_y");
            _name = row.TryParse<string>("name");
            _zone = row.TryParse<int>("zone");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_date = row.TryParse<string>("upd_date");
        }

        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_fleet_name, _pose_x.ToString(), _pose_y.ToString());
        }

        public override string LogData()
        {
            return $"fleetName : {_fleet_name} pose_x : {_pose_x} pose_y : {_pose_y}";
        }

        protected override FmsModel CloneRuntime()
        {
            return new VertexInfo(this);
        }


        public override double GetTransformPositionX()
        {
            return Convert.ToDouble(_pose_x);
        }
        public override double GetTransformPositionY()
        {
            return Convert.ToDouble(_pose_y);
        }

    }
}
