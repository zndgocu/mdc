using com.doosan.multiDatabaseDriver.DatabaseDriver.DatabaseResult;
using System;
using System.Linq;

namespace com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver
{
    public class DatabaseManger : IDatabaseManger, IDisposable
    {
        public void Dispose()
        {
            DisposeRuntime();
        }
        protected virtual void DisposeRuntime() { }

        public string IGetConnectionString(string userId, string userPw, string ip, string port, string databaseServerName)
        {
            return GetConnectionString(userId, userPw, ip, port, databaseServerName);
        }
        public virtual string GetConnectionString(string userId, string userPw, string ip, string port, string databaseServerName)
        {
            return "";
        }

        public bool Connect(string connectionString, ref string message)
        {
            return ConnectRuntime(connectionString, ref message);
        }

        protected virtual bool ConnectRuntime(string connectionString, ref string message)
        {
            return false;
        }

        public bool TryBeginTrans(ref string message)
        {
            return TryBeginTransRuntime(ref message);
        }

        protected virtual bool TryBeginTransRuntime(ref string message)
        {
            return false;
        }
        public bool CommitTrans(ref string message)
        {
            return CommitTransRuntime(ref message);
        }
        protected virtual bool CommitTransRuntime(ref string message)
        {
            return false;
        }

        public bool RollbackTrans(ref string message)
        {
            return RollbackTransRuntime(ref message);
        }
        protected virtual bool RollbackTransRuntime(ref string message)
        {
            return false;
        }


        public Result ExcuteQuery(string queryString)
        {
            var result = ExcuteQueryRuntime(queryString);
            if (result.Table != null)
            {
                result.Result_count = result.Table.Rows.Count;
            }
            return result;
        }
        protected virtual Result ExcuteQueryRuntime(string queryString)
        {
            return new Result();
        }

        public Result ExcuteNonQuery(string queryString)
        {
            return ExcuteNonQueryRuntime(queryString);
        }
        protected virtual Result ExcuteNonQueryRuntime(string queryString)
        {
            return new Result();
        }

        public Result[] ExcuteQuerys(params string[] qrys)
        {
            if (qrys == null) return null;
            if (qrys.Count() < 1) return null;
            Result[] results = new Result[qrys.Count()];
            for (var iteQry = 0; iteQry < qrys.Count(); iteQry++)
            {
                results[iteQry] = ExcuteQuery(qrys[iteQry]);
            }
            return results;
        }


    }
}
