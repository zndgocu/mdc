using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCardGrid.DataObject;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCardGrid
{
    public partial class InfoComponentCardGrid
    {
        RazorPage.ComponentCardGrid _componentCardGrid;
        CardGridDataObject _dataObjectCardGrid;

        [Inject]
        public ISnackbar SnackBar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            CardGridDataObjectProperties prop = new CardGridDataObjectProperties(12, Justify.Center);
            List<CardData> cardDatas = new List<CardData>();
            for (int ite = 0; ite < 12; ite++)
            {
                cardDatas.Add(new CardData(3, $"btn1{ite}", Icons.Material.Filled.ThumbUp));
            }

            _dataObjectCardGrid = new CardGridDataObject(prop, cardDatas);
            _dataObjectCardGrid.EventCardClick += OnCardClick;
            await base.OnInitializedAsync();
        }

        public async Task<bool> OnCardClick(CardData cardData)
        {
            SnackBar.Add($"click {cardData.Text}");
            await Task.Delay(0);
            return true;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}