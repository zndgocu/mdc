using System.Text.Json.Serialization;

namespace com.doosan.fms.webapp.Client.FmsMonitoringCore.JsInteropItem.DataStruct
{
    public class StructStartWorker
    {

        [JsonInclude]
        public string WorkerScript { get; set; }
        [JsonInclude]
        public string WorkerName { get; set; }
        [JsonInclude]
        public string Auth { get; set; }

        public StructStartWorker(string workerName, string workerScript, string auth)
        {
            WorkerScript = workerScript;
            WorkerName = workerName;
            Auth = auth;
        }
    }
}
