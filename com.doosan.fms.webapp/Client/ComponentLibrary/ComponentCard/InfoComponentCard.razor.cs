using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args;
using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Enum;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCard.DataObject;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCard
{
    public partial class InfoComponentCard
    {
        [Inject]
        public ISnackbar _snackbar { get; set; }

        RazorPage.ComponentCard _componentCard;
        CardDataObject _dataObjectCard;

        protected override async Task OnInitializedAsync()
        {
            ImageArgs cardDataObjectImageArgs = new ImageArgs();
            cardDataObjectImageArgs.ImgType = ImageType.base64;
            cardDataObjectImageArgs.ImgResource = "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAAJUlEQVR42u3NQQEAAAQEsJNcdFLw2gqsMukcK4lEIpFIJBLJS7KG6yVo40DbTgAAAABJRU5ErkJggg==";
            List<HrefArgs> cardDataObjectHerfArgs = new List<HrefArgs>();
            var args1 = new HrefArgs("Test1");
            var args2 = new HrefArgs("Test2");
            cardDataObjectHerfArgs.Add(args1);
            cardDataObjectHerfArgs.Add(args2);

            _dataObjectCard = new CardDataObject(cardDataObjectImageArgs, cardDataObjectHerfArgs);
            _dataObjectCard.EventClick += OnLinkClick;
            await base.OnInitializedAsync();
        }

        async void OnLinkClick(HrefArgs args)
        {
            _snackbar.Add($"OnLinkClick {args.Text}");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
