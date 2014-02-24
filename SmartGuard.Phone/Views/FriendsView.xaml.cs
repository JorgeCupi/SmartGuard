using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Controls;
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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartGuard.Phone.Views
{
    public partial class FriendsView : MvxPhonePage
    {
        private MapLayer layers;
        private MapOverlay overlay;
        private BitmapImage image;
        private ImageBrush brush;
        private Ellipse ellipse;

        public FriendsView()
        {
            InitializeComponent();
            lstFriends.Tap += lstFriends_Tap;
            lstFriends.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            LoadAppBar();

            myMap.Center = new GeoCoordinate(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            myMap.ZoomLevel = 18;
        }

        void lstFriends_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (lstFriends.SelectedItem != null)
            {
                Friend selectedFriend = lstFriends.SelectedItem as Friend;
                GeoCoordinate coordinates = new GeoCoordinate(selectedFriend.LastPosition.Latitude, selectedFriend.LastPosition.Longitude);
                coordinates.Latitude -= 0.0002;
                coordinates.Longitude += 0.0002;

                myMap.Center = coordinates;
                myMap.ZoomLevel = 18;
            }
        }

        private void LoadAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnAdd = new ApplicationBarIconButton();
            btnAdd.IconUri = new Uri("/Assets/AppBar/Add.png", UriKind.Relative);
            btnAdd.Text = AppResources.AppBar_Add;
            btnAdd.Click += btnAdd_Click;
            ApplicationBar.Buttons.Add(btnAdd);

            ApplicationBarIconButton btnSeeMore = new ApplicationBarIconButton();
            btnSeeMore.IconUri = new Uri("/Assets/AppBar/AR.png", UriKind.Relative);
            btnSeeMore.Text = AppResources.AppBar_SeeMore;
            btnSeeMore.Click += btnSeeMore_Click;
            ApplicationBar.Buttons.Add(btnSeeMore);

            ApplicationBarIconButton btnGetDirections = new ApplicationBarIconButton();
            btnGetDirections.IconUri = new Uri("/Assets/AppBar/Directions.png", UriKind.Relative);
            btnGetDirections.Text = AppResources.AppBar_Directions;
            btnGetDirections.Click += btnGetDirections_Click;
            ApplicationBar.Buttons.Add(btnGetDirections);

            ApplicationBarIconButton btnShare = new ApplicationBarIconButton();
            btnShare.IconUri = new Uri("/Assets/AppBar/Share.png", UriKind.Relative);
            btnShare.Text = AppResources.AppBar_Share;
            btnShare.Click += btnShare_Click;
            ApplicationBar.Buttons.Add(btnShare);
        }

        void btnShare_Click(object sender, EventArgs e)
        {
            if (lstFriends.SelectedItem != null)
            {
                Friend selectedFriend = lstFriends.SelectedItem as Friend;
                if (selectedFriend != null)
                {
                    ShareContent.Title = String.Format("[{0}]", AppResources.ApplicationTitle);
                    ShareContent.Message = String.Format(
                        AppResources.Share_ShareFriendPosition,
                        selectedFriend.Name,
                        selectedFriend.LastPosition.Address,
                        selectedFriend.LastPosition.RegisteredAt.ToShortDateString(),
                        selectedFriend.LastPosition.RegisteredAt.TimeOfDay);
                    ShareContent.Link = new Uri("http://bing.com/maps/default.aspx" +
                        "?cp=" + selectedFriend.LastPosition.Latitude + "~" + selectedFriend.LastPosition.Longitude +
                        "&lvl=18" +
                        "&style=r", UriKind.Absolute);

                    NavigationService.Navigate(new Uri("/Views/Share.xaml", UriKind.Relative));
                }
            }
            else MessageBox.Show(AppResources.FriendsView_SelectedItemError, AppResources.ShareView_Title, MessageBoxButton.OK);
        }

        void btnGetDirections_Click(object sender, EventArgs e)
        {
            if (lstFriends.SelectedItem != null)
            {
                Friend selectedFriend = lstFriends.SelectedItem as Friend;
                GPS.GetDirections(new GeoCoordinate(selectedFriend.LastPosition.Latitude,selectedFriend.LastPosition.Longitude));
            }
            else MessageBox.Show(AppResources.FriendsView_SelectedItemError, AppResources.GPS_GetDirections, MessageBoxButton.OK);
        }

        void btnSeeMore_Click(object sender, EventArgs e)
        {
            if (lstFriends.SelectedItem != null)
            {
                (this.DataContext as FriendsViewModel).SelectedFriend = lstFriends.SelectedItem as Friend;
                (this.DataContext as FriendsViewModel).TapOnFriend.Execute(null);
            }
            else MessageBox.Show(AppResources.FriendsView_SelectedItemError, AppResources.FriendsView_SeeMore,MessageBoxButton.OK);
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            FriendsViewModel context = (this.DataContext as FriendsViewModel);
            if (context.AddNewFriend.CanExecute(null))
                context.AddNewFriend.Execute(null);
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Friend addedFriend = (lstFriends.Items.Last() as Friend);
                layers = new MapLayer();
                overlay = new MapOverlay();

                image = new BitmapImage();
                image.UriSource = (new Uri(addedFriend.Picture, UriKind.Absolute));

                brush = new ImageBrush();
                brush.ImageSource = image;
                ellipse = new Ellipse();
                ellipse.DataContext = addedFriend;
                ellipse.Hold += ellipse_Hold;
                ellipse.Height = 100;
                ellipse.Width = 100;
                ellipse.Fill = brush;

                overlay.GeoCoordinate = new GeoCoordinate(addedFriend.LastPosition.Latitude, addedFriend.LastPosition.Longitude);
                overlay.Content = ellipse;
                layers.Add(overlay);
                myMap.Layers.Add(layers);
            }
        }

        void ellipse_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Friend selectedFriend= (sender as Ellipse).DataContext as Friend;
            ContextMenu menu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = AppResources.FriendsView_SeeMore;
            item.DataContext = selectedFriend;
            item.Tap += SeeMore;
            menu.Items.Add(item);

            MenuItem itemTwo = new MenuItem();
            itemTwo.Header = AppResources.GPS_GetDirections;
            itemTwo.Tap += GetDirections;
            itemTwo.DataContext = selectedFriend;
            menu.Items.Add(itemTwo);

            ContextMenuService.SetContextMenu(sender as Ellipse, menu);
        }

        private void GetDirections(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Friend selected = (sender as MenuItem).DataContext as Friend;
            GPS.GetDirections(new GeoCoordinate(selected.LastPosition.Latitude, selected.LastPosition.Longitude));
        }

        void SeeMore(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (this.DataContext as FriendsViewModel).SelectedFriend = (sender as MenuItem).DataContext as Friend;
            (this.DataContext as FriendsViewModel).TapOnFriend.Execute(null);
        }
    }

    public class GPSCoordinate
    {
        public Friend Friend { get; set; }

        public Position Position { get; set; }
    }
}