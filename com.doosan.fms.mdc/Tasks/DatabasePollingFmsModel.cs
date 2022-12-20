using com.doosan.fms.commonLib.Helper;
using com.doosan.fms.commonLib.Threading;
using com.doosan.fms.model.Base;
using com.doosan.fms.model.Runtime;
using com.doosan.multiDatabaseDriver.DatabaseDriver.DatabaseResult;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.doosan.fms.mdc.Tasks
{
    public class DatabasePollingFmsModelArgs
    {
        public DatabasePollingFmsModelArgs(object args)
        {
            Array argArray = new object[3];
            argArray = (Array)args;
            SafetySharedList = (SafetySharedList<FmsModel>)argArray.GetValue(0);
            CancellationToken = (CancellationToken)argArray.GetValue(1);
        }

        public DatabasePollingFmsModelArgs(SafetySharedList<FmsModel> safetySharedList, CancellationToken cancellationToken)
        {
            SafetySharedList = safetySharedList;
            CancellationToken = cancellationToken;
        }

        public SafetySharedList<FmsModel> SafetySharedList { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public object ToObject()
        {
            return new object[] { SafetySharedList, CancellationToken };
        }
    }

    public static class DatabasePollingFmsModel
    {
        public static Thread GenerateThread()
        {
            return new Thread(new ParameterizedThreadStart(DoWork));
        }

        public static async void DoWork(object args)
        {
            DatabasePollingFmsModelArgs argsRepo = new DatabasePollingFmsModelArgs(args);

            string message = "";
            List<Type> _initializeObjects = new List<Type>
            {
                typeof(VertexInfo),
                typeof(LaneInfo),
                typeof(RobotData),
                typeof(TaskMst),
                typeof(TaskMstHis),
            };

            List<string> qrys = new List<string>();
            List<Result> results = new List<Result>();
            List<Task<Result>> tasks = new List<Task<Result>>();
            foreach (var type in _initializeObjects)
            {
                var instance = (FmsModel)Activator.CreateInstance(type);
                qrys.Add(instance.GetInitializeQuery());
            }

            while (argsRepo.CancellationToken.IsCancellationRequested == false)
            {
                #region Db work
                try
                {

                    if (qrys == null && qrys.Count < 1) throw new Exception("_initializeObjects zero");

                    foreach (var qry in qrys)
                    {
                        tasks.Add(Task<Result>.Run<Result>(() => DatabasePollingFmsModel_dbtask.ExecuteQuery(qry)));
                    }
                    await Task<Result>.WhenAll<Result>(tasks);

                    List<FmsModel> models = new List<FmsModel>();
                    for (var iteType = 0; iteType < _initializeObjects.Count; iteType++)
                    {
                        var v = ReflectHelper.StaticMethodGeneric<List<FmsModel>>(_initializeObjects[iteType], true, "GetModels", new object[] { tasks[iteType].Result.Table }, new Type[] { _initializeObjects[iteType] });
                        models.AddRange(v);
                    }

                    argsRepo.SafetySharedList.ClearAddRange(models);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    results.Clear();
                    tasks.Clear();
                }
                #endregion
            }
        }
    }
}
