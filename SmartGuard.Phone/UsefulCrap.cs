using Microsoft.Phone.Globalization;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using SmartGuard.Core;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Facebook.Authenticate;
using SmartGuard.Core.Facebook.Classes;
using SmartGuard.Core.Facebook.Queries;
using SmartGuard.Core.Models;
using SmartGuard.Phone.Resources;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Phone.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
namespace SmartGuard.Phone  
{
    #region Converters
    public class ConvertVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool parameterString = (bool)value;

            if (parameterString)
                return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NoPositionsMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int Message = (int)value;
            if (Message == 35)
                return AppResources.Positions_NoPositionsToday;
            if (Message == 250)
                return AppResources.Positions_NoPositionsLastSevenDays;
            else return AppResources.Positions_NoPositionsLastMonth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class AddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return AppResources.Txt_Near + (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string currentValue;
            if (App.DistancePreferredIsKilometers)
                currentValue = (double)value + " kilometers ";
            else currentValue = ((double)(value) / 1.6) + " miles ";

            return (currentValue + "near you");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ConvertOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool parameterString = (bool)value;
            if (parameterString)
                return 0.5;
            else return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class CalcDistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] splitter = new string[] { "X" };
            string[] coords = ((string)value).Split(splitter, StringSplitOptions.None);
            double lat = Double.Parse(coords[0]);
            double lon = Double.Parse(coords[1]);

            double result = DistanceBetween(lat, lon, staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            if (App.DistancePreferredIsKilometers)
                return result.ToString("0.00") + " " + AppResources.General_Kilometers + AppResources.General_NearYou;
            else return ((double)(result / 1.6)).ToString("0.00") + " " + AppResources.General_Miles + AppResources.General_NearYou;
        }

