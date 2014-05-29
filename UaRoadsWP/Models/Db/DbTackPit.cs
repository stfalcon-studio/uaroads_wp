using System;

namespace UaRoadsWP.Models.Db
{
    public class DbTrackPit
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public short TrackId { get; set; }

        public float PitValue { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
