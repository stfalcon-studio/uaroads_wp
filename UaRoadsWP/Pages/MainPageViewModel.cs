using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UaRoadsWP.Models;

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
            StartCommand = new RelayCommand(() =>
            {
                IsRecordStarted = true;
                RaisePropertyChanged(() => IsRecordStarted);
                UpdateState();
            }, () =>
            {
                return !IsRecordStarted;
            });

            StopCommand = new RelayCommand(() =>
            {
                IsRecordStarted = false;
                RaisePropertyChanged(() => IsRecordStarted);
                UpdateState();
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

        public bool IsRecordStarted { get; set; }
        public string GpsStatus { get; set; }

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
    }
}
