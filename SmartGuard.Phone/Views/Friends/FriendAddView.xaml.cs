using Cirrious.MvvmCross.WindowsPhone.Views;
using Facebook.Methods;
using SmartGuard.Core.Facebook.Classes;
using System.Collections.Generic;
using System.Windows;
using SmartGuard.Core.ViewModels;
using SmartGuard.Core.Models;
using System.Windows.Input;
using SmartGuard.Phone.Resources;
namespace SmartGuard.Phone.Views.Friends
{
    public partial class FriendAddView : MvxPhonePage
    {
        public FriendAddView()
        {
            InitializeComponent();

            this.Loaded += AddFriendView_Loaded;
            lstFriends.Tap += lstFriends_Tap;
            pgrbDownloadedCompleted.Visibility = Visibility.Visible;
        }


        async void AddFriendView_Loaded(object sender, RoutedEventArgs e)
        {
            List < FacebookUser > data = await FacebookMethods.DownloadFriendsList();
            if (data!=null)
            {
                List<AlphaKeyGroup<FacebookUser>> DataSource = new List<AlphaKeyGroup<FacebookUser>>();

                DataSource = AlphaKeyGroup<FacebookUser>.CreateGroups(data,
                    System.Threading.Thread.CurrentThread.CurrentUICulture,
                    (FacebookUser s) => { return s.Name; }, true);

                lstFriends.ItemsSource = DataSource;
            }
            else MessageBox.Show(AppResources.FriendAddView_Txt_FriendDownloadError, AppResources.ApplicationTitle, MessageBoxButton.OK);
            pgrbDownloadedCompleted.Visibility = Visibility.Collapsed;
        }

        async void lstFriends_Tap(object sender, GestureEventArgs e)
        {
            if (lstFriends.SelectedItem != null)
            {
                
                FacebookUser selectedFriend = lstFriends.SelectedItem as FacebookUser;
                Permission permission = new Permission();

                MessageBoxResult result = MessageBox.Show(
                    string.Format(AppResources.FriendAddView_Txt_FriendAddQuestion,selectedFriend.Name),
                    AppResources.FriendAddView_Txt_FriendAddTitle,
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    permission.FBIDFromViewed = selectedFriend.Id;
                    permission.FBIDFromViewer = App.fbUserID;
                    permission.IsAllowed = false;
                    permission.PartitionKey = permission.FBIDFromViewer;
                    permission.RowKey = permission.FBIDFromViewed;
                    
                    bool addResult = await (this.DataContext as FriendAddViewModel).TryAddNewFriend(permission);
                    if (addResult)
                        MessageBox.Show(AppResources.FriendAddView_Txt_FriendAddSuccess,AppResources.FriendAddView_Txt_FriendAddTitle, MessageBoxButton.OK);
                    else MessageBox.Show(AppResources.FriendAddView_Txt_FriendAddError, AppResources.FriendAddView_Txt_FriendAddTitle, MessageBoxButton.OK);
                }   
            }
        }
    }
}