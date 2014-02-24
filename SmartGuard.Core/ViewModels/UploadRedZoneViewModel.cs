using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Models;
using System.Threading.Tasks;

namespace SmartGuard.Core.ViewModels
{
    public class UploadRedZoneViewModel : MvxViewModel
    {
        private bool downloadComplete;
        public bool DownloadComplete
        {
            get { return downloadComplete; }
            set { downloadComplete = value; RaisePropertyChanged(() => DownloadComplete); }
        }

        public async Task<bool>TryUploadNewRedZone(RedZone redZone)
        {
            return await Queries.TryInsertEntityAsync("RedZone", redZone);
        }
    }
}