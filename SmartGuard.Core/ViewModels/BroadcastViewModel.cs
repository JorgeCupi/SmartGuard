using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Facebook.Authenticate;
using SmartGuard.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGuard.Core.ViewModels
{
    public class BroadcastViewModel:MvxViewModel
    {
        private bool haventUploaded;
        public bool HaventUploaded
        {
            get { return haventUploaded; }
            set { haventUploaded = value; RaisePropertyChanged(() => HaventUploaded); }
        }

        private bool haventDownloaded;
        public bool HaventDownloaded
        {
            get { return haventDownloaded; }
            set { haventDownloaded = value; RaisePropertyChanged(() => HaventDownloaded); }
        }

        public ObservableCollection<Position> Positions { get; set; }

        public BroadcastViewModel()
        {
            Positions = new ObservableCollection<Position>();
            Positions.CollectionChanged += Positions_CollectionChanged;

            TryDownloadMyPositions(50);
        }

        void Positions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Positions);
        }
        public async Task<bool> TryUploadPosition(Position position)
        {
            HaventUploaded = true;
            bool result = await Services.TryUploadPosition(position);
            HaventUploaded = false;
            
            return result;
        }

        public async Task<int> TryDownloadMyPositions(int max)
        {
            Positions.Clear();
            DateTime TimeDifference = DateTime.UtcNow;
            if (max == 250)
                TimeDifference = DateTime.UtcNow.AddDays(-7);
            if (max == 1000)
                TimeDifference = DateTime.UtcNow.AddMonths(-1);

            HaventDownloaded = true;
            List<Position> lstPositions = await Queries.GetPositionsForThisCode (Utilities.fbUserID, max);
            IEnumerable<Position> datedPositions = from P in lstPositions
                                                   where P.RegisteredAt.Date >= TimeDifference.Date
                                                   select P;
            lstPositions = datedPositions.ToList();
            HaventDownloaded = false;
            if (lstPositions != null)
            {
                if (lstPositions.Count == 0)
                    return 1;
                else
                {
                    int counter = 1;
                    foreach (Position P in lstPositions)
                    {
                        P.Counter = counter;
                        P.LatLon = P.Latitude + "X" + P.Longitude;
                        counter++;
                        Positions.Add(P);
                    }
                    
                    return 2;
                }
            }
            else return 0;
            
        }
    }
}
