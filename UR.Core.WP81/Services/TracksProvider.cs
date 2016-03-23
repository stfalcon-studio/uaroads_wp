using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using AutoMapper;
using Newtonsoft.Json;
using UR.Core.WP81.Common;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{
    public class TracksProvider : ITracksProvider
    {
        private readonly IDbTracksProvider _db;
        private readonly ITrackDataProvider _fData;

        public TracksProvider(IDbTracksProvider db, ITrackDataProvider fData)
        {
            _db = db;
            _fData = fData;

            Mapper.CreateMap<DbTrack, ATrack>();
        }

        public async Task<ATrack> CreateTrackAsync()
        {
            var track = new DbTrack();
            await _db.SaveTrackAsync(track);
            return Mapper.Map<ATrack>(track);
        }

        public async Task<ATrack> GetTrackAsync(Guid trackId)
        {
            var track = await _db.GetTrackAsync(trackId);

            return Mapper.Map<ATrack>(track);
        }

        public async Task SaveTrackAsync(ATrack track)
        {
            await _db.SaveTrackAsync(track);
        }

        public Task<StorageFile> GetTrackDataFile(Guid trackId)
        {
            return _fData.GetTrackDataFile(trackId);
        }

        public Task<StorageFile[]> GetDataFilesAsync(Guid trackId)
        {
            return _fData.GetDataFilesAsync(trackId);
        }

        public Task WriteToTrackAsync(List<DataAccelerometer> accs, Guid currentTrackId)
        {
            return _fData.WriteToTrackAsync(accs, currentTrackId);
        }

        public Task WriteToTrackAsync(List<DataGeo> geos, Guid currentTrackId)
        {
            return _fData.WriteToTrackAsync(geos, currentTrackId);
        }

        public Task<string> GetTrackDataAsync(Guid trackId)
        {
            return _fData.GetTrackDataAsync(trackId);
        }

        public async Task<List<ATrack>> GetTracksAsync()
        {
            var tracks = await _db.GetTracksAsync();

            return tracks.Select(Mapper.Map<ATrack>).ToList();
        }

        public async Task DeleteTracksAsync()
        {
            var tracks = await _db.GetTracksAsync();

            var dTracks = tracks.Where(x => x.Status != ETrackStatus.Recording || x.Status != ETrackStatus.RecordPaused).Select(x => x.Id).ToList();

            await _db.DeleteTracksAsync(dTracks);

            await _fData.DeleteTracksAsync(dTracks);
        }
    }

    enum FileType
    {
        GpsTrack,
        AccTrack,
        PackedData
    }

    public interface ITrackDataProvider
    {
        /// <summary>
        /// return output file for track data
        /// </summary>
        /// <param name="trackId"></param>
        /// <param name="open">3</param>
        /// <returns></returns>
        Task<StorageFile> GetTrackDataFile(Guid trackId);

        Task<string> GetTrackDataAsync(Guid trackId);

        /// <summary>
        /// write small portion of data to new file
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="trackId"></param>
        /// <returns></returns>
        Task WriteToTrackAsync(List<DataGeo> geo, Guid trackId);

        /// <summary>
        /// write small portion of data to new file
        /// </summary>
        /// <param name="accs"></param>
        /// <param name="trackId"></param>
        /// <returns></returns>
        Task WriteToTrackAsync(List<DataAccelerometer> accs, Guid trackId);

        Task DeleteTracksAsync(List<Guid> ids);
        Task DeleteTrackAsync(Guid trackId);
        Task<StorageFile[]> GetDataFilesAsync(Guid trackId);
    }

    public class TrackDataDataProvider : ITrackDataProvider
    {
        private const string DataDirectoryPath = "routes";

        private const string TrackHeaderFileName = "route.json";

        private const string GpsTrackFileExtension = "gps";
        private const string AccelerometerTrackFileExtension = "acc";
        private const string TrackDataResultFileExtension = "txt";


        /// <summary>
        /// return output file for track data
        /// </summary>
        /// <param name="trackId"></param>
        /// <param name="open">3</param>
        /// <returns></returns>
        public async Task<StorageFile> GetTrackDataFile(Guid trackId)
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            var fileName = GetFileName(trackId, FileType.PackedData);

            return await trackFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }


        public async Task<string> GetTrackDataAsync(Guid trackId)
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            var fileName = GetFileName(trackId, FileType.PackedData);

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
                    //stringBuilder.append(aData.getTime()).append(";");
                    //stringBuilder.append(aData.getPit()).append(";");
                    //stringBuilder.append(aData.getLat()).append(";");
                    //stringBuilder.append(aData.getLon()).append(";");
                    //stringBuilder.append(aData.getType()).append(";");
                    //stringBuilder.append(aData.getAccuracy()).append(";");
                    //stringBuilder.append(aData.getSpeed()).append("#");


                    //time;0;lat;long;cp;accuracy;velocity
                    sb.AppendFormat("{0};0;{1};{2};cp;{3};{4}#",
                        DataRecorder.ConvertToUnix(point.TimeOffset),
                        point.Latitude.ToString(CultureInfo.InvariantCulture),
                        point.Longitude.ToString(CultureInfo.InvariantCulture),
                        double.IsNaN(point.Accuracy) ? (0.0).ToString(CultureInfo.InvariantCulture) : point.Accuracy.ToString(CultureInfo.InvariantCulture),
                         double.IsNaN(point.Speed) ? (0.0).ToString(CultureInfo.InvariantCulture) : point.Speed.ToString(CultureInfo.InvariantCulture));
                }

                await sw.WriteAsync(sb.ToString());
                await sw.FlushAsync();
            }

