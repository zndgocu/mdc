using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.DependencyItems.Js.Container
{
    public class JsContainer
    {
        private List<JsItem> _jss;
        private IJSRuntime _jsr;

        public JsContainer(IJSRuntime jsr)
        {
            _jss = new List<JsItem>();
            _jsr = jsr;
        }

        public async Task<List<KeyValuePair<string, bool>>> LoadJsItems()
        {
            List<KeyValuePair<string, bool>> result = new List<KeyValuePair<string, bool>>();
            foreach (var js in _jss)
            {
                result.Add(new KeyValuePair<string, bool>(js.Key, await js.ImportAsync(_jsr)));
            }
            return result;
        }

        public async Task<bool> LoadJsItem(string key)
        {
            JsItem prevJs = _jss.Where(x => x.Key == key).FirstOrDefault();
            if (prevJs == null) return false;
            if (await prevJs.ImportAsync(_jsr) == false)
            {
                return false;
            }
            return true;
        }
        public JsItem AddJsItem(JsItem js)
        {
            JsItem prevJs = _jss.Where(x => x.Key == js.Key).FirstOrDefault();
            if (prevJs == null)
            {
                prevJs = js;
                _jss.Add(prevJs);
            }
            return prevJs;
        }

        public JsItem AddJsItem(string key, string path)
        {
            JsItem prevJs = _jss.Where(x => x.Key == key).FirstOrDefault();
            if (prevJs == null)
            {
                prevJs = new JsItem(key, path);
                _jss.Add(prevJs);
            }
            return prevJs;
        }

        public JsItem GetJsItem(string key)
        {
            return _jss.Where(x => x.Key == key).FirstOrDefault();
        }

    }
}
