using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsMLDemos.Common.Helper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AnimeScale2x
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AnimeScaleModel model;
        public MainPage()
        {
            this.InitializeComponent();
        }



        private async Task EvaluteImageAsync(VideoFrame videoFrame)
        {
            var startTime = DateTime.Now;
            if (model == null)
            {
                var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/AnimeScale2x.onnx"));
                if (modelFile != null)
                {
                    model = new AnimeScaleModel();
                    await MLHelper.CreateModelAsync(modelFile, model);
                }
            }
            var input = new AnimeScaleModelInput()
            {
                 input = videoFrame
            };

            try
            {
                var res = await model.EvaluateAsync(input) as AnimeScaleModelOutput;
                if (res != null)
                {
                   
                }
            }
            catch (Exception ex)
            {
                await AlertHelper.ShowMessageAsync(ex.ToString());
            }
        }

        private async void ImagePickerControl_ImagePreviewReceived(object sender, WindowsMLDemos.Common.UI.ImagePreviewReceivedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                await EvaluteImageAsync(e.PreviewImage);
            });
        }
    }
}
