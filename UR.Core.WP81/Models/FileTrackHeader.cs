using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UR.Core.WP81.Models
{
    public class FileTrackHeader
    {
        public FileTrackHeader()
        {
            TrackId = Guid.NewGuid();
        }

        public Guid TrackId { get; set; }

        public DateTime StartedDateTime { get; set; }

        public DateTime? FinishedDateTime { get; set; }

        public ETrackStatus Status { get; set; }

        public string Comment { get; set; }


        public string TrackName
        {
            get { return GetName(TrackId); }
        }

        public static string GetName(Guid g)
        {
            return g.ToString("N");
        }
    }

    public enum ETrackStatus
    {
        /// <summary>
        /// recording
        /// </summary>
        Recording = 0,

        /// <summary>
        /// recorded
        /// </summary>
        Recorded = 1,

        /// <summary>
        /// paused
        /// </summary>
        RecordPaused = 2,

        /// <summary>
        /// processe
        /// </summary>
        Processed = 3,

        /// <summary>
        /// processe
        /// </summary>
        Processing = 4,

        /// <summary>
        /// in sending
        /// </summary>
        Sending = 5,

        /// <summary>
        /// ready for sending to server
        /// </summary>
        Sent = 6,

        /// <summary>
        /// some error
        /// </summary>
        Error = 7,
    }
}
