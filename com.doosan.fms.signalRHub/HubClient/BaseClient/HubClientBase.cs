using com.doosan.fms.commonLib.JWT.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace com.doosan.fms.signalRHub.HubClient.BaseClient
{
    public class HubClientBase<T> : IAsyncDisposable where T : new()
    {
        private HubConnection _connection;
        private bool _reconnect;
        private string _jwtToken;

        public HubClientBase(bool reconnect)
        {
            _reconnect = reconnect;
        }

        public event Func<Exception, Task> EventClosedHubConnection;
        protected virtual async void OnClosed(Exception e)
        {
            EventClosedHubConnection?.Invoke(e);
            if (_reconnect == true) await Connect();
        }

        protected void On(string funcName, Action<T> action)
        {
            if (_connection == null) return;
            _connection.On<T>(funcName, action);
        }
        protected void On(string funcName, Action action)
        {
            if (_connection == null) return;
            _connection.On(funcName, action);
        }

        /// <summary>
        /// closed 함수를 오버라이딩 할 경우 initialize 이전에 반드시 값을 지정해야 합니다.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool Initialize(string url, string identityName, ref string message)
        {
            try
            {
                string jwtToken = SignalRJWTHelper.GenerateJWTToken(identityName, SignalRPolicies.RoleRequest);
                _connection = new HubConnectionBuilder()
                    //.WithUrl("http://localhost:53353/ChatHub")
                    .WithUrl(url, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(jwtToken);
                    })
                    .Build();
                _connection.Closed += EventClosedHubConnection;
                _jwtToken = jwtToken;
                return true;
            }
            catch (Exception exception)
            {
                message = exception.Message;
                return false;
            }
        }

        public string GetToken()
        {
            return _jwtToken;
        }

        public string GetId()
        {
            if (_connection == null) return "";
            return _connection.ConnectionId;
        }
        public bool IsConnect()
        {
            if (_connection == null) return false;
            return (_connection.State == HubConnectionState.Connected);
        }

        public async Task DisConnect()
        {
            if (_connection == null) return;
            await _connection.StopAsync();
        }

        public async Task<bool> Connect()
        {
            try
            {
                if (_connection == null) return false;
                await _connection.StartAsync();
                return (_connection.State == HubConnectionState.Connected);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        public async void SendMessage(string funcName, T message)
        {
            try
            {
                if (_connection == null) throw new Exception("_connection is null");
                if (_connection.State != HubConnectionState.Connected) throw new Exception("_connection is not connected");
                await _connection.InvokeAsync(funcName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// connection은 base클래스에서 삭제됩니다. DisposeAsync()
        /// </summary>
        /// <returns></returns>
        protected virtual async ValueTask DisposeChild()
        {
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }

            await DisposeChild();
        }
    }
}
