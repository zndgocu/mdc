using com.doosan.fms.model.Runtime;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentGrid.DataObject;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentGrid.RazorPage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.Pages.Fms.Ntn
{
    public partial class PageNtnLang
    {
        ComponentGrid<NtnLang> _refNtnChoose;
        ComponentGrid<NtnLang> _refNtnAll;
        GridDataObject<NtnLang> _dataObjectNtnChoose;
        GridDataObject<NtnLang> _dataObjectNtnAll;

        protected override async Task OnInitializedAsync()
        {
            if (await InitializeGrid() == false)
            {
                SnackBar.Add("초기화에 실패했습니다. 새로고침 하세요");
            }
            await base.OnInitializedAsync();
        }

        private async Task<bool> InitializeGrid()
        {
            try
            {
                _dataObjectNtnChoose = new GridDataObject<NtnLang>();
                _dataObjectNtnChoose.GridData = new GridDataObjectData<NtnLang>();
                _dataObjectNtnChoose.GridProperties = new GridDataObjectPropertes(true, true, true, false, true, false, true);
                _dataObjectNtnChoose.HeaderText = nameof(NtnLang);
                _dataObjectNtnChoose.SearchText = "";

                _dataObjectNtnAll = new GridDataObject<NtnLang>();
                _dataObjectNtnAll.GridData = new GridDataObjectData<NtnLang>();
                _dataObjectNtnAll.GridProperties = new GridDataObjectPropertes(true, true, true, false, true, false, true);
                _dataObjectNtnAll.HeaderText = nameof(NtnLang);
                _dataObjectNtnAll.SearchText = "";


                _dataObjectNtnChoose.EventEditCommit += OnEditRow;
                _dataObjectNtnChoose.EventRemoveRow += OnRemoveRow;
                _dataObjectNtnChoose.EventAddRow += OnAddRow;

                _dataObjectNtnAll.EventEditCommit += OnEditRow;
                _dataObjectNtnAll.EventRemoveRow += OnRemoveRow;
                _dataObjectNtnAll.EventAddRow += OnAddRow;

                var result = await HttpClient.GetAsync("https://localhost:44304/fms/ntnlang/get");
                if (result.IsSuccessStatusCode)
                {
                    var resultString = await result.Content.ReadAsStringAsync();
                    var ntns = JsonConvert.DeserializeObject<List<NtnLang>>(resultString);
                    _dataObjectNtnChoose.GridData.Datas = GridDataObjectData<NtnLang>.GetDataDictionarys(ntns.Where(x => x.Ntn_cd == GetLang())?.ToList());

                    _dataObjectNtnAll.GridData.Datas = GridDataObjectData<NtnLang>.GetDataDictionarys(ntns);
                }
                else
                {
                    throw new Exception($"http call error");
                }
            }
            catch (Exception exception)
            {
                SnackBar.Add(exception.Message);
                return false;
            }
            return true;
        }

        public async Task<bool> OnEditRow(NtnLang item)
        {
            SnackBar.Add("수정");
            return true;
        }
        public async Task<bool> OnRemoveRow(List<NtnLang> items)
        {
            SnackBar.Add("삭제");
            return true;
        }
        public async Task<NtnLang> OnAddRow()
        {
            var a = new NtnLang();
            a.Ntn_cd = Guid.NewGuid().ToString();
            return a;
        }

    }
}
