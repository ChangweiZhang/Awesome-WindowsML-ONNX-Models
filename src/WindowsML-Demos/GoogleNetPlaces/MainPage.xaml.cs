using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WindowsMLDemos.Common;
using WindowsMLDemos.Common.Helper;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoogleNetPlaces
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GoogLeNetPlacesModelModel model;
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
                        var softImg = await ImageHelper.ResizeImageToSquareAsync(tempStream, 224);

                        if (model == null)
                        {
                            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/GoogLeNetPlaces.onnx"));
                            if (modelFile != null)
                            {
                                model = new GoogLeNetPlacesModelModel();
                                await MLHelper.CreateModelAsync(modelFile, model);
                            }
                        }
                        var input = new GoogLeNetPlacesModelModelInput()
                        {
                            sceneImage = VideoFrame.CreateWithSoftwareBitmap(softImg)
                        };

                        try
                        {
                            var res = await model.EvaluateAsync(input) as GoogLeNetPlacesModelModelOutput;
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
                            await AlertHelper.ShowMessageAsync(ex.ToString());
                        }
                    }
                }
            }
        }
    }
}
