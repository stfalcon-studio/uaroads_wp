using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Caliburn.Micro;
using UR.Core.WP81.API.Models;
using UR.Core.WP81.DataRecorders;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{
    public class RecordService
    {
        private const int RunCheckGeoOn = 50;
        private const int RunCheckAccOn = 200;

        private const int GeoMaxElements = 1000;
        private const int AccMaxElements = 200;

        private static RecordService _instance;

        public bool IsStarted { get; private set; }

        public static RecordService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RecordService();
            }

            return _instance;
        }

        private RecordService()
        {
            _geoList = new List<DataGeo>(GeoMaxElements + 20);
            _accelerometerList = new List<DataAccelerometer>(AccMaxElements + 20);
        }


        private LocationRecordService _locationRecordService;
        private AccelerometerRecordService _accelerometerRecordService;
        public async Task StartAsync()
        {
            IsStarted = true;

            if (StateService.Instance.CurrentTrack == null)
            {
                if (SettingsService.CurrentTrackId == Guid.Empty)
                {
                    var track = await new TracksProvider().TrackAsync(Guid.NewGuid());

                    SettingsService.CurrentTrackId = track.TrackId;

                    StateService.Instance.CurrentTrack = track;
                }
                else
                {
                    var track = await new TracksProvider().TrackAsync(SettingsService.CurrentTrackId);

                    StateService.Instance.CurrentTrack = track;
                }
            }
            else
            {
                if (StateService.Instance.CurrentTrack.TrackId != SettingsService.CurrentTrackId)
                {
                    var track = await new TracksProvider().TrackAsync(SettingsService.CurrentTrackId);

                    StateService.Instance.CurrentTrack = track;
                }
            }


            var cTrack = StateService.Instance.CurrentTrack;


            cTrack.Status = ETrackStatus.Started;

            cTrack.StartedDateTime = DateTime.Now;

            await new TracksProvider().SaveAsync(cTrack);

            _locationRecordService = new LocationRecordService();
            _locationRecordService.Start();

            _accelerometerRecordService = new AccelerometerRecordService();
            _accelerometerRecordService.Start();

            await Task.Delay(2000);

            IoC.Get<IEventAggregator>().PublishOnUIThread(new DataHandlerStatusChanged(true));
        }


        public async Task StopAsync()
        {
            if (_locationRecordService != null)
            {
                _locationRecordService.Stop();
                _locationRecordService = null;
            }

            if (_accelerometerRecordService != null)
            {
                _accelerometerRecordService.Stop();
                _accelerometerRecordService = null;
            }

            await FlushAsync();

            var cTrack = StateService.Instance.CurrentTrack;

            cTrack.Status = ETrackStatus.Finished;

            cTrack.FinishedDateTime = DateTime.Now;

            await new TracksProvider().SaveAsync(cTrack);

            StateService.Instance.CurrentTrack = null;

            SettingsService.CurrentTrackId = Guid.Empty;

            await Task.Delay(2000);

            IsStarted = false;

            IoC.Get<IEventAggregator>().PublishOnUIThread(new DataHandlerStatusChanged(false));
        }


        private List<DataGeo> _geoList;
        private List<DataAccelerometer> _accelerometerList;

        public void AddGpsPoint(Geoposition position)
        {
            //position.Coordinate.

            Debug.WriteLine("GPS lat{0}-lon{1}-alt{2}", position.Coordinate.Point.Position.Latitude, position.Coordinate.Longitude, position.Coordinate.Altitude);


            _geoList.Add(new DataGeo()
            {
                Latitude = position.Coordinate.Point.Position.Latitude,
                Longitude = position.Coordinate.Point.Position.Longitude,
                TimeOffset = position.Coordinate.Timestamp
            });

            CheckGeo();
        }

        public void AddAccelerometerPoint(DateTimeOffset offcet, double value)
        {
            //Debug.WriteLine("ACC {0}", value);

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

                await new TracksProvider().WriteAsync(tmp);
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

                await new TracksProvider().WriteAsync(tmp);
            }
        }

        private async Task FlushAsync()
        {
            await new TracksProvider().WriteAsync(_accelerometerList);
            _accelerometerList = null;
            await new TracksProvider().WriteAsync(_geoList);
            _geoList = null;
        }
    }

    public struct DataGeo
    {
        public DateTimeOffset TimeOffset { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }


    public struct DataAccelerometer
    {
        public DateTimeOffset TimeOffset { get; set; }
        public double Value { get; set; }
    }
}
