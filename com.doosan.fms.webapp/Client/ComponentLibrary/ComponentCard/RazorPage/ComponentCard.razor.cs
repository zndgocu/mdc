using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCard.DataObject;
using Microsoft.AspNetCore.Components;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCard.RazorPage
{
    public partial class ComponentCard
    {
        [Parameter]
        public CardDataObject CardDataObject { get; set; }
    }
}
