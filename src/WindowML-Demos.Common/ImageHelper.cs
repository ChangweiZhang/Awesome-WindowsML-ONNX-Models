using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace WindowML_Demos.Common
{
    public class ImageHelper
    {
        public static async Task<StorageFile> PickerImageAsync()
        {
            var imagePicker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.Desktop
            };

            imagePicker.ViewMode = PickerViewMode.List;
            imagePicker.FileTypeFilter.Add(".jpg");
            imagePicker.FileTypeFilter.Add(".png");
            var file = await imagePicker.PickSingleFileAsync();
            return file;
        }
        
        public static Task<SoftwareBitmap> ResizeImageToSquareAsync(IRandomAccessStream stream,
            int targetSize,
            BitmapPixelFormat format = BitmapPixelFormat.Unknown,
            BitmapAlphaMode alphaMode = BitmapAlphaMode.Ignore)
        {
            return ResizeImageAsync(stream, targetSize, targetSize);
        }

        public static async Task<SoftwareBitmap> ResizeImageAsync(IRandomAccessStream stream,
            int targetWidth,
            int targetHeight,
            BitmapPixelFormat format = BitmapPixelFormat.Unknown,
            BitmapAlphaMode alphaMode = BitmapAlphaMode.Ignore)
        {

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            var originalPixelWidth = decoder.PixelWidth;
            var originalPixelHeight = decoder.PixelHeight;

            using (var outputStream = new InMemoryRandomAccessStream())
            {
                //create encoder based on decoder of the source file
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);
                double widthRatio = (double)targetWidth / originalPixelWidth;
                double heightRatio = (double)targetHeight / originalPixelHeight;
                uint aspectHeight = (uint)targetHeight;
                uint aspectWidth = (uint)targetWidth;
                uint cropX = 0, cropY = 0;
                var scaledWith = (uint)targetWidth;
                var scaledHeight = (uint)targetHeight;
                if (originalPixelWidth > originalPixelHeight)
                {
                    aspectWidth = (uint)(heightRatio * originalPixelWidth);
                    cropX = (aspectWidth - aspectHeight) / 2;
                }
                else
                {
                    aspectHeight = (uint)(widthRatio * originalPixelHeight);
                    cropY = (aspectHeight - aspectWidth) / 2;
                }
                //you can adjust interpolation and other options here, so far linear is fine for thumbnails
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                encoder.BitmapTransform.ScaledHeight = aspectHeight;
                encoder.BitmapTransform.ScaledWidth = aspectWidth;
                encoder.BitmapTransform.Bounds = new BitmapBounds()
                {
                    Width = scaledWith,
                    Height = scaledHeight,
                    X = cropX,
                    Y = cropY,
                };
                await encoder.FlushAsync();

                //get reszied image
                var outputDecoder = await BitmapDecoder.CreateAsync(outputStream);
                var outputImg = await outputDecoder.GetSoftwareBitmapAsync();
                // apply alpha mode
                outputImg = SoftwareBitmap.Convert(outputImg, outputImg.BitmapPixelFormat, alphaMode);
                //apply piexl format
                if (format != BitmapPixelFormat.Unknown && format != decoder.BitmapPixelFormat)
                {
                    return SoftwareBitmap.Convert(outputImg, format);
                }
                return outputImg;
            }
        }
    }
}
