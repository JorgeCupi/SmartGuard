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
using CustomLiveTiles;
using Facebook.Methods;
using SmartGuard.Core.Facebook.Authenticate;
using Windows.UI;
using Windows.UI.Popups;
using System.Threading.Tasks;
using SmartGuard.Core.ViewModels;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
namespace SmartGuard.Store.Views
{
    public sealed partial class MainView : SmartGuard.Store.Common.LayoutAwarePage
    {
        public MainView()
        {
            this.InitializeComponent();
             
            CheckAuthentication();
            TileFacebook.Tapped += tileFacebook_Tapped;
            TileFriends.Tapped += TileFriends_Tapped;
            TileNews.Tapped += TileNews_Tapped;
            TileRedZones.Tapped += TileRedZones_Tapped;
            TileBroadcast.Tapped += TileBroadcast_Tapped;

            SetSharing();
            GPS.InitializeGPS();
        }

        private void SetSharing()
        {
            Share.InitializeSharing(DataTransferManager.GetForCurrentView(), "");
            Share.Title = "Share Smartguard!";
            Share.Description = "Download SmartGuard for different platforms";
            Share.Message = "You can download SmartGuard if your smartphone / tablet / computer has Windows Phone, Windows 8, iOS or Android as it's OS. The links for the different stores are below. ";
            Share.Link = "http://windowsphone.com";
        }

        void TileRedZones_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (this.DataContext as MainViewModel).NavigateToRedZones.Execute(null);
        }

        void TileNews_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (this.DataContext as MainViewModel).NavigateToNotifications.Execute(null);
        }

        void TileBroadcast_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (this.DataContext as MainViewModel).NavigateToPanic.Execute(null);
        }

        void TileFriends_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (this.DataContext as MainViewModel).NavigateToFriends.Execute(null);
        }

        async void tileFacebook_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.isAuthenticated)
            {
                MessageBoxResult result = await MessageBox.ShowAsync("You're already logged in. Do you want to sign out of this account?. If yes, press Sign Out. Or press Cancel to return to the main menu", "SmartGuard Account", MessageBoxButton.SignOutCancel);

                if (result == MessageBoxResult.SignOut)
                {
                    App.isAuthenticated = false;
                    FacebookMethods.LogOut();
                    txtProfileName.Text = string.Empty;
                    imgProfilPicture.Source = null;
                }
            }

            else await FacebookMethods.LogIn();

            CheckAuthentication();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void CheckAuthentication()
        {
            SmartGuard.Core.Azure.Authenticate.Utilities.azureAccount = "smartguard";
            SmartGuard.Core.Azure.Authenticate.Utilities.azureKey = "KcBfnaweUFH5TSnxkiO8GuSHa9AhqwELpdeWT4gJ0hdMxJ+fHpe5adtr8EZ8gQmeruFfqe8FikP0KrvkOYH5rA==";

            if (App.isAuthenticated)
            {
                Utilities.fbAppID = App.fbAppID;
                Utilities.fbToken = App.fbToken;
                Utilities.fbSecret = App.fbSecret;
                Utilities.fbUserID = App.fbUserID;

                txtProfileName.Text = App.fbProfileName;
                BitmapImage source = new BitmapImage();
                source.UriSource = new Uri(App.fbProfilePicture, UriKind.Absolute);
                imgProfilPicture.Source = source;
                EnableVisibility();
            }
            else
            {
                App.fbSecret = "a811eda1592b1c3f4f6bd42c322e1734";
                App.fbAppID = "214619712026208";
                App.isAuthenticated = false;

                CollapseVisibility();
            }
        }
        private void EnableVisibility()
        {
            (TileFriends.Child as LargeTileImage).BackgroundColor = Colors.Red;
            TileFriends.IsTapEnabled= true;

            (TileNews.Child as WideTileIcon).BackgroundColor = Colors.Red;
            TileNews.IsTapEnabled = true;

            (TileRedZones.Child as WideTileIcon).BackgroundColor = Colors.Red;
            TileRedZones.IsTapEnabled = true;

            (TileBroadcast.Child as LargeTileIcon).BackgroundColor = Colors.Red;
            TileBroadcast.IsTapEnabled = true;

            (TileAbout.Child as MediumTileIcon).BackgroundColor = Colors.Orange;
            TileAbout.IsTapEnabled = true;

            (TileRate.Child as MediumTileIcon).BackgroundColor = Colors.Green;
            TileRate.IsTapEnabled = true;

            (TileSettings.Child as WideTileFlipIcon).BackgroundColor = Colors.Red;
            TileSettings.IsTapEnabled = true;
        }

        private void CollapseVisibility()
        {
            Color color = Colors.Gray;

            TileFriends.IsTapEnabled = false;
            (TileFriends.Child as LargeTileImage).BackgroundColor = color;

            TileNews.IsTapEnabled = false;
            (TileNews.Child as WideTileIcon).BackgroundColor = color;

            TileRedZones.IsTapEnabled = false;
            (TileRedZones.Child as WideTileIcon).BackgroundColor = color;

            TileBroadcast.IsTapEnabled = false;
            (TileBroadcast.Child as LargeTileIcon).BackgroundColor = color;

            TileAbout.IsTapEnabled = false;
            (TileAbout.Child as MediumTileIcon).BackgroundColor = color;

            TileRate.IsTapEnabled = false;
            (TileRate.Child as MediumTileIcon).BackgroundColor = color;

            TileSettings.IsTapEnabled = false;
            (TileSettings.Child as WideTileFlipIcon).BackgroundColor = color;
        }
    }
}
