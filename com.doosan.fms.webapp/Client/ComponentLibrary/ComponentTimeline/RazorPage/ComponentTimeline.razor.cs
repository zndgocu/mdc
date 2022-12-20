using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args;
using com.doosan.fms.webapp.Client.ComponentLibrary.ComponentTimeline.DataObject;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentTimeline.RazorPage
{
    public partial class ComponentTimeline
    {
        [Parameter]
        public TimelineDataObject TimelineDataObject { get; set; }

        private int _severitiesIndex = 0;
        private Severity[] _severities =
        {
            Severity.Error,
            Severity.Info,
            Severity.Normal,
            Severity.Success,
            Severity.Warning,
        };


        void OnTimelineItemClick(HrefArgs args)
        {
            TimelineDataObject.OnEventClick(args);
        }

        public Severity GetRandomServerity()
        {
            _severitiesIndex++;
            if (_severitiesIndex > _severities.Length - 1)
            {
                _severitiesIndex = 0;
            }
            return _severities[_severitiesIndex];
        }

    }
}
