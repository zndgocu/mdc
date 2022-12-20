using com.doosan.fms.webapp.Client.DependencyItems.Js;
using com.doosan.fms.webapp.Client.FmsMonitoringCore.JsInteropItem.DataStruct;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.FmsMonitoringCore.JsInteropItem.Main
{
    public class FmsMonitoringJsItem : JsItem
    {
        public FmsMonitoringJsItem(string key, string path) : base(key, path)
        {

        }

        [JSInvokable]
        public async void Foucs(string key)
        {
            try
            {
                await CallVoidParm<string>("focusModel", key);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<bool> InitializeCore<T>(StructInitializeCore<T> param) where T : class
        {
            try
            {
                return await Call<bool, StructInitializeCore<T>>("initializeCore", param);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<bool> StartRenderWorker()
        {
            return await CallNonParm<bool>("startRenderWorker");
        }

        public async Task<bool> StartWorker(StructStartWorker workerData)
        {
            return await Call<bool, StructStartWorker>("startWorker", workerData);
        }
    }
}
