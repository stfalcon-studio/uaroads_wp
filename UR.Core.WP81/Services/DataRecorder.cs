using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Caliburn.Micro;
using Eve.Core.Extensions;
using UR.Core.WP81.API.Models;
using UR.Core.WP81.Common;
using UR.Core.WP81.Services.DataRecorders;
using Microsoft.ApplicationInsights;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{
    public class DataRecorder
    {
        private const int RunCheckGeoOn = 50;

        private const int RunCheckAccOn = 200;

        /// <summary>
        /// max elements to store in memory
        /// </summary>
        private const int GeoMaxElements = 1000;


        /// <summary>
        /// max elements to store in memory
        /// </summary>
        private const int AccMaxElements = 200;

        private static DataRecorder _instance;

        public bool IsStarted { get; private set; }

        private Guid _currentTrackId;

        public static DataRecorder GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataRecorder();
            }

            return _instance;
        }

        private DataRecorder()
        {

        }


        private LocationDataCollector _locationDataCollector;
        private AccelerometerDataCollector _accelerometerDataCollector;
        public async Task StartAsync()
        {
            var task = new Task[] { InnerStart() };
            await Task.Factory.ContinueWhenAll(task, (a) =>
            {


            });
        }


        public async Task StopAsync()
        {
            var task = new Task[] { StopInner() };
            await Task.Factory.ContinueWhenAll(task, (a) =>
            {


            });
        }

        private async Task InnerStart()
        {
            if (IsStarted) return;
            IsStarted = true;

            try
            {
                _geoList = new List<DataGeo>(GeoMaxElements + 20);
                _accelerometerList = new List<DataAccelerometer>(AccMaxElements + 20);

                //var ssTrack = StateService.Instance.CurrentTrack;
                //var ssTrackId = SettingsService.CurrentTrackId;

                ATrack ssTrack;
                Guid ssTrackId;

                ssTrack = await IoC.Get<ITracksProvider>().CreateTrackAsync();
                ssTrackId = ssTrack.Id;

                //if (ssTrack == null)
                //{
                //    if (ssTrackId == Guid.Empty)
                //    {
                //        var track = await IoC.Get<ITracksProvider>().GetTrackAsync(Guid.NewGuid());

                //        ssTrackId = track.Id;

                //        ssTrack = track;
                //    }
                //    else
                //    {
                //        var track = await IoC.Get<ITracksProvider>().GetTrackAsync(ssTrackId);

                //        ssTrack = track;
                //    }
                //}
                //else
                //{
                //    if (ssTrack.Id != ssTrackId)
                //    {
                //        var track = await IoC.Get<ITracksProvider>().GetTrackAsync(ssTrackId);

                //        ssTrack = track;
                //    }
                //}

                ssTrack.Status = ETrackStatus.Recording;

                ssTrack.StartedDateTime = DateTime.Now;

                _currentTrackId = ssTrackId;

                ssTrack.Comment = DateTime.Now.ToString("G");

                await IoC.Get<ITracksProvider>().SaveTrackAsync(ssTrack);

                await Task.Delay(1000);
                StateService.Instance.CurrentTrack = ssTrack;
                SettingsService.CurrentTrackId = ssTrackId;
                MsgAppState.Publish(EGlobalState.Recording);

                _locationDataCollector = new LocationDataCollector();
                _locationDataCollector.Start();

                _accelerometerDataCollector = new AccelerometerDataCollector();
                _accelerometerDataCollector.Start();
                return;
            }
            catch (Exception err)
            {
                IsStarted = false;
                StateService.Instance.CurrentTrack = null;
                SettingsService.CurrentTrackId = Guid.Empty;
                new TelemetryClient().TrackException(err);

                if (_locationDataCollector != null)
                {
                    _locationDataCollector.Stop();
                    _locationDataCollector = null;
                }

                if (_accelerometerDataCollector != null)
                {
                    _accelerometerDataCollector.Stop();
                    _accelerometerDataCollector = null;
                }
            }
        }


        private async Task StopInner()
        {
            try
            {
                IsStarted = false;

                if (_locationDataCollector != null)
                {
                    _locationDataCollector.Stop();
                    _locationDataCollector = null;
                }

                if (_accelerometerDataCollector != null)
                {
                    _accelerometerDataCollector.Stop();
                    _accelerometerDataCollector = null;
                }

                await FlushAsync();

                var cTrack = StateService.Instance.CurrentTrack;

                cTrack.Status = ETrackStatus.Recorded;

                cTrack.FinishedDateTime = DateTime.Now;

                cTrack.TrackDuration = DateTime.Now - cTrack.StartedDateTime;

                await IoC.Get<ITracksProvider>().SaveTrackAsync(cTrack);

                await Task.Delay(1000);

                //IoC.Get<IEventAggregator>().PublishOnUIThread(new MsgDataHandlerStatusChanged(false));
            }
            catch (Exception err)
            {
                new TelemetryClient().TrackException(err);

            }
            finally
            {
                StateService.Instance.CurrentTrack = null;

                SettingsService.CurrentTrackId = Guid.Empty;

                _currentTrackId = Guid.Empty;

                MsgAppState.Publish(EGlobalState.Normal);
            }
        }


        private List<DataGeo> _geoList;
        private List<DataAccelerometer> _accelerometerList;

        public void AddGpsPoint(Geoposition position)
        {
            var cTrack = StateService.Instance.CurrentTrack;

           

            var bgp = new BasicGeoposition()
            {
                Latitude = position.Coordinate.Latitude,
                Longitude = position.Coordinate.Longitude,
                Altitude = position.Coordinate.Altitude ?? 0
            };

            if (cTrack.LastGeoposition.HasValue)
            {
                var tmpDistance = Haversine.Distance(cTrack.LastGeoposition.Value, bgp, Haversine.DistanceType.Meters);

                Debug.WriteLine($"DISTANCE: {tmpDistance}m");

                if (tmpDistance >= AppConstant.MINIMAL_DISTANCE_BETWEEN_GPS_POINTS_METERS)
                {
                    cTrack.LocationPointsCount++;
                    cTrack.TrackLength += tmpDistance;
                    cTrack.LastGeoposition = bgp;

                    if (position.Coordinate.Speed != null)
                    {
                        cTrack.CurrentSpeed = ConvertSpeed(position.Coordinate.Speed.Value);
                        cTrack.SummOfSpeed += cTrack.CurrentSpeed;
                        cTrack.AvgSpeedPointsCount++;
                    }
                }
            }
            else
            {
                cTrack.LastGeoposition = bgp;
            }

            _geoList.Add(new DataGeo()
            {
                Latitude = position.Coordinate.Point.Position.Latitude,
                Longitude = position.Coordinate.Point.Position.Longitude,
                TimeOffset = position.Coordinate.Timestamp,
                //Speed = position.Coordinate.Speed.HasValue ? ((position.Coordinate.Speed.Value * 60.0 * 60.0) / 100.0) : 0.0,

                //в оригинал = V * 100 / 60 / 60
                Speed = position.Coordinate.Speed.HasValue ? ConvertSpeed(position.Coordinate.Speed.Value) : 0.0,
                Accuracy = position.Coordinate.Accuracy
            });

            CheckGeo();
        }

        public void AddAccelerometerPoint(DateTimeOffset offcet, double value)
        {
            //Debug.WriteLine("ACC {0}", value);

            var cTrack = StateService.Instance.CurrentTrack;

            cTrack.PitPointsCount++;

            _accelerometerList.Add(new DataAccelerometer()
            {
                Value = value,
                TimeOffset = offcet
            });

            CheckAcc();
        }

        private int _checkGeoCount = 0;
        private int _checkAccCount = 0;

        private async void CheckGeo()
        {
            if (_checkGeoCount++ <= RunCheckGeoOn)
            {
                return;
            }

            _checkGeoCount = 0;

            if (_geoList.Count >= GeoMaxElements)
            {
                var tmp = _geoList;

                _geoList = new List<DataGeo>(GeoMaxElements + 20);

                await IoC.Get<ITracksProvider>().WriteToTrackAsync(tmp, _currentTrackId);
            }
        }

        private async void CheckAcc()
        {
            if (_checkAccCount++ <= RunCheckAccOn)
            {
                return;
            }

            _checkAccCount = 0;


            if (_accelerometerList.Count >= AccMaxElements)
            {
                var tmp = _accelerometerList;

                _accelerometerList = new List<DataAccelerometer>(AccMaxElements + 20);

                await IoC.Get<ITracksProvider>().WriteToTrackAsync(tmp, _currentTrackId);
            }
        }

        private async Task FlushAsync()
        {
            await IoC.Get<ITracksProvider>().WriteToTrackAsync(_accelerometerList, _currentTrackId);
            _accelerometerList = null;
            await IoC.Get<ITracksProvider>().WriteToTrackAsync(_geoList, _currentTrackId);
            _geoList = null;
        }

        public static string ConvertToUnix(DateTimeOffset offset)
        {
            return (offset.DateTime.ToUnixTimeMilliseconds()).ToString(CultureInfo.InvariantCulture);
        }

        private double ConvertSpeed(double inMeters)
        {
            return 3.6 * inMeters;
        }
    }
}
