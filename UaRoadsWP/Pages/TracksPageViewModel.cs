using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Command;
using UaRoadsWP.Models;
using UaRoadsWP.Models.Db;
using UaRoadsWP.Services;

namespace UaRoadsWP.Pages
{
    public class TracksPageViewModel : UaRoadsViewModel
    {
        public ObservableCollection<DbTrack> Tracks { get; set; }

        public ICommand ProcessTrackCommand { get; set; }

        public bool CanUpload { get; set; }

        public TracksPageViewModel()
        {
            Tracks = new ObservableCollection<DbTrack>();


            Tracks.CollectionChanged += TracksOnCollectionChanged;

            ProcessTrackCommand = new RelayCommand(async () =>
            {

                if (IsBusy) return;
                IsBusy = true;

                if (Tracks.Any())
                {
                    var tProc = new TrackRawProcessing();

                    var track = Tracks.First();

                    await tProc.ProcessTrack(track.Id);
                }

                IsBusy = false;
            });
        }

        private void TracksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var collection = (ObservableCollection<DbTrack>)sender;

            var pre = CanUpload;

            CanUpload = collection.Any(x => x.TrackStatus == ETrackStatus.Finished || x.TrackStatus == ETrackStatus.ReadyToSend);

            if (pre != CanUpload)
            {
                RaisePropertyChanged(() => CanUpload);
            }
        }

        public void OnLoaded()
        {
            Load();


        }


        public async void Load()
        {
            if (IsBusy) return;

            IsBusy = true;
            Tracks = new ObservableCollection<DbTrack>();

            var allTracks = await new DbStorageService().TracksGet();

            foreach (var allTrack in allTracks)
            {
                Tracks.Add(allTrack);
            }
            IsBusy = false;
        }


        public async void ClearAll()
        {
            if (IsBusy) return;

            IsBusy = true;

            foreach (var track in Tracks)
            {
                await new DbStorageService().TrackDeleteFull(track.Id);
            }

            Tracks.Clear();

            IsBusy = false;
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            OnLoaded();
        }
    }
}
