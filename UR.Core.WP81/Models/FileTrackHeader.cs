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
