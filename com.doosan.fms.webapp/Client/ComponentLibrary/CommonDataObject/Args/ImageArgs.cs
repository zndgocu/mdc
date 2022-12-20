using com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Enum;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.CommonDataObject.Args
{
    public class ImageArgs
    {
        public string Text;
        public ImageType ImgType;
        public string ImgResource;

        public bool IsText()
        {
            return !(string.IsNullOrEmpty(Text));
        }
        public string GetImageSrc()
        {
            if (ImgType == ImageType.base64)
            {
                return $"data:image/png;base64, {ImgResource}";
            }
            return ImgResource;
        }
    }
}
