using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace UaRoadsWP.Services
{

    class LocationService
    {
        Geolocator geolocator = null;
        bool tracking = false;

        public Windows.Devices.Geolocation.PositionStatus Status { get; internal set; }

        public LocationService()
        {
            Sdfsd();
        }


        public async void Sdfsd()
        {
            geolocator = new Geolocator();

            Status = PositionStatus.NotAvailable;

            geolocator.MovementThreshold = 1;

            geolocator.StatusChanged += GeolocatorOnStatusChanged;

            try
            {
                var geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));
            }
            catch (Exception)
            {

            }
            //geoposition.Coordinate.
        }

        private void GeolocatorOnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            Debug.WriteLine("geoposition {0}", args.Status);
        }
    }
}
