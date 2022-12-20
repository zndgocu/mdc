using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using com.doosan.multiDatabaseDriver.DatabaseDriver.DatabaseResult;
using com.doosan.multiDatabaseDriver.DatabaseDriver.Postgresql;
using System;
using System.Threading.Tasks;
using static com.doosan.fms.commonLib.ConstItem.ConstItems;

namespace com.doosan.fms.mdc.Tasks
{
    public static class DatabasePollingFmsModel_dbtask
    {
        public static Task<Result> ExecuteQuery(string qry)
        {
            string message = "";
            Result result = new Result();

            try
            {
                using (DatabaseManger dm = new PostgresqlManager())
                {
                    if (dm.Connect(DB_CONNECT_STRING, ref message) == false)
                    {
                        throw new Exception(message);
                    }

                    result = dm.ExcuteQuery(qry);
                    if (result.Success == false) throw new Exception(result.Message);
                }

                return Task.FromResult<Result>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult<Result>(result);
            }
        }
    }
}
