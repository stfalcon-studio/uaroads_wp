using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using UaRoadsWP.Pages;

namespace UaRoadsWP.ViewModel
{

    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
            SimpleIoc.Default.Register<StartPageViewModel>();

        }

        public LoginPageViewModel LoginPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginPageViewModel>();
            }
        }

        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }

        public SettingsPageViewModel SettingsPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsPageViewModel>();
            }
        }
        public StartPageViewModel StartPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartPageViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}