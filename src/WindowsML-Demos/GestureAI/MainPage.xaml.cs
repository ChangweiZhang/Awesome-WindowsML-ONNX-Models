using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Input.Inking.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GestureAI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GestureAIModel model;
        public MainPage()
        {
            this.InitializeComponent();

            gestureCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Pen
                | Windows.UI.Core.CoreInputDeviceTypes.Touch;

            CoreWetStrokeUpdateSource coreWetStrokeUpdateSource = CoreWetStrokeUpdateSource.Create(gestureCanvas.InkPresenter);

            coreWetStrokeUpdateSource.WetStrokeContinuing += CoreWetStrokeUpdateSource_WetStrokeContinuing;

            gestureCanvas.InkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;

        }

        List<float> gesturePointList = new List<float>();

        private void CoreWetStrokeUpdateSource_WetStrokeContinuing(CoreWetStrokeUpdateSource sender, CoreWetStrokeUpdateEventArgs args)
        {
            var points = args.NewInkPoints;
            foreach (var point in points)
            {
                gesturePointList.Add((float)point.Position.X);
                gesturePointList.Add((float)point.Position.Y);
                gesturePointList.Add(0f);
            }

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {


            var offset = (int)(gesturePointList.Count / 120.0) * 3;
            var inputData = new List<float>();
            for (var i = 0; i < gesturePointList.Count - 3; i += offset + 3)
            {
                inputData.Add(gesturePointList[i] / 10000);
                inputData.Add(gesturePointList[i + 1] / 10000);
                inputData.Add(gesturePointList[i + 2] / 10000);
            }

            var inputList = new List<float>();
            if (inputData.Count > 120)
            {
                for (var i = 0; i < 120; i++)
                {
                    inputList.Add(inputData[i]);
                }

            }
            else
            {
                for (var i = 0; i < 120; i++)
                {
                    if (i < inputData.Count)
                    {
                        inputList.Add(inputData[i]);
                    }
                    else
                    {
                        inputList.Add(0f);
                    }
                }
            }

            var modelInput = new GestureAIModelInput
            {
                input1 = inputList
            };
            if (model == null)
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/GestureAI.onnx"));

                model = await GestureAIModel.CreateGestureAIModel(file);
            }

            var output = await model.EvaluateAsync(modelInput);
            gesturePointList.Clear();
            gestureCanvas.InkPresenter.StrokeContainer.Clear();
        }

        private void StrokeInput_StrokeEnded(Windows.UI.Input.Inking.InkStrokeInput sender, Windows.UI.Core.PointerEventArgs args)
        {
        }
    }
}
