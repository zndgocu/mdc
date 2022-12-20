using com.doosan.fms.webapp.Client.FmsMonitoringCore.Three;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace com.doosan.fms.webapp.Client.FmsMonitoringCore.JsInteropItem.DataStruct
{
    public class StructInitializeCore<T> where T : class
    {
        [JsonInclude]
        public DotNetObjectReference<T> DotNet { get; set; }
        [JsonInclude]
        public string RenderElementId { get; set; }
        [JsonInclude]
        public ThreeManagerDataRepo ManagerRepo { get; set; }

        public StructInitializeCore(DotNetObjectReference<T> dotNet, string renderElementId, ThreeManagerDataRepo managerRepo)
        {
            DotNet = dotNet;
            RenderElementId = renderElementId;
            ManagerRepo = managerRepo;
        }
    }
}
