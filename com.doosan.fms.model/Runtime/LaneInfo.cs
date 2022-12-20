using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.model.Base;
using System;
using System.Data;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.model.Runtime
{
    public class LaneInfo : FmsModel
    {
        public LaneInfo()
        {
        }
        public LaneInfo(LaneInfo laneInfo) : base(laneInfo)
        {
            _fleet_name = laneInfo.Fleet_name;
            _start_vertex_no = laneInfo.Start_vertex_no;
            _end_vertex_no = laneInfo.End_vertex_no;
        }

        public LaneInfo(string fleetName, int startVertexNo, int endVertexNo)
        {
            _fleet_name = fleetName;
            _start_vertex_no = startVertexNo;
            _end_vertex_no = endVertexNo;
        }

        private string _fleet_name;
        private int _start_vertex_no;
        private int _end_vertex_no;
        private string _ins_date;
        private string _upd_date;

        public string Fleet_name { get => _fleet_name; set => _fleet_name = value; }
        public int Start_vertex_no { get => _start_vertex_no; set => _start_vertex_no = value; }
        public int End_vertex_no { get => _end_vertex_no; set => _end_vertex_no = value; }
        public string Ins_date { get => _ins_date; set => _ins_date = value; }
        public string Upd_date { get => _upd_date; set => _upd_date = value; }

        public static string GetRemoveAllQuery()
        {
            string qry;
            qry = $" delete from fms.lane ";
            return qry;
        }

        protected override string GetCreateQueryRuntime()
        {
            string qry = default(string);
            qry = $" insert into fms.lane_info {CRLF}" +
                  $" (fleet_name, start_vertex_no, end_vertex_no, ins_date, upd_date) {CRLF}" +
                  $" values ({_fleet_name.Quot()},{_start_vertex_no},{_end_vertex_no}, now(), now())";
            return qry;
        }

        protected override string GetInitializeQueryRuntime()
        {
            string qry = default(string);
            qry = $" select * {CRLF}" +
                  $"   from fms.lane_info {CRLF}";
            return qry;
        }
        protected override void SetMemberRuntime(DataRow row)
        {
            _fleet_name = row.TryParse<string>("fleet_name");
            _start_vertex_no = row.TryParse<int>("start_vertex_no");
            _end_vertex_no = row.TryParse<int>("end_vertex_no");
            _ins_date = row.TryParse<string>("ins_date");
            _upd_date = row.TryParse<string>("upd_date");
        }
        protected override string GetKey()
        {
            return StringExtension.GenerateKey(_fleet_name, _start_vertex_no.ToString(), _end_vertex_no.ToString());
        }

        public override string LogData()
        {
            return $"fleetName : {_fleet_name.Quot()} start_vertex_no : {_start_vertex_no} end_vertex_no : {_end_vertex_no}";
        }

        protected override FmsModel CloneRuntime()
        {
            return new LaneInfo(this);
        }

        public override double GetTransformPositionX()
        {
            return Convert.ToDouble(_start_vertex_no);
        }
        public override double GetTransformPositionY()
        {
            return Convert.ToDouble(_end_vertex_no);
        }

    }
}
