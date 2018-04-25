using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WindowML_Demos.Common;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoogleNetPlaces
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GoogLeNetPlacesModelModel model;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LabelHelper.Labels.Count == 0)
            {
                await LabelHelper.LoadLabelAsync();
            }
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
                        var softImg = await ImageHelper.ResizeImageToSquareAsync(tempStream, 224);

                        if (model == null)
                        {
                            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/GoogLeNetPlaces.onnx"));
                            if (modelFile != null)
                            {
                                model = await GoogLeNetPlacesModelModel.CreateGoogLeNetPlacesModelModel(modelFile);
                            }
                        }
                        var input = new GoogLeNetPlacesModelModelInput()
                        {
                            sceneImage = VideoFrame.CreateWithSoftwareBitmap(softImg)
                        };

                        try
                        {
                            var res = await model.EvaluateAsync(input);
                            if (res != null)
                            {
                                outputText.Text = res.sceneLabel.FirstOrDefault();
                                var results = new List<LabelResult>();
                                foreach (var kv in res.sceneLabelProbs)
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
                            await AlertHelper.ShowMessage(ex.ToString());
                        }
                    }
                }
            }
        }
    }
}
