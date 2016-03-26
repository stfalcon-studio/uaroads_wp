using Caliburn.Micro;
using Eve.Caliburn;
using UR.WP81.ViewModels;

namespace UR.WP81.Common
{
    public static class AppNavigationExtensions
    {
        public static void ToLoginPage(this Caliburn.Micro.INavigationService ns, bool backOnSucsseful = false)
        {
            ns.UriFor<LoginPageViewModel>().WithParam(x => x.BackOnSuccessfulLogin, backOnSucsseful).Navigate();
        }

        public static void ToSettingsPage(this Caliburn.Micro.INavigationService ns)
        {
            ns.UriFor<SettingsPageViewModel>().Navigate();
        }

      
        public static void ToMainPage(this Caliburn.Micro.INavigationService ns)
        {
            ns.CleanNavigationStackAfter().UriFor<MainPageViewModel>().Navigate();
        }
    }

}
