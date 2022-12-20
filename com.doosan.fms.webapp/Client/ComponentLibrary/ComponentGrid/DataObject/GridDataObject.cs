using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentGrid.DataObject
{
    public class GridDataObjectPropertes
    {
        public GridDataObjectPropertes(bool addRow, bool removeRow, bool loadingAnim = false, bool dense = false, bool hover = false, bool readOnly = false, bool editBlock = false, bool cancelEdit = false, bool multiSelect = false)
        {
            AddRow = addRow;
            RemoveRow = removeRow;
            LoadingAnim = loadingAnim;
            Dense = dense;
            Hover = hover;
            ReadOnly = readOnly;
            EditBlock = editBlock;
            CancelEdit = cancelEdit;
            MultiSelect = multiSelect;
        }

        public bool AddRow { get; set; }
        public bool RemoveRow { get; set; }
        public bool LoadingAnim { get; set; }
        public bool Dense { get; set; }
        public bool Hover { get; set; }
        public bool ReadOnly { get; set; }
        public bool EditBlock { get; set; }
        public bool CancelEdit { get; set; }
        public bool MultiSelect { get; set; }
    }

    public class GridDataObjectData<T>
    {
        public GridDataObjectData()
        {
            Datas = new List<Dictionary<string, string>>();
            BindProps = GetProps();
        }

        public static Type DATA_TYPE = typeof(T);
        public static PropertyInfo[] DATA_PROPS = typeof(T).GetProperties();


        public List<Dictionary<string, string>> Datas { get; set; }
        public string[] BindProps { get; set; }



        public static T GetDictionaryToData(Dictionary<string, string> dict)
        {
            try
            {
                if (dict == null) return default(T);
                var type = typeof(T);
                var typeObject = Activator.CreateInstance(type);
                foreach (var pair in dict)
                {
                    PropertyInfo prop = typeObject.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null) continue;
                    var propTypVal = Convert.ChangeType(pair.Value, prop.PropertyType);
                    prop.SetValue(typeObject, propTypVal, null);
                }
                return (T)typeObject;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static List<Dictionary<string, string>> GetDataDictionarys(List<T> items)
        {
            if (items == null) return null;
            if (items.Count < 1) return null;
            List<Dictionary<string, string>> datas = new List<Dictionary<string, string>>();
            foreach (var item in items)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (var prop in DATA_PROPS)
                {
                    data.Add(prop.Name, prop.GetValue(item)?.ToString());
                }
                datas.Add(data);
            }
            return datas;
        }

        public static Dictionary<string, string> GetDataDictionary(T item)
        {
            if (item == null) return null;
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var prop in DATA_PROPS)
            {
                data.Add(prop.Name, prop.GetValue(item)?.ToString());
            }
            return data;
        }

        public static string[] GetProps()
        {
            return DATA_PROPS.Select(x => x.Name).ToArray();
        }
    }


    public class GridDataObject<T>
    {
        public GridDataObjectPropertes GridProperties;
        public GridDataObjectData<T> GridData;

        public GridDataObject()
        {
            HeaderText = nameof(T);
        }

        public string HeaderText { get; set; }
        public string SearchText { get; set; }


        public event Func<T, Task<bool>> EventEditCommit;
        public async Task<bool> OnEventEditCommit(T arg)
        {
            if (EventEditCommit == null) return false;
            return await EventEditCommit?.Invoke(arg);
        }

        public event Func<List<T>, Task<bool>> EventRemoveRow;
        public async Task<bool> OnEventRemoveRow(List<T> args)
        {
            if (EventRemoveRow == null) return false;
            return await EventRemoveRow.Invoke(args);
        }

        public event Func<Task<T>> EventAddRow;
        public async Task<T> OnEventAddRow()
        {
            if (EventAddRow == null) return default(T);
            return await EventAddRow.Invoke();
        }



    }
}
