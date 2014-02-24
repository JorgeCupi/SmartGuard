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
using Windows.System;

namespace SmartGuard.Phone.Views
{
    public partial class SettingsView : MvxPhonePage
    {
        public SettingsView()
        {
            InitializeComponent();

            //tglDB.Checked += tglDB_Checked;
            //tglDB.Unchecked += tglDB_Unchecked;
            tglDistance.Checked += tglDistance_Checked;
            tglDistance.Unchecked += tglDistance_Unchecked;
            tglLockScreen.Checked += tglLockScreen_Checked;
            tglLockScreen.Unchecked += tglLockScreen_Unchecked;

            tglQuickIntense.Checked += tglQuickIntense_Checked;
            tglQuickIntense.Unchecked += tglQuickIntense_Unchecked;
            tglQuickBroadcast.Checked += tglQuickBroadcast_Checked;
            tglQuickBroadcast.Unchecked += tglQuickBroadcast_Unchecked;
            tglIntenseMode.Checked += tglIntenseMode_Checked;
            tglIntenseMode.Unchecked += tglIntenseMode_Unchecked;

            txtLock.Tap += txtLock_Tap;
            lstpMaxPositions.SelectionChanged += lstpMaxPositions_SelectionChanged;
            LoadSettings();
        }

        void tglIntenseMode_Checked(object sender, RoutedEventArgs e)
        {
            App.IntenseModeEnabled = true;
            tglIntenseMode.Content = AppResources.General_Started;
            if (staticObjects.Address != null)
                BackAgent.StartIntenseMode();
            else {
                MessageBox.Show(AppResources.IntenseModeCantStart, AppResources.SettingsView_IntenseMode, MessageBoxButton.OK);
                tglIntenseMode.IsChecked = false;
            }
        }

        void tglIntenseMode_Unchecked(object sender, RoutedEventArgs e)
        {
            App.IntenseModeEnabled = false;
            tglIntenseMode.Content = AppResources.General_Stopped;
            BackAgent.RemoveAgent("SmartGuardPeriodicAgent");
        }

        async void txtLock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool op = await Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }

        void tglQuickBroadcast_Checked(object sender, RoutedEventArgs e)
        {
            tglQuickBroadcast.Content = AppResources.General_Enabled;
            App.QuickBroadcastEnabled = true;

            tglQuickIntense.IsChecked = false;
            tglQuickIntense.Content = AppResources.General_Disabled;
            App.QuickIntenseModeEnabled = false;
        }
        void tglQuickBroadcast_Unchecked(object sender, RoutedEventArgs e)
        {
            tglQuickBroadcast.Content = AppResources.General_Disabled;
            App.QuickBroadcastEnabled = false;

            tglQuickIntense.IsChecked = true;
            tglQuickIntense.Content = AppResources.General_Enabled;
            App.QuickIntenseModeEnabled = true;
        }

        void lstpMaxPositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.MaxPositionsIndex = lstpMaxPositions.SelectedIndex;
            App.MaxPositions = int.Parse((lstpMaxPositions.SelectedItem as ListPickerItem).Content.ToString());
        }

        void tglQuickIntense_Checked(object sender, RoutedEventArgs e)
        {
            tglQuickIntense.Content = AppResources.General_Enabled;
            App.QuickIntenseModeEnabled = true;

            tglQuickBroadcast.IsChecked = false;
            tglQuickBroadcast.Content = AppResources.General_Disabled;
            App.QuickBroadcastEnabled = false;
        }
        void tglQuickIntense_Unchecked(object sender, RoutedEventArgs e)
        {
            tglQuickIntense.Content = AppResources.General_Disabled;
            App.QuickIntenseModeEnabled = false;

            tglQuickBroadcast.IsChecked = true;
            tglQuickBroadcast.Content = AppResources.General_Enabled;
            App.QuickBroadcastEnabled = true;
        }

        void tglLockScreen_Checked(object sender, RoutedEventArgs e)
        {
            tglLockScreen.Content = AppResources.General_On;
            App.LockScreenEnabled = true;
        }
        void tglLockScreen_Unchecked(object sender, RoutedEventArgs e)
        {
            tglLockScreen.Content = AppResources.General_Off;
            App.LockScreenEnabled = false;
        }

        //void tglDB_Checked(object sender, RoutedEventArgs e)
        //{
        //    tglDB.Content = AppResources.General_Enabled;
        //    App.OfflineDBEnabled = true;
        //}
        //void tglDB_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    tglDB.Content = AppResources.General_Disabled;
        //    App.OfflineDBEnabled = false;
        //}

        void tglDistance_Checked(object sender, RoutedEventArgs e)
        {
            tglDistance.Content = AppResources.General_Kilometers;
            App.DistancePreferredIsKilometers = true;
        }
        void tglDistance_Unchecked(object sender, RoutedEventArgs e)
        {
            tglDistance.Content = AppResources.General_Miles;
            App.DistancePreferredIsKilometers = false;
        }

        private void LoadSettings()
        {
            #region General
            if (App.DistancePreferredIsKilometers){
                tglDistance.IsChecked = true;
                tglDistance.Content = AppResources.General_Kilometers;
            }

            else {
                tglDistance.IsChecked = false;
                tglDistance.Content = AppResources.General_Miles;
            }

            //if (App.OfflineDBEnabled){
            //    tglDB.IsChecked = true;
            //    tglDB.Content = AppResources.General_Enabled;
            //}
            //else {
            //    tglDB.IsChecked = false;
            //    tglDB.Content = AppResources.General_Disabled;
            //}

            if (App.LockScreenEnabled){
                tglLockScreen.IsChecked = true;
                tglLockScreen.Content = AppResources.General_On;
            }
            else {
                tglLockScreen.IsChecked = false;
                tglLockScreen.Content = AppResources.General_Off;
            }
            #endregion

            #region Position
            if (App.IntenseModeEnabled)
            {
                tglIntenseMode.IsChecked = true;
                tglIntenseMode.Content = AppResources.General_Started;
            }
            else
            {
                tglIntenseMode.IsChecked = false;
                tglIntenseMode.Content = AppResources.General_Stopped;
            }

            if(App.QuickIntenseModeEnabled){
                tglQuickIntense.IsChecked = true;
                tglQuickIntense.Content = AppResources.General_Enabled;
            }
            else{
                tglQuickIntense.IsChecked = false;
                tglQuickIntense.Content = AppResources.General_Disabled;
            }

            if (App.QuickBroadcastEnabled)
            {
                tglQuickBroadcast.IsChecked = true;
                tglQuickBroadcast.Content = AppResources.General_Enabled;
            }
            else
            {
                tglQuickBroadcast.IsChecked = false;
                tglQuickBroadcast.Content = AppResources.General_Disabled;
            }
            #endregion

            #region Friends
            lstpMaxPositions.SelectedIndex = App.MaxPositionsIndex;
            #endregion
        }
    }
}