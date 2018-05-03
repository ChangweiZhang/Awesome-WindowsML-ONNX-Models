using Windows.Media;

namespace WindowsMLDemos.Common.UI
{
    public class ImagePreviewReceivedEventArgs
    {
        public VideoFrame PreviewImage { get; set; }
        public bool IsFileImage { get; set; }
        public ImagePreviewReceivedEventArgs(VideoFrame frame, bool isFileImage = false)
        {
            PreviewImage = frame;
            IsFileImage = isFileImage;
        }
    }
}
