using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SmartGuard.Store.Views
{
    public sealed partial class RequestsView : SmartGuard.Store.Common.LayoutAwarePage
    {
        string typeOfPermission = string.Empty;
        public RequestsView()
        {
            this.InitializeComponent();
            btnAddContact.Tapped += btnAddContact_Tapped;
            lstIncoming.Tapped += lst_Tap;
            lstOutgoing.Tapped += lst_Tap;
        }

        async void lst_Tap(object sender, TappedRoutedEventArgs e)
        {
            string a, b, c, d, ee = string.Empty;
            ListView selected = sender as ListView;
            if (selected.Name == "lstIncoming")
            {
                a = AppResources.General_Txt_Revoke;
                b = AppResources.General_Txt_Concede;
                c = AppResources.RequestsView_Txt_UpdatePermission;
                d = AppResources.RequestView_Txt_UpdatePermissionSuccess;
                ee = AppResources.RequestView_Txt_UpdatePermissionError;
            }
            else
            {
                a = AppResources.General_Txt_Unfollow;
                b = AppResources.General_Txt_Follow;
                c = AppResources.RequestsView_Txt_UpdateFollowingStatus;
                d = AppResources.RequestView_Txt_UpdatePermissionSuccess;
                ee = AppResources.RequestView_Txt_UpdatePermissionError;
            }

            await AskQuestion(a, b, c, d, ee);
        }

        void btnAddContact_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (this.DataContext as RequestsViewModel).AddNewFriend.Execute(null);
        }

        static Permission GetPermission(Friend selectedFriend)
        {
            Permission per = new Permission();
            per.FBIDFromViewed = selectedFriend.FBIDFromViewed;
            per.FBIDFromViewer = selectedFriend.FBIDFromViewer;
            per.IsAllowed = !selectedFriend.IsAllowed;
            per.PartitionKey = per.FBIDFromViewer;
            per.RowKey = per.FBIDFromViewed;
            return per;
        }

        async Task AskQuestion(string a, string b, string c, string d, string e)
        {
            Friend selectedFriend = (this.DataContext as RequestsViewModel).SelectedFriend;
            if (selectedFriend != null)
            {
                Permission per = GetPermission(selectedFriend);
                if (selectedFriend.IsAllowed)
                    typeOfPermission = a;
                else typeOfPermission = b;

                #region AskQuestion

                MessageBoxResult result = await MessageBox.ShowAsync(String.Format(
                    c,
                    typeOfPermission,
                    selectedFriend.Name), pageTitle.Text, MessageBoxButton.OKCancel);

                #region CheckResult

                if (result == MessageBoxResult.OK)
                {
                    bool updateResultSucceded = await (this.DataContext as RequestsViewModel).TryUpdatePermission(per);
                    if (updateResultSucceded)
                    {
                        if (a == "revocar")
                        {
                            (this.DataContext as RequestsViewModel).FriendsRequests.Clear();
                            (this.DataContext as RequestsViewModel).DownloadPermissionsRequests();
                        }
                        else
                        {
                            (this.DataContext as RequestsViewModel).MyRequests.Clear();
                            (this.DataContext as RequestsViewModel).DownloadMyPermissionsRequests();
                        }
                        await MessageBox.ShowAsync(d,
                        pageTitle.Text);
                    }
                    else await MessageBox.ShowAsync(e,
                        pageTitle.Text);
                }
            }

                #endregion CheckResult

                #endregion AskQuestion
        }
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
