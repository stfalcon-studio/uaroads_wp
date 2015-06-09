using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using UR.Core.WP81.API.Models;
using UR.Core.WP81.Services;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class MainPageViewModel : AppBasePageViewModel, IHandle<DataHandlerStatusChanged>
    {
        public bool IsStarted { get; set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            IoC.Get<IEventAggregator>().Unsubscribe(this);
        }


        public async void StartCommand()
        {
            if (IsBusy) return;

            IsBusyStatusBar = true;
            await RecordService.GetInstance().StartAsync();
            IsBusyStatusBar = false;
        }

        public async void StopCommand()
        {
            if (IsBusy) return;

            IsBusyStatusBar = true;
            await RecordService.GetInstance().StopAsync();
            IsBusyStatusBar = false;
        }

        public void Handle(DataHandlerStatusChanged message)
        {
            IsStarted = message.IsStarted;
            NotifyOfPropertyChange(() => IsStarted);
        }


        public void ViewTrackListCommand()
        {
            NavigationService.NavigateToViewModel<TrackListPageViewModel>();
        }
    }
}
