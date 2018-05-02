using Windows.Media;

namespace WindowsMLDemos.Common.UI
{
    public class ImagePreviewReceivedEventArgs
    {
        public VideoFrame PreviewImage { get; set; }
        public ImagePreviewReceivedEventArgs(VideoFrame frame)
        {
            PreviewImage = frame;
        }
    }
}
