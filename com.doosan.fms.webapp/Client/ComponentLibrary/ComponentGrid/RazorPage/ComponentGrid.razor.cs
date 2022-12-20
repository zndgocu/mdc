using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentGrid.DataObject;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentGrid.RazorPage
{
    public partial class ComponentGrid<Titem> where Titem : new()
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public GridDataObject<Titem> GridDataObject { get; set; }

        private MudTable<Dictionary<string, string>> _refTable;
        private HashSet<Dictionary<string, string>> _selectedItems;
        private Dictionary<string, string> _editBeforeItem;

        private bool _editMode = false;

        public string LastMessage = "";

        private void Toggle(object b, string switchId)
        {
            if (switchId == "EDIT")
            {
                _editMode = (bool)b;
                _refTable.SetEditingItem(null);
            }
        }

        private void AA()
        {

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            GridDataObject.GridProperties.LoadingAnim = false;
            await base.OnAfterRenderAsync(firstRender);
        }


        private bool GetMultiSelect()
        {
            if (_editMode == true) return false;
            if (GridDataObject.GridProperties.EditBlock == true) return true;
            return false;
        }

        private bool GetReadOnly()
        {
            if (GridDataObject.GridProperties.ReadOnly == true) return true;
            if (_editMode == true) return false;
            return true;
        }
        private void OnSwitchChanged()
        {
            _refTable.SetEditingItem(null);
            //_editMode = false;
        }
        protected override async Task OnInitializedAsync()
        {
            _selectedItems = new HashSet<Dictionary<string, string>>();
            _editBeforeItem = new Dictionary<string, string>();
            await base.OnInitializedAsync();
        }

        private bool FilterFunc(Dictionary<string, string> dict)
        {

            if (string.IsNullOrWhiteSpace(GridDataObject.SearchText))
                return true;

            foreach (var item in dict.Values)
            {
                if (item == null || item.Length < 1) continue;
                if (item.Contains(GridDataObject.SearchText, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        private void BackupItem(object element)
        {
            var dict = (Dictionary<string, string>)element;
            _editBeforeItem = new Dictionary<string, string>(dict);
        }
        private void ResetItemToOriginalValues(object element)
        {
            var dict = (Dictionary<string, string>)element;
            foreach (var pair in dict)
            {
                dict[pair.Key] = _editBeforeItem[pair.Key];
            }
            SnackBar.Add("ResetItemToOriginalValues", Severity.Info);
        }

        private async Task ItemHasBeenCommitted(object element)
        {
            try
            {
                if (GridDataObject != null)
                {
                    var dict = (Dictionary<string, string>)element;
                    var typeObject = GridDataObjectData<Titem>.GetDictionaryToData(dict);
                    var result = await GridDataObject.OnEventEditCommit(typeObject);

                    if (result == false)
                    {
                        ResetItemToOriginalValues(element);
                        return;
                    }
                    SnackBar.Add("update", Severity.Success);
                }
                else
                {
                    throw new Exception("gridDataObject not found");
                }
            }
            catch (Exception ex)
            {
                SnackBar.Add(ex.Message, Severity.Error);
            }
        }
        private async Task AddRow()
        {
            var result = await GridDataObject.OnEventAddRow();
            if (result == null) return;
            GridDataObject.GridData.Datas.Add(GridDataObjectData<Titem>.GetDataDictionary(result));
            await _refTable.ReloadServerData();

        }
        private async Task RemoveRow()
        {
            try
            {
                _refTable.SetEditingItem(null);
                if (_selectedItems == null) throw new Exception("selecteditem is nothing");
                if (_selectedItems.Count < 1)
                {
                    SnackBar.Add("select zero");
                    return;
                }
                List<Titem> selectedTitems = new List<Titem>();
                foreach (var dict in _selectedItems)
                {
                    var typeObject = GridDataObjectData<Titem>.GetDictionaryToData(dict);
                    if (typeObject != null)
                    {
                        selectedTitems.Add(typeObject);
                    }
                }
                var result = await GridDataObject.OnEventRemoveRow(selectedTitems);
                if (result == false)
                {
                    throw new Exception("result faile");
                }

                GridDataObject.GridData.Datas.RemoveAll(x => _selectedItems.Any(z => z == x));
                _selectedItems.Clear();
                await _refTable.ReloadServerData();
                SnackBar.Add("remove", Severity.Success);
            }
            catch (Exception ex)
            {
                SnackBar.Add(ex.Message);
            }
        }
    }
}