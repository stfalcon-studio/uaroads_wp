﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.ApplicationInsights;
using UR.Core.WP81.Common;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{
    public class TrackProcessor
    {
        public async Task ProcessAsync(Guid trackId)
        {
            var tProvider = IoC.Get<ITracksProvider>();

            try
            {
                Debug.WriteLine("begin process track {0}", trackId);

                var track = await tProvider.GetTrackAsync(trackId);

                if (track.Status != ETrackStatus.Recorded)
                {
                    Debug.WriteLine("skip track {0} - status not recorded", trackId);
                    return;
                }

                track.Status = ETrackStatus.Processing;

                await tProvider.SaveTrackAsync(track);
                //todo send change status message

                var outputFile = await tProvider.GetTrackDataFile(trackId);

                //gzip & base64

                string tmp = "";

                using (var fileStream = await outputFile.OpenStreamForWriteAsync())
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var compressionStream = new GZipStream(ms, CompressionMode.Compress))
                        {
                            var dataFiles = await tProvider.GetDataFilesAsync(trackId);

                            foreach (var file in dataFiles)
                            {
                                Debug.WriteLine("read dataFile {0}", file.Name);
                                using (var dataFileStream = await file.OpenStreamForReadAsync())
                                {
                                    CopyStream(dataFileStream, compressionStream);
                                }
                            }
                        }

                        var array = ms.ToArray();

                        //hack to pass server header checking system :(
                        array[8] = 0;
                        //array[10] = 237;

                        //var msin = new MemoryStream(array);
                        //using (var decompress = new GZipStream(msin, CompressionMode.Decompress))
                        //{
                        //    var sr = new StreamReader(decompress);
                        //    var s = await sr.ReadToEndAsync();
                        //}

                        tmp = Convert.ToBase64String(array);
                    }

                    string delimeter = "\n";


                    if (tmp.Length > 0)
                    {
                        var insertCount = 0;
                        var lineCharsCount = 77;
                        var insertPos = lineCharsCount - 1;


                        var sb = new StringBuilder(tmp.Length + (tmp.Length / lineCharsCount) * 2 + 50);

                        sb.Insert(0, tmp);

                        tmp = null;

                        //var sbLength = sb.Length;

                        while (true)
                        {
                            sb.Insert(insertPos, delimeter);

                            insertPos += lineCharsCount;

                            insertCount++;

                            if (insertPos >= sb.Length) break;
                        }




                        //var resstr = sb.ToString();

                        using (var writer = new StreamWriter(fileStream))
                        {
                            await writer.WriteAsync(sb.ToString());
                        }
                    }
                }

                track.Status = ETrackStatus.Processed;

                await tProvider.SaveTrackAsync(track);

                Debug.WriteLine("end processing track {0}", trackId);

            }
            catch (Exception err)
            {
                new TelemetryClient().TrackException(err);
                var track = await tProvider.GetTrackAsync(trackId);
                track.Status = ETrackStatus.Recorded;
                await tProvider.SaveTrackAsync(track);
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


    //internal class TrackRawProcessing
    //{
    //    public TrackRawProcessing()
    //    {
    //        _origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(0));
    //    }

    //    private DateTimeOffset _origin;


    //    public Task<bool> ProcessTrack(short trackId)
    //    {
    //        var tcs = new TaskCompletionSource<bool>();
    //        Task.Factory.StartNew(() => TrackProccessInternal(tcs, trackId));
    //        return tcs.Task;
    //    }

    //    private async void TrackProccessInternal(TaskCompletionSource<bool> tcs, short trackId)
    //    {
    //        var track = await new DbStorageService().GetTrack(trackId);


    //        //await Test(tcs, track.TrackId);


    //        var trackLoc = await new DbStorageService().GetTrackLocations(trackId);

    //        var trackPit = await new DbStorageService().GetTrackPits(trackId);

    //        var list = new List<DbTrackBase>();

    //        list.AddRange(trackLoc);

    //        list.AddRange(trackPit);

    //        trackPit = null;
    //        trackLoc = null;


    //        var resList = list.OrderBy(x => x.TimeStamp);



    //        //var dic = new SortedDictionary<double, object>();

    //        //foreach (var loc in trackLoc)
    //        //{
    //        //    var time = Math.Floor((loc.TimeStamp - origin).TotalMilliseconds);

    //        //    if (!dic.ContainsKey(time))
    //        //    {
    //        //        dic.Add(time, loc);
    //        //    }
    //        //}

    //        //foreach (var dbTrackPit in trackPit)
    //        //{
    //        //    var time = Math.Floor((dbTrackPit.TimeStamp - origin).TotalMilliseconds);

    //        //    if (!dic.ContainsKey(time))
    //        //    {
    //        //        dic.Add(time, dbTrackPit);
    //        //    }
    //        //    else
    //        //    {
    //        //        var cVal = dic[time];
    //        //        if (cVal as DbTrackLocation != null)
    //        //        {
    //        //            ((DbTrackLocation)cVal).PitRef = dbTrackPit;
    //        //        }
    //        //    }
    //        //}

    //        //byte[] bytesToEncode = Encoding.UTF8.GetBytes("qqqqqqqqqqqqq");

    //        //var bytesToEncode = Encoding.UTF8.GetBytes("1");

    //        //string encodedText = Convert.ToBase64String(bytesToEncode);




    //        var targetFile = await FileService.GetFileForWrite(track.TrackId.ToString());

    //        using (var fileStream = await targetFile.OpenStreamForWriteAsync())
    //        {
    //            using (var compressionStream = new GZipStream(fileStream, CompressionMode.Compress))
    //            {
    //                using (var sw = new StreamWriter(compressionStream, Encoding.UTF8))
    //                {
    //                    //sw.Write("1");

    //                    foreach (var val in resList)
    //                    {
    //                        if (val.IsPit)
    //                        {//time;pit;lat;lng;type#time;pit;lat;lng;type#..... , де:
    //                            sw.Write("{0};{1};0;0;origin#", GetTime(val.TimeStamp), ((DbTrackPit)val).PitValue);
    //                        }
    //                        else
    //                        {
    //                            sw.Write("{0};0;{1};{2};cp#", GetTime(val.TimeStamp), ((DbTrackLocation)val).Lattitude, ((DbTrackLocation)val).Longitude);
    //                        }
    //                    }

    //                    //foreach (var o in dic)
    //                    //{
    //                    //    float pit = 0;
    //                    //    double lat = 0;
    //                    //    double lon = 0;

    //                    //    if (o.Value as DbTrackPit != null)
    //                    //    {
    //                    //        pit = ((DbTrackPit)o.Value).PitValue;
    //                    //    }

    //                    //    if (o.Value as DbTrackLocation != null)
    //                    //    {
    //                    //        var tl = (DbTrackLocation)o.Value;

    //                    //        lat = tl.Lattitude;
    //                    //        lon = tl.Longitude;

    //                    //        if (tl.PitRef != null)
    //                    //        {
    //                    //            pit = tl.PitRef.PitValue;
    //                    //        }
    //                    //    }

    //                    //    sw.Write("{0};{1};{2};{3};new#", o.Key, pit, lat, lon);
    //                    //}

    //                    //dic = null;

    //                    resList = null;
    //                }
    //            }
    //        }

    //        //dic = null;

    //        targetFile = null;


    //        await ConvertZipToBase64(tcs, track.TrackId);




    //        await SentTrack(track.TrackId);



    //        //tcs.SetResult(true);
    //    }

    //    private async Task SentTrack(Guid trackId)
    //    {
    //        var trackData = await ReadRouteData(trackId);


    //        var res = await new ApiClient().Add("___________________DEVICE_ID_", Guid.NewGuid(), trackData, Guid.NewGuid().ToString());
    //        ApiResponseProcessor.ProcessAsync(res);
    //    }

    //    private double GetTime(DateTimeOffset o)
    //    {
    //        return Math.Floor((o - _origin).TotalMilliseconds);
    //    }

    //    private async Task ConvertZipToBase64(TaskCompletionSource<bool> tcs, Guid trackId)
    //    {
    //        var targetFile = await FileService.GetFileForRead(trackId.ToString());

    //        string text = "";

    //        byte[] fileBytes;
    //        using (var fileStream = await targetFile.OpenStreamForReadAsync())
    //        {
    //            using (var ms = new MemoryStream())
    //            {
    //                fileStream.CopyTo(ms);

    //                fileBytes = ms.ToArray();
    //            }
    //        }

    //        var res = Convert.ToBase64String(fileBytes);

    //        //var bytes = StringToAscii(res);


    //        var bytes = Encoding.UTF8.GetBytes(res);

    //        using (var fileStream = await targetFile.OpenStreamForWriteAsync())
    //        {
    //            fileStream.Write(bytes, 0, bytes.Length);
    //        }

    //        //tcs.SetResult(true);
    //    }

    //    private async Task Test(TaskCompletionSource<bool> tcs, Guid trackId)
    //    {
    //        var targetFile = await FileService.GetFileForRead(trackId.ToString());

    //        using (var fileStream = await targetFile.OpenStreamForReadAsync())
    //        {

    //            using (var ms = new MemoryStream())
    //            {
    //                fileStream.CopyTo(ms);

    //                var bytes = ms.ToArray();

    //                var charArray = new char[bytes.Length];

    //                for (int i = 0; i < bytes.Length; i++)
    //                {
    //                    charArray[i] = (char)bytes[i];
    //                }

    //                var bytes2 = Convert.FromBase64CharArray(charArray, 0, charArray.Length);


    //                using (var ms2 = new MemoryStream(bytes2, false))
    //                {
    //                    using (var decompressed = new GZipStream(ms2, CompressionMode.Decompress))
    //                    using (var reader = new StreamReader(decompressed, Encoding.UTF8))
    //                    {
    //                        string text = reader.ReadToEnd();

    //                        Debug.WriteLine("ROUTE DATA \r\n{0}", text);
    //                    }
    //                }
    //            }
    //        }

    //        tcs.SetResult(true);
    //    }

    //    private async Task<string> ReadRouteData(Guid trackId)
    //    {
    //        var targetFile = await FileService.GetFileForRead(trackId.ToString());

    //        using (var fileStream = await targetFile.OpenStreamForReadAsync())
    //        {
    //            using (var sr = new StreamReader(fileStream, Encoding.UTF8))
    //            {
    //                return await sr.ReadToEndAsync();
    //            }
    //        }
    //    }

    //    public static byte[] StringToAscii(string s)
    //    {
    //        byte[] retval = new byte[s.Length];
    //        for (int ix = 0; ix < s.Length; ++ix)
    //        {
    //            char ch = s[ix];
    //            if (ch <= 0x7f) retval[ix] = (byte)ch;
    //            else retval[ix] = (byte)'?';
    //        }
    //        return retval;
    //    }
    //}
}
