using System;

namespace UaRoadsWP.Models.Db
{
    public class DbTrack
    {
        public DbTrack()
        {
            TrackId = Guid.NewGuid();
        }

        /// <summary>
        /// internal small usage memory / storage primary key and ID
        /// </summary>
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public short Id { get; set; }

        /// <summary>
        /// external unique ID (GUID)
        /// </summary>
        public Guid TrackId { get; set; }

        public string TrackComment { get; set; }

        public ETrackStatus TrackStatus { get; set; }
    }


    public enum ETrackStatus
    {
        /// <summary>
        /// recording
        /// </summary>
        Started,

        /// <summary>
        /// recorded
        /// </summary>
        Finished,

        /// <summary>
        /// paused
        /// </summary>
        Paused,

        /// <summary>
        /// sended to server
        /// </summary>
        Sended,

        /// <summary>
        /// ready for sending to server
        /// </summary>
        ReadyToSend,
    }
}