using com.doosan.multiDatabaseDriver.DatabaseDriver.DatabaseResult;

namespace com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver
{
    public interface IDatabaseManger
    {
        public bool Connect(string connectionString, ref string message);
        public bool TryBeginTrans(ref string message);
        public bool CommitTrans(ref string message);
        public bool RollbackTrans(ref string message);
        public Result ExcuteQuery(string queryString);
        public Result[] ExcuteQuerys(params string[] qrys);
        public Result ExcuteNonQuery(string queryString);
        public string IGetConnectionString(string userId, string userPw, string ip, string port, string databaseServerName);
    }
}
