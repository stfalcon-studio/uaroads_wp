namespace UR.Core.WP81
{
    public static class AppConstant
    {
        public const int MaxSavedTracksAtStorage = 50;

        public const int SensorCacheCount = 10;

        public const int LocationCacheCount = 10;


        /// <summary>
        /// ms  SENSOR_DELAY_36Hz = 27777;
        /// </summary>
        public const int SensorUpdateInterval = 28;

        /// <summary>
        /// ms
        /// </summary>
        public const int LocationUpdateInterval = 1000;

        public const string DataFolderName = "Tracks";

        public const string BaseApiUrl = "http://api.uaroads.com/";
    }
}
