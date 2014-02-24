using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Input;
using SmartGuard.Phone.Resources;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
namespace SmartGuard.Phone.Views
{
    public partial class UploadRedZoneView : MvxPhonePage
    {
        MapLayer layer;
        Pushpin pushpin;
        MapOverlay overlay;
        Point point;
        GeoCoordinate coordinate;
        public UploadRedZoneView()
        {
            InitializeComponent();
            myMap.Tap += myMap_Tap;
            myMap.Layers.Add(new MapLayer());

            LoadAppBar();
            LoadPosition();
            coordinate = new GeoCoordinate();
        }

        private void LoadPosition()
        {
            myMap.ZoomLevel = 18;
            myMap.Center = new GeoCoordinate(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
        }

        private void LoadAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnAdd = new ApplicationBarIconButton();
            btnAdd.IconUri = new Uri("/Assets/AppBar/Add.png", UriKind.Relative);
            btnAdd.Text = AppResources.AppBar_Add;
            btnAdd.Click += btnAdd_Click;
            ApplicationBar.Buttons.Add(btnAdd);
        }

        async void btnAdd_Click(object sender, EventArgs e)
        {
            RedZone redZone = new RedZone();
            redZone.City = staticObjects.City;
            redZone.Country = staticObjects.Country;
            redZone.Description = txtDescription.Text;
            redZone.FacebookID = App.fbUserID;
            redZone.Latitude = coordinate.Latitude;
            redZone.Longitude = coordinate.Longitude;
            redZone.Radius = int.Parse(txtRadius.Text);
            redZone.PartitionKey = (redZone.City + redZone.Country).Replace(" ","");
            redZone.RegisteredAt = DateTime.UtcNow;
            redZone.RowKey = redZone.Latitude.ToString() + redZone.Longitude.ToString();


            bool result = await (this.DataContext as UploadRedZoneViewModel).TryUploadNewRedZone(redZone);
        }

        void myMap_Tap(object sender, GestureEventArgs e)
        {
            myMap.Layers.RemoveAt(0);

            point = e.GetPosition(myMap);
            point.X -= 25;
            point.Y -= 45;

            pushpin = new Pushpin();
            pushpin.Background = this.Resources["PhoneAccentBrush"] as SolidColorBrush;

            overlay = new MapOverlay();
            overlay.Content = pushpin;
            coordinate = myMap.ConvertViewportPointToGeoCoordinate(point);
            overlay.GeoCoordinate = coordinate;
            layer = new MapLayer();
            layer.Add(overlay);
            myMap.Layers.Add(layer);
        }
    }
}