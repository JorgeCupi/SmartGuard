using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Models;
using System.Windows.Input;
using System.Windows;
namespace SmartGuard.Core.ViewModels
{
    public class MainViewModel 
		: MvxViewModel
    {
        public MainViewModel()
        {   }


        public ICommand NavigateToFriends
        {
            get { return new MvxCommand(()=> ShowViewModel<FriendsViewModel>()); }
        }
        public ICommand NavigateToNotifications
        {
            get { return new MvxCommand(() => ShowViewModel<RequestsViewModel>()); }
        }
        public ICommand NavigateToRedZones
        {
            get { return new MvxCommand(() => ShowViewModel<RedZonesViewModel>()); }
        }
        public ICommand NavigateToSettings
        {
            get { return new MvxCommand(() => ShowViewModel<SettingsViewModel>()); }
        }
        public ICommand NavigateToAbout
        {
            get { return new MvxCommand(() => ShowViewModel<AboutViewModel>()); }
        }
        public ICommand NavigateToRate
        {
            get { return new MvxCommand(() => ShowViewModel<RateViewModel>()); }
        }
        public ICommand NavigateToPanic
        {
            get { return new MvxCommand(() => ShowViewModel<BroadcastViewModel>()); }
        }
    }
}
