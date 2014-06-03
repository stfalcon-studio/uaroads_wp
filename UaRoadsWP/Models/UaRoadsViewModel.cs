using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;

namespace UaRoadsWP.Models
{
    public class UaRoadsViewModel : BaseViewModel
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


    public class BaseViewModel : ViewModelBase
    {
        public PhoneApplicationFrame RootFrame
        {
            get { return (Microsoft.Phone.Controls.PhoneApplicationFrame)Application.Current.RootVisual; }
        }

        public PhoneApplicationPage RootPage
        {
            get { return (PhoneApplicationPage)((Microsoft.Phone.Controls.PhoneApplicationFrame)Application.Current.RootVisual).Content; }
        }

        public NavigationService NavigationService
        {
            get { return RootPage.NavigationService; }
        }
    }
}