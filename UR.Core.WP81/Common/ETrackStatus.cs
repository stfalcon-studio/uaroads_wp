namespace UR.Core.WP81.Common
{
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
        /// processed
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