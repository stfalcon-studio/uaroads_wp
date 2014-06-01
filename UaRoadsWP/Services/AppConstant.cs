using System;
using System.Collections.Generic;
using UaRoadsWP.Base;

namespace UaRoadsWP.Services
{
    public static class AppConstant
    {
        public const int MaxSavedTracksAtStorage = 50;

        public const int SensorCacheCount = 10;

        public const int LocationCacheCount = 10;


        /// <summary>
        /// ms
        /// </summary>
        public const int SensorUpdateInterval = 500;

        /// <summary>
        /// ms
        /// </summary>
        public const int LocationUpdateInterval = 1000;

        public const string DataFolderName = "Tracks";
    }
}
