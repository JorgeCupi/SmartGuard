using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using SmartGuard.Phone.Resources;
using System;
using System.Collections.Specialized;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartGuard.Phone.Views.Friends
{
    public partial class FriendInfoView : MvxPhonePage
    {
        private MapLayer layers;
        private MapOverlay overlay;
        private BitmapImage image;
        private ImageBrush brush;
        private Ellipse ellipse;
        private Friend Friend;
        private Grid grid;
        private TextBlock textBlock;

        public FriendInfoView()
        {
            InitializeComponent();
            this.Loaded += FriendInfoView_Loaded;
            lstPositions.Tap += lstPositions_Tap;
            LoadAppBar();
        }

        private void FriendInfoView_Loaded(object sender, RoutedEventArgs e)
        {
            Friend = ((FriendInfoViewModel)this.DataContext).Friend;
            ((FriendInfoViewModel)this.DataContext).DownloadLastPositions(35);
            image = new BitmapImage();
            image.UriSource = (new Uri(Friend.Picture, UriKind.Absolute));

            lstPositions.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;

            myMap.ZoomLevel = 18;
            myMap.Center = new GeoCoordinate(Friend.LastPosition.Latitude, Friend.LastPosition.Longitude);
        }

        private void lstPositions_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (lstPositions.SelectedItem != null)
            {
                Position selectedPosition = lstPositions.SelectedItem as Position;

                GeoCoordinate coordinates = new GeoCoordinate(selectedPosition.Latitude, selectedPosition.Longitude);
                coordinates.Latitude -= 0.0002;
                coordinates.Longitude += 0.0002;

                myMap.Center = coordinates;
                myMap.ZoomLevel = 18;
            }
        }

        #region AppBar

        private void LoadAppBar()
        {
            ApplicationBar = new ApplicationBar();
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
            if (lstPositions.SelectedItem != null)
            {
                Position selectedPosition= lstPositions.SelectedItem as Position;
                GPS.GetDirections(new GeoCoordinate(selectedPosition.Latitude, selectedPosition.Longitude));
            }
            else MessageBox.Show(AppResources.PositionSelectedError, AppResources.GPS_GetDirections, MessageBoxButton.OK);
        }
        void btnShare_Click(object sender, EventArgs e)
        {
            if (lstPositions.SelectedItem != null)
            {
                Position selectedPosition = lstPositions.SelectedItem as Position;
                if (selectedPosition != null)
                {
                    ShareContent.Title = String.Format("[{0}]", AppResources.ApplicationTitle);
                    ShareContent.Message = String.Format(
                        AppResources.Share_ShareFriendPosition,
                        Friend.Name,
                        selectedPosition.Address,
                        selectedPosition.RegisteredAt.ToShortDateString(),
                        selectedPosition.RegisteredAt.TimeOfDay);
                    ShareContent.Link = new Uri("http://bing.com/maps/default.aspx" +
                        "?cp=" + selectedPosition.Latitude + "~" + selectedPosition.Longitude +
                        "&lvl=18" +
                        "&style=r", UriKind.Absolute);

                    NavigationService.Navigate(new Uri("/Views/Share.xaml", UriKind.Relative));
                }
            }
            else MessageBox.Show(AppResources.PositionSelectedError, AppResources.ShareView_Title, MessageBoxButton.OK);
        }

        private void menuMonth_Click(object sender, EventArgs e)
        {
            (this.DataContext as FriendInfoViewModel).DownloadLastPositions(1000);
            myMap.Layers.Clear();
        }

        private void menuWeek_Click(object sender, EventArgs e)
        {
            (this.DataContext as FriendInfoViewModel).DownloadLastPositions(250);
            myMap.Layers.Clear();
        }

        private void menuToday_Click(object sender, EventArgs e)
        {
            (this.DataContext as FriendInfoViewModel).DownloadLastPositions(35);
            myMap.Layers.Clear();
        }

        #endregion AppBar

        private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Position item = lstPositions.Items.Last() as Position;
                layers = new MapLayer();
                image = new BitmapImage();
                image.UriSource = (new Uri(Friend.Picture, UriKind.Absolute));

                grid = new Grid();
                grid.DataContext = item;
                grid.Tap += grid_Tap;
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
                overlay.GeoCoordinate = new GeoCoordinate(item.Latitude, item.Longitude);
                overlay.Content = grid;
                layers.Add(overlay);
                myMap.Layers.Add(layers);
            }
        }

        private void grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }
    }
}