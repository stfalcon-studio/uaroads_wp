using System;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Caliburn.Micro;
using Eve.Core.UI;
using Microsoft.ApplicationInsights;
using UR.Core.WP81.API;
using UR.Core.WP81.Services;
using UR.WP81.Common;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class LoginPageViewModel : AppBasePageViewModel
    {
        public string DeviceName { get; set; }

        public string Login { get; set; }

        public bool BackOnSuccessfulLogin { get; set; }

        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            try
            {
                var t = new EasClientDeviceInformation();
                DeviceName = t.FriendlyName; //"Microsoft Virtual"
            }
            catch (Exception)
            {
            }
        }


        public async void LoginButton()
        {
            //if (string.IsNullOrEmpty())
            //{
            //    NotOk();
            //    return;
            //}

            if (string.IsNullOrEmpty(Login) || !Login.Contains("@") || !Login.Contains("."))
            {
                NotOk();
                return;
            }

            IsBusyScreen = true;

            try
            {
                Login = Login.ToLowerInvariant();

                var guid = Guid.NewGuid().ToString("N");

                var res = await ApiClient.Create().RegisterDevice(Login, DeviceName, guid);

                if (res.IsSuccess)
                {
                    SettingsService.DeviceId = guid;
                    SettingsService.UserEmail = Login;
                    SettingsService.DeviceName = DeviceName;

                    new TelemetryClient().TrackEvent("DEVICEADDED");

                    if (BackOnSuccessfulLogin)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {
                        NavigationService.ToMainPage();
                    }
                }
                else
                {
                    await MessageDialogExt.ShowAsync("навдалий запит, спробуйте потім ще раз");
                }


                //if (!StateService.Instance.DeviceIsRegistred)
                //{
                //    //var guid = Guid.NewGuid().ToString("N");
                //    var guid = "91b24068cfe342748410f7836825fb1f";



                //    if (ApiResponseProcessor.Process(res))
                //    {
                //        SettingsService.DeviceId = guid;

                //        NavigationService.NavigateToViewModel<MainPageViewModel>();
                //    }

                //}
                //else
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

                IsBusyScreen = false;
            }
        }

        private void NotOk()
        {
            MessageDialogExt.ShowAsync("перевірте формат email'у");
        }
    }
}