#if DEBUG

            var trackFolder = await GetTrackFolderAsync(trackId);

            var targetFile = await trackFolder.CreateFileAsync(Guid.NewGuid().ToString("N") + ".cjson", CreationCollisionOption.ReplaceExisting);

            using (var sw = await targetFile.OpenStreamForWriteAsync())
            {
                var json = JsonConvert.SerializeObject(geo, Formatting.Indented);
                using (var sr = new StreamWriter(sw))
                {
                    await sr.WriteAsync(json);
                }
            }

#endif
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

                    //stringBuilder.append(aData.getTime()).append(";");
                    //stringBuilder.append(aData.getPit()).append(";");
                    //stringBuilder.append(aData.getLat()).append(";");
                    //stringBuilder.append(aData.getLon()).append(";");
                    //stringBuilder.append(aData.getType()).append(";");
                    //stringBuilder.append(aData.getAccuracy()).append(";");
                    //stringBuilder.append(aData.getSpeed()).append("#");

                    //time;pit;0;0;cp;0;0
                    sb.AppendFormat("{0};{1};0;0;origin;0;0#",
                        DataRecorder.ConvertToUnix(point.TimeOffset),
                        point.Value.ToString(CultureInfo.InvariantCulture));
                }

                await sw.WriteAsync(sb.ToString());

                await sw.FlushAsync();
            }

#if DEBUG

            var trackFolder = await GetTrackFolderAsync(trackId);

            var targetFile = await trackFolder.CreateFileAsync(Guid.NewGuid().ToString("N") + ".ojson", CreationCollisionOption.ReplaceExisting);

            using (var sw = await targetFile.OpenStreamForWriteAsync())
            {
                var json = JsonConvert.SerializeObject(accs, Formatting.Indented);
                using (var sr = new StreamWriter(sw))
                {
                    await sr.WriteAsync(json);
                }
            }

#endif
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

            var res = await folder.CreateFolderAsync(ATrack.GetName(id), CreationCollisionOption.OpenIfExists);

            return res;
        }


        public async Task<StorageFile[]> GetDataFilesAsync(Guid trackId)
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            var resFilesRaw = await trackFolder.GetFilesAsync();

            var files = resFilesRaw.Where(
                x => x.Name.EndsWith(GpsTrackFileExtension) || x.Name.EndsWith(AccelerometerTrackFileExtension));

            return files.ToArray();
        }

        private async Task<StreamWriter> GetWriteStreamAsync(FileType fType, Guid trackId)//bool isForGpsTrack
        {
            var trackFolder = await GetTrackFolderAsync(trackId);

            // new guid for gps/acc files
            // track id for track result file
            var fileName = GetFileName(fType == FileType.PackedData ? trackId : Guid.NewGuid(), fType);

            // multilply acc\gps files, only one for result
            var targetFile = await trackFolder.CreateFileAsync(fileName, fType == FileType.PackedData ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);

            var sw = new StreamWriter(await targetFile.OpenStreamForWriteAsync());

            return sw;
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
                case FileType.PackedData:
                    ext = TrackDataResultFileExtension;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fType");
            }

            return String.Format("{0}.{1}", ATrack.GetName(id), ext);
        }

        public async Task DeleteTracksAsync(List<Guid> ids)
        {
            foreach (var trackId in ids)
            {
                await DeleteTrackAsync(trackId);
            }
        }

        public async Task DeleteTrackAsync(Guid trackId)
        {
            var folder = await GetTrackFolderAsync(trackId);

            await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

    }

}
