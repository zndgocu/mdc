using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCardGrid.DataObject;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCardGrid.RazorPage
{
    public partial class ComponentCardGrid
    {
        [Parameter]
        public CardGridDataObject CardGridDataObject { get; set; }

        public async Task OnClickIcon(CardData cardData)
        {
            await CardGridDataObject.OnEventCardClick(cardData);
        }
    }
}
