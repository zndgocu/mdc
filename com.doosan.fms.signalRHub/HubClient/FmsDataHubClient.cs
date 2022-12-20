using com.doosan.fms.signalRHub.HubClient.BaseClient;
using com.doosan.fms.signalRHub.SignalR.DataSet;
using System;
using System.Threading.Tasks;

namespace com.doosan.fms.signalRHub.HubClient
{
    public class FmsDataHubClient<T> : HubClientBase<T> where T : FmsData, new()
    {
        public event Action<T> EventHUB_CLI_FUNC_JOIN_SUCCESS;
        protected virtual void OnHUB_CLI_FUNC_JOIN_SUCCESS(T e)
        {
            EventHUB_CLI_FUNC_JOIN_SUCCESS?.Invoke(e);
        }

        public event Action<T> EventHUB_CLI_FUNC_JOIN_FAIL;
        protected virtual void OnHUB_CLI_FUNC_JOIN_FAIL(T e)
        {
            EventHUB_CLI_FUNC_JOIN_FAIL?.Invoke(e);
        }

        public event Action<T> EventHUB_CLI_FUNC_BROADCAST_MDC;
        protected virtual void OnHUB_CLI_FUNC_BROADCAST_MDC(T e)
        {
            EventHUB_CLI_FUNC_BROADCAST_MDC?.Invoke(e);
        }

        public event Action EventHUB_CLI_FUNC_MDCINIT;
        protected virtual void OnEventHUB_CLI_FUNC_MDCINIT()
        {

            EventHUB_CLI_FUNC_MDCINIT?.Invoke();
        }

        public FmsDataHubClient(bool reconnect) : base(reconnect)
        {

        }

        public void OnHubFuncDefault()
        {
            On(FmsData.HUB_CLI_FUNC_BROADCAST_MDC, EventHUB_CLI_FUNC_BROADCAST_MDC);
            On(FmsData.HUB_CLI_FUNC_MDCINIT, EventHUB_CLI_FUNC_MDCINIT);
        }

        protected override ValueTask DisposeChild()
        {
            return base.DisposeChild();
        }

        protected override void OnClosed(Exception e)
        {
            Console.WriteLine(e.Message);
            base.OnClosed(e);
        }


        //protected override void OnReceive(T e)
        //{
        //    Console.WriteLine(e);
        //    base.OnReceive(e);
        //}
    }
}
