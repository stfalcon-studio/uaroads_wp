using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{
    public class TracksProvider
    {
        private const string DataDirectoryPath = "Routes_raw";

        private const string TrackHeaderFileName = "route.json";

        private const string GpsTrackExtension = ".gps";
        private const string AccelerometerTrackExtension = ".acc";

        private async Task<StorageFolder> GetDataFolderAsync()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            var res = await folder.CreateFolderAsync(DataDirectoryPath, CreationCollisionOption.OpenIfExists);

            return res;
        }

        private async Task<StorageFolder> GetTrackFolderAsync(Guid id)
        {
            var folder = await GetDataFolderAsync();

            var res = await folder.CreateFolderAsync(FileTrackHeader.GetName(id), CreationCollisionOption.OpenIfExists);

            return res;
        }

        public async Task<List<FileTrackHeader>> TracksAsync()
        {
            List<FileTrackHeader> resultList = new List<FileTrackHeader>(1);

            var folder = await GetDataFolderAsync();

            var childrenFolders = await folder.GetFoldersAsync();

            if (childrenFolders != null)
            {
                resultList = new List<FileTrackHeader>(childrenFolders.Count);

                Guid g;

                foreach (var cf in childrenFolders)
                {
                    if (Guid.TryParse(cf.Name, out g))
                    {
                        try
                        {
                            var file = await cf.GetFileAsync(TrackHeaderFileName);
                            //var contents = await file.OpenReadAsync();

                            using (var sr = new StreamReader((await file.OpenReadAsync()).AsStreamForRead()))
                            {
                                var content = await sr.ReadToEndAsync();

                                var obj = JsonConvert.DeserializeObject<FileTrackHeader>(content);

                                resultList.Add(obj);
                            }
                        }
                        catch (Exception err)
                        {
                            Debug.WriteLine("DATA FILE FOR {0} TRACK NOT FOUND OR CORREPTED --\r\n {1}", cf.Name, err.Message);
                        }
                    }
                }
            }

            return resultList;
        }

        public async Task SaveAsync(FileTrackHeader track)
        {
            //var folder = await GetDataFolder();

            var trackFolder = await GetTrackFolderAsync(track.TrackId);

            var strValue = JsonConvert.SerializeObject(track, Formatting.Indented);

            var targetFile = await trackFolder.CreateFileAsync(TrackHeaderFileName, CreationCollisionOption.ReplaceExisting);

            using (var sw = new StreamWriter(await targetFile.OpenStreamForWriteAsync()))
            {
                await sw.WriteAsync(strValue);
            }
        }

        public async Task<FileTrackHeader> TrackAsync(Guid id)
        {
            var trackFolder = await GetTrackFolderAsync(id);

            //var strValue = JsonConvert.SerializeObject(track, Formatting.Indented);

            var targetFile = await trackFolder.CreateFileAsync(FileTrackHeader.GetName(id), CreationCollisionOption.ReplaceExisting);

            using (var sw = new StreamReader(await targetFile.OpenStreamForReadAsync()))
            {
                var content = await sw.ReadToEndAsync();
                if (string.IsNullOrEmpty(content))
                {
                    var track = new FileTrackHeader()
                    {
                        TrackId = id
                    };

                    await SaveAsync(track);

                    return track;
                }
                else
                {
                    try
                    {
                        var res = JsonConvert.DeserializeObject<FileTrackHeader>(content);
                        return res;
                    }
                    catch (Exception err)
                    {
                        Debug.WriteLine("DATA FILE FOR {0} TRACK NOT EXISTS OR CORRUPTED --\r\n {1}", FileTrackHeader.GetName(id), err.Message);

                        return new FileTrackHeader()
                        {
                            TrackId = id
                        };
                    }
                }
            }
        }


        public async Task WriteAsync(List<DataGeo> geo)
        {
            using (var sw = await GetOutputStreamAsync(true, StateService.Instance.CurrentTrack.TrackId))
            {
                var sb = new StringBuilder();

                foreach (var point in geo)
                {
                    //“1402477210710;47.9464569091797;37.6872901916504;cp#”
                    sb.AppendFormat("{0};{1};{2};cp#",
                        point.TimeOffset.Ticks.ToString(CultureInfo.InvariantCulture),
                        point.Latitude.ToString(CultureInfo.InvariantCulture),
                        point.Longitude.ToString(CultureInfo.InvariantCulture));
                }

                await sw.WriteAsync(sb.ToString());

                await sw.FlushAsync();
            }
        }

        public async Task WriteAsync(List<DataAccelerometer> accs)
        {
            using (var sw = await GetOutputStreamAsync(false, StateService.Instance.CurrentTrack.TrackId))
            {
                var sb = new StringBuilder();

                foreach (var point in accs)
                {
                    //1402477210711;0.99;origin#
                    sb.AppendFormat("{0};{1};origin#",
                        point.TimeOffset.Ticks.ToString(CultureInfo.InvariantCulture),
                        point.Value.ToString(CultureInfo.InvariantCulture));
                }

                await sw.WriteAsync(sb.ToString());

                await sw.FlushAsync();
            }
        }


        private async Task<StreamWriter> GetOutputStreamAsync(bool isForGpsTrack, Guid trackId)
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            var targetFile = await trackFolder.CreateFileAsync(GetFileName(Guid.NewGuid(), isForGpsTrack), CreationCollisionOption.ReplaceExisting);

            var sw = new StreamWriter(await targetFile.OpenStreamForWriteAsync());

            return sw;
        }

        private string GetFileName(Guid id, bool isForGpsTrack)
        {
            return String.Format("{0}{1}", FileTrackHeader.GetName(id),
                isForGpsTrack ? GpsTrackExtension : AccelerometerTrackExtension);
        }
    }
}
