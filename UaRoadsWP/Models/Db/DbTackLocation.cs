using System;

namespace UaRoadsWP.Models.Db
{
    public class DbTrackLocation
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public short TrackId { get; set; }

        public double Lattitude { get; set; }

        public double Longitude { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        [SQLite.Ignore]
        public DbTrackPit PitRef { get; set; }
    }
}
