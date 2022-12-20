using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentTimeline.DataObject
{
    public class TimelineDataObjectPropertes
    {
        public TimelineDataObjectPropertes(TimelinePosition position = TimelinePosition.Alternate, TimelineOrientation orientation = TimelineOrientation.Vertical, bool reverse = false)
        {
            Position = position;
            Orientation = orientation;
            Reverse = reverse;
        }

        public TimelinePosition Position { get; set; }
        public TimelineOrientation Orientation { get; set; }
        public bool Reverse { get; set; }
    }
    public class TimelineDataObject
    {
        public TimelineDataObjectPropertes TimelineProperties;
        public List<HrefArgs> HrefList;

        public TimelineDataObject(TimelineDataObjectPropertes timelineProperties, List<HrefArgs> hrefList)
        {
            TimelineProperties = timelineProperties;
            HrefList = hrefList;
        }

        public event Action<HrefArgs> EventClick;

        public void OnEventClick(HrefArgs args)
        {
            EventClick?.Invoke(args);
        }

        public void OnOrientationChange(TimelineOrientation value)
        {
            TimelineProperties.Orientation = value;
            switch (value)
            {
                case TimelineOrientation.Vertical:
                    if (TimelineProperties.Position is TimelinePosition.Top or TimelinePosition.Bottom)
                        TimelineProperties.Position = TimelinePosition.Start;
                    break;
                case TimelineOrientation.Horizontal:
                    if (TimelineProperties.Position is TimelinePosition.Start or TimelinePosition.Left or TimelinePosition.Right or TimelinePosition.End)
                        TimelineProperties.Position = TimelinePosition.Top;
                    break;
            }
        }

        public bool IsSwitchDisabled()
        {
            if (TimelineProperties.Position == TimelinePosition.Alternate)
                return false;
            else
                TimelineProperties.Reverse = false;
            return true;
        }
    }
}
