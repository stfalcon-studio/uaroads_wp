using System.Diagnostics;
using Windows.Devices.Geolocation;

namespace UR.Core.WP81.Services.DataRecorders
{
    public class LocationRecordService
    {
        private Geolocator _geolocator;

        public LocationRecordService()
        {
        }


        public void Start()
        {
            _geolocator = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.Default,
                //DesiredAccuracyInMeters = 70,
                MovementThreshold = 30,
                //ReportInterval = 500
            };

            _geolocator.StatusChanged += GeolocatorOnStatusChanged;

            _geolocator.PositionChanged += GeolocatorOnPositionChanged;
        }

        public void Stop()
        {
            if (_geolocator != null)
            {
                _geolocator.StatusChanged -= GeolocatorOnStatusChanged;

                _geolocator.PositionChanged -= GeolocatorOnPositionChanged;
            }

            _geolocator = null;

            StateService.Instance.GeoStatus = null;
        }

        private void GeolocatorOnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            RecordService.GetInstance().AddGpsPoint(args.Position);
        }

        private void GeolocatorOnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            StateService.Instance.GeoStatus = args.Status;

            Debug.WriteLine("STATUS {0}", args.Status);
        }
    }
}
