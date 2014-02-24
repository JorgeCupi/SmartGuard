using Bing.Maps;
using BingMapsRESTService.Common.JSON;
using Newtonsoft.Json;
using SmartGuard.Core;
using SmartGuard.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
namespace SmartGuard.Store
{
    #region Converters

    public class ConvertVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool parameterString = (bool)value;

            if (parameterString)
                return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class AddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return AppResources.Near + (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string currentValue;
            if (SmartGuard.Store.App.DistancePreferredIsKilometers)
                currentValue = (double)value + AppResources.General_Kilometers;
            else currentValue = ((double)(value) / 1.6) + AppResources.General_Miles;

            return (currentValue + AppResources.General_NearYou);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class ConvertOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool parameterString = (bool)value;
            if (parameterString)
                return 0.5;
            else return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class CalcDistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string[] splitter = new string[] { "X" };
            string[] coords = ((string)value).Split(splitter, StringSplitOptions.None);
            double lat = Double.Parse(coords[0]);
            double lon = Double.Parse(coords[1]);

            double result = DistanceBetween(lat, lon, staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            if (SmartGuard.Store.App.DistancePreferredIsKilometers)
                return result.ToString("0.00") + " " + AppResources.General_KilometersNearYou;
            else return ((double)(result / 1.6)).ToString("0.00") + " " + AppResources.General_MilesNearYou;
        }

        private double DistanceBetween(double lat, double lon, double p1, double p2)
        {
            double x = 69.1 * (p1 - lat);
            double y = 53.0 * (p2 - lon);
            return Math.Sqrt(x * x + y * y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    #endregion Converters

    public static class staticObjects
    {
        public static double CurrentLatitude { get; set; }

        public static double CurrentLongitude { get; set; }

        public static string Address { get; set; }

        public static string City { get; set; }

        public static string Country { get; set; }

        public static double GoalLatitude { get; set; }

        public static double GoalLongitude { get; set; }

        public static Uri FriendProfilePicture { get; set; }
    }

    #region GPS
    public static class GPS
    {
        private static Geolocator watcher;
        private static string GooglesReverseGeocodingUri = "http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&sensor=true";
        public static async void InitializeGPS()
        {
            watcher = new Geolocator();
            try
            {
                Geoposition position = await watcher.GetGeopositionAsync();
                staticObjects.CurrentLatitude = position.Coordinate.Latitude;
                staticObjects.CurrentLongitude = position.Coordinate.Longitude;
                GetAddress(position.Coordinate.Latitude, position.Coordinate.Longitude);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                MessageBox.ShowAsync("Lo sentimos!. No pudimos averiguar tu posicion, intentalo de nuevo mas tarde", "Servicio de Geolocalizacion");
            }
        }
        public static async void GetDirections(Map myMap, Border RouteResults)
        {
            MapShapeLayer routeLayer = new MapShapeLayer();
            myMap.ShapeLayers.Add(routeLayer);

            Uri routeRequest = new Uri(string.Format(
                "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0},{1}&wp.1={2},{3}&rpo=Points&key={4}", staticObjects.CurrentLatitude, staticObjects.CurrentLongitude, staticObjects.GoalLatitude, staticObjects.GoalLongitude, myMap.Credentials));

            //Make a request and get the response
            Response r = await GetResponse(routeRequest);

            if (r != null &&
                r.ResourceSets != null &&
                r.ResourceSets.Length > 0 &&
                r.ResourceSets[0].Resources != null &&
                r.ResourceSets[0].Resources.Length > 0)
            {
                Route route = r.ResourceSets[0].Resources[0] as Route;

                //Get the route line data
                double[][] routePath = route.RoutePath.Line.Coordinates;
                LocationCollection locations = new LocationCollection();

                for (int i = 0; i < routePath.Length; i++)
                {
                    if (routePath[i].Length >= 2)
                    {
                        locations.Add(new Bing.Maps.Location(routePath[i][0],
                                        routePath[i][1]));
                    }
                }

                MapPolyline routeLine = new MapPolyline()
                {
                    Color = Colors.Indigo,
                    Locations = locations,
                    Width = 7
                };

                routeLayer.Shapes.Add(routeLine);

                BitmapImage srcStart = new BitmapImage();
                srcStart.UriSource = new Uri(App.fbProfilePicture, UriKind.Absolute);
                ImageBrush imgStart = new ImageBrush();
                imgStart.ImageSource = srcStart;
                Ellipse start = new Ellipse() { Fill = imgStart, Height = 70, Width = 70 };

                myMap.Children.Add(start);
                MapLayer.SetPosition(start,
                    new Bing.Maps.Location(route.RouteLegs[0].ActualStart.Coordinates[0],
                        route.RouteLegs[0].ActualStart.Coordinates[1]));

                BitmapImage srcEnd = new BitmapImage();
                srcEnd.UriSource = staticObjects.FriendProfilePicture;
                ImageBrush imgEnd = new ImageBrush();
                imgEnd.ImageSource = srcEnd;
                Ellipse end = new Ellipse() { Fill = imgEnd, Height = 70, Width = 70 };

                myMap.Children.Add(end);
                MapLayer.SetPosition(end,
                    new Bing.Maps.Location(route.RouteLegs[0].ActualEnd.Coordinates[0],
                    route.RouteLegs[0].ActualEnd.Coordinates[1]));

                myMap.SetView(new LocationRect(locations));
                RouteResults.DataContext = route;
            }
        }

        private static async Task<Response> GetResponse(Uri uri)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(uri);

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                return ser.ReadObject(stream) as Response;
            }
        }
        private static async void GetAddress(double lat, double lon)
        {
            GoogleReverseGeocoding.RootObject reverseGeocodingResult = new GoogleReverseGeocoding.RootObject();
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync(String.Format(GooglesReverseGeocodingUri,lat,lon));
            reverseGeocodingResult = JsonConvert.DeserializeObject<GoogleReverseGeocoding.RootObject>(result);

            staticObjects.City = reverseGeocodingResult.results[0].address_components[1].long_name;
            staticObjects.Country = reverseGeocodingResult.results[4].address_components[0].long_name;
            staticObjects.Address = reverseGeocodingResult.results[1].address_components[0].long_name + ", " +reverseGeocodingResult.results[0].address_components[0].long_name;
        }
    }
        #region Google's Reverse geocoding
    namespace GoogleReverseGeocoding
    {
        public class AddressComponent
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public List<string> types { get; set; }
        }

        public class Northeast
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Southwest
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Bounds
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Northeast2
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Southwest2
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Viewport
        {
            public Northeast2 northeast { get; set; }
            public Southwest2 southwest { get; set; }
        }

        public class Geometry
        {
            public Bounds bounds { get; set; }
            public Location location { get; set; }
            public string location_type { get; set; }
            public Viewport viewport { get; set; }
        }

        public class Result
        {
            public List<AddressComponent> address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public List<string> types { get; set; }
        }

        public class RootObject
        {
            public List<Result> results { get; set; }
            public string status { get; set; }
        }
    }
    #endregion
    #endregion
    public static class Broadcast
    {
        public static async void TryUploadPosition(bool intenseMode)
        {
            if (staticObjects.Address != null)
            {
                Position position = new Position();
                position.Address = staticObjects.Address;
                position.FacebookID = SmartGuard.Store.App.fbUserID;
                position.Latitude = staticObjects.CurrentLatitude;
                position.Country = staticObjects.Country;
                position.City = staticObjects.City;
                position.Longitude = staticObjects.CurrentLongitude;
                position.RegisteredAt = DateTime.UtcNow;
                position.PartitionKey = position.FacebookID;
                position.RowKey = string.Format("{0:d10}", DateTime.MaxValue.Ticks - position.RegisteredAt.Ticks);
                bool insertSucceeded = await Services.TryUploadPosition(position);

                string message = string.Empty;
                if (insertSucceeded)
                    message = AppResources.BroadcastView_BroadcastSucceeded;
                else message = AppResources.BroadcastView_BroadcastError;

                if (!intenseMode)
                    await MessageBox.ShowAsync(message, AppResources.BroadcastView_Title);
            }
            else await MessageBox.ShowAsync(AppResources.LocationService_NoService, AppResources.LocationService_Title);
        }
    }

