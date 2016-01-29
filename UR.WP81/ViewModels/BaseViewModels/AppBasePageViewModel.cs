using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Windows.Graphics.Display;
using Windows.System;
using Windows.System.Display;
using Caliburn.Micro;
using Eve.Caliburn;
using Eve.Core.Helpers;
using UR.Core.WP81.Common;
using UR.Core.WP81.Services;
using UR.WP81.Common;

namespace UR.WP81.ViewModels.BaseViewModels
{
    public class AppBasePageViewModel : CanBusyViewModel, IHandle<MsgAppState>
    {
        protected override string StatusBarText()
        {
            return "Обробка...";
            //return base.StatusBarText();
        }

        public readonly INavigationService NavigationService;

        public string AppVersion
        {
            get { return Package.Current.Id.Version.GetAsString(); }
        }


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

        public bool StateIsNormal
        {
            get { return StateService.Instance.AppState == EGlobalState.Normal; }
        }

        public bool StateIsRecording
        {
            get { return StateService.Instance.AppState == EGlobalState.Recording; }
        }

        public bool StateIsPaused
        {
            get { return StateService.Instance.AppState == EGlobalState.Paused; }
        }

        public bool DeviceIsNotRegistred
        {
            get { return !StateService.Instance.DeviceIsRegistred; }
        }

        public void UpdateState()
        {
            NotifyOfPropertyChange(() => StateIsNormal);
            NotifyOfPropertyChange(() => StateIsRecording);
            NotifyOfPropertyChange(() => StateIsPaused);
        }


        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public void AppBtnLoginButton()
        {
            NavigationService.ToLoginPage(true);
        }

        public void AppBtnMenuLoginButton()
        {
            AppBtnLoginButton();
        }

        public void AppBtnMenuFeedback()
        {
            SendBugReportCommand();
        }

        public void AppBtnMenuSettingsButton()
        {
            NavigationService.ToSettingsPage();
        }

        public async void SendBugReportCommand()
        {
            EmailRecipient sendTo = new EmailRecipient
            {
                Name = "розробник додатку",
                Address = "bondarenkod+uaroads@windowslive.com"
            };

            EmailRecipient bcc = new EmailRecipient
            {
                Name = "stfalcon.com",
                Address = "uaroads@stfalcon.com"
            };

            EmailMessage mail = new EmailMessage
            {
                Subject = "Зворотній зв'язок UAROADS WP",
                Body = "\r\n\r\n\r\n\r\nappver:" + Package.Current.Id.Version.GetAsString()
            };

            mail.To.Add(sendTo);
            mail.Bcc.Add(bcc);

            // Open the share contract with Mail only:
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        public async void RateAppCommand()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }

        public void Handle(MsgAppState message)
        {
            StateService.Instance.TrySetLockScreenState(message.State != EGlobalState.Recording);

            UpdateState();
        }
    }
}
