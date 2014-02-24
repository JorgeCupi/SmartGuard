using Bing.Maps;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.Devices.Geolocation;
namespace SmartGuard.Store.Views
{
    public sealed partial class FriendInfoView : SmartGuard.Store.Common.LayoutAwarePage
    {
        private MapLayer layers;
        private BitmapImage image;
        private ImageBrush brush;
        private Ellipse ellipse;
        private Friend SelectedFriend;
        private Grid grid;
        private TextBlock textBlock;

        public FriendInfoView()
        {
            this.InitializeComponent();
            this.Loaded += FriendInfoView_Loaded;
            lstPositions.Tapped += lstPositions_Tapped;
            Share.InitializeSharing(DataTransferManager.GetForCurrentView(), AppResources.FriendsInfoView_SharingError);
        }

        void lstPositions_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (lstPositions.SelectedItem != null)
            {
                Position selectedPosition = lstPositions.SelectedItem as Position;
                myMap.Center = new Location(selectedPosition.Latitude, selectedPosition.Longitude);
                myMap.ZoomLevel = 17;

                SetSharingContent(selectedPosition, SelectedFriend.Name);
            }
        }

        private void FriendInfoView_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedFriend = ((FriendInfoViewModel)this.DataContext).Friend;
            ((FriendInfoViewModel)this.DataContext).DownloadLastPositions(50);
            image = new BitmapImage();
            image.UriSource = (new Uri(SelectedFriend.Picture, UriKind.Absolute));
            pageTitle.Text = "Ultimas posiciones de " + SelectedFriend.Name;


            myMap.Center = new Location(SelectedFriend.LastPosition.Latitude, SelectedFriend.LastPosition.Longitude);
            myMap.ZoomLevel = 15;
            lstPositions.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == 1)
            {
                Position item = lstPositions.Items.Last() as Position;
                layers = new MapLayer();
                image = new BitmapImage();
                image.UriSource = (new Uri(SelectedFriend.Picture, UriKind.Absolute));

                grid = new Grid();
                grid.DataContext = item;
                grid.RightTapped += grid_RightTapped;
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
                layers.Children.Add(grid);
                MapLayer.SetPosition(grid, new Location(item.Latitude, item.Longitude));
                myMap.Children.Add(layers);
            }
        }

        private async void grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Position selectedPosition = (sender as Grid).DataContext as Position;
            (DataContext as FriendInfoViewModel).SelectedFriend = SelectedFriend;
            (DataContext as FriendInfoViewModel).SelectedPosition = selectedPosition;
            PopupMenu menu = new PopupMenu();

            menu.Commands.Add(new UICommand(AppResources.General_GetDirections, (command) =>
            {
                staticObjects.GoalLongitude = selectedPosition.Longitude;
                staticObjects.GoalLatitude = selectedPosition.Latitude;
                staticObjects.FriendProfilePicture = new Uri(SelectedFriend.Picture, UriKind.Absolute);
                (DataContext as FriendInfoViewModel).GetDirections.Execute(null);
            }));
            menu.Commands.Add(new UICommand(AppResources.General_ShareFacebook, (command) =>
            {
                Share.ShareToFacebook("http://bing.com/maps/default.aspx" +
                "?cp=" + selectedPosition.Latitude + "~" + selectedPosition.Longitude +
                "&lvl=18" +
                "&style=r",
                String.Format(AppResources.General_LastPositionMessageFriend,
                    SelectedFriend.Name,
                    selectedPosition.Address,
                    selectedPosition.RegisteredAt.ToString() + " GMT",
                    "http://bing.com/maps/default.aspx"), SelectedFriend.Picture);
            }));
            menu.Commands.Add(new UICommand(AppResources.General_ShareEmail, (command) =>
            {
                SetSharingContent(selectedPosition, SelectedFriend.Name);
                Share.ShowShareBar();
            }));

            await menu.ShowForSelectionAsync(Screen.GetElementRect((FrameworkElement)sender));
        }

        private static void SetSharingContent(Position selectedPosition, string friendName)
        {
            Share.Title = "SmartGuard - " + friendName;
            Share.Description = "Comparte la posicion de tus amigos";
            DateTime date = selectedPosition.RegisteredAt;
            Share.Message = String.Format(AppResources.FacebookMessageFriendPost, 
                
                
                friendName, 
                selectedPosition.Address, 
                date.Date.ToString(), 
                date.TimeOfDay.ToString(), 
                "http://bing.com/maps/default.aspx" +
                "?cp=" + selectedPosition.Latitude + "~" + selectedPosition.Longitude +
                "&lvl=18" +
                "&style=r");

        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}