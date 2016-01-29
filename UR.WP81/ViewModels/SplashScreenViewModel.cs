using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Eve.Caliburn;
using UR.Core.WP81.API;
using UR.Core.WP81.Services;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class SplashScreenPageViewModel : AppBasePageViewModel
    {
        public SplashScreenPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        protected async override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            StateService.Instance.Init();

            //if (!StateService.Instance.DeviceIsRegistred)
            //{
            //    //var guid = Guid.NewGuid().ToString("N");
            //    var guid = "91b24068cfe342748410f7836825fb1f";

            //var res = await ApiClient.Create().RegisterDevice("bondarenkod@windowslive.com", "phone", "wp81", guid);

            //    if (ApiResponseProcessor.Process(res))
            //    {
            //        SettingsService.DeviceId = guid;

            //        NavigationService.NavigateToViewModel<MainPageViewModel>();
            //    }

            //}
            //else
            //{
            NavigationService.CleanNavigationStackAfter().NavigateToViewModel<MainPageViewModel>();
            //}
        }
    }
}
