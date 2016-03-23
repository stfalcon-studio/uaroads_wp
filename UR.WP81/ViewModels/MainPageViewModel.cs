using System;
using Windows.UI.Xaml;
using Caliburn.Micro;
using UR.Core.WP81.Services;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class MainPageViewModel : AppBasePageViewModel
    {
        private DispatcherTimer _timer;

        public string TrackDuration { get; set; }

        public string TrackLength { get; set; }

        public string Speed { get; set; }

        public string AccValue { get; set; }

        public string GeoStatus { get; set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            _timer.Tick += TimerOnTick;
        }


        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            UpdateState();

            if (_timer != null)
            {
                _timer.Start();
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        public async void RecordingStart()
        {
            //var r = new TestConverter();
            //await r.RunAsync();

            //return;

            //throw new ArrayTypeMismatchException("test");

            if (IsBusy) return;

            IsBusyStatusBar = true;
            await DataRecorder.GetInstance().StartAsync();
            IsBusyStatusBar = false;
        }

        public async void RecordingStop()
        {
            if (IsBusy) return;

            IsBusyStatusBar = true;
            await DataRecorder.GetInstance().StopAsync();
            IsBusyStatusBar = false;
        }

        //public void Handle(MsgDataHandlerStatusChanged message)
        //{
        //    IsStarted = message.IsStarted;
        //    NotifyOfPropertyChange(() => IsStarted);
        //}


        public void AppBtnTracksButton()
        {
            NavigationService.NavigateToViewModel<TrackListPageViewModel>();
        }

        private void TimerOnTick(object sender, object o)
        {
            var track = StateService.Instance.CurrentTrack;

            if (track != null)
            {
                TrackDuration = (DateTime.Now - track.StartedDateTime).ToString(@"hh\:mm\:ss");
                TrackLength = $"{(track.TrackLength / 1000).ToString("F2")}  km";
                Speed = track.CurrentSpeed.ToString("F1");
            }
            else
            {
                TrackDuration = String.Empty;
                TrackLength = String.Empty;
                Speed = String.Empty;
            }


            var val = StateService.Instance.AccValue;

            if (val.HasValue)
            {
                AccValue = val.Value.ToString("F2");
            }
            else
            {
                AccValue = string.Empty;
            }

            var geo = StateService.Instance.GeoStatus;

            if (geo != null)
            {
                GeoStatus = geo.Value.ToString();
            }
            else
            {
                GeoStatus = string.Empty;
            }


            NotifyOfPropertyChange(() => TrackLength);
            NotifyOfPropertyChange(() => TrackDuration);
            NotifyOfPropertyChange(() => Speed);
            NotifyOfPropertyChange(() => AccValue);
            NotifyOfPropertyChange(() => GeoStatus);
        }
    }
}
