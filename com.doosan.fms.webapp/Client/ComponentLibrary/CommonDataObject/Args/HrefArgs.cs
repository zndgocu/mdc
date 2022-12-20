using MudBlazor;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args
{
    public class HrefArgs
    {
        public string Key;
        public string Text;
        public string Icon;
        public string Time;
        public string Contents;

        public HrefArgs(string key = "", string text = "", string icon = "", string time = "", string contents = "")
        {
            Key = key;
            Text = text;
            Icon = icon;
            Time = time;
            Contents = contents;
        }

        public string GetKey()
        {
            return (string.IsNullOrEmpty(Key)) ? Text : Key;
        }
        public string GetIcon()
        {
            if (string.IsNullOrEmpty(Icon))
            {
                return Icons.Filled.Workspaces;
            }
            return Icon;
        }
        public string GetMaterialIcon(string icon)
        {
            if (icon == "Camera")
            {
                return Icons.Material.Filled.Camera;
            }
            else
            {
                return Icons.Material.Filled.Workspaces;
            }
        }

        public string GetHref()
        {
            if (string.IsNullOrEmpty(Icon))
            {
                return Icons.Filled.Workspaces;
            }
            return Icon;
        }

        public string GetText()
        {
            if (string.IsNullOrEmpty(Text)) return "";
            return Text;
        }

        public string GetTime()
        {
            if (string.IsNullOrEmpty(Text)) return "";
            return Time;
        }

        public string GetContens()
        {
            if (string.IsNullOrEmpty(Text)) return "";
            return Contents;
        }


    }
}
