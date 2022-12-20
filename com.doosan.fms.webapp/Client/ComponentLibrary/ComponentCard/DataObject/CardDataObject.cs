using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args;
using System;
using System.Collections.Generic;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCard.DataObject
{
    public class CardDataObject
    {
        public ImageArgs HrefImage;
        public List<HrefArgs> HrefArgss;
        public event Action<HrefArgs> EventClick;
        public void OnEventClick(HrefArgs args)
        {
            EventClick?.Invoke(args);
        }

        public string GetImageSrc()
        {
            if (HrefImage == null) return "";
            return HrefImage.GetImageSrc();
        }

        public CardDataObject(ImageArgs imageArgs, List<HrefArgs> hrefList)
        {
            HrefImage = imageArgs;
            HrefArgss = hrefList;
        }
    }
}
