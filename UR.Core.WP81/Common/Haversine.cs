
using System;
using Windows.Devices.Geolocation;

namespace UR.Core.WP81.Common
{
    internal static class Haversine
    {
        /// <summary>  
        /// Returns the distance in miles or kilometers of any two  
        /// latitude / longitude points.  
        /// </summary>  
        public static double Distance(BasicGeoposition point1, BasicGeoposition point2, DistanceType type = DistanceType.Kilometers)
        {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;
            double dLat = ToRadian(point2.Latitude - point1.Latitude);
            double dLon = ToRadian(point2.Longitude - point1.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadian(point1.Latitude)) * Math.Cos(ToRadian(point2.Latitude)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
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
            Kilometers
        };

    }
}
