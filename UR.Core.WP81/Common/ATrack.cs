using System;
using Windows.ApplicationModel.Appointments;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;

namespace UR.Core.WP81.Common
{

    public class DbTrack
    {
        [SQLite.PrimaryKey]
        public Guid Id { get; set; }

        /// <summary>
        /// when track recording started
        /// </summary>
        public DateTime StartedDateTime { get; set; }

        /// <summary>
        /// when thack recording finished
        /// </summary>
        public DateTime? FinishedDateTime { get; set; }

        public TimeSpan TrackDuration { get; set; }

        public double TrackAvgSpeed { get; set; }

        /// <summary>
        /// in meters
        /// </summary>
        public double TrackLength { get; set; }

        /// <summary>
        /// pit-records count
        /// </summary>
        public int PitPointsCount { get; set; }

        /// <summary>
        /// location records count
        /// </summary>
        public int LocationPointsCount { get; set; }

        public ETrackStatus Status { get; set; }

        public string Comment { get; set; }



        /// <summary>
        /// point to calc avg speed
        /// </summary>
        public int AvgSpeedPointsCount { get; set; }

        public DbTrack()
        {
            Id = Guid.NewGuid();
        }
    }

    public class ATrack : DbTrack
    {
        public ATrack()
        {
            TrackLength = 0;
        }

        public double CurrentSpeed { get; set; }

        public double SummOfSpeed { get; set; }

        public BasicGeoposition? LastGeoposition { get; set; }

        public string TrackDurationStr
        {
            get { return TrackDuration.ToString(@"hh\:mm\:ss"); }
        }

        public string TrackLengthStr
        {
            get { return String.Format("{0:F2} km", TrackLength / 1000); }
        }

        public string TrackName
        {
            get { return GetName(Id); }
        }

        public static string GetName(Guid g)
        {
            return g.ToString("N");
        }
    }
}
