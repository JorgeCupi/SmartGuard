using Bing.Maps;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SmartGuard.Store.Views
{
    public sealed partial class UploadRedZoneView : SmartGuard.Store.Common.LayoutAwarePage
    {
        MapLayer layer;
        Pushpin pushpin;
        Point point;
        Location coordinate;
        public UploadRedZoneView()
        {
            InitializeComponent();
            btnAdd.Click += btnAdd_Click;
            coordinate = new Location(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            LoadPosition();
        }

        async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            RedZone redZone = new RedZone();
            redZone.City = staticObjects.City;
            redZone.Address = staticObjects.Address;
            redZone.Country = staticObjects.Country;
            redZone.Description = txtDescription.Text;
            redZone.FacebookID = App.fbUserID;
            redZone.Latitude = coordinate.Latitude;
            redZone.Longitude = coordinate.Longitude;
            redZone.Radius = int.Parse(txtRadius.Text);
            redZone.PartitionKey = (redZone.City + redZone.Country).Replace(" ", "");
            redZone.RegisteredAt = DateTime.UtcNow;
            redZone.RowKey = redZone.Latitude.ToString() + redZone.Longitude.ToString();


            bool result = await (this.DataContext as UploadRedZoneViewModel).TryUploadNewRedZone(redZone);
        }

        private void LoadPosition()
        {
            myMap.ZoomLevel = 18;
            myMap.Center = coordinate;

            pushpin = new Pushpin();
            pushpin.Background = new SolidColorBrush(Colors.Red);

            layer = new MapLayer();
            layer.Children.Add(pushpin);
            MapLayer.SetPosition(pushpin, coordinate);
            myMap.Children.Add(layer);
        }
       
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
