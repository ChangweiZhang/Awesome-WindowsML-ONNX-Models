using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.System.Display;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WindowsMLDemos.Common;
using WindowsMLDemos.Common.Helper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InceptionV3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture mediaCapture;
        ThreadPoolTimer timer;
        private bool isPreviewing = false;
        DisplayRequest displayRequest = new DisplayRequest();
        IMachineLearningModel model;
        public MainPage()
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
                    var img = new BitmapImage();
                    await img.SetSourceAsync(fs);
                    inputImage.Source = img;
                    using (var tempStream = fs.CloneStream())
                    {
                        var softImg = await ImageHelper.ResizeImageToSquareAsync(tempStream, 299);

                        if (model == null)
                        {
                            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/Inceptionv3.onnx"));
                            if (modelFile != null)
                            {
                                model = new Inceptionv3Model();
                                await MLHelper.CreateModelAsync(modelFile, model);
                            }
                        }
                        var input = new Inceptionv3ModelInput()
                        {
                            image = VideoFrame.CreateWithSoftwareBitmap(softImg)
                        };

                        try
                        {
                            var startTime = DateTime.Now;
                            var res = await model.EvaluateAsync(input) as Inceptionv3ModelOutput;
                            if (res != null)
                            {
                                evaluateTimeText.Text = (DateTime.Now - startTime).TotalSeconds.ToString();
                                outputText.Text = res.classLabel.FirstOrDefault();
                                var results = new List<LabelResult>();
                                foreach (var kv in res.classLabelProbs)
                                {
                                    results.Add(new LabelResult
                                    {
                                        Label = kv.Key,
                                        Result = (float)Math.Round(kv.Value * 100, 2)
                                    });
                                }
                                results.Sort((p1, p2) =>
                                {
                                    return p2.Result.CompareTo(p1.Result);
                                });
                                resultList.ItemsSource = results;
                            }
                        }
                        catch (Exception ex)
                        {
                            await AlertHelper.ShowMessageAsync(ex.ToString());
                        }
                    }
                }
            }
        }

        private async void captureBtn_Click(object sender, RoutedEventArgs e)
        {
            PreviewControl.Visibility = Visibility.Visible;
            inputImage.Visibility = Visibility.Collapsed;
            if (model == null)
            {
                var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/Inceptionv3.onnx"));
                if (modelFile != null)
                {
                    model = new Inceptionv3Model();
                    await MLHelper.CreateModelAsync(modelFile, model);
                }
            }
            if (mediaCapture == null)
            {

                timer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    if (mediaCapture != null)
                    {
                        try
                        {
                            // Get information about the preview
                            var previewProperties = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

                            // Create a video frame in the desired format for the preview frame
                            VideoFrame videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, 299, 299);

                            VideoFrame previewFrame = await mediaCapture.GetPreviewFrameAsync(videoFrame);
                            //await Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                            //{
                            //    var ss = new SoftwareBitmapSource();
                            //    await ss.SetBitmapAsync(previewFrame.SoftwareBitmap);
                            //    inputImage.Source = ss;
                            //});

                            var input = new Inceptionv3ModelInput()
                            {
                                image = previewFrame
                            };

                            var startTime = DateTime.Now;
                            var res = await model.EvaluateAsync(input) as Inceptionv3ModelOutput;
                            if (res != null)
                            {
                                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                                {
                                    evaluateTimeText.Text = (DateTime.Now - startTime).TotalSeconds.ToString();
                                    outputText.Text = res.classLabel.FirstOrDefault();
                                    var results = new List<LabelResult>();
                                    foreach (var kv in res.classLabelProbs)
                                    {
                                        results.Add(new LabelResult
                                        {
                                            Label = kv.Key,
                                            Result = (float)Math.Round(kv.Value * 100, 2)
                                        });
                                    }
                                    results.Sort((p1, p2) =>
                                    {
                                        return p2.Result.CompareTo(p1.Result);
                                    });
                                    resultList.ItemsSource = results;
                                });

                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                            // await AlertHelper.ShowMessageAsync(ex.ToString());
                        }

                    }
                }, TimeSpan.FromSeconds(3));
                await StartPreviewAsync();
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
