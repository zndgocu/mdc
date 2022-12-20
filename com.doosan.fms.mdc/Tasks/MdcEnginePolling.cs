using com.doosan.fms.commonLib.Threading;
using com.doosan.fms.mdc.MdcEngine.Managers;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using com.doosan.fms.signalRHub.HubClient;
using com.doosan.fms.signalRHub.SignalR.DataSet;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.doosan.fms.mdc.Tasks
{
    public class MdcEnginePollingArgs
    {
        public MdcEnginePollingArgs(object args)
        {
            Array argArray = new object[3];
            argArray = (Array)args;
            WorldManager = (WorldManager)argArray.GetValue(0);
            SafetySharedList = (SafetySharedList<FmsModel>)argArray.GetValue(1);
            CancellationToken = (CancellationToken)argArray.GetValue(2);
        }

        public MdcEnginePollingArgs(WorldManager worldManager, SafetySharedList<FmsModel> safetySharedList, CancellationToken cancellationToken)
        {
            WorldManager = worldManager;
            SafetySharedList = safetySharedList;
            CancellationToken = cancellationToken;
        }

        public WorldManager WorldManager { get; set; }
        public SafetySharedList<FmsModel> SafetySharedList { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public object ToObject()
        {
            return new object[] { WorldManager, SafetySharedList, CancellationToken };
        }
    }

    public static class MdcEnginePolling
    {
        static bool ReceiveFirst = false;
        const double MILSEC_TEN = 10000d;
        const double MILSEC_ONE = 1000d;
        const int SEC_ONE = 1000;
        const int SEC_FIVE = 5000;
        const int SEC_TEN = 10000;
        public static Thread GenerateThread()
        {
            return new Thread(new ParameterizedThreadStart(DoWork));
        }


        static FmsData fmsData = new FmsData();
        static bool CallerResult = false;
        static bool FirstUpdate = false;

        private static async Task OnClosedHubConnection(Exception arg)
        {
            await Task.Delay(0);
        }

        private static void OnHUB_CLI_FUNC_BROADCAST_MDC(FmsData obj)
        {
            Console.WriteLine(obj.Message);
        }

        private static void OnEventHUB_CLI_FUNC_MDCINIT()
        {
            ReceiveFirst = true;
        }

        /// <summary>
        /// args
        /// </summary>
        /// <param name="args">
        /// WorldManager
        /// SafetySharedList<FmsModel>
        /// CancellationToken
        /// </param>
        public static async void DoWork(object args)
        {
            FmsData fmsData = new FmsData();
            MdcEnginePollingArgs argsRepo = new MdcEnginePollingArgs(args);
            await using (FmsDataHubClient<FmsData> hubClient = new FmsDataHubClient<FmsData>(false))
            {
                List<FmsModel> _engineDatas = new List<FmsModel>();
                double useFps = 180;
                argsRepo.WorldManager.StartDelta(useFps);
                double deltaTime = 0;
                bool updateFisrt = false;
                bool sendFirst = false;
                DateTime sendDate = DateTime.Now;
                string message = "";

                hubClient.EventClosedHubConnection += OnClosedHubConnection;
                while (argsRepo.CancellationToken.IsCancellationRequested == false)
                {
                    try
                    {
                        if (hubClient.Initialize($"https://localhost:44304/{FmsData.HUB_NAME}", FmsData.HUB_REQUEST_IDENTITY_NAME, ref message) == true)
                        {
                            Console.WriteLine($"hubClient Initialize");
                            break;
                        }
                        Console.WriteLine($"hubClient Initialize fail retry, reason : {message}");
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"throw hubClient Initialize fail retry, reason : {ex.Message}");
                        await Task.Delay(1000);
                    }
                }

                hubClient.EventHUB_CLI_FUNC_BROADCAST_MDC += OnHUB_CLI_FUNC_BROADCAST_MDC;
                hubClient.EventHUB_CLI_FUNC_MDCINIT += OnEventHUB_CLI_FUNC_MDCINIT;
                hubClient.OnHubFuncDefault();

                while (argsRepo.CancellationToken.IsCancellationRequested == false)
                {
                    try
                    {
                        argsRepo.WorldManager.StateProgramCounter++;
                        if (argsRepo.WorldManager.StateProgramCounter > 99999999) argsRepo.WorldManager.StateProgramCounter = 0;

                        //connect
                        if (hubClient.IsConnect() == false)
                        {
                            argsRepo.WorldManager.StateHubReconnectCounter++;
                            if (argsRepo.WorldManager.StateHubReconnectCounter > 99999999) argsRepo.WorldManager.StateHubReconnectCounter = 0;

                            Console.WriteLine($"DataHub와 접속 시도중입니다. 시도 수 {argsRepo.WorldManager.StateHubReconnectCounter}");
                            if (await hubClient.Connect() == false)
                            {
                                Console.WriteLine("접속 실패");
                                await Task.Delay(1000);
                                continue;
                            }
                            Console.WriteLine($"접속 성공 connectionId : {hubClient.GetId()} token : {hubClient.GetToken()}");
                            argsRepo.WorldManager.StateHubReconnectCounter = 0;
                        }

                        List<FmsModel> sharedData = null;

                        if (updateFisrt == false)
                        {
                            sharedData = argsRepo.SafetySharedList.Clone();
                            if (sharedData == null)
                            {
                                await Task.Delay(SEC_ONE);
                                continue;
                            }
                            argsRepo.WorldManager.UpdatePrefab(sharedData, deltaTime, 0, 0, PrefabUpdateType.All);
                            fmsData.Message = argsRepo.WorldManager.GetJson().ToString();
                            updateFisrt = true;
                        }

                        if (sendFirst == false)
                        {
                            hubClient.SendMessage(FmsData.HUB_SRV_FUNC_MDCINIT, fmsData);
                            sendDate = DateTime.Now;
                            sendFirst = true;
                            await Task.Delay(SEC_FIVE);
                            continue;

                        }

                        if (ReceiveFirst == false)
                        {
                            if ((DateTime.Now - sendDate).TotalMilliseconds > MILSEC_TEN)
                            {
                                sendFirst = false;
                            }
                            await Task.Delay(SEC_FIVE);
                            continue;
                        }


                        argsRepo.WorldManager.UpdateDelta();
                        if (argsRepo.WorldManager.IsUpdate(out deltaTime))
                        {
                            sharedData = argsRepo.SafetySharedList.Clone();
                            if (sharedData == null)
                            {
                                await Task.Delay(SEC_ONE);
                                continue;
                            }

                            double mps, resolution;
                            argsRepo.WorldManager.GetDeltaMapData(out mps, out resolution);
                            argsRepo.WorldManager.UpdatePrefab(sharedData, deltaTime, mps, resolution, PrefabUpdateType.Animate);
                            fmsData.Message = argsRepo.WorldManager.GetJson(PrefabUpdateType.Animate).ToString();
                            hubClient.SendMessage(FmsData.HUB_SRV_FUNC_BROADCAST_MDC, fmsData);
                            argsRepo.WorldManager.HitDelta();

                            double millsec = argsRepo.WorldManager.GetHitMillsec();
                            double fps = argsRepo.WorldManager.GetCalcFps();
                            Console.WriteLine($"delta : {millsec} fps : {fps}");
                        }
                        else
                        {
                            int a = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
