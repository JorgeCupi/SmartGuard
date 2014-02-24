using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using SmartGuard.Core.Models;
using System.Device.Location;
using SmartGuard.Core.ViewModels;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Services;
using SmartGuard.Phone.Resources;
namespace SmartGuard.Phone.Views
{
    public partial class RedZonesView : MvxPhonePage
    {
        MapLayer layers;
        MapOverlay overlay;
        BitmapImage image;
        Image img;
        Ellipse ellipse;
        ImageBrush brush;
        public RedZonesView()
        {
            InitializeComponent();
            this.Loaded += RedZonesView_Loaded;
            lstRedZones.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            lstRedZones.Tap += lstRedZones_Tap;
            img = new Image();
            img.Height = 100;
            img.Width = 100;
            image = new BitmapImage();
        }

        void RedZonesView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAppBar();
            myMap.ZoomLevel = 13;
            myMap.Center = new GeoCoordinate(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            LoadPosition();

            if (staticObjects.Address == null)
            {
                txtMessage.Visibility = Visibility.Visible;
                txtMessage.Text = AppResources.LocationService_NoService;
            }

        }

        void lstRedZones_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (lstRedZones.SelectedItem != null)
            {
                RedZone selectedRedZone = lstRedZones.SelectedItem as RedZone;
                GeoCoordinate coordinates = new GeoCoordinate(selectedRedZone.Latitude, selectedRedZone.Longitude);
                coordinates.Latitude -= 0.0002;
                coordinates.Longitude += 0.0002;

                myMap.Center = coordinates;
                myMap.ZoomLevel = 18;
            }
        }


        private void LoadPosition()
        {
            myMap.Center = new GeoCoordinate(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            myMap.ZoomLevel = 12;

            string stateCountry;
            stateCountry =
                       (staticObjects.City + staticObjects.Country).Replace(" ", "");
            (this.DataContext as RedZonesViewModel).DownloadRedZones(stateCountry);
        }

        private void LoadAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnAdd = new ApplicationBarIconButton();
            btnAdd.IconUri = new Uri("/Assets/AppBar/Add.png", UriKind.Relative);
            btnAdd.Text = AppResources.AppBar_Add;
            btnAdd.Click += btnAdd_Click;
            ApplicationBar.Buttons.Add(btnAdd);

            ApplicationBarIconButton btnShare = new ApplicationBarIconButton();
            btnShare.IconUri = new Uri("/Assets/AppBar/Share.png", UriKind.Relative);
            btnShare.Text = AppResources.AppBar_Share;
            btnShare.Click += btnShare_Click;
            ApplicationBar.Buttons.Add(btnShare);

            ApplicationBarIconButton btnGetDirections = new ApplicationBarIconButton();
            btnGetDirections.IconUri = new Uri("/Assets/AppBar/Directions.png", UriKind.Relative);
            btnGetDirections.Text = AppResources.AppBar_Directions;
            btnGetDirections.Click += btnGetDirections_Click;
            ApplicationBar.Buttons.Add(btnGetDirections);
        }

        void btnGetDirections_Click(object sender, EventArgs e)
        {
            if (lstRedZones.SelectedItem != null)
            {
                RedZone eli = lstRedZones.SelectedItem as RedZone;
                GPS.GetDirections(new GeoCoordinate(eli.Latitude, eli.Longitude));
            }
            else MessageBox.Show(AppResources.RedZone_SelectedItemError, AppResources.GPS_GetDirections , MessageBoxButton.OK);
        }   

        void btnShare_Click(object sender, EventArgs e)
        {
            if (lstRedZones.SelectedItem != null)
                ShareRedZone(lstRedZones.SelectedItem as RedZone);
            else MessageBox.Show(AppResources.RedZone_SelectedItemError, AppResources.ShareView_Title, MessageBoxButton.OK);
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            (this.DataContext as RedZonesViewModel).UploadRedZone.Execute(null);
        }

        void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
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
                ellipse.Hold+=ellipse_Hold;
                
                overlay = new MapOverlay();
                overlay.GeoCoordinate = new GeoCoordinate(newRedZone.Latitude, newRedZone.Longitude);
                overlay.Content = ellipse;
                layers.Add(overlay);

                myMap.Layers.Add(layers);
            }
        }

        void ellipse_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RedZone zone = (sender as Ellipse).DataContext as RedZone;
            ContextMenu menu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = AppResources.ShareView_Title;
            item.DataContext = zone;
            item.Tap += ShareRedZone;
            menu.Items.Add(item);

            MenuItem itemTwo = new MenuItem();
            itemTwo.Header = AppResources.GPS_GetDirections;
            itemTwo.Tap += GetDirections;
            itemTwo.DataContext = zone;
            menu.Items.Add(itemTwo);

            ContextMenuService.SetContextMenu(sender as Ellipse, menu);
        }

        void ShareRedZone(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RedZone redZone = (sender as MenuItem).DataContext as RedZone;
            ShareRedZone(redZone);
        }

        private void ShareRedZone(RedZone redZone)
        {
            if (redZone != null)
            {
                ShareContent.Title = String.Format("[{0}] - {1}", AppResources.ApplicationTitle, AppResources.General_RedZoneWarning);
                ShareContent.Message = String.Format(
                    AppResources.General_RedZoneWarning_Message,
                    redZone.FbUser.Name,
                    AppResources.Says,
                    redZone.Description);
                ShareContent.Link = new Uri("http://bing.com/maps/default.aspx" +
                    "?cp=" + redZone.Latitude + "~" + redZone.Longitude +
                    "&lvl=18" +
                    "&style=r", UriKind.Absolute);

                NavigationService.Navigate(new Uri("/Views/Share.xaml", UriKind.Relative));
            }
        }

        void GetDirections(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RedZone eli = (sender as MenuItem).DataContext as RedZone;
            GPS.GetDirections(new GeoCoordinate(eli.Latitude, eli.Longitude));
        }
    }
}