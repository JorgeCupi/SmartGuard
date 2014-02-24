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
using System.Windows.Input;
using SmartGuard.Core.Facebook.Authenticate;
using Facebook.Methods;
using SmartGuard.Phone.Resources;
using System.Windows.Media;
using SmartGuard.Phone;
using SmartGuard.Core;
using SmartGuard.Core.Models;
using System.Globalization;
using Microsoft.Phone.Scheduler;
using System.Threading.Tasks;
using System.Device.Location;
namespace SmartGuard.Phone.Views
{
    public partial class MainView : MvxPhonePage
    {
        bool firstTime = true;
        public MainView()
        {
            InitializeComponent();
            CheckAuthentication();
            TileFacebook.Tap += TileFacebook_Tap;
            TileBroadcast.Hold+=TileBroadcast_Hold;

            this.Loaded += MainView_Loaded;

        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            FillTileTitles();
            if (firstTime)
            {
                pivotMain.Opacity = 0.2;
                InitializeGPS();
                firstTime = false;
            }
        }

        public async void InitializeGPS()
        {
            bool completed = false;
            txtMessage.Text = AppResources.LocationService_Completing;
            completed = await GPS.InitializeGPS();
            pgrBar.Visibility = Visibility.Collapsed;

            if (!completed)
                txtMessage.Text = AppResources.LocationService_NoAddressInfo;
            else
            {
                txtMessage.Visibility = Visibility.Collapsed;
                pivotMain.Opacity = 1;
            }
        }

        void TileBroadcast_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {

            if (App.QuickBroadcastEnabled)
                UploadPosition();
            else StartIntenseMode();
        }

        private void StartIntenseMode()
        {
            if (App.IntenseModeEnabled)
            {
                App.IntenseModeEnabled = false;
                BackAgent.RemoveAgent("SmartGuardPeriodicAgent");
                MessageBox.Show(AppResources.IntenseModeOFF, AppResources.ApplicationTitle, MessageBoxButton.OK);
            }
            else
            {
                if (staticObjects.Address != null)
                    BackAgent.StartIntenseMode();
                else
                    MessageBox.Show(AppResources.IntenseModeCantStart, AppResources.SettingsView_IntenseMode, MessageBoxButton.OK);

            }
            
        }

        private void UploadPosition()
        {
            Broadcast.TryUploadPosition(false);
        }

        void FillTileTitles()
        {
            TileFriends.Title = AppResources.MainView_Tile_Friends;
            TileNotifications.Title = AppResources.MainView_Tile_Notifications;
            TileBroadcast.Title = AppResources.MainView_Tile_Broadcast;
            TileRedZones.Title = AppResources.MainView_Tile_RedZones;
            TileRedZones.Message = AppResources.MainView_Tile_RedZones_Message;
            TileNotifications.Message = AppResources.MainView_Tile_Notifications_Message;
            if (App.QuickBroadcastEnabled)
                TileBroadcast.Message = AppResources.MainView_Tile_BroadcastMessage_BroadcastEnabled;
            else TileBroadcast.Message = AppResources.MainView_Tile_BroadcastMessage_IntenseEnabled;
        }

        async void TileFacebook_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.isAuthenticated)
            {
                MessageBoxResult result = MessageBox.Show(
                    AppResources.Facebook_LogOut_Question,
                    AppResources.ApplicationTitle, 
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    App.isAuthenticated = false;
                    FacebookMethods.LogOut();
                }
            }

            else
            {
                string result = await FacebookMethods.LogIn();
                if (result == "OK")
                    MessageBox.Show(AppResources.Facebook_LogIn_Success, AppResources.ApplicationTitle, MessageBoxButton.OK);

                else MessageBox.Show(AppResources.Facebook_LogIn_Error, AppResources.ApplicationTitle, MessageBoxButton.OK);
            }

            CheckAuthentication();
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
                EnableVisibility();
            }
            else {
                App.fbSecret = "a811eda1592b1c3f4f6bd42c322e1734";
                App.fbAppID = "214619712026208";
                App.isAuthenticated = false;

                CollapseVisibility();
            }
        }

        private void EnableVisibility()
        {
            TileFriends.Background = (Color)(this.Resources["PhoneAccentColor"]);
            TileFriends.IsEnabled = true;

            TileNotifications.Background = (Color)(this.Resources["PhoneAccentColor"]);
            TileNotifications.IsEnabled = true;

            TileRedZones.Background = (Color)(this.Resources["PhoneAccentColor"]);
            TileRedZones.IsEnabled = true;

            TileBroadcast.Background = (Color)(this.Resources["PhoneAccentColor"]); ;
            TileBroadcast.IsEnabled = true;

            TileAbout.Background = Colors.Orange;
            TileAbout.IsEnabled = true;

            TileRate.Background = Colors.Green;
            TileRate.IsEnabled = true;

            TileSettings.Background = Colors.Red;
            TileSettings.IsEnabled = true;
        }
        private void CollapseVisibility()
        {
            Color color = Colors.Gray;

            TileFriends.Background = color;
            TileFriends.IsEnabled = false;

            TileNotifications.Background = color;
            TileNotifications.IsEnabled = false;

            TileRedZones.Background = color;
            TileRedZones.IsEnabled = false;

            TileBroadcast.Background = color;
            TileBroadcast.IsEnabled = false;

            TileAbout.Background = color;
            TileAbout.IsEnabled = false;

            TileRate.Background = color;
            TileRate.IsEnabled = false;

            TileSettings.Background = color;
            TileSettings.IsEnabled = false;
        }
    }
}