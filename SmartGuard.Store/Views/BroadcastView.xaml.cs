using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Shapes;
using Bing.Maps;
using Windows.UI;
namespace SmartGuard.Store.Views
{
    public sealed partial class BroadcastView : SmartGuard.Store.Common.LayoutAwarePage
    {
        public BroadcastView()
        {
            this.InitializeComponent();
            LoadPosition();
            btnBroadcast.Click += btnBroadcast_Click;
        }

        void btnBroadcast_Click(object sender, RoutedEventArgs e)
        {
            Broadcast.TryUploadPosition(false);
        }

        private void LoadPosition()
        {
            Location center = new Location(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            myMap.Center = center;
            myMap.ZoomLevel = 17;

            Ellipse pushpin= new Ellipse();
            pushpin.Height = 50;
            pushpin.Width = 50;
            pushpin.Fill = new SolidColorBrush(Colors.Blue);

            MapLayer layer = new MapLayer();

            layer.Children.Add(pushpin);
            MapLayer.SetPosition(pushpin, center);
            myMap.Children.Add(layer);
        }

        async void btnShare_Click(object sender, EventArgs e)
        {
            if (staticObjects.Address != null)
            {
                Share.Title = String.Format("[{0}] - {1}", AppResources.ApplicationTitle, AppResources.General_LastPositionTitle);
                Share.Message = String.Format(
                    AppResources.General_LastPositionMessage,
                    DateTime.Now.Date.ToFileTimeUtc(),
                    DateTime.Now.TimeOfDay.ToString());
                    Share.Link = "http://bing.com/maps/default.aspx" +
                    "?cp=" + staticObjects.CurrentLatitude + "~" + staticObjects.CurrentLongitude +
                    "&lvl=18" +
                    "&style=r";

            }
            else await MessageBox.ShowAsync(AppResources.LocationService_NoService, AppResources.LocationService_Title);
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
