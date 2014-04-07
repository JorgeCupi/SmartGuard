using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Facebook.Authenticate;
using SmartGuard.Core.Facebook.Classes;
using SmartGuard.Core.Facebook.Queries;
using SmartGuard.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartGuard.Core.ViewModels
{
    public class RequestsViewModel : MvxViewModel
    {
        private bool haventUploaded;

        public bool HaventUploaded
        {
            get { return haventUploaded; }
            set { haventUploaded = value; RaisePropertyChanged(() => HaventUploaded); }
        }

        public ObservableCollection<Friend> FriendsRequests { get; set; }

        public ObservableCollection<Friend> MyRequests { get; set; }

        private Friend selectedFriend;

        public Friend SelectedFriend
        {
            get { return selectedFriend; }
            set
            {
                selectedFriend = value;
                RaisePropertyChanged(() => SelectedFriend);
            }
        }

        public RequestsViewModel()
        {
            FriendsRequests = new ObservableCollection<Friend>();
            FriendsRequests.CollectionChanged += Requests_CollectionChanged;
            MyRequests = new ObservableCollection<Friend>();
            MyRequests.CollectionChanged += MyRequests_CollectionChanged;
            DownloadMyPermissionsRequests();
            DownloadPermissionsRequests();
        }

        public async void DownloadMyPermissionsRequests()
        {
            HaventUploaded = true;
            await AddPermissionsToThisList("PartitionKey", MyRequests);
            HaventUploaded = false;
        }

        public async void DownloadPermissionsRequests()
        {
            HaventUploaded = true;
            await AddPermissionsToThisList("RowKey", FriendsRequests);
            HaventUploaded = false;
        }

        private async Task  AddPermissionsToThisList(string keyName, ObservableCollection<Friend> collection)
        {
            List<Permission> permissions = await Queries.GetPermissionsForThisCode(keyName, Utilities.fbUserID);
            foreach (Permission Allowed in permissions)
            {
                try
                {
                    FacebookUser fbUser;
                    if (keyName == "RowKey")
                        fbUser = await Users.DownloadUserInfo(Allowed.FBIDFromViewer);
                    else fbUser = await Users.DownloadUserInfo(Allowed.FBIDFromViewed);
                    collection.Add(new Friend
                    {
                        FacebookID = fbUser.Id,
                        Name = fbUser.Name,
                        Picture = fbUser.Picture.data.url,
                        IsAllowed = Allowed.IsAllowed,
                        FBIDFromViewer = Allowed.FBIDFromViewer,
                        FBIDFromViewed = Allowed.FBIDFromViewed
                    });
                }
                catch (Exception)
                {
                    ///ToDo:
                    ///Remover el permiso del codigo que fue rechazado
                }
            }
        }

        private void Requests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => FriendsRequests);
        }

        private void MyRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => MyRequests);
        }

        public async Task<bool> TryUpdatePermission(Permission permission)
        {
            HaventUploaded = true;
            bool result = await Queries.TryUpdateEntityAsync("AllowedOne", permission, permission.PartitionKey, permission.RowKey);
            HaventUploaded = false;

            return result;
        }

        public ICommand AddNewFriend
        {
            get { return new MvxCommand(() => ShowViewModel<FriendAddViewModel>()); }
        }
    }
}