using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
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
            if (!StateService.Instance.DeviceIsRegistred)
            {
                var guid = Guid.NewGuid().ToString("N");

                var res = await ApiClient.Create().RegisterDevice("bondarenkod@windowslive.com", "phone", "wp81", guid);

                SettingsService.DeviceId = guid;

                if (ApiResponseProcessor.Process(res))
                {
                    NavigationService.NavigateToViewModel<MainPageViewModel>();
                }

            }
            else
            {
                NavigationService.NavigateToViewModel<MainPageViewModel>();
            }
        }
    }
}
