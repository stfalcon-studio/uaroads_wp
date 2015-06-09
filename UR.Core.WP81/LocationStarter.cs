using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UR.Core.WP81
{
    //public class LocationStarter
    //{
    //    public Task<GeoPositionStatus> PreheatService()
    //    {
    //        var tcs = new TaskCompletionSource<GeoPositionStatus>();

    //        Task.Factory.StartNew(() =>
    //        {
    //            try
    //            {
    //                var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

    //                var rezult = watcher.TryStart(true, TimeSpan.FromSeconds(30));

    //                tcs.SetResult(watcher.Status);

    //                watcher.Stop();

    //                Debug.WriteLine(rezult);
    //            }
    //            catch (Exception err)
    //            {
    //                tcs.SetResult(GeoPositionStatus.NoData);
    //            }
    //        });

    //        return tcs.Task;
    //    }
    //}
}
