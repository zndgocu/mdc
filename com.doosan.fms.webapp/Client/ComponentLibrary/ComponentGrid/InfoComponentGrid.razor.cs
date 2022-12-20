using com.doosan.fms.model.Runtime;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentGrid.DataObject;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentGrid
{
    public partial class InfoComponentGrid
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        RazorPage.ComponentGrid<VertexInfo> _componentGrid;
        GridDataObject<VertexInfo> _dataObjectGrid;

        protected override async Task OnInitializedAsync()
        {
            List<VertexInfo> vertices = new List<VertexInfo>();
            for (var ite = 0; ite < 100; ite++)
            {
                vertices.Add(new VertexInfo($"fleetname{ite}", ite, ite, $"name{ite}", ite));
            }

            _dataObjectGrid = new GridDataObject<VertexInfo>();

            GridDataObjectPropertes gridProperties = new GridDataObjectPropertes(true, true, true, false, true, true, true);
            GridDataObjectData<VertexInfo> gridData = new GridDataObjectData<VertexInfo>();
            gridData.Datas = GridDataObjectData<VertexInfo>.GetDataDictionarys(vertices);
            gridData.BindProps = GridDataObjectData<VertexInfo>.GetProps();

            _dataObjectGrid.GridData = gridData;
            _dataObjectGrid.GridProperties = gridProperties;
            _dataObjectGrid.HeaderText = "test1";
            _dataObjectGrid.SearchText = "";


            _dataObjectGrid.EventEditCommit += OnEditRow;
            _dataObjectGrid.EventRemoveRow += OnRemoveRow;
            _dataObjectGrid.EventAddRow += OnAddRow;

            await base.OnInitializedAsync();
        }

        public async Task<bool> OnEditRow(VertexInfo vertexInfo)
        {
            SnackBar.Add("수정");
            return true;
        }
        public async Task<bool> OnRemoveRow(List<VertexInfo> vertexInfos)
        {
            SnackBar.Add("삭제");
            return true;
        }
        public async Task<VertexInfo> OnAddRow()
        {
            var a = new VertexInfo();
            a.Fleet_name = Guid.NewGuid().ToString();
            return a;
        }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
