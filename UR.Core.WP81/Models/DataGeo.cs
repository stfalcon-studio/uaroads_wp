using System;

namespace UR.Core.WP81.Models
{
    public struct DataGeo
    {
        public DateTimeOffset TimeOffset { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Speed { get; set; }
        public double Accuracy { get; set; }
    }
}