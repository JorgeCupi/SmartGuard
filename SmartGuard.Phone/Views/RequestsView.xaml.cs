using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Shell;
using SmartGuard.Core.Models;
using SmartGuard.Core.ViewModels;
using SmartGuard.Phone.Resources;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartGuard.Phone.Views
{
    public partial class RequestsView : MvxPhonePage
    {
        string typeOfPermission = string.Empty;

        public RequestsView()
        {
            InitializeComponent();
            LoadAppBar();
            lstIncoming.Tap += lst_Tap;
            lstOutgoing.Tap += lst_Tap;
        }

        void LoadAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnAdd = new ApplicationBarIconButton();
            btnAdd.IconUri = new Uri("/Assets/AppBar/Add.png", UriKind.Relative);
            btnAdd.Text = AppResources.AppBar_Add;
            btnAdd.Click += btnAdd_Click;
            ApplicationBar.Buttons.Add(btnAdd);
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            (this.DataContext as RequestsViewModel).AddNewFriend.Execute(null);
        }

        async void lst_Tap(object sender, GestureEventArgs e)
        {
            string a, b, c, d, ee = string.Empty;
            if (myPivot.SelectedIndex == 0)
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

                MessageBoxResult result = MessageBox.Show(String.Format(
                    c,
                    typeOfPermission,
                    selectedFriend.Name), AppResources.RequestsView_Title, MessageBoxButton.OKCancel);

                #region CheckResult

                if (result == MessageBoxResult.OK)
                {
                    bool updateResultSucceded = await (this.DataContext as RequestsViewModel).TryUpdatePermission(per);
                    if (updateResultSucceded)
                    {
                        if (a == "revoke" || a == "revocar")
                        {
                            (this.DataContext as RequestsViewModel).FriendsRequests.Clear();
                            (this.DataContext as RequestsViewModel).DownloadPermissionsRequests();
                        }
                        else {
                            (this.DataContext as RequestsViewModel).MyRequests.Clear();
                            (this.DataContext as RequestsViewModel).DownloadMyPermissionsRequests();
                        }
                        MessageBox.Show(d,
                        AppResources.RequestsView_Title,
                        MessageBoxButton.OK);
                    }
                    else MessageBox.Show(e,
                        AppResources.RequestsView_Title,
                        MessageBoxButton.OK);
                }
            }

                #endregion CheckResult

                #endregion AskQuestion
        }
    }
}