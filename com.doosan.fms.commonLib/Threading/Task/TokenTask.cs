using System;
using System.Threading;

namespace com.doosan.fms.commonLib.Threading.Task
{
    public class TokenTask<T>
    {
        private T _key;
        private CancellationTokenSource _token;
        private Thread _thread;

        public TokenTask(T key, CancellationTokenSource token, Thread thread)
        {
            _key = key;
            _token = token;
            _thread = thread;
        }

        public TokenTask(T key)
        {
            _key = key;
        }
        public bool StartParam(CancellationTokenSource token, object args, Thread thread, ref string message)
        {
            try
            {
                _thread = thread;
                _token = token;
                thread.Start(args);
                return true;
            }
            catch (Exception ex)
            {
                _thread = null;
                _token = null;
                return false;
            }
        }

        public bool Start(CancellationTokenSource token, Thread thread, ref string message)
        {
            try
            {
                if (token == null) token = new CancellationTokenSource();
                _thread = thread;
                _token = token;
                thread.Start();
                return true;
            }
            catch (Exception ex)
            {
                _thread = null;
                _token = null;
                return false;
            }
        }

        public bool Start(Thread thread, ref string message)
        {
            try
            {
                return Start(new CancellationTokenSource(), thread, ref message);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public T Key { get => _key; set => _key = value; }

        public static CancellationTokenSource GenerateToken()
        {
            return new CancellationTokenSource();
        }


        /// <summary>
        /// 호출이후 해당 객체는 소멸됩니다.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Dispose(ref string message)
        {
            try
            {
                if (_token == null)
                {
                    message = "Token Is Null";
                    return false;
                }
                if (_token.Token.IsCancellationRequested == true)
                {
                    message = "already request";
                    return false;
                }

                _token.Cancel();
                _token.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
