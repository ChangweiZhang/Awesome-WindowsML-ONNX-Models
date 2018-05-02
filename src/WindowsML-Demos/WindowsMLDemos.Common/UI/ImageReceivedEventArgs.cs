using System;
using Windows.Graphics.Imaging;

namespace WindowsMLDemos.Common.UI
{
    public class ImageReceivedEventArgs : EventArgs
    {
        public SoftwareBitmap PickedImage { get; set; }
        public ImageReceivedEventArgs(SoftwareBitmap softwareBitmap)
        {
            PickedImage = softwareBitmap;
        }
    }
}
