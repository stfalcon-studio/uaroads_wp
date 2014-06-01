using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using UaRoadsWP.Models.Db;
using Accelerometer = Microsoft.Devices.Sensors.Accelerometer;
using AccelerometerReading = Microsoft.Devices.Sensors.AccelerometerReading;

namespace UaRoadsWP.Services
{
    public class LocationRecordService
    {
        private short _currentTrackId;
        private List<DbTrackLocation> _locationCacheList;

        private bool _accelStarted;

        private Timer _timer;

        private bool _isEnabled;


        private GeoCoordinateWatcher _watcher;

        public LocationRecordService()
        {
            _isEnabled = false;
            _locationCacheList = new List<DbTrackLocation>(AppConstant.LocationCacheCount);
        }


        public void Start()
        {
            _isEnabled = true;
            
            _currentTrackId = SettingsService.CurrentTrack.Value;

            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

            _watcher.PositionChanged += WatcherOnPositionChanged;

            _watcher.StatusChanged += WatcherOnStatusChanged;

            _watcher.Start();


            //    if (!Accelerometer.IsSupported)
            //    {
            //        Debug.WriteLine("SENSOR NOT SUPPORTED");
            //        return;
            //    }

            //    if (!SettingsService.CurrentTrack.HasValue) return;
        }


        private async void TimerCallback(object state)
        {
            if (!_isEnabled) return;

            var currentPosition = _watcher.Position;

            _locationCacheList.Add(new DbTrackLocation()
            {
                TimeStamp = currentPosition.Timestamp,
                TrackId = _currentTrackId,
                Lattitude = currentPosition.Location.Latitude,
                Longitude = currentPosition.Location.Longitude
            });

            if (_locationCacheList.Count == AppConstant.LocationCacheCount)
            {
                await FlushBuffer();
            }
        }


        private void WatcherOnPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> args)
        {
            //Debug.WriteLine("geo {0}", args.Position.Location);
        }

        private void WatcherOnStatusChanged(object sender, GeoPositionStatusChangedEventArgs args)
        {
            if (args.Status == GeoPositionStatus.Ready)
            {
                _timer = new Timer(TimerCallback, null, 0, AppConstant.LocationUpdateInterval);
            }

            Debug.WriteLine("geo {0}", args.Status);
        }


        public async void Stop()
        {
            _isEnabled = false;

            if (_timer != null)
            {
                _timer.Dispose();
            }

            await FlushBuffer();

            _currentTrackId = 0;

            if (_watcher != null)
            {
                _watcher.PositionChanged -= WatcherOnPositionChanged;
                _watcher.StatusChanged -= WatcherOnStatusChanged;
                _watcher.Stop();
                _watcher.Dispose();
            }
        }

        private async Task FlushBuffer()
        {
            if (_locationCacheList != null)
            {
                if (_locationCacheList.Any())
                {
                    var recordsCurrent = _locationCacheList;

                    _locationCacheList = new List<DbTrackLocation>(AppConstant.LocationCacheCount);

                    await new DbStorageService().TackLocationInsert(recordsCurrent);

                    recordsCurrent.Clear();
                }
            }
        }
    }
}
