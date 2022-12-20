using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.commonLib.Helper;
using com.doosan.fms.commonLib.Threading;
using com.doosan.fms.commonLib.Threading.Task;
using com.doosan.fms.mdc.MapData.Yaml;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaEnum;
using com.doosan.fms.mdc.MdcEngine.Managers;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.mdc.Tasks;
using com.doosan.fms.model.Base;
using com.doosan.fms.model.Runtime;
using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using com.doosan.multiDatabaseDriver.DatabaseDriver.DatabaseResult;
using com.doosan.multiDatabaseDriver.DatabaseDriver.Postgresql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.mdc
{
    public enum ReadCommand
    {
        [Description("Read Yaml, Database Insert")]
        ReadYaml,
        [Description("SharedMemeory View")]
        Logging,
        [Description("FPS View")]
        Fps,
        [Description("Start MDCEnginePolling")]
        MdcEngine,
        [Description("Start DatabasePolling")]
        DatabasePolling,
        [Description("Terminate")]
        Terminate
    }



    class Program
    {
        static async Task Main(string[] args)
        {
            WorldManager manager = new WorldManager();
            SafetySharedList<FmsModel> sharedList = new SafetySharedList<FmsModel>();

            Dictionary<TaskType, TokenTask<TaskType>> tasks = new Dictionary<TaskType, TokenTask<TaskType>>();

            string message;

            while (true)
            {
                message = "";
                //write
                Console.WriteLine("Choose the Command");
                foreach (ReadCommand enumValue in Enum.GetValues(typeof(ReadCommand)))
                {
                    Console.WriteLine($" {enumValue.ToInt()} : {enumValue.GetDescription()}");
                }
                Console.Write("Enter Input : ");

                //read
                int read = 0;
                try
                {
                    read = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    continue;
                }

                if (Enum.IsDefined(typeof(ReadCommand), read) == true)
                {
                    ReadCommand enumRead = (ReadCommand)read;
                    if (enumRead == ReadCommand.ReadYaml)
                    {
                        string filePath, alreadyDelete, dbUserId, dbUserPw, dbIp, dbPort, databaseServerName;

#if (!DEBUG)
                        {
                            Console.Write("Enter Yaml FilePath : ");
                            filePath = Console.ReadLine();

                            Console.Write("기존 데이터를 지웁니까? 이 작업은 취소되지 않습니다.  (Y OR N) : ");
                            alreadyDelete = Console.ReadLine();

                            Console.Write("Enter DB user id : ");
                            dbUserId = Console.ReadLine();

                            Console.Write("Enter DB user Pw : ");
                            dbUserPw = Console.ReadLine();

                            Console.Write("Enter DB Ip : ");
                            dbIp = Console.ReadLine();

                            Console.Write("Enter DB Port : ");
                            dbPort = Console.ReadLine();

                            Console.Write("Enter DB Name : ");
                            databaseServerName = Console.ReadLine();

                            alreadyDelete = "Y";
                            filePath = @"D:\gitlab\com.doosan.fms.webserver\wwwroot\monitoring\mon_data\yaml\topological_map.yaml";
                        }
#else
                        {
                            alreadyDelete = "Y";
                            filePath = @"D:\gitlab\com.doosan.fms.webserver\wwwroot\monitoring\mon_data\yaml\topological_map.yaml";
                        }
#endif
                        try
                        {
                            FileStream fileStream = new FileStream(filePath, FileMode.Open);
                            FmsYaml fyml = new FmsYaml(fileStream);
                            if (fyml.InitializeYaml() == false)
                            {
                                Console.WriteLine($"fyml InitializeYaml fail");
                                continue;
                            }
                            if (fyml.SetYaml() == false)
                            {
                                Console.WriteLine($"fyml SetYaml fail");
                                continue;
                            }
                            fileStream.Close();

                            try
                            {
                                List<string> qrys = new List<string>();
                                List<Result> results = new List<Result>();
                                List<VertexInfo> vertices = MapData.Dependency.com.doosan.fms.model.Convert.GetFmsModelVertices(fyml);
                                List<LaneInfo> lanes = MapData.Dependency.com.doosan.fms.model.Convert.GetFmsModelLanes(fyml);

                                using (DatabaseManger dm = new PostgresqlManager())
                                {
                                    string connectionString;
#if (!DEBUG)
{
                                connectionString = dm.GetConnectionString(dbUserId, dbUserPw, dbIp, dbPort, databaseServerName);
}
#else
                                    {
                                        connectionString = com.doosan.fms.commonLib.ConstItem.ConstItems.DB_CONNECT_STRING;
                                    }
#endif
                                    if (string.IsNullOrEmpty(connectionString) == true) throw new Exception("connectionstring is null");
                                    if (dm.Connect(connectionString, ref message) == false)
                                    {
                                        throw new Exception(message);
                                    }

                                    if (dm.TryBeginTrans(ref message) == false) throw new Exception(message);
                                    if (alreadyDelete == "Y")
                                    {
                                        string qry = VertexInfo.GetRemoveAllQuery();
                                        Result result = new Result();
                                        qrys.Add(qry);
                                        results.Add(result);

                                        result = dm.ExcuteNonQuery(qry);
                                        if (result.Success == false) throw new Exception($"throw : {result.Message} 진행(QUERY) {qrys.ToStringLF()}");

                                        string qry2 = LaneInfo.GetRemoveAllQuery();
                                        Result result2 = new Result();
                                        qrys.Add(qry2);
                                        results.Add(result2);

                                        result2 = dm.ExcuteNonQuery(qry2);
                                        if (result2.Success == false) throw new Exception($"throw : {result.Message} 진행(QUERY) {qrys.ToStringLF()}");
                                    }

                                    {
                                        foreach (var vertex in vertices)
                                        {
                                            string qry = vertex.GetCreateQuery();
                                            Result result = new Result();
                                            qrys.Add(qry);
                                            results.Add(result);

                                            result = dm.ExcuteNonQuery(qry);
                                            if (result.Success == false) throw new Exception($"throw : {result.Message} 진행(QUERY) {qrys.ToStringLF()}");
                                        }
                                    }

                                    {
                                        foreach (var lane in lanes)
                                        {
                                            string qry = lane.GetCreateQuery();
                                            Result result = new Result();
                                            qrys.Add(qry);
                                            results.Add(result);

                                            result = dm.ExcuteNonQuery(qry);
                                            if (result.Success == false) throw new Exception($"throw : {result.Message} 진행(QUERY) {qrys.ToStringLF()}");
                                        }
                                    }
                                    if (dm.CommitTrans(ref message) == false) throw new Exception(message);
                                }
                                Console.WriteLine($"정상적으로 완료되었습니다. 총 개수 : {results.Count} vertices : {vertices.Count} lanes : {lanes.Count}");
                            }
                            catch (Exception exAll)
                            {
                                Console.WriteLine(exAll.Message);
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Fail : {ex.Message}");
                            continue;
                        }
                    }
                    else if (enumRead == ReadCommand.Logging)
                    {
                        var logList = sharedList.Clone();
                        foreach (var log in logList)
                        {
                            if (log != null)
                            {
                                Console.WriteLine(log.LogData());
                            }
                        }
                    }
                    else if (enumRead == ReadCommand.Fps)
                    {
                        while (true)
                        {
                            if (Console.KeyAvailable)
                            {
                                ConsoleKeyInfo key = Console.ReadKey(true);
                                if (key.Key == ConsoleKey.F1)
                                {
                                    Console.Clear();
                                    break;
                                }
                            }
                            else
                            {
                                double millsec = manager.GetHitMillsec();
                                double fps = manager.GetCalcFps();
                                Console.Write("F1을 누르면 이전 화면으로 이동됩니다. \t");
                                Console.WriteLine($"delta : {millsec} fps : {fps}");
                                Console.WriteLine($"HubReconnect Counter : {manager.StateHubReconnectCounter}");
                                Console.WriteLine($"Engine Loop Counter : {manager.StateProgramCounter}");
                            }
                        }
                    }
                    else if (enumRead == ReadCommand.MdcEngine)
                    {
                        while (true)
                        {
                            try
                            {
                                Console.Clear();
                                Console.WriteLine($"try read database");

                                string qry;
                                Result result;
                                Dictionary<PrefabType, string> qrys = new Dictionary<PrefabType, string>();
                                Dictionary<PrefabType, Result> results = new Dictionary<PrefabType, Result>();
                                Dictionary<PrefabType, List<FmsModel>> prefabDatas = new Dictionary<PrefabType, List<FmsModel>>();

                                foreach (PrefabType type in Enum.GetValues(typeof(PrefabType)))
                                {
                                    var realPrefabType = MapData.Dependency.com.doosan.fms.model.TypeConverter.ConvertPrefabTypeToFmsModelType(type);
                                    if (realPrefabType == default(Type)) continue;

                                    qry = ReflectHelper.StaticMethodGeneric<string>(realPrefabType, true, "GenInitializeQuery", null, new Type[] { realPrefabType });
                                    if (string.IsNullOrEmpty(qry)) continue;
                                    using (DatabaseManger dm = new PostgresqlManager())
                                    {
                                        if (dm.Connect(DB_CONNECT_STRING, ref message) == false)
                                        {
                                            throw new Exception($"DB CONNECTION FAIL : {message}");
                                        }
                                        result = dm.ExcuteQuery(qry);
                                        if (result.Success == false) throw new Exception($"DB Query Fail : {result.Message} QRY : {qry}");
                                    }
                                    qrys.Add(type, qry);
                                    results.Add(type, result);
                                    prefabDatas.Add(type, ReflectHelper.StaticMethodGeneric<List<FmsModel>>(realPrefabType, true, "GetModels", new object[] { result.Table }, new Type[] { realPrefabType }));
                                }

                                Console.WriteLine($"try manager.initialize ");
                                if (manager.InitializePrefabs(prefabDatas, ref message) == false) throw new Exception(message);
                                if (manager.InitializeDeltaItems(ref message, new DeltaObjectType[] { DeltaObjectType.DeltaTimer, DeltaObjectType.DeltaMap }) == false) throw new Exception(message);

                                Console.WriteLine($"try MdcEngine Start ");
                                TokenTask<TaskType> task;
                                if (tasks.TryGetValue(TaskType.MdcEnginePolling, out task))
                                {
                                    while (true)
                                    {
                                        Console.WriteLine($"try terminate : prev MdcEngine Task");
                                        if (task.Dispose(ref message))
                                        {
                                            Console.WriteLine($"terminated : prev MdcEngine Task");
                                            break;
                                        }
                                        Console.WriteLine(message);
                                        await Task.Delay(2500);
                                    }
                                    tasks.Remove(task.Key);
                                    task = null;
                                }

                                task = new TokenTask<TaskType>(TaskType.MdcEnginePolling);
                                tasks.Add(TaskType.MdcEnginePolling, task);
                                CancellationTokenSource token = new CancellationTokenSource();
                                tasks[TaskType.MdcEnginePolling].StartParam(token, new MdcEnginePollingArgs(manager, sharedList, token.Token).ToObject(), MdcEnginePolling.GenerateThread(), ref message);
                                Console.WriteLine($"MdcEngine Start Success");
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"MdcEngine Start Fail : {ex.Message}");
                                break;
                            }
                        }
                    }
                    else if (enumRead == ReadCommand.DatabasePolling)
                    {
                        try
                        {
                            Console.WriteLine($"try DatabasePollingFmsModel Start ");
                            TokenTask<TaskType> task;

                            if (tasks.TryGetValue(TaskType.DatabasePollingFmsModel, out task))
                            {
                                while (true)
                                {
                                    Console.WriteLine($"try terminate : prev DatabasePollingFmsModel Task");
                                    if (task.Dispose(ref message))
                                    {
                                        Console.WriteLine($"terminated : prev DatabasePollingFmsModel Task");
                                        break;
                                    }
                                    Console.WriteLine(message);
                                    await Task.Delay(2500);
                                }
                                tasks.Remove(task.Key);
                                task = null;
                            }

                            task = new TokenTask<TaskType>(TaskType.DatabasePollingFmsModel);
                            tasks.Add(TaskType.DatabasePollingFmsModel, task);
                            CancellationTokenSource token = new CancellationTokenSource();
                            tasks[TaskType.DatabasePollingFmsModel].StartParam(token, new DatabasePollingFmsModelArgs(sharedList, token.Token).ToObject(), DatabasePollingFmsModel.GenerateThread(), ref message);
                            Console.WriteLine($"DatabasePollingFmsModel Start Success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else if (enumRead == ReadCommand.Terminate)
                    {
                        foreach (var task in tasks)
                        {
                            task.Value.Dispose(ref message);
                        }
                        Environment.Exit(-1);
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid Command Code Not Found : {read}");
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid Command : {read}");
                }
            }
        }

    }
}
