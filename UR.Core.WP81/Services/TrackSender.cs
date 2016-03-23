using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.ApplicationInsights;
using UR.Core.WP81.API;
using UR.Core.WP81.Common;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{

    public class TrackSender
    {
        public async Task SendAsync(Guid trackId)
        {
            try
            {
                Debug.WriteLine("begin sending track {0}", trackId);
                var track = await IoC.Get<ITracksProvider>().GetTrackAsync(trackId);

                if (track.Status != ETrackStatus.Processed)
                {
                    Debug.WriteLine("skip track {0} - status not Processed", trackId);
                    return;
                }

                track.Status = ETrackStatus.Sending;

                await IoC.Get<ITracksProvider>().SaveTrackAsync(track);
                //todo send change status message

                var data = await IoC.Get<ITracksProvider>().GetTrackDataAsync(trackId);

                var res = await ApiClient.Create().Add(trackId, data, track.TrackName, track.Comment);

                if (ApiResponseProcessor.Process(res))
                {
                    //sent
                    track.Status = ETrackStatus.Sent;
                    await IoC.Get<ITracksProvider>().SaveTrackAsync(track);
                    Debug.WriteLine("end sending track {0}", trackId);
                }
            }
            catch (Exception err)
            {
                new TelemetryClient().TrackException(err);

                var track = await IoC.Get<ITracksProvider>().GetTrackAsync(trackId);
                track.Status = ETrackStatus.Processed;
                await IoC.Get<ITracksProvider>().SaveTrackAsync(track);
            }
        }


        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            long tempPos = input.Position;
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
            input.Position = tempPos;// or you make Position = 0 to set it at the start
        }
    }
}
