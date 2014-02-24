using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Facebook.Authenticate;
using SmartGuard.Core.Facebook.Classes;
using SmartGuard.Core.Facebook.Queries;
using SmartGuard.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartGuard.Core.ViewModels
{
    public class FriendsViewModel : MvxViewModel
    {
        private bool haventDownloaded;
        public bool HaventDownloaded
        {
            get { return haventDownloaded; }
            set { haventDownloaded = value; RaisePropertyChanged(() => HaventDownloaded); }
        }

        private bool downloadedError;
        public bool DownloadedError
        {
            get { return downloadedError; }
            set { downloadedError = value; RaisePropertyChanged(() => DownloadedError); }
        }

        private Friend selectedFriend;
        public Friend SelectedFriend
        {
            get { return selectedFriend; }
            set { 
                selectedFriend = value; 
                RaisePropertyChanged(() => SelectedFriend);
                StaticClasses.StaticFriend = SelectedFriend;
            }
        }
        public ObservableCollection<Friend> Friends { get; set; }


        public FriendsViewModel()
        {
            Friends = new ObservableCollection<Friend>();
            Friends.CollectionChanged += Friends_CollectionChanged;
            DownloadFriends();
        }

        private async void DownloadFriends()
        {
            HaventDownloaded = true;
            List<Permission> permissions = await Queries.GetPermissionsForThisCode("PartitionKey",Utilities.fbUserID);
            if (permissions!=null)
            {
                foreach (Permission Allowed in permissions)
                {
                    if(Allowed.IsAllowed)
                    {
                        List<Position> positions = await Queries.GetPositionsForThisCode(Allowed.FBIDFromViewed, 1);
                        FacebookUser fbUser;
                        if (positions.Count == 1)
                        {
                            positions.First().RegisteredAt = positions.First().RegisteredAt.ToLocalTime();
                            fbUser = await Users.DownloadUserInfo(Allowed.FBIDFromViewed);
                            if (fbUser != null)
                            {
                                Friends.Add(new Friend
                                {
                                    FacebookID = Allowed.FBIDFromViewed,
                                    LastPosition = positions.First(),
                                    Name = fbUser.Name,
                                    Picture = fbUser.Picture.data.url,
                                });
                            }
                            else DownloadedError = true;
                        }
                    }
                }
                DownloadedError = false;
            }
            else DownloadedError = true;
            HaventDownloaded = false;
        }

        private void Friends_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Friends);
        }

        public ICommand TapOnFriend
        {
            get
            { return new MvxCommand(() => ShowViewModel<FriendInfoViewModel>());}
        }

        public ICommand GetDirections
        {
            get
            { return new MvxCommand(() => ShowViewModel<GetDirectionsViewModel>()); }
        }

        public ICommand AddNewFriend
        {
            get { return new MvxCommand(() => ShowViewModel<FriendAddViewModel>()); }
        }
    }
}