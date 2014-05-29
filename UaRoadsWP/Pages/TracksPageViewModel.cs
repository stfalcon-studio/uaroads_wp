using System.Collections.ObjectModel;
using UaRoadsWP.Models;
using UaRoadsWP.Models.Db;
using UaRoadsWP.Services;

namespace UaRoadsWP.Pages
{
    public class TracksPageViewModel : UaRoadsViewModel
    {
        public ObservableCollection<DbTrack> Tracks { get; set; }

        public TracksPageViewModel()
        {
            Tracks = new ObservableCollection<DbTrack>();
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
