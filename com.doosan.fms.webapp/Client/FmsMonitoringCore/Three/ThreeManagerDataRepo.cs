using com.doosan.fms.commonLib.Struct;
using System.Text.Json.Serialization;

namespace com.doosan.fms.webapp.Client.FmsMonitoringCore.Three
{
    public class ThreeManagerDataRepo
    {
        [JsonInclude]
        public SetsStringBool Orbit;
        [JsonInclude]
        public SetsStringBool Light;
        [JsonInclude]
        public SetsStringBool AxesHelper;
        [JsonInclude]
        public SetsStringBool Stats;
        [JsonInclude]
        public SetsStringBool GridHelper;


        public ThreeManagerDataRepo()
        {
            Orbit = new SetsStringBool();
            Light = new SetsStringBool();
            AxesHelper = new SetsStringBool();
            Stats = new SetsStringBool();
            GridHelper = new SetsStringBool();
        }
    }
}
