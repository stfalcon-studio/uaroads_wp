using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using UR.Core.WP81.Services;

namespace UR.Core.WP81.DataRecorders
{
    public class LocationRecordService
    {
        //private short _currentTrackId;
        //private List<DbTrackLocation> _locationCacheList;

        //private bool _accelStarted;

        //private Timer _timer;

        //private bool _isEnabled;

        private Geolocator _geolocator;

        public LocationRecordService()
        {
            //_isEnabled = false;
            //_locationCacheList = new List<DbTrackLocation>(AppConstant.LocationCacheCount);
        }


        public void Start()
        {
            //_isEnabled = true;

            //_currentTrackId = SettingsService.CurrentTrack.Value;

            _geolocator = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.High,
                DesiredAccuracyInMeters = 70,
                MovementThreshold = 30,
                //ReportInterval = 500
            };


            _geolocator.StatusChanged += GeolocatorOnStatusChanged;

            _geolocator.PositionChanged += GeolocatorOnPositionChanged;

            //_geolocator.StatusChanged += GeolocatorOnStatusChanged;

            //_geolocator.

            //_geolocator.Start();


            //    if (!Accelerometer.IsSupported)
            //    {
            //        Debug.WriteLine("SENSOR NOT SUPPORTED");
            //        return;
            //    }

            //    if (!SettingsService.CurrentTrack.HasValue) return;
        }


        public void Stop()
        {

            if (_geolocator != null)
            {
                _geolocator.StatusChanged -= GeolocatorOnStatusChanged;

                _geolocator.PositionChanged -= GeolocatorOnPositionChanged;
            }

            _geolocator = null;
        }

        private void GeolocatorOnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            RecordService.GetInstance().AddGpsPoint(args.Position);
        }

        private void GeolocatorOnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            Debug.WriteLine("STATUS {0}", args.Status);
        }


        //private async void TimerCallback(object state)
        //{
        //    if (!_isEnabled) return;

        //    var currentPosition = _geolocator.Position;

        //    _locationCacheList.Add(new DbTrackLocation()
        //    {
        //        TimeStamp = currentPosition.Timestamp,
        //        TrackId = _currentTrackId,
        //        Lattitude = currentPosition.Location.Latitude,
        //        Longitude = currentPosition.Location.Longitude
        //    });

        //    if (_locationCacheList.Count == AppConstant.LocationCacheCount)
        //    {
        //        await FlushBuffer();
        //    }
        //}


        //private void GeolocatorOnPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> args)
        //{
        //    //Debug.WriteLine("geo {0}", args.Position.Location);
        //}

        //private void GeolocatorOnStatusChanged(object sender, GeoPositionStatusChangedEventArgs args)
        //{
        //    if (args.Status == GeoPositionStatus.Ready)
        //    {
        //        _timer = new Timer(TimerCallback, null, 0, AppConstant.LocationUpdateInterval);
        //    }

        //    Debug.WriteLine("geo {0}", args.Status);
        //}



        //private async Task FlushBuffer()
        //{
        //    if (_locationCacheList != null)
        //    {
        //        if (_locationCacheList.Any())
        //        {
        //            var recordsCurrent = _locationCacheList;

        //            _locationCacheList = new List<DbTrackLocation>(AppConstant.LocationCacheCount);

        //            await new DbStorageService().TackLocationInsert(recordsCurrent);

        //            recordsCurrent.Clear();
        //        }
        //    }
        //}
    }
}
