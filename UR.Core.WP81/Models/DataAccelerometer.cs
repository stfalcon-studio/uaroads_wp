using System;

namespace UR.Core.WP81.Models
{
    public struct DataAccelerometer
    {
        public DateTimeOffset TimeOffset { get; set; }
        public double Value { get; set; }
    }
}