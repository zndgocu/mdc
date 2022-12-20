using com.doosan.fms.webapp.Client.DependencyItems.Js;
using com.doosan.fms.webapp.Client.DependencyItems.Js.Container;
using com.doosan.fms.webapp.Client.FmsMonitoringCore.JsInteropItem.DataStruct;
using com.doosan.fms.webapp.Client.FmsMonitoringCore.JsInteropItem.Main;
using com.doosan.fms.webapp.Client.FmsMonitoringCore.Three;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.FmsMonitoringCore.Manager
{
    public class FmsMonitoringManager
    {
        //private const string COMMAND_MONITORING_FMS_MAIN_JS_SIGNALR_START = "start";
        //
        //private const string KEY_SIGNALR_DEFAULT = "signalr";
        //private const string PATH_SIGNALR_DEFAULT = "./lib/microsoft/signalr/dist/browser/signalr.js";
        //private const string PATH_EXTEND_LOAD_SIGNALR_FMS_JS = "./fms/worker/signalRWorkerfmsDataHubClient.js";

        private const string KEY_MONITORING_FMS_MAIN_JS = "fmsMonitoring";
        private const string PATH_MONITORING_FMS_MAIN_JS = "./fms/main/fmsMonitoring.js";

        private const string KEY_MONITORING_FMS_REDNER_WORKER_JS = "fmsMonitoringRenderWorker";
        private const string PATH_MONITORING_FMS_REDNER_WORKER_JS = "./fms/worker/fmsMonitoringRenderWorker.js";

        private JsContainer _jsContainer;

        private FmsMonitoringJsItem _core;
        IJSRuntime _jsr;

        ThreeManagerDataRepo _managerRepo;

        public FmsMonitoringManager(IJSRuntime jsr)
        {
            _jsr = jsr;
            _jsContainer = new JsContainer(jsr);
            _managerRepo = new ThreeManagerDataRepo();
        }

        public async Task<bool> Initialize()
        {
            _core = new FmsMonitoringJsItem(KEY_MONITORING_FMS_MAIN_JS, PATH_MONITORING_FMS_MAIN_JS);
            var resultCore = await _core.ImportAsync(_jsr);
            if (resultCore == false) return false;

            _jsContainer.AddJsItem(new JsItem("A", "./monitoring/js/three/Vector2.js"));
            _jsContainer.AddJsItem(new JsItem("B", "./monitoring/js/three/three.js"));
            _jsContainer.AddJsItem(new JsItem("C", "./monitoring/js/three/OrbitControls.js"));
            _jsContainer.AddJsItem(new JsItem("D", "./monitoring/js/three/stats.js"));
            _jsContainer.AddJsItem(new JsItem("E", "./monitoring/js/three/FBXLoader.js"));
            _jsContainer.AddJsItem(new JsItem("F", "./monitoring/js/three/SVGLoader.js"));
            _jsContainer.AddJsItem(new JsItem("G", "./monitoring/js/three/fflate.min.js"));
            _jsContainer.AddJsItem(new JsItem("H", "./monitoring/js/three/geometries/CircleGeometry.js"));
            _jsContainer.AddJsItem(new JsItem("I", "./monitoring/js/three/geometries/TextGeometry.js"));
            _jsContainer.AddJsItem(new JsItem("J", "./monitoring/js/three/geometries/EdgesGeometry.js"));
            _jsContainer.AddJsItem(new JsItem("K", "./monitoring/js/three/helpers/AxesHelper.js"));
            _jsContainer.AddJsItem(new JsItem("L", "./monitoring/js/three/utils/BufferGeometryUtils.js"));

            var result = await _jsContainer.LoadJsItems();
            if (result.Where(x => x.Value == false).Count() > 0) return false;
            return true;
        }

        public async Task<bool> InitializeCore<T>(DotNetObjectReference<T> dotNet, string renderElementId) where T : class
        {
            try
            {
                _managerRepo.Orbit.Sets(Guid.NewGuid().ToString(), true);
                _managerRepo.Light.Sets(Guid.NewGuid().ToString(), true);
                _managerRepo.AxesHelper.Sets(Guid.NewGuid().ToString(), true);
                _managerRepo.Stats.Sets(Guid.NewGuid().ToString(), true);
                _managerRepo.GridHelper.Sets(Guid.NewGuid().ToString(), true);
                return await _core.InitializeCore(new StructInitializeCore<T>(dotNet, renderElementId, _managerRepo));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<bool> StartWorker(string defaultUri, string auth)
        {
            try
            {
                StructStartWorker workerData = new StructStartWorker(KEY_MONITORING_FMS_REDNER_WORKER_JS, PATH_MONITORING_FMS_REDNER_WORKER_JS, auth);
                return await _core.StartWorker(workerData);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
