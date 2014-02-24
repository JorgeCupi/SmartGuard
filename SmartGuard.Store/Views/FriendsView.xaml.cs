using Bing.Maps;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace SmartGuard.Store.Views
{
    public sealed partial class FriendsView : SmartGuard.Store.Common.LayoutAwarePage
    {
        private MapLayer layers;
        private BitmapImage image;
        private ImageBrush brush;
        private Ellipse ellipse;

        public FriendsView()
        {
            this.InitializeComponent();
            myMap.ZoomLevel = 14;
            myMap.Center = new Location(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            lstFriends.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            lstFriends.Tapped += lstFriends_Tapped;
            Share.InitializeSharing(DataTransferManager.GetForCurrentView(), AppResources.FriendsView_SharingError);
        }

        private void lstFriends_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (lstFriends.SelectedItem != null)
            {
                Friend selectedFriend = lstFriends.SelectedItem as Friend;
                myMap.Center = new Location(selectedFriend.LastPosition.Latitude, selectedFriend.LastPosition.Longitude);
                myMap.ZoomLevel = 15;

                SetSharingContent(selectedFriend);
            }
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == 1)
            {
                Friend addedFriend = (lstFriends.Items.Last() as Friend);
                layers = new MapLayer();
                image = new BitmapImage();
                image.UriSource = (new Uri(addedFriend.Picture, UriKind.Absolute));

                brush = new ImageBrush();
                brush.ImageSource = image;
                ellipse = new Ellipse();
                ellipse.DataContext = addedFriend;
                ellipse.RightTapped += ellipse_RightTapped;
                ellipse.Height = 100;
                ellipse.Width = 100;
                ellipse.Fill = brush;

                layers.Children.Add(ellipse);
                myMap.Children.Add(layers);
                MapLayer.SetPosition(ellipse, new Location(addedFriend.LastPosition.Latitude, addedFriend.LastPosition.Longitude));
            }
        }

        private async void ellipse_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Friend selectedFriend = (sender as Ellipse).DataContext as Friend;
            (DataContext as FriendsViewModel).SelectedFriend = selectedFriend;
            PopupMenu menu = new PopupMenu();

            menu.Commands.Add(new UICommand(AppResources.General_SeeMore, (command) =>
            {
                (DataContext as FriendsViewModel).TapOnFriend.Execute(null);
            }));
            menu.Commands.Add(new UICommand(AppResources.General_GetDirections, (command) =>
            {
                staticObjects.GoalLongitude = selectedFriend.LastPosition.Longitude;
                staticObjects.GoalLatitude = selectedFriend.LastPosition.Latitude;
                staticObjects.FriendProfilePicture = new Uri(selectedFriend.Picture, UriKind.Absolute);
                (DataContext as FriendsViewModel).GetDirections.Execute(null);
            }));

            menu.Commands.Add(new UICommand(AppResources.General_ShareFacebook, (command) =>
            {
                Share.ShareToFacebook("http://bing.com/maps/default.aspx" +
                "?cp=" + selectedFriend.LastPosition.Latitude + "~" + selectedFriend.LastPosition.Longitude +
                "&lvl=18" +
                "&style=r",
                String.Format(AppResources.General_LastPositionMessageFriend, 
                    selectedFriend.Name, 
                    selectedFriend.LastPosition.Address, 
                    selectedFriend.LastPosition.RegisteredAt.ToString()+ " GMT", 
                    "http://bing.com/maps/default.aspx"),selectedFriend.Picture);
            }));
            menu.Commands.Add(new UICommand(AppResources.General_ShareEmail, (command) =>
            {
                SetSharingContent(selectedFriend);
                Share.ShowShareBar();
            }));
            await menu.ShowForSelectionAsync(Screen.GetElementRect((FrameworkElement)sender));
        }

        private static void SetSharingContent(Friend selectedFriend)
        {
            Share.Title = "SmartGuard - " + selectedFriend.Name;
            Share.Description = "Comparte la posicion de tus amigos";
            DateTime date = selectedFriend.LastPosition.RegisteredAt;
            Share.Message = String.Format(AppResources.FacebookMessageFriendPost, selectedFriend.Name, selectedFriend.LastPosition.Address, date.Date.ToString(), date.TimeOfDay.ToString(), "http://bing.com/maps/default.aspx" +
                "?cp=" + selectedFriend.LastPosition.Latitude + "~" + selectedFriend.LastPosition.Longitude +
                "&lvl=18" +
                "&style=r");

        }
    }
}