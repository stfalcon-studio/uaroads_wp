using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.SecureElement;

namespace UaRoadsWP.Services
{
    public class LocationStarter
    {
        public Task<GeoPositionStatus> PreheatService()
        {
            var tcs = new TaskCompletionSource<GeoPositionStatus>();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

                    var rezult = watcher.TryStart(true, TimeSpan.FromSeconds(30));

                    tcs.SetResult(watcher.Status);

                    watcher.Stop();

                    Debug.WriteLine(rezult);
                }
                catch (Exception err)
                {
                    tcs.SetResult(GeoPositionStatus.NoData);
                }
            });

            return tcs.Task;
        }
    }
}
