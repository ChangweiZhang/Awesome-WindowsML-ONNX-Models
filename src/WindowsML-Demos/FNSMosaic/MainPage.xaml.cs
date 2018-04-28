using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsMLDemos.Common;
using WindowsMLDemos.Common.Helper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FNSMosaic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        FNSMosaicModel model;
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
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
                        var softImg = await ImageHelper.ResizeImageToSquareAsync(tempStream, 720);

                        if (model == null)
                        {
                            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/FNS-Mosaic.onnx"));
                            if (modelFile != null)
                            {
                                model = new FNSMosaicModel();
                                await MLHelper.CreateModelAsync(modelFile, model);
                            }
                        }
                        var input = new FNSMosaicModelInput()
                        {
                            inputImage = VideoFrame.CreateWithSoftwareBitmap(softImg)
                        };

                        try
                        {
                            var res = await model.EvaluateAsync(input) as FNSMosaicModelOutput;
                            if (res != null)
                            {
                                var outputSource = new SoftwareBitmapSource();
                                await outputSource.SetBitmapAsync(res.outputImage.SoftwareBitmap);
                                outputImage.Source = outputSource;
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
    }
}

