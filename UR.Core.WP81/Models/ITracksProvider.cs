using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using UR.Core.WP81.Common;

namespace UR.Core.WP81.Models
{
    public interface ITracksProvider
    {
        Task<ATrack> CreateTrackAsync();
        Task<ATrack> GetTrackAsync(Guid trackId);
        Task SaveTrackAsync(ATrack track);
        Task<StorageFile> GetTrackDataFile(Guid trackId);
        Task<StorageFile[]> GetDataFilesAsync(Guid trackId);
        Task WriteToTrackAsync(List<DataAccelerometer> tmp, Guid currentTrackId);
        Task WriteToTrackAsync(List<DataGeo> geoList, Guid currentTrackId);
        Task<string> GetTrackDataAsync(Guid trackId);
        Task<List<ATrack>> GetTracksAsync();
        Task DeleteTracksAsync();
    }
}