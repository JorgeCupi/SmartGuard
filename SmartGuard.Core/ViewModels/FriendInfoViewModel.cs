using Cirrious.MvvmCross.ViewModels;
using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SmartGuard.Core.ViewModels
{
    public partial class FriendInfoViewModel : MvxViewModel
    {
        private Friend selectedFriend;

        public Friend SelectedFriend
        {
            get { return selectedFriend; }
            set
            {
                selectedFriend = value;
                RaisePropertyChanged(() => SelectedFriend);
                StaticClasses.StaticFriend = SelectedFriend;
            }
        }

        private int positionsToBeDownloaded;

        public int PositionsToBeDownloaded
        {
            get { return positionsToBeDownloaded; }
            set { positionsToBeDownloaded = value; RaisePropertyChanged(() => PositionsToBeDownloaded); }
        }

        private Position selectedPosition;

        public Position SelectedPosition
        {
            get { return selectedPosition; }
            set
            {
                selectedPosition = value;
                RaisePropertyChanged(() => SelectedPosition);
                StaticClasses.StaticPosition = SelectedPosition;
            }
        }

        private bool downloadedError;

        public bool DownloadedError
        {
            get { return downloadedError; }
            set { downloadedError = value; RaisePropertyChanged(() => DownloadedError); }
        }

        private bool haventDownloaded;

        public bool HaventDownloaded
        {
            get { return haventDownloaded; }
            set { haventDownloaded = value; RaisePropertyChanged(() => HaventDownloaded); }
        }

        public ObservableCollection<Position> Positions { get; set; }

        private Friend friend;

        public Friend Friend
        {
            get { return friend; }
            set { friend = value; RaisePropertyChanged(() => Friend); }
        }

        public FriendInfoViewModel()
        {
            Friend = StaticClasses.StaticFriend;
            Positions = new ObservableCollection<Position>();
            Positions.CollectionChanged += Positions_CollectionChanged;
            DownloadedError = false;

            PositionsToBeDownloaded = 35;
        }

        private void Positions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Positions);
        }

        public async void DownloadLastPositions(int max)
        {
            PositionsToBeDownloaded = max;
            Positions.Clear();
            DateTime TimeDifference = DateTime.UtcNow.AddHours(-24);
            if (max == 250)
                TimeDifference = DateTime.UtcNow.AddDays(-7);
            if (max == 1000)
                TimeDifference = DateTime.UtcNow.AddMonths(-1);

            int count = 1;
            HaventDownloaded = true;
            try
            {
                List<Position> temp = await Queries.GetPositionsForThisCode(Friend.FacebookID, max);
                IEnumerable<Position> datedPositions = from P in temp
                                                       where P.RegisteredAt.Date >= TimeDifference.Date
                                                       select P;
                temp = datedPositions.ToList();
                foreach (Position Pos in temp)
                {
                    Pos.Counter = count;
                    Pos.LatLon = Pos.Latitude + "X" + Pos.Longitude;
                    Pos.RegisteredAt = Pos.RegisteredAt.ToLocalTime();
                    count++;
                    Positions.Add(Pos);
                }
            }
            catch (Exception)
            {
                DownloadedError = true;
            }

            HaventDownloaded = false;
            if (Positions.Count > 0)
                DownloadedError = false;
            else DownloadedError = true;
        }

        public ICommand GetDirections
        {
            get { return new MvxCommand(() => ShowViewModel<GetDirectionsViewModel>()); }
        }
    }
}