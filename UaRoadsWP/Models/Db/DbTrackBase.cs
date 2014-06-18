using System;

namespace UaRoadsWP.Models.Db
{
    public class DbTrackBase
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public short TrackId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }


        [SQLite.Ignore]
        public bool IsPit
        {
            get { return GetType() == typeof(DbTrackPit); }
        }
    }
}