using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Newtonsoft.Json;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{
    public class TracksProvider
    {
        private const string DataDirectoryPath = "routes";

        private const string TrackHeaderFileName = "route.json";

        private const string GpsTrackFileExtension = "gps";
        private const string AccelerometerTrackFileExtension = "acc";
        private const string TrackDataResultFileExtension = "txt";


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

        public async Task SaveTrackAsync(FileTrackHeader track)
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

        public async Task<FileTrackHeader> GetTrackAsync(Guid id)
        {
            var trackFolder = await GetTrackFolderAsync(id);

            //var strValue = JsonConvert.SerializeObject(track, Formatting.Indented);

            var targetFile = await trackFolder.CreateFileAsync(TrackHeaderFileName, CreationCollisionOption.OpenIfExists);

            using (var sw = new StreamReader(await targetFile.OpenStreamForReadAsync()))
            {
                var content = await sw.ReadToEndAsync();
                if (string.IsNullOrEmpty(content))
                {
                    var track = new FileTrackHeader()
                    {
                        TrackId = id
                    };

                    await SaveTrackAsync(track);

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


        /// <summary>
        /// return output file for track data
        /// </summary>
        /// <param name="trackId"></param>
        /// <param name="open">3</param>
        /// <returns></returns>
        public async Task<StorageFile> GetTrackDataFile(Guid trackId)
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            var fileName = GetFileName(trackId, FileType.Output);

            return await trackFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }


        public async Task<string> GetTrackDataAsync(Guid trackId)
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            var fileName = GetFileName(trackId, FileType.Output);

            var file = await trackFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            using (var read = await file.OpenStreamForReadAsync())
            {
                using (var sr = new StreamReader(read))
                {
                    var str = await sr.ReadToEndAsync();

                    return str;
                }
            }
        }


        /// <summary>
        /// write small portion of data to new file
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public async Task WriteToTrackAsync(List<DataGeo> geo, Guid trackId)
        {
            using (var sw = await GetWriteStreamAsync(FileType.GpsTrack, trackId))
            {
                var sb = new StringBuilder();

                foreach (var point in geo)
                {
                    //time;0;long;lat;cp;accuracy;velocity
                    sb.AppendFormat("{0};0;{1};{2};cp;{3};{4}\n",
                        point.TimeOffset.Ticks.ToString(CultureInfo.InvariantCulture),
                        point.Longitude.ToString(CultureInfo.InvariantCulture),
                        point.Latitude.ToString(CultureInfo.InvariantCulture),
                        0,
                        0);
                }

                await sw.WriteAsync(sb.ToString());

                await sw.FlushAsync();
            }
        }

        /// <summary>
        /// write small portion of data to new file
        /// </summary>
        /// <param name="accs"></param>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public async Task WriteToTrackAsync(List<DataAccelerometer> accs, Guid trackId)
        {
            using (var sw = await GetWriteStreamAsync(FileType.AccTrack, trackId))
            {
                var sb = new StringBuilder();

                foreach (var point in accs)
                {
                    //time;pit;0;0;cp;0;0
                    sb.AppendFormat("{0};{1};0;0;origin;0;0\n",
                        point.TimeOffset.Ticks.ToString(CultureInfo.InvariantCulture),
                        point.Value.ToString(CultureInfo.InvariantCulture));
                }

                await sw.WriteAsync(sb.ToString());

                await sw.FlushAsync();
            }
        }


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

        private async Task<StreamWriter> GetWriteStreamAsync(FileType fType, Guid trackId)//bool isForGpsTrack
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            // new guid for gps/acc files
            // track id for track result file
            var fileName = GetFileName(fType == FileType.Output ? trackId : Guid.NewGuid(), fType);

            // multilply acc\gps files, only one for result
            var targetFile = await trackFolder.CreateFileAsync(fileName, fType == FileType.Output ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);

            var sw = new StreamWriter(await targetFile.OpenStreamForWriteAsync());

            return sw;
        }


        public async Task<StorageFile[]> GetDataFilesAsync(Guid trackId)
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            var resFilesRaw = await trackFolder.GetFilesAsync();

            var files = resFilesRaw.Where(
                x => x.Name.EndsWith(GpsTrackFileExtension) || x.Name.EndsWith(AccelerometerTrackFileExtension));

            return files.ToArray();
        }

        private string GetFileName(Guid id, FileType fType)
        {
            var ext = string.Empty;

            switch (fType)
            {
                case FileType.GpsTrack:
                    ext = GpsTrackFileExtension;
                    break;
                case FileType.AccTrack:
                    ext = AccelerometerTrackFileExtension;
                    break;
                case FileType.Output:
                    ext = TrackDataResultFileExtension;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fType");
            }

            return String.Format("{0}.{1}", FileTrackHeader.GetName(id), ext);
        }

        private enum FileType
        {
            GpsTrack,
            AccTrack,
            Output
        }
    }
}
