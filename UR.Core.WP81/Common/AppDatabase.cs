using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using SQLite;
using UR.Core.WP81.Services;

namespace UR.Core.WP81.Common
{
    public class AppDatabase : IDbTracksProvider
    {
        private const string DbPath = "data.db";

        public AppDatabase()
        {

        }

        public async Task InitiliaseAsync()
        {
            var db = Conn();
            await db.CreateTableAsync<DbTrack>();
            AutoMapper.Mapper.CreateMap<ATrack, DbTrack>();
        }

        private SQLiteAsyncConnection Conn()
        {
            return new SQLiteAsyncConnection(DbPath);
        }

        public Task<List<DbTrack>> GetTracksAsync()
        {
            var conn = Conn();

            return conn.Table<DbTrack>().ToListAsync();
        }

        public async Task SaveTrackAsync(DbTrack track)
        {
            var tr = Mapper.Map<DbTrack>(track);

            var conn = Conn();

            var items = await conn.Table<DbTrack>().Where(x => x.Id == tr.Id).ToListAsync();

            if (items.Any())
            {
                await conn.UpdateAsync(tr);
            }
            else
            {
                await conn.InsertAsync(tr);
            }
        }

        public async Task<DbTrack> GetTrackAsync(Guid id)
        {
            var conn = Conn();

            var items = await conn.Table<DbTrack>().Where(x => x.Id == id).ToListAsync();

            return items.FirstOrDefault();
        }

        public async Task DeleteTracksAsync(List<Guid> ids)
        {
            foreach (var dbTrack in ids)
            {
                await DeleteTrackAsync(dbTrack);
            }
        }

        public async Task DeleteTrackAsync(Guid trackId)
        {
            var conn = Conn();

            var items = await conn.Table<DbTrack>().Where(x => x.Id == trackId).ToListAsync();

            foreach (var dbTrack in items)
            {
                await conn.DeleteAsync(dbTrack);
            }
        }
    }

    public interface IDbTracksProvider
    {
        Task InitiliaseAsync();
        Task<List<DbTrack>> GetTracksAsync();
        Task SaveTrackAsync(DbTrack track);
        Task<DbTrack> GetTrackAsync(Guid id);

        /// <summary>
        /// return output file for track data
        /// </summary>
        /// <param name="trackId"></param>
        /// <param name="open">3</param>
        /// <returns></returns>
        /// <summary>
        /// write small portion of data to new file
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="trackId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        //Task WriteToTrackAsync(List<DataGeo> geo, Guid trackId);

        ///// <summary>
        ///// write small portion of data to new file
        ///// </summary>
        ///// <param name="accs"></param>
        ///// <param name="trackId"></param>
        ///// <returns></returns>
        //Task WriteToTrackAsync(List<DataAccelerometer> accs, Guid trackId);
        Task DeleteTracksAsync(List<Guid> ids);
        Task DeleteTrackAsync(Guid trackId);
        //Task<StorageFile[]> GetDataFilesAsync(Guid trackId);
    }
}
