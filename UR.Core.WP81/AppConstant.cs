namespace UR.Core.WP81
{
    public static class AppConstant
    {
        public const uint MaxSavedTracksAtStorage = 50;

        public const uint SensorCacheCount = 10;

        public const uint LocationCacheCount = 10;

        /// <summary>
        /// ms  SENSOR_DELAY_36Hz = 27777;
        /// </summary>
        public const uint SensorUpdateInterval = 28;

        /// <summary>
        /// ms
        /// </summary>
        public const uint LocationReportInterval = 1000;

        /// <summary>
        /// in meters
        /// </summary>
        public const uint MINIMAL_DISTANCE_BETWEEN_GPS_POINTS_METERS = 30;

        public const string DataFolderName = "Tracks";

        public const string BaseApiUrl = "http://api.uaroads.com/";
    }
}
