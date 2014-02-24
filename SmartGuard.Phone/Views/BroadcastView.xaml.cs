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
using SmartGuard.Phone.Resources;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Toolkit;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;

namespace SmartGuard.Phone.Views
{
    public partial class BroadcastView : MvxPhonePage
    {
        BitmapImage image;
        Grid grid;
        TextBlock textBlock;
        ImageBrush brush;
        Ellipse ellipse;
        MapLayer layer;
        MapOverlay overlay;
        GeoCoordinate location;
        Position selectedPosition;
        public BroadcastView()
        {
            InitializeComponent();

            LoadPosition();
            LoadAppBar();
            lstPositions.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            lstPositions.Tap += lstPositions_Tap;
            mapPositions.ZoomLevel = 18;
            image = new BitmapImage();
            image.UriSource = (new Uri(App.fbProfilePicture, UriKind.Absolute));
            selectedPosition = new Position();
            
        }

        void lstPositions_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (lstPositions.SelectedItem != null)
            {
                selectedPosition = lstPositions.SelectedItem as Position;

                GeoCoordinate coordinates = new GeoCoordinate(selectedPosition.Latitude, selectedPosition.Longitude);
                coordinates.Latitude -= 0.0002;
                coordinates.Longitude += 0.0002;

                mapPositions.Center = coordinates;
                mapPositions.ZoomLevel = 18;
            }
        }

        void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Position item = lstPositions.Items.Last() as Position;
                location = new GeoCoordinate(item.Latitude, item.Longitude);
                mapPositions.Center = location;
                layer = new MapLayer();
                grid = new Grid();
                grid.DataContext = item;
                grid.Hold += grid_Hold;
                textBlock = new TextBlock();
                textBlock.Text = item.Counter.ToString();
                textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                brush = new ImageBrush();
                brush.ImageSource = image;
                ellipse = new Ellipse();
                ellipse.Height = 100;
                ellipse.Width = 100;
                ellipse.Fill = brush;
                grid.Children.Add(ellipse);
                grid.Children.Add(textBlock);
                overlay = new MapOverlay();
                overlay.Content = grid;
                overlay.GeoCoordinate = location;
                layer.Add(overlay);
                mapPositions.Layers.Add(layer);
            }
        }

        void grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
        }

        private void LoadPosition()
        {
            GeoCoordinate center = new GeoCoordinate(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            myMap.Center = center;
            myMap.ZoomLevel = 15;

            Pushpin pushpin = new Pushpin();
            pushpin.Background = this.Resources["PhoneAccentBrush"] as SolidColorBrush;

            MapOverlay overlay = new MapOverlay();
            overlay.GeoCoordinate = center;
            overlay.Content =pushpin;
            
            MapLayer layer = new MapLayer();
            layer.Add(overlay);

            myMap.Layers.Add(layer);
        }

        #region AppBar
        private void LoadAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnBroadcast = new ApplicationBarIconButton();
            btnBroadcast.IconUri = new Uri("/Assets/AppBar/Broadcast.png",UriKind.Relative);
            btnBroadcast.Text = AppResources.BroadcastView_Title;
            btnBroadcast.Click += btnBroadcast_Click;
            ApplicationBar.Buttons.Add(btnBroadcast);

            ApplicationBarMenuItem menuIntense = new ApplicationBarMenuItem();
            menuIntense.Text = AppResources.AppBar_Intense;
            menuIntense.Click+=menuIntense_Click;
            ApplicationBar.MenuItems.Add(menuIntense);

            ApplicationBarIconButton btnShare = new ApplicationBarIconButton();
            btnShare.IconUri = new Uri("/Assets/AppBar/Share.png", UriKind.Relative);
            btnShare.Text = AppResources.AppBar_Share;
            btnShare.Click += btnShare_Click;
            ApplicationBar.Buttons.Add(btnShare);

            ApplicationBarMenuItem menuToday = new ApplicationBarMenuItem();
            menuToday.Text = AppResources.AppBar_Today;
            menuToday.Click += menuToday_Click;
            ApplicationBar.MenuItems.Add(menuToday);

            ApplicationBarMenuItem menuWeek = new ApplicationBarMenuItem();
            menuWeek.Text = AppResources.AppBar_LastWeek;
            menuWeek.Click += menuWeek_Click;
            ApplicationBar.MenuItems.Add(menuWeek);

            ApplicationBarMenuItem menuMonth = new ApplicationBarMenuItem();
            menuMonth.Text = AppResources.AppBar_LastMonth;
            menuMonth.Click += menuMonth_Click;
            ApplicationBar.MenuItems.Add(menuMonth);
        }

        void menuToday_Click(object sender, EventArgs e)
        {
            (this.DataContext as BroadcastViewModel).TryDownloadMyPositions(35);
        }

        void menuWeek_Click(object sender, EventArgs e)
        {
            (this.DataContext as BroadcastViewModel).TryDownloadMyPositions(250);
        }

        void menuMonth_Click(object sender, EventArgs e)
        {
            (this.DataContext as BroadcastViewModel).TryDownloadMyPositions(1000);
        }

        void btnShare_Click(object sender, EventArgs e)
        {
            if(staticObjects.Address !=null)
            {
                ShareContent.Title = String.Format("[{0}] - {1}",AppResources.ApplicationTitle,AppResources.General_LastPositionTitle);
                ShareContent.Message = String.Format(
                    AppResources.General_LastPositionMessage,
                    DateTime.Now.ToLongDateString(), 
                    DateTime.Now.ToLongTimeString());
                ShareContent.Link = new Uri("http://bing.com/maps/default.aspx" +
                    "?cp=" + staticObjects.CurrentLatitude + "~" + staticObjects.CurrentLongitude +
                    "&lvl=18" +
                    "&style=r", UriKind.Absolute);

                NavigationService.Navigate(new Uri("/Views/Share.xaml",UriKind.Relative));
            }
            else MessageBox.Show(AppResources.LocationService_NoService, AppResources.LocationService_Title,MessageBoxButton.OK);
        }

        void btnBroadcast_Click(object sender, EventArgs e)
        {
            Broadcast.TryUploadPosition(false);
        }

        void menuIntense_Click(object sender, EventArgs e)
        {
            if (staticObjects.Address != null)
            {
                BackAgent.StartIntenseMode();
                MessageBox.Show(AppResources.IntenseModeON, AppResources.ApplicationTitle, MessageBoxButton.OK);
            }
            else MessageBox.Show(AppResources.LocationService_NoService, AppResources.LocationService_Title,MessageBoxButton.OK);
        }
        #endregion
    }
}