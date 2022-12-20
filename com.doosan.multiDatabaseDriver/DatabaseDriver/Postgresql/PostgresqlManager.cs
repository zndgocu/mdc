using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using com.doosan.multiDatabaseDriver.DatabaseDriver.DatabaseResult;
using Npgsql;
using System;
using System.Data;

namespace com.doosan.multiDatabaseDriver.DatabaseDriver.Postgresql
{
    public class PostgresqlManager : DatabaseManger
    {
        NpgsqlConnection _connection;
        NpgsqlTransaction _transaction;
        bool _bistransaction;

        public override string GetConnectionString(string userId, string userPw, string ip, string port, string databaseServerName)
        {
            return $"User ID = {userId}; Password = {userPw}; Host = {ip}; Port = {port}; Database = {databaseServerName}; Pooling = true; ";
        }

        #region dispose
        protected override void DisposeRuntime()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
        #endregion

        #region connect, disconnect
        protected override bool ConnectRuntime(string connectionString, ref string message)
        {
            try
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();

                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    _connection.Dispose();
                    message = "Database Open Fail";
                    return false;
                }

                return true;
            }
            catch (Exception exMessage)
            {
                message = exMessage.Message;
                return false;
            }
            finally
            {

            }
        }
        #endregion

        #region transaction
        protected override bool TryBeginTransRuntime(ref string message)
        {
            try
            {
                _transaction = _connection.BeginTransaction();
                _bistransaction = true;

                return true;
            }
            catch (Exception exMessage)
            {
                _bistransaction = false;
                message = exMessage.Message;
                return false;
            }
            finally
            {

            }
        }

        protected override bool CommitTransRuntime(ref string message)
        {
            try
            {
                if (_bistransaction == true)
                {
                    _transaction.Commit();
                    _bistransaction = false;
                }
                return true;
            }
            catch (Exception exMessage)
            {
                message = exMessage.Message;
                return false;
            }
            finally
            {

            }
        }

        protected override bool RollbackTransRuntime(ref string message)
        {
            try
            {
                if (_bistransaction == true)
                {
                    _transaction.Rollback();
                    _bistransaction = false;
                }

                return true;
            }
            catch (Exception exMessage)
            {
                message = exMessage.Message;
                return false;
            }
            finally
            {

            }
        }

        #endregion

        #region dml
        protected override Result ExcuteQueryRuntime(string queryString)
        {
            Result result = new Result();
            DataTable table = new DataTable();

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(queryString, _connection);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

                da.Fill(table);

                result.SetDataTable(table);
                result.SetSuccess(true);
            }
            catch (Exception exAll)
            {
                result.SetSuccess(false);
                result.SetMessage(exAll.Message);
            }
            return result;
        }

        protected override Result ExcuteNonQueryRuntime(string queryString)
        {
            Result result = new Result();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(queryString, _connection);

                cmd.ExecuteNonQuery();

                result.SetSuccess(true);
            }
            catch (Exception exAll)
            {
                result.SetSuccess(false);
                result.SetMessage(exAll.Message);
            }
            return result;
        }
        #endregion
    }
}
