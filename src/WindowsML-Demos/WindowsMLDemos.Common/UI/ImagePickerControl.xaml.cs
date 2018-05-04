using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.System.Display;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WindowsMLDemos.Common.Helper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WindowsMLDemos.Common.UI
{
    public sealed partial class ImagePickerControl : UserControl
    {
        public Canvas DetectorCanvas
        {
            get
            {
                return previewCanvas;
            }
        }
        public VideoEncodingProperties EncodingProperties
        {
            get;
            private set;
        }



        public bool WideImage
        {
            get { return (bool)GetValue(WideImageProperty); }
            set { SetValue(WideImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WideImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WideImageProperty =
            DependencyProperty.Register("WideImage", typeof(bool), typeof(ImagePickerControl), new PropertyMetadata(false));


        public string EvalutionTime
        {
            get { return (string)GetValue(EvalutionTimeProperty); }
            set
            {
                SetValue(EvalutionTimeProperty, value);
                evaluateTimeText.Text = value;
            }
        }

        // Using a DependencyProperty as the backing store for EvalutionTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EvalutionTimeProperty =
            DependencyProperty.Register("EvalutionTime", typeof(string), typeof(ImagePickerControl), new PropertyMetadata(null));



        public int ImageTargetWidth
        {
            get { return (int)GetValue(ImageTargetWidthProperty); }
            set { SetValue(ImageTargetWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageTargetWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageTargetWidthProperty =
            DependencyProperty.Register("ImageTargetWidth", typeof(int), typeof(ImagePickerControl), new PropertyMetadata(299));




        public int ImageTargetHeight
        {
            get { return (int)GetValue(ImageTargetHeightProperty); }
            set { SetValue(ImageTargetHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageTargetHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageTargetHeightProperty =
            DependencyProperty.Register("ImageTargetHeight", typeof(int), typeof(ImagePickerControl), new PropertyMetadata(299));



        public int PreviewInterval
        {
            get { return (int)GetValue(PreviewIntervalProperty); }
            set { SetValue(PreviewIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewIntervalProperty =
            DependencyProperty.Register("PreviewInterval", typeof(int), typeof(ImagePickerControl), new PropertyMetadata(3));





        MediaCapture mediaCapture;
        ThreadPoolTimer timer;
        private bool isPreviewing = false;
        DisplayRequest displayRequest = new DisplayRequest();

        //public event EventHandler<ImageReceivedEventArgs> ImageReceived;
        public event EventHandler<ImagePreviewReceivedEventArgs> ImagePreviewReceived;

        public ImagePickerControl()
        {
            this.InitializeComponent();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
            if (mediaCapture != null)
            {
                if (isPreviewing)
                {
                    await mediaCapture.StopPreviewAsync();
                    isPreviewing = false;
                }
                mediaCapture.Dispose();
                mediaCapture = null;
            }
            PreviewControl.Visibility = Visibility.Collapsed;
            inputImage.Visibility = Visibility.Visible;
            var file = await ImageHelper.PickerImageAsync();
            if (file != null)
            {
                using (var fs = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {

                    using (var tempStream = fs.CloneStream())
                    {
                        SoftwareBitmap softImg;
                        if (WideImage)
                        {
                            softImg = await ImageHelper.GetImageAsync(tempStream);
                        }
                        else
                        {
                            softImg = await ImageHelper.ResizeImageAsync(tempStream, ImageTargetWidth, ImageTargetHeight);
                        }
                        var img = new SoftwareBitmapSource();
                        await img.SetBitmapAsync(SoftwareBitmap.Convert(softImg, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore));
                        inputImage.Source = img;


                        //if (ImageReceived != null)
                        //{
                        //    ImageReceived(this, new ImageReceivedEventArgs(softImg));
                        //}
                        if (ImagePreviewReceived != null)
                        {
                            ImagePreviewReceived(this, new ImagePreviewReceivedEventArgs(VideoFrame.CreateWithSoftwareBitmap(softImg), true));
                        }
                    }
                }
            }
        }

        private async void captureBtn_Click(object sender, RoutedEventArgs e)
        {
            PreviewControl.Visibility = Visibility.Visible;
            inputImage.Visibility = Visibility.Collapsed;

            if (mediaCapture == null)
            {
                var targetWidth = ImageTargetWidth;
                var targetHeight = ImageTargetHeight;
                try
                {
                    var isWideImage = WideImage;
                    timer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                    {
                        if (mediaCapture != null)
                        {
                            try
                            {

                                var previewProperties = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                                if (previewProperties != null)
                                {
                                    EncodingProperties = previewProperties;
                                }
                                //if (previewProperties.Width != (uint)ImageTargetWidth &&
                                //previewProperties.Height != (uint)ImageTargetHeight)
                                //{
                                //    previewProperties.Width = (uint)ImageTargetWidth;
                                //    previewProperties.Height = (uint)ImageTargetHeight;
                                //    await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, previewProperties);
                                //}
                                // Get information about the preview
                                VideoFrame previewFrame;
                                if (isWideImage)
                                {
                                    var wideFrame = new VideoFrame(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, targetWidth, targetHeight);
                                    previewFrame = await mediaCapture.GetPreviewFrameAsync(wideFrame);
                                    previewFrame = wideFrame;
                                }
                                else
                                {
                                    previewFrame = await mediaCapture.GetPreviewFrameAsync();
                                }
                                if (previewFrame != null)
                                {
                                    VideoFrame resizedFrame;
                                    if (isWideImage)
                                    {
                                        resizedFrame = previewFrame;
                                    }
                                    else
                                    {
                                        resizedFrame = await ImageHelper.ResizeVideoFrameAsync(previewFrame, previewProperties, targetWidth, targetHeight);
                                    }

                                    if (ImagePreviewReceived != null)
                                    {
                                        ImagePreviewReceived(this, new ImagePreviewReceivedEventArgs(resizedFrame));
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }

                        }
                    }, TimeSpan.FromSeconds(PreviewInterval));
                    await StartPreviewAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {

                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                await AlertHelper.ShowMessageAsync("The app was denied access to the camera");
                return;
            }

            try
            {
                PreviewControl.Source = mediaCapture;

                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
            }
            catch (System.IO.FileLoadException)
            {
                mediaCapture.CaptureDeviceExclusiveControlStatusChanged += _mediaCapture_CaptureDeviceExclusiveControlStatusChanged;
            }

        }

        private void _mediaCapture_CaptureDeviceExclusiveControlStatusChanged(MediaCapture sender, MediaCaptureDeviceExclusiveControlStatusChangedEventArgs args)
        {
        }

    }
}
