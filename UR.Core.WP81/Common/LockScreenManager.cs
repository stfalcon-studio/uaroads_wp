using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Display;
using Caliburn.Micro;
using Microsoft.ApplicationInsights;
using UR.Core.WP81.Services;

namespace UR.Core.WP81.Common
{
    public interface ILockScreenManager
    {
        bool TrySetLockScreenState(bool @lock);
    }

    public class LockScreenManager : ILockScreenManager
    {
        private DisplayRequest _displayRequest;


        public LockScreenManager()
        {
            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        public bool TrySetLockScreenState(bool @lock)
        {
            if (_displayRequest == null)
            {
                _displayRequest = new DisplayRequest();
            }

            try
            {
                if (!StateService.Instance.IsEmu)
                {
                    if (@lock)
                    {
                        _displayRequest.RequestActive();
                    }
                    else
                    {
                        _displayRequest.RequestRelease();
                    }

                    return true;
                }
            }
            catch (Exception err)
            {
                new TelemetryClient().TrackException(err);
            }

            return false;
        }
    }
}
