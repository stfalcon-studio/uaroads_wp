using System;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Caliburn.Micro;

namespace UR.WP81.ViewModels.BaseViewModels
{
    public class AppBasePageViewModel : Screen
    {
        public readonly INavigationService NavigationService;

        public AppBasePageViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;

            //NavigationService.BackStack.
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            IoC.Get<IEventAggregator>().Unsubscribe(this);
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            IoC.Get<IEventAggregator>().Subscribe(this);
        }


        protected override void OnActivate()
        {
            base.OnActivate();
        }

     

        #region Props

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        #endregion

        public async void SendBugReportCommand()
        {
            EmailRecipient sendTo = new EmailRecipient
            {
                Name = "Поддержка пользователей MEGOGO",
                Address = "support@megogo.net"
            };

            EmailMessage mail = new EmailMessage
            {
                Subject = "Отзыв о MEGOGO для Windows Phone 8.1",
                Body = ""
            };

            // Add recipients to the mail object
            mail.To.Add(sendTo);
            mail.Bcc.Add(new EmailRecipient
            {
                Name = "Поддержка DevRain Solutions",
                Address = "info@devrain.com"
            });

            // Open the share contract with Mail only:
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        public async void RateAppCommand()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }
    }
}
