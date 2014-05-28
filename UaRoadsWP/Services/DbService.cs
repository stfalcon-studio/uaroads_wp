using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;
using UaRoadsWP.Models.Db;

namespace UaRoadsWP.Services
{
    public class DbService
    {
        public static string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");

        public SQLiteConnection GetConnection()
        {
            string dbRootPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            return new SQLiteConnection(Path.Combine(dbRootPath, DB_PATH));
        }

        public SQLiteAsyncConnection GetAsyncConnection()
        {
            string dbRootPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            return new SQLiteAsyncConnection(Path.Combine(dbRootPath, DB_PATH));
        }

        public async void Initialize()
        {
            using (var db = GetConnection())
            {
                db.CreateTable<DbTrack>();
                db.CreateTable<DbTrackPit>();
                db.CreateTable<DbTrackLocation>();
            }
        }
    }
}
