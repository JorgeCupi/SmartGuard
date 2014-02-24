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
using SmartGuard.Core.ViewModels;
using Microsoft.Phone.Tasks;
namespace SmartGuard.Phone.Views
{
    public partial class RateView : MvxPhonePage
    {
        public RateView()
        {
            InitializeComponent();

            btnRate.Tap += btnRate_Tap;
            btnSurvey.Tap += btnSurvey_Tap;
        }

        void btnSurvey_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (this.DataContext as RateViewModel).GoToSurvey.Execute(null);
        }

        void btnRate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }
    }
}