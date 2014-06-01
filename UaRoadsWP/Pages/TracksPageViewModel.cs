using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
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

        public TracksPageViewModel()
        {
            Tracks = new ObservableCollection<DbTrack>();


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
    }
}
