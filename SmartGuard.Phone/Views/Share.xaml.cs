using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using QuickMethods;
namespace SmartGuard.Phone.Views
{
    public partial class Share : PhoneApplicationPage
    {
        string title, message;
        Uri link;
        public Share()
        {
            InitializeComponent();
            txtMail.Tap += txtMail_Tap;
            txtSMS.Tap += txtSMS_Tap;
            txtSocial.Tap += txtSocial_Tap;
            this.Loaded += Share_Loaded;
        }

        void Share_Loaded(object sender, RoutedEventArgs e)
        {
            title = ShareContent.Title;
            message = ShareContent.Message;
            link = ShareContent.Link;
        }


        void txtSocial_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txtSocial.Foreground = this.Resources["PhoneAccentBrush"] as SolidColorBrush;
            SocialNetworks.shareLink(link, message, title);
        }

        void txtSMS_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txtSMS.Foreground = this.Resources["PhoneAccentBrush"] as SolidColorBrush;
            QuickMethods.Phone.sendSMS(title + "\n" + message + "\n" + link);

        }

        void txtMail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txtMail.Foreground = this.Resources["PhoneAccentBrush"] as SolidColorBrush;
            Email.sendEmail(message + "\n\n\n" + link, title);
        }
    }
}


// Must be the most global namespace of the App
namespace SmartGuard.Phone
{
    public static class ShareContent
    {
        public static string Title { get; set; }
        public static string Message { get; set; }
        public static Uri Link { get; set; }
    }
}