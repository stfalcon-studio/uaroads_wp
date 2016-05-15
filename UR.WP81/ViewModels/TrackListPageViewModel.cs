using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.ApplicationInsights;
using UR.Core.WP81.Common;
using UR.Core.WP81.Models;
using UR.Core.WP81.Services;
using UR.WP81.Common;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class TrackListPageViewModel : AppBasePageViewModel//, IHandle<MsgDataHandlerStatusChanged>
    {
        public ObservableCollection<ATrack> TrackList { get; set; }

        public TrackListPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            TrackList = new ObservableCollection<ATrack>();
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
            IsBusyStatusBar = true;
            var tracks = await IoC.Get<ITracksProvider>().GetTracksAsync();
            IsBusyStatusBar = false;

            TrackList.Clear();

            foreach (var track in tracks)
            {
                TrackList.Add(track);
            }
        }

        public async void AppBtnProcess()
        {
            if (IsBusy) return;

            if (!StateService.Instance.DeviceIsRegistred)
            {
                NavigationService.ToLoginPage(true);
                return;
            }

            IsBusyStatusBar = true;

            try
            {
                foreach (var track in TrackList)
                {
                    await UploaTrackAsync(track);
                }
            }
            catch (Exception err)
            {
                new TelemetryClient().TrackException(err);
            }
            finally
            {
                Load();
                IsBusyStatusBar = false;
            }
        }

        private async Task UploaTrackAsync(ATrack track, bool resetTrackState = false)
        {
            await new TrackProcessor().ProcessAsync(track.Id, resetTrackState);
            await new TrackSender().SendAsync(track.Id);
            new TelemetryClient().TrackEvent("TRACKSENT", new Dictionary<string, string>
                    {
                        { "TrackID", track.Id.ToString("N") },
                        { "TrackLength", track.TrackLength.ToString() },
                        { "TrackAvgSpeed", track.TrackAvgSpeed.ToString() },
                        { "TrackPitPointsCount", track.PitPointsCount.ToString() },
                        { "TrackLocationPointsCount", track.LocationPointsCount.ToString() },
                    });
        }

        public async void ResendTrack(ATrack track)
        {
            if (IsBusy) return;

            if (!StateService.Instance.DeviceIsRegistred)
            {
                NavigationService.ToLoginPage(true);
                return;
            }

            IsBusyStatusBar = true;

            try
            {
                await UploaTrackAsync(track);
            }
            catch (Exception err)
            {
                new TelemetryClient().TrackException(err);
            }
            finally
            {
                Load();
                IsBusyStatusBar = false;
            }
        }


        public async void AppBtnMenuDeleteEverything()
        {
            if (IsBusy) return;

            IsBusyStatusBar = true;
            try
            {
                await IoC.Get<ITracksProvider>().DeleteTracksAsync();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Load();
                IsBusyStatusBar = false;
            }
        }
    }
}
