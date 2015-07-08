using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using UR.Core.WP81.API.Models;
using UR.Core.WP81.DataRecorders;
using UR.Core.WP81.Models;
using UR.Core.WP81.Services;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class TrackListPageViewModel : AppBasePageViewModel//, IHandle<DataHandlerStatusChanged>
    {
        public ObservableCollection<FileTrackHeader> TrackList { get; set; }

        public TrackListPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            TrackList = new ObservableCollection<FileTrackHeader>();
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            IoC.Get<IEventAggregator>().Subscribe(this);

            Load();
        }



        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            IoC.Get<IEventAggregator>().Unsubscribe(this);
        }


        private async void Load()
        {
            IsBusyScreen = true;
            var tracks = await new TracksProvider().TracksAsync();
            IsBusyScreen = false;

            TrackList.Clear();

            foreach (var track in tracks)
            {
                TrackList.Add(track);
            }
        }


        public async void ProcessTracks()
        {
            var processor = new TrackPreprocessing();

            foreach (var track in TrackList)
            {
                await processor.ProcessAsync(track.TrackId);
            }

            Load();
        }
        public async void SendTracks()
        {
            var processor = new TrackSender();

            foreach (var track in TrackList)
            {
                await processor.SendAsync(track.TrackId);
            }

            Load();
        }
    }
}
