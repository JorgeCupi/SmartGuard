using Bing.Maps;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace SmartGuard.Store.Views
{
    public sealed partial class RedZonesView : SmartGuard.Store.Common.LayoutAwarePage
    {
        MapLayer layers;
        BitmapImage image;
        Image img;
        Ellipse ellipse;
        ImageBrush brush;
        public RedZonesView()
        {
            InitializeComponent();
            this.Loaded += RedZonesView_Loaded;
            lstRedZones.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            lstRedZones.Tapped+=lstRedZones_Tapped;
            btnUpload.Click += btnUpload_Click;
            img = new Image();
            img.Height = 100;
            img.Width = 100;
            image = new BitmapImage();
        }

        void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as RedZonesViewModel).UploadRedZone.Execute(null);
        }

        void lstRedZones_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (lstRedZones.SelectedItem != null)
            {
                RedZone zone = lstRedZones.SelectedItem as RedZone;
                myMap.ZoomLevel = 15;
                myMap.Center = new Location(zone.Latitude, zone.Longitude);
                SetSharingContent(zone, zone.FbUser.Name);
            }
        }


        void RedZonesView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPosition();

            if (staticObjects.Address == null)
            {
                txtMessage.Visibility = Visibility.Visible;
                txtMessage.Text = AppResources.LocationService_NoService;
            }

        }

        private void LoadPosition()
        {
            myMap.Center = new Location(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            myMap.ZoomLevel = 18;

            string stateCountry;
            stateCountry =
                       (staticObjects.City + staticObjects.Country).Replace(" ", "");
            (this.DataContext as RedZonesViewModel).DownloadRedZones(stateCountry);
        }

        void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == 1)
            {
                RedZone newRedZone = lstRedZones.Items.Last() as RedZone;
                layers = new MapLayer();

                image = new BitmapImage();
                image.UriSource = (new Uri(newRedZone.FbUser.Picture.data.url, UriKind.Absolute));

                brush = new ImageBrush();
                brush.ImageSource = image;
                ellipse = new Ellipse();
                ellipse.DataContext = newRedZone;
                ellipse.Height = newRedZone.Radius / 5;
                ellipse.Width = newRedZone.Radius / 5;
                ellipse.Fill = brush;
                ellipse.RightTapped += ellipse_RightTapped;

                layers.Children.Add(ellipse);
                MapLayer.SetPosition(ellipse, new Location(newRedZone.Latitude, newRedZone.Longitude));

                myMap.Children.Add(layers);
            }
        }

        async void ellipse_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            RedZone zone = (sender as Ellipse).DataContext as RedZone;
            PopupMenu menu = new PopupMenu();

            menu.Commands.Add(new UICommand(AppResources.General_GetDirections, (command) =>
            {
                staticObjects.GoalLongitude = zone.Longitude;
                staticObjects.GoalLatitude = zone.Latitude;
                staticObjects.FriendProfilePicture = new Uri(zone.FbUser.Picture.data.url, UriKind.Absolute);
                (DataContext as RedZonesViewModel).GetDirections.Execute(null);
            }));

            menu.Commands.Add(new UICommand(AppResources.General_ShareFacebook, (command) =>
            {
                Share.ShareToFacebook("http://bing.com/maps/default.aspx" +
                "?cp=" + zone.Latitude + "~" + zone.Longitude +
                "&lvl=18" +
                "&style=r",
                "Sobre la zona " + zone.Address +"."+  
                String.Format(String.Format(
                    AppResources.General_RedZoneWarning_Message,
                    zone.Address,
                    zone.FbUser.Name,
                    zone.Description)),
                zone.FbUser.Picture.data.url);
            }));
            menu.Commands.Add(new UICommand(AppResources.General_ShareEmail, (command) =>
            {
                SetSharingContent(zone, zone.FbUser.Name);
                Share.ShowShareBar();
            }));
            await menu.ShowForSelectionAsync(Screen.GetElementRect((FrameworkElement)sender));
        }

        private void SetSharingContent(RedZone zone, string fbUserName)
        {
            Share.Title = "SmartGuard - Zona peligrosa";
            Share.Description = "Comparte la posicion de tus amigos";
            DateTime date = zone.RegisteredAt;
            Share.Message = String.Format(AppResources.General_RedZoneWarning_Message,zone.Address, fbUserName, zone.Description) + "\n\n Para ver la zona en un mapa, haz click en el siguiente link:\n http://bing.com/maps/default.aspx" +
                "?cp=" + zone.Latitude + "~" + zone.Longitude +
                "&lvl=18" +
                "&style=r";
        }
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
