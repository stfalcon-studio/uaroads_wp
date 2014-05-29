using System.Windows.Input;
using System.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace UaRoadsWP.Models
{
    public class UaRoadsViewModel : ViewModelBase
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }


        public ICommand NavigateToAllTacksPageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //todo
                });
            }
        }
    }
}