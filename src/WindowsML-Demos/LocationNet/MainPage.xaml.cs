using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.AI.MachineLearning;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using WindowsMLDemos.Common.Helper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LocationNet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        RN1015k500Model rnModel;
        public MainPage()
        {
            this.InitializeComponent();
            MapService.ServiceToken = "l7QWfzg1pPJNRPPzlEQg~_fVLj1OfFUe94HFOTybBMQ~AoGGvitgFze_nDATIPyFOj91OtKiMw6DZuP-kDw-KChZ6R1PckLNSgfqdViTkUPU";

        }

        private async void ImagePickerControl_ImagePreviewReceived(object sender, WindowsMLDemos.Common.UI.ImagePreviewReceivedEventArgs e)
        {
            try
            {
                myMap.Visibility = Visibility.Visible;
                if (rnModel == null)
                {
                    var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/RN1015k500.onnx"));
                    if (modelFile != null)
                    {
                        rnModel = new RN1015k500Model();
                        await MLHelper.CreateModelAsync(modelFile, rnModel);
                    }
                }
                if (e.PreviewImage != null)
                {
                    var output = await rnModel.EvaluateAsync(new RN1015k500Input
                    {
                        data = ImageFeatureValue.CreateFromVideoFrame(e.PreviewImage)//TensorFloat16Bit.CreateFromArray(new long[] { -1, 3, 224, 224 }, imageData)
                    }) as RN1015k500Output;

                    if (output != null)
                    {
                        var res = output.classLabel.GetAsVectorView().ToArray();
                        var b = output.softmax_output?.ToList();
                        if (res.Length > 0)
                        {
                            var locationData = res[0].Split('\t');
                            var city = locationData[0];
                            var lat = float.Parse(locationData[1]);
                            var lon = float.Parse(locationData[2]);
                            var sPoint = new Geopoint(new BasicGeoposition
                            {
                                Latitude = lat,
                                Longitude = lon
                            });


                            Geopoint pointToReverseGeocode = sPoint;

                            // Reverse geocode the specified geographic location.
                            MapLocationFinderResult result =
                                  await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

                            // If the query returns results, display the name of the town
                            // contained in the address of the first result.
                            if (result.Status == MapLocationFinderStatus.Success)
                            {
                                city = result.Locations[0].DisplayName;// + result.Locations[0].Address.FormattedAddress + result.Locations[0].Address.Country;
                            }
                            myMap.Center = sPoint;

                            myMap.Layers.Add(new MapElementsLayer
                            {
                                ZIndex = 1,
                                MapElements = new List<MapElement>()
                                  {
                                      new MapIcon
                                      {
                                          Location = sPoint,
                                          NormalizedAnchorPoint = new Windows.Foundation.Point(0.5,1),
                                          ZIndex = 0,
                                          Title = city,
                                          CollisionBehaviorDesired= MapElementCollisionBehavior.RemainVisible,
                                          Visible= true
                                      }
                                  }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await AlertHelper.ShowMessageAsync(ex.ToString());
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Set your current location.
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // Get the current location.
                    Geolocator geolocator = new Geolocator();
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;
                    // Set the map location.
                    myMap.Center = myLocation;
                    myMap.ZoomLevel = 11;
                    myMap.LandmarksVisible = true;
                    break;

                case GeolocationAccessStatus.Denied:
                    // Handle the case  if access to location is denied.
                    break;

                case GeolocationAccessStatus.Unspecified:
                    // Handle the case if  an unspecified error occurs.
                    break;
            }
        }
    }
}
