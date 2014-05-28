using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using UaRoadsWP.Models.Db;

namespace UaRoadsWP.Services
{
    public class DbStorageService
    {
        SQLiteAsyncConnection GetAsyncConnection()
        {
            return new DbService().GetAsyncConnection();
        }

        public async Task<List<DbTrack>> TracksGet()
        {
            return await GetAsyncConnection().Table<DbTrack>().ToListAsync();
        }

        public async Task TrackInsertUpdate(DbTrack item)
        {
            var c = GetAsyncConnection();

            if (await c.FindAsync<DbTrack>(item.Id) == null)
            {
                await c.InsertAsync(item);
            }
            else
            {
                await c.UpdateAsync(item);
            }
        }


        public async Task TackPitInsert(List<DbTrackPit> pits)
        {
            var c = GetAsyncConnection();
            await c.InsertAllAsync(pits);
        }

        public async Task TackLocationInsert(List<DbTrackLocation> pits)
        {
            var c = GetAsyncConnection();

            await c.InsertAllAsync(pits);
        }

        public async Task TrackDeleteFull(short trackId)
        {
            //todo maybe replace all delete operations with sql commands
            var c = GetAsyncConnection();

            var track = await c.FindAsync<DbTrack>(trackId);
            if (track != null)
            {
                await c.DeleteAsync(track);
            }


            var pits = await c.Table<DbTrackPit>().Where(x => x.TrackId == trackId).ToListAsync();

            if (pits != null)
            {
                foreach (var pit in pits)
                {
                    await c.DeleteAsync(pit);
                }
            }

            var locs = await c.Table<DbTrackLocation>().Where(x => x.TrackId == trackId).ToListAsync();

            if (locs != null)
            {
                foreach (var loc in locs)
                {
                    await c.DeleteAsync(loc);
                }
            }
        }
    }
}
