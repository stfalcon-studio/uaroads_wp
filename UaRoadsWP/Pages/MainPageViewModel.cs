using System;
using System.Device.Location;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using UaRoadsWP.Models;
using UaRoadsWP.Models.Db;
using UaRoadsWP.Services;

namespace UaRoadsWP.Pages
{
    public class MainPageViewModel : UaRoadsViewModel
    {
        public MainPageViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            StartCommand = new RelayCommand(async () =>
            {
                if (IsBusy) return;
                IsBusy = true;


                var res = await new LocationStarter().PreheatService();

                if (res == GeoPositionStatus.Ready)
                {
                    Debug.WriteLine("START OK");
                }




                //var track = new DbTrack()
                //{
                //    TrackComment = "comment " + DateTime.Now.ToString("g"),
                //    TrackStatus = ETrackStatus.Started
                //};

                //await new DbStorageService().TrackInsertUpdate(track);
                //SettingsService.LastRecordedRoad = track.TrackId;
                //SettingsService.CurrentTrack = track.Id;
                //SimpleIoc.Default.GetInstance<AccelerometerRecordService>().Start();

                //SimpleIoc.Default.GetInstance<LocationRecordService>().Start();

                //IsRecordStarted = true;
                //RaisePropertyChanged(() => IsRecordStarted);
                UpdateState();
                IsBusy = false;
            }, () =>
            {
                return !IsRecordStarted;
            });

            StopCommand = new RelayCommand(async () =>
            {
                if (IsBusy) return;
                IsBusy = true;

                SimpleIoc.Default.GetInstance<AccelerometerRecordService>().Stop();

                SimpleIoc.Default.GetInstance<LocationRecordService>().Stop();

                if (SettingsService.CurrentTrack.HasValue)
                {
                    var track = await new DbStorageService().GetTrack(SettingsService.CurrentTrack.Value);
                    track.TrackStatus = ETrackStatus.Finished;
                    await new DbStorageService().TrackInsertUpdate(track);
                }

                IsRecordStarted = false;
                RaisePropertyChanged(() => IsRecordStarted);
                UpdateState();

                IsBusy = false;
            }, () =>
            {
                return IsRecordStarted;
            });
        }

        private void UpdateState()
        {
            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopCommand).RaiseCanExecuteChanged();
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        public bool IsRecordStarted { get; set; }
        public string GpsStatus { get; set; }

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
    }
}
