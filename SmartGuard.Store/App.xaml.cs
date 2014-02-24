using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
namespace SmartGuard.Store
{
    sealed partial class App : Application
    {
        #region FB Attributes
        public static ApplicationDataContainer Settings;
        public static string fbToken
        {
            get
            {
                if (Settings.Values["fbToken"] !=null)
                    return (Settings.Values["fbToken"]).ToString();
                else return String.Empty;
            }
            set { Settings.Values["fbToken"] = value; }
        }

        public static string fbUserID
        {
            get
            {
                if (Settings.Values["fbUserID"] != null)
                    return (Settings.Values["fbUserID"]).ToString();
                else return String.Empty;
            }
            set { Settings.Values["fbUserID"] = value; }
        }

        public static string fbSecret
        {
            get
            {
                if (Settings.Values["fbSecret"] != null)
                    return (Settings.Values["fbSecret"]).ToString();
                else return String.Empty;
            }
            set { Settings.Values["fbSecret"] = value; }
        }

        public static string fbProfilePicture
        {
            get
            {
                if (Settings.Values["fbProfilePicture"] != null)
                    return (Settings.Values["fbProfilePicture"]).ToString();
                else return String.Empty;
            }
            set { Settings.Values["fbProfilePicture"] = value; }
        }

        public static string fbProfileName
        {
            get
            {
                if (Settings.Values["fbProfileName"] != null)
                    return (Settings.Values["fbProfileName"]).ToString();
                else return String.Empty;
            }
            set { Settings.Values["fbProfileName"] = value; }
        }

        public static string fbAppID
        {
            get
            {
                if (Settings.Values["fbAppID"] != null)
                    return (Settings.Values["fbAppID"]).ToString();
                else return String.Empty;
            }
            set { Settings.Values["fbAppID"] = value; }
        }

        public static bool isAuthenticated
        {
            get
            {
                if (Settings.Values["isAuthenticated"] != null)
                    return (bool)(Settings.Values["isAuthenticated"]);
                else return false;
            }
            set { Settings.Values["isAuthenticated"] = value; }
        }

        #endregion FB Attributes

        #region Settings

        public static bool DistancePreferredIsKilometers
        {
            get
            {
                if (Settings.Values["DistancePreferredIsKilometers"] != null)
                    return (bool)(Settings.Values["DistancePreferredIsKilometers"]);
                else return false;
            }
            set { Settings.Values["DistancePreferredIsKilometers"] = value; }
        }

        public static bool LockScreenEnabled
        {
            get
            {
                if (Settings.Values["LockScreenEnabled"] != null)
                    return (bool)(Settings.Values["LockScreenEnabled"]);
                else return false;
            }
            set { Settings.Values["LockScreenEnabled"] = value; }
        }

        public static int MaxPositionsIndex
        {
            get
            {
                if (Settings.Values["MaxPositionsIndex"] !=null)
                    return int.Parse((Settings.Values["MaxPositionsIndex"]).ToString());
                else return 0;
            }
            set { Settings.Values["MaxPositionsIndex"] = value; }
        }

        public static int MaxPositions
        {
            get
            {
                if (Settings.Values["MaxPositions"] != null)
                    return int.Parse((Settings.Values["MaxPositions"]).ToString());
                else return 0;
            }
            set { Settings.Values["MaxPositions"] = value; }
        }

        public static bool IntenseModeEnabled
        {
            get
            {
                if (Settings.Values["IntenseModeEnabled"] != null)
                    return (bool)(Settings.Values["IntenseModeEnabled"]);
                else return false;
            }
            set { Settings.Values["IntenseModeEnabled"] = value; }
            
        }

        public static bool QuickIntenseModeEnabled
        {
            get
            {
                if (Settings.Values["QuickIntenseModeEnabled"] != null)
                    return (bool)(Settings.Values["QuickIntenseModeEnabled"]);
                else return false;
            }
            set { Settings.Values["QuickIntenseModeEnabled"] = value; }
        }

        public static bool QuickBroadcastEnabled
        {
            get
            {
                if (Settings.Values["QuickBroadcastEnabled"] != null)
                    return (bool)(Settings.Values["QuickBroadcastEnabled"]);
                else return false;
            }
            set { Settings.Values["QuickBroadcastEnabled"] = value; }
        }

        #endregion Settings
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            Settings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }

     protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                var setup = new Setup(rootFrame);
                setup.Initialize();

                var start = Cirrious.CrossCore.Mvx.Resolve<Cirrious.MvvmCross.ViewModels.IMvxAppStart>();
                start.Start();
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
