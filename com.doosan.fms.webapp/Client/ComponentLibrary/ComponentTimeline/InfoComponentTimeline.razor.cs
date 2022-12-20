using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args;
using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Enum;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentTimeline.DataObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentTimeline
{
    public partial class InfoComponentTimeline
    {
        RazorPage.ComponentTimeline _componentTimeline;
        TimelineDataObject _dataObjectTimeline;

        protected override async Task OnInitializedAsync()
        {
            ImageArgs timelineDataObjectImageHref = new ImageArgs();
            timelineDataObjectImageHref.ImgType = ImageType.base64;
            timelineDataObjectImageHref.ImgResource = "";
            List<HrefArgs> timelineDataObjectHerfArgs = new List<HrefArgs>();
            TimelineDataObjectPropertes timelineDataProperties = new TimelineDataObjectPropertes();

            string title = "";
            string time = "";
            string contents = "";

            for (int Ite = 0; Ite < 5; Ite++)
            {
                title = "Test" + Ite.ToString();
                time = "2022-08-26 00:00:0" + Ite.ToString();
                contents = "작업번호 : " + Ite.ToString();
                var args1 = new HrefArgs(title, "", time, contents);
                timelineDataObjectHerfArgs.Add(args1);
            }

            _dataObjectTimeline = new TimelineDataObject(timelineDataProperties, timelineDataObjectHerfArgs);
            _dataObjectTimeline.EventClick += OnItemClick;
            await base.OnInitializedAsync();
        }

        async void OnItemClick(HrefArgs args)
        {

        }

    }
}
