using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SmartGuard.Phone.Resources;
using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;
namespace SmartGuard.Phone
{
    public partial class App : Application
    {
        #region FB Attributes

        public static string fbToken
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("fbToken"))
                {
                    return (IsolatedStorageSettings.ApplicationSettings["fbToken"]).ToString();
                }
                else return String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["fbToken"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static string fbProfilePicture
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("fbProfilePicture"))
                {
                    return (IsolatedStorageSettings.ApplicationSettings["fbProfilePicture"]).ToString();
                }
                else return String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["fbProfilePicture"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static string fbProfileName
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("fbProfileName"))
                {
                    return (IsolatedStorageSettings.ApplicationSettings["fbProfileName"]).ToString();
                }
                else return String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["fbProfileName"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static string fbUserID
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("fbUserID"))
                {
                    return (IsolatedStorageSettings.ApplicationSettings["fbUserID"]).ToString();
                }
                else return String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["fbUserID"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static string fbSecret
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("fbSecret"))
                {
                    return (IsolatedStorageSettings.ApplicationSettings["fbSecret"]).ToString();
                }
                else return String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["fbSecret"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static string fbAppID
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("fbAppID"))
                {
                    return (IsolatedStorageSettings.ApplicationSettings["fbAppID"]).ToString();
                }
                else return String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["fbAppID"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static bool isAuthenticated
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("isAuthenticated"))
                {
                    return (bool)(IsolatedStorageSettings.ApplicationSettings["isAuthenticated"]);
                }
                else return false;
            }

            set
            {
                IsolatedStorageSettings.ApplicationSettings["isAuthenticated"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        #endregion FB Attributes

        #region Settings

        public static bool DistancePreferredIsKilometers
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("DistancePreferredIsKilometers"))
                {
                    return (bool)(IsolatedStorageSettings.ApplicationSettings["DistancePreferredIsKilometers"]);
                }
                else return false;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["DistancePreferredIsKilometers"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static bool OfflineDBEnabled
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("OfflineDBEnabled"))
                {
                    return (bool)(IsolatedStorageSettings.ApplicationSettings["OfflineDBEnabled"]);
                }
                else return false;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["OfflineDBEnabled"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static bool LockScreenEnabled
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("LockScreenEnabled"))
                {
                    return (bool)(IsolatedStorageSettings.ApplicationSettings["LockScreenEnabled"]);
                }
                else return false;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["LockScreenEnabled"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static int MaxPositionsIndex
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("MaxPositionsIndex"))
                {
                    return int.Parse((IsolatedStorageSettings.ApplicationSettings["MaxPositionsIndex"]).ToString());
                }
                else return 0;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["MaxPositionsIndex"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static int MaxPositions
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("MaxPositions"))
                {
                    return int.Parse((IsolatedStorageSettings.ApplicationSettings["MaxPositions"]).ToString());
                }
                else return 5;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["MaxPositions"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static bool IntenseModeEnabled
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("IntenseModeEnabled"))
                {
                    return (bool)(IsolatedStorageSettings.ApplicationSettings["IntenseModeEnabled"]);
                }
                else return false;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["IntenseModeEnabled"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static bool QuickIntenseModeEnabled
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("QuickIntenseModeEnabled"))
                {
                    return (bool)(IsolatedStorageSettings.ApplicationSettings["QuickIntenseModeEnabled"]);
                }
                else return false;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["QuickIntenseModeEnabled"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static bool QuickBroadcastEnabled
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("QuickBroadcastEnabled"))
                {
                    return (bool)(IsolatedStorageSettings.ApplicationSettings["QuickBroadcastEnabled"]);
                }
                else return true;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["QuickBroadcastEnabled"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        #endregion Settings

        private static bool runningInBackground;

        public static bool RunningInBackground
        {
            get { return runningInBackground; }
            set { runningInBackground = value; }
        }
        
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            var setup = new Setup(RootFrame);
            setup.Initialize();
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            RootFrame.Navigating += RootFrameOnNavigating;
        }

        private void RootFrameOnNavigating(object sender, NavigatingCancelEventArgs args)
        {
            args.Cancel = true;
            RootFrame.Navigating -= RootFrameOnNavigating;
            RootFrame.Dispatcher.BeginInvoke(() =>
            {
                Cirrious.CrossCore.Mvx.Resolve<Cirrious.MvvmCross.ViewModels.IMvxAppStart>().Start();
            });
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        { 
            RunningInBackground = false;
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            App.RunningInBackground = true;
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        { }

        private void Application_RunningInBackground(object sender, RunningInBackgroundEventArgs e)
        {
            RunningInBackground = true;
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // LOOOOL You're fucked
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // LOOOOL You're fucked
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion Phone application initialization

        private void InitializeLanguage()
        {
            try
            {
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}