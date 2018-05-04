using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using WindowsMLDemos.Common;
using WindowsMLDemos.Common.Helper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ResNet50
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ResNet50Model model;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ImagePickerControl_ImagePreviewReceived(object sender, WindowsMLDemos.Common.UI.ImagePreviewReceivedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                await EvaluteImageAsync(e.PreviewImage);
            });
        }
        private async Task EvaluteImageAsync(VideoFrame videoFrame)
        {
            try
            {
                var startTime = DateTime.Now;
                if (model == null)
                {
                    var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/Resnet50.onnx"));
                    if (modelFile != null)
                    {
                        model = new ResNet50Model();
                        await MLHelper.CreateModelAsync(modelFile, model);
                    }
                }

                var input = new ResNet50ModelInput()
                {
                    image = videoFrame
                };

                var res = await model.EvaluateAsync(input) as ResNet50ModelOutput;
                if (res != null)
                {
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
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        previewControl.EvalutionTime = (DateTime.Now - startTime).TotalSeconds.ToString();
                        outputText.Text = res.classLabel.FirstOrDefault();
                        resultList.ItemsSource = results;
                    });
                }
            }
            catch (Exception ex)
            {
                await AlertHelper.ShowMessageAsync(ex.ToString());
            }
        }
    }
}
