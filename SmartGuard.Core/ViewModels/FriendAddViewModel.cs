using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Models;
using System.Threading.Tasks;

namespace SmartGuard.Core.ViewModels
{
    public partial class FriendAddViewModel : MvxViewModel
    {
        private bool haventUploaded;
        public bool HaventUploaded
        {
            get { return haventUploaded; }
            set { haventUploaded = value; RaisePropertyChanged(() => HaventUploaded); }
        }

        public FriendAddViewModel()
        {
            HaventUploaded = false;
        }

        public async Task<bool> TryAddNewFriend(Permission permission)
        {
            HaventUploaded = true;
            bool result = await Queries.TryInsertEntityAsync("AllowedOne", permission);
            HaventUploaded = false;

            return result;
        }
    }
}