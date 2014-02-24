using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Facebook.Classes;
using SmartGuard.Core.Facebook.Queries;
using SmartGuard.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.Specialized;  
namespace SmartGuard.Core.ViewModels
{
    public class RedZonesViewModel:MvxViewModel
    {

        List<RedZone> redZones;

        private bool haventDownloaded;
        public bool HaventDownloaded
        {
            get { return haventDownloaded; }
            set { haventDownloaded = value; RaisePropertyChanged(() => HaventDownloaded); }
        }
        private RedZone selectedRedZone;
        public RedZone SelectedRedZone
        {
            get { return selectedRedZone; }
            set { 
                selectedRedZone = value; 
                RaisePropertyChanged(() => SelectedRedZone);
            }
        }
        public ObservableCollection<RedZone> RedZones { get; set; }
        public RedZonesViewModel()
        {
            RedZones = new ObservableCollection<RedZone>();
            RedZones.CollectionChanged += RedZones_CollectionChanged;
            HaventDownloaded = true;
        }

        public async void DownloadRedZones(string stateCountry)
        {
            if (HaventDownloaded)
            {
                redZones = await Queries.GetRedZonesForThisCode(stateCountry);
                FacebookUser fbUser;
                foreach (RedZone rz in redZones)
                {
                    fbUser = await Users.DownloadUserInfo(rz.FacebookID);
                    rz.FbUser = fbUser;
                    rz.Description = "\"" + rz.Description + "\"";
                    rz.RegisteredAt = rz.RegisteredAt.ToLocalTime();
                    RedZones.Add(rz);
                }
                HaventDownloaded = false;
            }
        }

        public ICommand UploadRedZone
        {
            get { return new MvxCommand(() => ShowViewModel<UploadRedZoneViewModel>()); }
        }

        public ICommand GetDirections
        {
            get { return new MvxCommand(() => ShowViewModel<GetDirectionsViewModel>()); }
        }

        void RedZones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => RedZones);
        }
    }
}
