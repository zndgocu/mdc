using System.Data;

namespace com.doosan.multiDatabaseDriver.DatabaseDriver.DatabaseResult
{
    public class Result
    {
        private bool _success;
        private DataTable _table;
        private string _message;
        private int _result_count;

        public bool Success { get => _success; }
        public DataTable Table { get => _table; }
        public string Message { get => _message; }
        public int Result_count { get => _result_count; set => _result_count = value; }

        public Result()
        {
            _success = false;
            _table = null;
            _message = default(string);
            _result_count = default(int);
        }
        public void SetSuccess(bool success)
        {
            _success = success;
        }
        public void SetDataTable(DataTable dataTable)
        {
            _table = dataTable;
        }
        public void SetMessage(string message)
        {
            _message = message;
        }
        public void SetResultCount(int result_count)
        {
            _result_count = result_count;
        }
    }
}