    #region MessageBox

    internal class MessageBox
    {
        public static async Task<MessageBoxResult> ShowAsync(string messageBoxText,
                                                             string caption,
                                                             MessageBoxButton button)
        {
            MessageDialog md = new MessageDialog(messageBoxText, caption);
            MessageBoxResult result = MessageBoxResult.None;
            if (button.HasFlag(MessageBoxButton.OK))
            {
                md.Commands.Add(new UICommand("OK",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.OK)));
            }
            if (button.HasFlag(MessageBoxButton.Yes))
            {
                md.Commands.Add(new UICommand("Yes",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.Yes)));
            }
            if (button.HasFlag(MessageBoxButton.No))
            {
                md.Commands.Add(new UICommand("No",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.No)));
            }
            if (button.HasFlag(MessageBoxButton.Cancel))
            {
                md.Commands.Add(new UICommand("Cancel",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.Cancel)));
                md.CancelCommandIndex = (uint)md.Commands.Count - 1;
            }
            if (button.HasFlag(MessageBoxButton.SignOut))
            {
                md.Commands.Add(new UICommand("Sign out",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.SignOut)));
                md.CancelCommandIndex = (uint)md.Commands.Count - 1;
            }
            var op = await md.ShowAsync();
            return result;
        }

        public static async Task<MessageBoxResult> ShowAsync(string messageBoxText, string title)
        {
            return await MessageBox.ShowAsync(messageBoxText, title, MessageBoxButton.OK);
        }
    }

    // Summary:
    //     Specifies the buttons to include when you display a message box.
    [Flags]
    public enum MessageBoxButton
    {
        OK = 1,
        Cancel = 2,
        OKCancel = OK | Cancel,
        Yes = 4,
        No = 8,
        YesNo = Yes | No,
        SignOut = 16,
        SignOutCancel = SignOut | Cancel
    }

    // Summary:
    //     Represents a user's response to a message box.
    public enum MessageBoxResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 3,
        No = 4,
        SignOut = 5
    }

    #endregion MessageBox

    public static class Screen
    {
        public static Windows.Foundation.Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Windows.Foundation.Point point = buttonTransform.TransformPoint(new Windows.Foundation.Point());
            return new Windows.Foundation.Rect(point, new Windows.Foundation.Size(element.ActualWidth, element.ActualHeight));
        }
    }

    public static class Share
    {
        public static string Title { get; set; }

        public static string Message { get; set; }

        public static string Description { get; set; }

        public static string Link { get; set; }

        public static DataTransferManager Manager;

        public static void InitializeSharing(DataTransferManager manager, string ErrorMessage)
        {
            Manager = DataTransferManager.GetForCurrentView();
            Manager.DataRequested -= dataTransferManager_DataRequested;
            Manager.DataRequested += dataTransferManager_DataRequested;

            Title = String.Empty;
            Description = ErrorMessage;
            Message = String.Empty;
        }

        private static void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            request.Data.Properties.Description = Description;
            if (Title.Length > 0)
            {
                request.Data.Properties.Title = Title;
                request.Data.Properties.Description = Description;
                request.Data.SetText(Message);
            }
        }

        public static void ShowShareBar()
        {
            DataTransferManager.ShowShareUI();
        }

        public async static void ShareToFacebook(
            string Link,
            string Message,
            string FriendPicture)
        {
            var facebookClient = new Facebook.FacebookClient(App.fbToken);

            var postParams = new
            {
                name = AppResources.ApplicationTitle,
                caption = AppResources.ApplicationCaption,
                message = Message,
                picture = FriendPicture,
                link = Link
            };

            try
            {
                dynamic fbPostTaskResult = await facebookClient.PostTaskAsync("/me/feed", postParams);
                var result = (IDictionary<string, object>)fbPostTaskResult;

                MessageBox.ShowAsync(AppResources.FacebookSuccesfulPost, AppResources.Facebook);
            }
            catch (Exception ex)
            {
                MessageBox.ShowAsync(AppResources.FacebookErrorPost, AppResources.Facebook);
            }
        }
    }
}