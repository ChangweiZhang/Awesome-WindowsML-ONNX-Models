using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsMLDemos.Common.Helper;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TinyYOLO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TinyYOLOModelModel model;
        SolidColorBrush LineBrush = new SolidColorBrush(color: Windows.UI.Colors.Red);
        SolidColorBrush LabelBrush = new SolidColorBrush(color: Windows.UI.Colors.White);
        List<SolidColorBrush> ClassColors = new List<SolidColorBrush>();
        public MainPage()
        {
            this.InitializeComponent();
            // Make colors for the bounding boxes. There is one color for each class,
            // 20 classes in total.
            foreach (var r in new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 })
            {
                foreach (var g in new double[] { 0.3, 0.7 })
                {
                    foreach (var b in new double[] { 0.4, 0.8 })
                    {
                        var color = Color.FromArgb(255, (byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
                        ClassColors.Add(new SolidColorBrush(color));
                    }
                }
            }
        }

        private async Task EvaluteImageAsync(VideoFrame videoFrame, bool isImage)
        {
            try
            {
                var startTime = DateTime.Now;
                if (model == null)
                {
                    var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/TinyYOLO.onnx"));
                    if (modelFile != null)
                    {
                        model = new TinyYOLOModelModel();
                        await MLHelper.CreateModelAsync(modelFile, model);
                    }
                }

                var input = new TinyYOLOModelModelInput()
                {
                    image = videoFrame
                };

                var res = await model.EvaluateAsync(input) as TinyYOLOModelModelOutput;
                if (res != null)
                {
                    var boxes = model.ComputeBoundingBoxes(res.grid);
                    await DrawDetectedObjectRectAsync(boxes, isImage);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        previewControl.EvalutionTime = (DateTime.Now - startTime).TotalSeconds.ToString();
                    });
                }
            }
            catch (Exception ex)
            {
                await AlertHelper.ShowMessageAsync(ex.ToString());
            }
        }

        private async Task DrawDetectedObjectRectAsync(List<Prediction> boxes, bool isImage, int width = 419, int height = 419)
        {
            var canvas = previewControl.DetectorCanvas;
            float sourceAspect = 1f;
            if (isImage)
            {
                sourceAspect = height * 1f / width;
            }
            else
            {
                sourceAspect = previewControl.EncodingProperties.Height * 1.0f / previewControl.EncodingProperties.Width;
            }
            float ch = 0, cw = 0;
            float sl = 0, aspect = 0, x = 0, y = 0;

            if (canvas.ActualHeight * 1.0f / canvas.ActualWidth > sourceAspect)
            {
                cw = (float)canvas.ActualWidth;
                ch = cw * sourceAspect;
                y = (float)(canvas.ActualHeight - ch) / 2;
            }
            else
            {
                ch = (float)canvas.ActualHeight;
                cw = ch / sourceAspect;
                x = (float)(canvas.ActualWidth - cw) / 2;
            }
            if (cw > ch)
            {
                sl = ch;
                aspect = sl / previewControl.ImageTargetHeight;
                x += (cw - sl) / 2;
            }
            else
            {
                sl = cw;
                aspect = sl / previewControl.ImageTargetWidth;
                y += (ch - sl) / 2;
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                canvas.Children.Clear();
                //var border = new Rectangle();
                //border.Width = 419 * aspect;
                //border.Height = 419 * aspect;
                //Canvas.SetLeft(border, x);
                //Canvas.SetTop(border, y);
                //border.Stroke = LineBrush;
                //border.StrokeThickness = 2;
                //canvas.Children.Add(border);

                foreach (var box in boxes)
                {
                    var rectGeo = new Rectangle();
                    rectGeo.StrokeThickness = 2;
                    rectGeo.Stroke = ClassColors[box.ClassIndex];
                    Canvas.SetLeft(rectGeo, box.Rect.X * aspect + x);
                    Canvas.SetTop(rectGeo, box.Rect.Y * aspect + y);
                    rectGeo.Width = box.Rect.Width * aspect;
                    rectGeo.Height = box.Rect.Height * aspect;
                    //Debug.WriteLine($"{box.Rect.X * aspect + x},{box.Rect.Y * aspect + y};{rectGeo.Width},{rectGeo.Height}");
                    canvas.Children.Add(rectGeo);

                    var titleBorder = new Border();
                    titleBorder.Background = ClassColors[box.ClassIndex];
                    var titleText = new TextBlock();
                    titleText.Foreground = LabelBrush;
                    titleText.FontSize = 18;
                    titleText.Margin = new Thickness(10, 1, 10, 1);
                    titleText.Text = $"{model.Labels[box.ClassIndex]}  {(float)Math.Round(box.Score * 100, 2)}";
                    Canvas.SetLeft(titleBorder, box.Rect.X * aspect + x);
                    Canvas.SetTop(titleBorder, box.Rect.Y * aspect + y - 25);
                    titleBorder.Child = titleText;
                    canvas.Children.Add(titleBorder);
                }
            });
        }

        private async void previewControl_ImagePreviewReceived(object sender, WindowsMLDemos.Common.UI.ImagePreviewReceivedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
             {
                 await EvaluteImageAsync(e.PreviewImage, e.IsFileImage);
             });
        }
    }
}
