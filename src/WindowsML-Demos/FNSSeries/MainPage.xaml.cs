using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
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
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
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
                          await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                          {
                              previewControl.EvalutionTime = (DateTime.Now - startTime).TotalSeconds.ToString();
                          });
                          var sbmp = await ImageHelper.GetImageFromTensorFloatDataAsync(output.outputImage, (uint)imageWidth,
                              (uint)imageHeigth, frame.SoftwareBitmap.DpiX, frame.SoftwareBitmap.DpiY);
                          sbmp = SoftwareBitmap.Convert(sbmp, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);
                          var tsbs = new SoftwareBitmapSource();
                          await tsbs.SetBitmapAsync(sbmp);
                          previewImage.Source = tsbs;
                      }
                  }
              });
        }
    }
}
