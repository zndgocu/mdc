using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.DependencyItems.Js.IRuntime
{
    public interface IJsItem
    {
        public Task<bool> ImportAsync(IJSRuntime jsr);


        /// <summary>
        /// JsInterop Call
        /// </summary>
        /// <param name="func">JS 펑션 이름</param>
        /// <returns></returns>
        public Task CallVoidNonParm(string func);

        /// <summary>
        /// JsInterop Call
        /// </summary>
        /// <typeparam name="T">리턴 타입</typeparam>
        /// <param name="func">펑션 네임</param>
        /// <returns></returns>
        public Task<T> CallNonParm<T>(string func);

        /// <summary>
        /// JsInterop Call
        /// </summary>
        /// <typeparam name="U">파라미터 타입</typeparam>
        /// <param name="func">JS 펑션 이름</param>
        /// <param name="parm">파라미터</param>
        /// <returns></returns>
        public Task CallVoidParm<U>(string func, U parm);

        /// <summary>
        /// JsInterop Call
        /// </summary>
        /// <typeparam name="T">리턴 타입</typeparam>
        /// <typeparam name="U">파라미터 타입</typeparam>
        /// <param name="func">JS 펑션 이름</param>
        /// <param name="parm">파라미터</param>
        /// <returns></returns>
        public Task<T> Call<T, U>(string func, U parm);
    }
}
