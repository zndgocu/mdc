using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using com.doosan.multiDatabaseDriver.DatabaseDriver.Postgresql;
using System;

namespace com.doosan.multiDatabaseDriver.DatabaseDriver
{
    public class test
    {

        void A()
        {
            using (DatabaseManger dm = new PostgresqlManager())
            {
                string connectionString = "";
                string message = "";
                string qryString = "";
                try
                {
                    if (dm.Connect(connectionString, ref message) == false) throw new Exception(message);
                    var result = dm.ExcuteQuery(qryString);
                    if (result.Success == false) throw new Exception(result.Message);
                }
                catch (Exception exAll)
                {
                    Console.WriteLine(exAll.Message);
                }
            }
        }
    }
}
