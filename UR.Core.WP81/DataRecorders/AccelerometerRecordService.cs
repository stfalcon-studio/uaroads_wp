using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using UaRoadsWP.Models.Db;
using Accelerometer = Microsoft.Devices.Sensors.Accelerometer;
using AccelerometerReading = Microsoft.Devices.Sensors.AccelerometerReading;

namespace UaRoadsWP.Services
{
    public class AccelerometerRecordService
    {
        private Accelerometer _accelSensor;
        private Vector3 _accelReading;

        private short _currentTrackId;
        private List<DbTrackPit> _accelerometerReadings;

        private bool _accelStarted;

        public AccelerometerRecordService()
        {
            _accelerometerReadings = new List<DbTrackPit>(AppConstant.SensorCacheCount);
        }


        public void Start()
        {
            if (!Accelerometer.IsSupported)
            {
                Debug.WriteLine("SENSOR NOT SUPPORTED");
                return;
            }

            if (!SettingsService.CurrentTrack.HasValue) return;

            _currentTrackId = SettingsService.CurrentTrack.Value;

            _accelSensor = new Accelerometer();

            _accelSensor.CurrentValueChanged += AccelSensorOnCurrentValueChanged;

            _accelSensor.TimeBetweenUpdates = TimeSpan.FromMilliseconds(AppConstant.SensorUpdateInterval);

            _accelSensor.Start();
        }


        public async void Stop()
        {
            if (!Accelerometer.IsSupported)
            {
                Debug.WriteLine("SENSOR NOT SUPPORTED");
                return;
            }

            await FlushBuffer();

            _currentTrackId = 0;

            if (_accelSensor != null)
            {
                _accelSensor.CurrentValueChanged -= AccelSensorOnCurrentValueChanged;
                if (_accelSensor.State == SensorState.Ready)
                {
                    _accelSensor.Stop();
                }
                _accelSensor.Dispose();
            }
        }

        private async void AccelSensorOnCurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> args)
        {
            var res = Math.Sqrt(args.SensorReading.Acceleration.X * args.SensorReading.Acceleration.X +
                args.SensorReading.Acceleration.Y * args.SensorReading.Acceleration.Y +
                args.SensorReading.Acceleration.Z * args.SensorReading.Acceleration.Z);
            //Debug.WriteLine("{0} {1}", args.SensorReading.Timestamp, res);

            _accelerometerReadings.Add(new DbTrackPit()
            {
                TimeStamp = args.SensorReading.Timestamp,
                PitValue = (float)res,
                TrackId = _currentTrackId
            });

            if (_accelerometerReadings.Count == AppConstant.SensorCacheCount)
            {
                await FlushBuffer();
            }
        }

        private async Task FlushBuffer()
        {
            if (_accelerometerReadings != null)
            {
                if (_accelerometerReadings.Any())
                {
                    var recordsCurrent = _accelerometerReadings;

                    _accelerometerReadings = new List<DbTrackPit>(AppConstant.SensorCacheCount);

                    await new DbStorageService().TackPitInsert(recordsCurrent);

                    recordsCurrent.Clear();
                }
            }
        }
    }
}
