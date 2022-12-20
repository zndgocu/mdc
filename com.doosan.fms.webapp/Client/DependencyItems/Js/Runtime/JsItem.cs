using com.doosan.fms.webapp.Client.DependencyItems.Js.IRuntime;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.DependencyItems.Js
{
    public class JsItem : IJsItem
    {
        protected string _key;
        protected string _path;
        protected bool _loaded = false;
        protected IJSObjectReference _js;
        protected string _lastMessage;

        public JsItem(string key, string path)
        {
            _key = key;
            _path = path;
            _loaded = false;
            _js = null;
            _lastMessage = "";
        }

        public string Key { get => _key; set => _key = value; }

        public async Task<T> Call<T, U>(string func, U parm)
        {
            try
            {
                return await _js.InvokeAsync<T>(func, parm);
            }
            catch (Exception ex)
            {
                _lastMessage = ex.Message.ToString();
                return default(T);
            }
        }

        public async Task<T> CallNonParm<T>(string func)
        {
            try
            {
                return await _js.InvokeAsync<T>(func);
            }
            catch (Exception ex)
            {
                _lastMessage = ex.Message.ToString();
                return default(T);
            }
        }

        public async Task CallVoidNonParm(string func)
        {
            try
            {
                await _js.InvokeVoidAsync(func);
            }
            catch (Exception ex)
            {
                _lastMessage = ex.Message.ToString();
            }
        }

        public async Task CallVoidParm<U>(string func, U parm)
        {
            try
            {
                await _js.InvokeVoidAsync(func, parm);
            }
            catch (Exception ex)
            {
                _lastMessage = ex.Message.ToString();
            }
        }

        public async Task<bool> ImportAsync(IJSRuntime jsr)
        {
            try
            {
                if (_loaded == false)
                {
                    _js = await jsr.InvokeAsync<IJSObjectReference>("import", _path);
                }
                _loaded = (_js != null);
                return _loaded;
            }
            catch (Exception ex)
            {
                _loaded = false;
                _lastMessage = ex.Message.ToString();
                return false;
            }
        }
    }
}
