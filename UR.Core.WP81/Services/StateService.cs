
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.System.Display;
using Caliburn.Micro;
using UR.Core.WP81.Common;

namespace UR.Core.WP81.Services
{
    public class StateService : IHandle<MsgAppState>
    {
        private static StateService _context;

        private DisplayRequest _displayRequest;

        public static StateService Instance
        {
            get { return _context ?? (_context = new StateService()); }
        }

        private StateService()
        {

        }


        public ATrack CurrentTrack { get; set; }
        public bool DeviceIsRegistred
        {
            get
            {
                if (string.IsNullOrEmpty(SettingsService.DeviceId))
                {
                    return false;
                }
                return true;
            }
        }

        public EGlobalState AppState { get; set; }

        //public bool IsEmu { get; set; }

        public double? AccValue { get; set; }

        public PositionStatus? GeoStatus { get; set; }

        public void UpdateAppState(EGlobalState newState)
        {
            AppState = newState;
            IoC.Get<IEventAggregator>().PublishOnUIThread(new MsgAppState(AppState));
        }

        public void Init()
        {
            IoC.Get<IEventAggregator>().Subscribe(this);

            if (AppState == EGlobalState.Unready)
            {
                //AppState = EGlobalState.Normal;
                MsgAppState.Publish(EGlobalState.Normal);
                //IoC.Get<IEventAggregator>().PublishOnUIThread(new MsgAppState(AppState));
            }
        }

        public void Handle(MsgAppState message)
        {
            AppState = message.State;
        }
    }
}
