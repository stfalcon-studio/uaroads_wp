using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.Storage;
using UaRoadsWP.Models.Db;

namespace UaRoadsWP.Services
{
    internal class TrackRawProcessing
    {
        public TrackRawProcessing()
        {

        }




        public Task<bool> ProcessTrack(short trackId)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => TrackProccessInternal(tcs, trackId));
            return tcs.Task;
        }

        private async void TrackProccessInternal(TaskCompletionSource<bool> tcs, short trackId)
        {
            var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(0));

            var track = await new DbStorageService().GetTrack(trackId);

            var trackLoc = await new DbStorageService().GetTrackLocations(trackId);

            var trackPit = await new DbStorageService().GetTrackPits(trackId);

            var dic = new SortedDictionary<double, object>();

            foreach (var loc in trackLoc)
            {
                var time = Math.Floor((loc.TimeStamp - origin).TotalMilliseconds);

                if (!dic.ContainsKey(time))
                {
                    dic.Add(time, loc);
                }
            }

            foreach (var dbTrackPit in trackPit)
            {
                var time = Math.Floor((dbTrackPit.TimeStamp - origin).TotalMilliseconds);

                if (!dic.ContainsKey(time))
                {
                    dic.Add(time, dbTrackPit);
                }
                else
                {
                    var cVal = dic[time];
                    if (cVal as DbTrackLocation != null)
                    {
                        ((DbTrackLocation)cVal).PitRef = dbTrackPit;
                    }
                }
            }

            trackLoc = null;

            trackPit = null;

            var targetFile = await FileService.GetFileForWrite(track.TrackId.ToString());

            using (var fileStream = await targetFile.OpenStreamForWriteAsync())
            {
                using (var compressionStream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    using (var sw = new StreamWriter(compressionStream, Encoding.Unicode))
                    {
                        foreach (var o in dic)
                        {
                            float pit = 0;
                            double lat = 0;
                            double lon = 0;

                            if (o.Value as DbTrackPit != null)
                            {
                                pit = ((DbTrackPit)o.Value).PitValue;
                            }

                            if (o.Value as DbTrackLocation != null)
                            {
                                var tl = (DbTrackLocation)o.Value;

                                lat = tl.Lattitude;
                                lon = tl.Longitude;

                                if (tl.PitRef != null)
                                {
                                    pit = tl.PitRef.PitValue;
                                }
                            }

                            sw.Write("{0};{1};{2};{3};new#", o.Key, pit, lat, lon);
                        }

                        dic = null;
                    }
                }
            }

            dic = null;

            targetFile = null;


            await ConvertZipToBase64(tcs, track.TrackId);


            await Test(tcs, track.TrackId);


            //tcs.SetResult(true);
        }

        private async Task ConvertZipToBase64(TaskCompletionSource<bool> tcs, Guid trackId)
        {
            var targetFile = await FileService.GetFileForRead(trackId.ToString());

            string text = "";

            byte[] fileBytes;
            using (var fileStream = await targetFile.OpenStreamForReadAsync())
            {
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);

                    fileBytes = ms.ToArray();
                }
            }

            var res = Convert.ToBase64String(fileBytes);

            var bytes = StringToAscii(res);

            using (var fileStream = await targetFile.OpenStreamForWriteAsync())
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }

            //tcs.SetResult(true);
        }

        private async Task Test(TaskCompletionSource<bool> tcs, Guid trackId)
        {
            var targetFile = await FileService.GetFileForRead(trackId.ToString());

            using (var fileStream = await targetFile.OpenStreamForReadAsync())
            {

                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);

                    var bytes = ms.ToArray();

                    var charArray = new char[bytes.Length];

                    for (int i = 0; i < bytes.Length; i++)
                    {
                        charArray[i] = (char)bytes[i];
                    }

                    var bytes2 = Convert.FromBase64CharArray(charArray, 0, charArray.Length);


                    using (var ms2 = new MemoryStream(bytes2, false))
                    {
                        using (var decompressed = new GZipStream(ms2, CompressionMode.Decompress))
                        using (var reader = new StreamReader(decompressed))
                        {
                            string text = reader.ReadToEnd();

                            Debug.WriteLine("ROUTE DATA \r\n{0}", text);
                        }
                    }
                }
            }

            tcs.SetResult(true);
        }

        public static byte[] StringToAscii(string s)
        {
            byte[] retval = new byte[s.Length];
            for (int ix = 0; ix < s.Length; ++ix)
            {
                char ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte)ch;
                else retval[ix] = (byte)'?';
            }
            return retval;
        }
    }
}
