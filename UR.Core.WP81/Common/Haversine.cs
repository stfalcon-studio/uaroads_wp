
using System;
using Windows.Devices.Geolocation;

namespace UR.Core.WP81.Common
{
    internal static class Haversine
    {
        /// <summary>  
        /// Returns the distance in miles/kilometers/meters
        /// latitude / longitude points.  
        /// </summary>  
        public static double Distance(BasicGeoposition point1, BasicGeoposition point2, DistanceType type)
        {
            double r = 0;

            switch (type)
            {
                case DistanceType.Miles:
                    r = 3960;
                    break;
                case DistanceType.Kilometers:
                    r = 6371;
                    break;
                case DistanceType.Meters:
                    r = 6371000;
                    break;
                default:
                    r = 1;
                    break;
            }

            double dLat = ToRadian(point2.Latitude - point1.Latitude);
            double dLon = ToRadian(point2.Longitude - point1.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(ToRadian(point1.Latitude)) * Math.Cos(ToRadian(point2.Latitude)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = r * c;
            return d;
        }

        /// <summary>  
        /// Convert to Radians.  
        /// </summary>  
        private static double ToRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

        /// <summary>  
        /// The distance type to return the results in.  
        /// </summary>  
        public enum DistanceType
        {
            Miles,
            Kilometers,
            Meters
        };
    }
}
