using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsMLDemos.Common;
using WindowsMLDemos.Common.Helper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FNSSeries
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ModelInfo model;
        VideoFrame currentFile = null;
        class ModelInfo
        {
            public string Name { get; set; }
            public string File { get; set; }
            public IMachineLearningModel Model { get; set; }
        }

        List<ModelInfo> models = new List<ModelInfo>
        {

            new ModelInfo
            {
                Name="Mosaic",
                File="ms-appx:///Model/FNS-Mosaic.onnx"
            },
            new ModelInfo
            {
                Name="Feathers",
                File="ms-appx:///Model/FNS-Feathers.onnx"
            },
            new ModelInfo
            {
                Name="Candy",
                File="ms-appx:///Model/FNS-Candy.onnx"
            },
            new ModelInfo
            {
                Name="La-Muse",
                File="ms-appx:///Model/FNS-La-Muse.onnx"
            },
            new ModelInfo
            {
                Name="Scream",
                File="ms-appx:///Model/FNS-The-Scream.onnx"
            },
            new ModelInfo
            {
                Name="Udnie",
                File="ms-appx:///Model/FNS-Udnie.onnx"
            }

        };
        public MainPage()
        {
            this.InitializeComponent();
            ModelList.ItemsSource = models;
            ModelList.SelectedIndex = 0;
        }

        private async void ModelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentFile != null)
            {
                await ApplyEffectAsync(currentFile);
            }
        }

        private async void ImagePickerControl_ImagePreviewReceived(object sender, WindowsMLDemos.Common.UI.ImagePreviewReceivedEventArgs e)
        {
            currentFile = e.PreviewImage;
            await ApplyEffectAsync(e.PreviewImage);
        }

        private async Task ApplyEffectAsync(VideoFrame frame)
        {

            if (frame != null)
            {

                var imageWidth = frame.SoftwareBitmap != null ? frame.SoftwareBitmap.PixelWidth :
                              frame.Direct3DSurface.Description.Width;
                var imageHeigth = frame.SoftwareBitmap != null ? frame.SoftwareBitmap.PixelHeight :
                           frame.Direct3DSurface.Description.Height;

                model = ModelList.SelectedItem as ModelInfo;
                if (model == null)
                {
                    model = models.FirstOrDefault();
                }
                if (model.Model == null)
                {
                    var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(model.File));
                    if (modelFile != null)
                    {
                        model.Model = new FNSModel();
                        await MLHelper.CreateModelAsync(modelFile, model.Model);
                    }
                }
                var startTime = DateTime.Now;
                var output = await model.Model.EvaluateAsync(new FNSInput
                {
                    inputImage = ImageFeatureValue.CreateFromVideoFrame(frame)
                }) as FNSOutput;

                if (output != null)
                {
                    previewControl.EvalutionTime = (DateTime.Now - startTime).TotalSeconds.ToString();
                    var lineLength = imageHeigth * imageWidth;
                    var newImageData = new byte[4 * lineLength];
                    var outData = output.outputImage.GetAsVectorView().ToArray();
                    if (outData.Length > 0)
                    {
                        var bData = outData.Take(lineLength).ToArray();
                        var rData = outData.Skip(lineLength * 2).Take(lineLength).ToArray();
                        var gData = outData.Skip(lineLength).Take(lineLength).ToArray();


                        for (var i = 0; i < lineLength; i++)
                        {
                            var b = (bData[i]);//* 255);
                            if (b < 0)
                            {
                                b = 0;
                            }
                            else if (b > 255)
                            {
                                b = 255;
                            }
                            newImageData[i * 4 + 0] = (byte)b;
                            var g = (gData[i]);// * 255);
                            if (g < 0)
                            {
                                g = 0;
                            }
                            else if (g > 255)
                            {
                                g = 255;
                            }
                            newImageData[i * 4 + 1] = (byte)g;
                            var r = (rData[i]);// * 255);
                            if (r < 0)
                            {
                                r = 0;
                            }
                            else if (r > 255)
                            {
                                r = 255;
                            }
                            newImageData[i * 4 + 2] = (byte)r;
                            newImageData[i * 4 + 3] = 255;
                        }

                    }

                    using (var ms = new InMemoryRandomAccessStream())
                    {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)imageWidth, (uint)imageHeigth, frame.SoftwareBitmap.DpiX, frame.SoftwareBitmap.DpiY, newImageData);
                        await encoder.FlushAsync();

                        var decoder = await BitmapDecoder.CreateAsync(ms);
                        var sbmp = await decoder.GetSoftwareBitmapAsync();
                        sbmp = SoftwareBitmap.Convert(sbmp, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);
                        var tsbs = new SoftwareBitmapSource();
                        await tsbs.SetBitmapAsync(sbmp);
                        previewImage.Source = tsbs;
                    }
                }
            }

        }
    }
}