        private double DistanceBetween(double lat, double lon, double p1, double p2)
        {
            double x = 69.1 * (p1 - lat);
            double y = 53.0 * (p2 - lon);
            return Math.Sqrt(x * x + y * y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
#endregion

    public static class staticObjects
    {
        public static double CurrentLatitude { get; set; }

        public static double CurrentLongitude { get; set; }

        public static string Address { get; set; }

        public static string City { get; set; }

        public static string Country { get; set; }
    }

    public static class GPS
    {
        public static Geolocator watcher;
        public static async Task<bool> InitializeGPS()
        {
            bool completed = false;

            watcher = new Geolocator();
            watcher.MovementThreshold = 500;
            watcher.PositionChanged += watcher_PositionChanged;
            watcher.StatusChanged += watcher_StatusChanged;

            Geoposition position = await watcher.GetGeopositionAsync();
            staticObjects.CurrentLatitude = position.Coordinate.Latitude;
            staticObjects.CurrentLongitude = position.Coordinate.Longitude;
            completed = await GetAddress(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            return completed;
        }

        static void watcher_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            switch (args.Status)
            {
                case PositionStatus.Disabled:
                     BackAgent.RemoveAgent("SmartGuardPeriodicAgent");
                    App.IntenseModeEnabled = false;
                    break;
                case PositionStatus.Initializing:
                    break;
                case PositionStatus.NoData:
                    MessageBox.Show(AppResources.LocationService_NoData, AppResources.LocationService_Title, MessageBoxButton.OK);
                    break;
                case PositionStatus.NotAvailable:
                    break;
                case PositionStatus.NotInitialized:
                    break;
                case PositionStatus.Ready:
                    break;
                default:
                    break;
            }
        }

        static async void watcher_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (App.RunningInBackground)
            {
                if (App.fbAppID != String.Empty)
                {
                    staticObjects.CurrentLatitude = args.Position.Coordinate.Latitude;
                    staticObjects.CurrentLongitude = args.Position.Coordinate.Longitude;
                    await GPS.GetAddress(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
                    Broadcast.TryUploadPosition(true);

                    Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
                    toast.Content = AppResources.BroadcastView_BroadcastSucceeded;
                    toast.Title = "It Fuckin works!";
                    toast.Show();
                }
            }    
        }


        public static async Task<bool> GetAddress(double lat, double lon)
        {   
            string GooglesReverseGeocodingUri = "http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&sensor=true";
            GoogleReverseGeocoding.RootObject reverseGeocodingResult = new GoogleReverseGeocoding.RootObject();
            HttpClient client = new HttpClient();
            try
            {
                string result = await client.GetStringAsync(String.Format(GooglesReverseGeocodingUri, lat, lon));
            reverseGeocodingResult = JsonConvert.DeserializeObject<GoogleReverseGeocoding.RootObject>(result);

            if (reverseGeocodingResult != null)
            {
                staticObjects.City = reverseGeocodingResult.results[0].address_components[1].long_name;
                staticObjects.Country = reverseGeocodingResult.results[4].address_components[0].long_name;
                staticObjects.Address = reverseGeocodingResult.results[1].address_components[0].long_name + ", " + reverseGeocodingResult.results[0].address_components[0].long_name;
                return true;
            }
            else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void GetDirections(GeoCoordinate end) 
        {
            BingMapsDirectionsTask task = new BingMapsDirectionsTask();
            task.Start = new LabeledMapLocation(AppResources.GPS_CurrentPosition,new GeoCoordinate(staticObjects.CurrentLatitude,staticObjects.CurrentLongitude));
            task.End = new LabeledMapLocation(AppResources.GPS_EndPosition, end);
            task.Show();
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
    public static class Broadcast
    {
        public static async void TryUploadPosition(bool intenseMode)
        {
            if (staticObjects.Address != null)
            {
                Position position = new Position();
                position.Address = staticObjects.Address;
                position.FacebookID = App.fbUserID;
                position.Latitude = staticObjects.CurrentLatitude;
                position.Longitude = staticObjects.CurrentLongitude;
                position.Country = staticObjects.Country;
                position.City = staticObjects.City;
                position.RegisteredAt = DateTime.UtcNow;
                position.PartitionKey = position.FacebookID;
                position.RowKey = string.Format("{0:d10}", DateTime.MaxValue.Ticks - position.RegisteredAt.Ticks);
                bool insertSucceeded = await Services.TryUploadPosition(position);

                string message = string.Empty;
                if (insertSucceeded)
                    message = AppResources.BroadcastView_BroadcastSucceeded;
                else message = AppResources.BroadcastView_BroadcastError;

                if (!intenseMode)
                    MessageBox.Show(message, AppResources.BroadcastView_Title, MessageBoxButton.OK);
            }
            else MessageBox.Show(AppResources.LocationService_NoService, AppResources.LocationService_Title,MessageBoxButton.OK);
            
        }
    }

    public class AlphaKeyGroup<T> : List<T>
    {
        /// <summary>
        /// The delegate that is used to get the key information.
        /// </summary>
        /// <param name="item">An object of type T</param>
        /// <returns>The key value to use for this object</returns>
        public delegate string GetKeyDelegate(T item);

        /// <summary>
        /// The Key of this group.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="key">The key for this group.</param>
        public AlphaKeyGroup(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The </param>
        /// <returns>Theitems source for a LongListSelector</returns>
        private static List<AlphaKeyGroup<T>> CreateGroups(SortedLocaleGrouping slg)
        {
            List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();

            foreach (string key in slg.GroupDisplayNames)
            {
                list.Add(new AlphaKeyGroup<T>(key));
            }

            return list;
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, GetKeyDelegate getKey, bool sort)
        {
            SortedLocaleGrouping slg = new SortedLocaleGrouping(ci);
            List<AlphaKeyGroup<T>> list = CreateGroups(slg);

            foreach (T item in items)
            {
                int index = 0;
                if (slg.SupportsPhonetics)
                {
                    //check if your database has yomi string for item
                    //if it does not, then do you want to generate Yomi or ask the user for this item.
                    //index = slg.GetGroupIndex(getKey(Yomiof(item)));
                }
                else
                {
                    index = slg.GetGroupIndex(getKey(item));
                }
                if (index >= 0 && index < list.Count)
                {
                    list[index].Add(item);
                }
            }

            if (sort)
            {
                foreach (AlphaKeyGroup<T> group in list)
                {
                    group.Sort((c0, c1) => { return ci.CompareInfo.Compare(getKey(c0), getKey(c1)); });
                }
            }

            return list;
        }
    }

    public static class BackAgent
    {
        public static void StartIntenseMode()
        {
            App.IntenseModeEnabled = true;
            StartPeriodicAgent();
            Broadcast.TryUploadPosition(true);

            LockScreen();
        }

        private static async void LockScreen()
        {
            List<Permission> permissions = await Queries.GetFavoritePermissionsForThisCode("PartitionKey", Utilities.fbUserID);
            if (permissions.Count > 0)
            {
                int ran = new Random().Next(0, permissions.Count - 1);
                string fbID = permissions.ElementAt(ran).FBIDFromViewed;
                List<Position> position = await Queries.GetPositionsForThisCode(fbID, 1);

                FacebookUser user = await Users.DownloadUserInfo(fbID);
                if (position.Count > 0)
                {
                    Position pos = position.First();
                    ShellTile.ActiveTiles.First().Update(
                new FlipTileData()
                {
                    WideBackContent = user.Name + " was last seen in " + pos.Address + " on " + pos.RegisteredAt.ToLongDateString() + " at " + pos.RegisteredAt.ToLongTimeString(),
                    Title = "New update from a friend"
                });
                }
                else
                {
                    ShellTile.ActiveTiles.First().Update(
                    new FlipTileData()
                    {
                        WideBackContent = user.Name + "has never updated his position.",
                        BackTitle = AppResources.ApplicationTitle,
                        BackContent = user.Name + "has never updated his position."
                    });
                }
            }
        }

        #region BackAgent

        private static void StartPeriodicAgent()
        {
            string periodicTaskName = "SmartGuardPeriodicAgent";
            PeriodicTask periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
                RemoveAgent(periodicTaskName);

            periodicTask = new PeriodicTask(periodicTaskName);
            periodicTask.Description = AppResources.SmartGuardBackgroundAgent;

            #region Try Catch Finally

            try
            {
                ScheduledActionService.Add(periodicTask);
            }

            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                { }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                { }
            }
            catch (SchedulerServiceException)
            { }
            #endregion Try Catch Finally
        }

        public static void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            { }
        }

        #endregion BackAgent
    }
}