using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Sensors;

namespace UR.Core.WP81.Services.DataRecorders
{
    public class AccelerometerRecordService
    {
        private Accelerometer _accelSensor;
        private DateTimeOffset _offset;

        private Timer _timer;

        private double _maxValue;
        private const double GravityEarth = 9.81;

        //private Vector3 _accelReading;

        //private short _currentTrackId;
        //private List<DbTrackPit> _accelerometerReadings;

        //private bool _accelStarted;


        public AccelerometerRecordService()
        {
            //_accelerometerReadings = new List<DbTrackPit>(AppConstant.SensorCacheCount);

            _maxValue = 0;
        }



        public void Start()
        {
            StateService.Instance.AccValue = 0;

            //if (!Accelerometer.IsSupported)
            //{
            //    Debug.WriteLine("SENSOR NOT SUPPORTED");
            //    return;
            //}

            //if (!SettingsService.CurrentTrack.HasValue) return;

            //_currentTrackId = SettingsService.CurrentTrack.Value;

            _timer = new Timer(TimerCallback, null, 500, 500);

            _accelSensor = Accelerometer.GetDefault();

            if (_accelSensor != null)
            {
                _accelSensor.ReadingChanged += AccelSensorOnCurrentValueChanged;

                _accelSensor.ReportInterval = AppConstant.SensorUpdateInterval;
                //_accelSensor.Start();
            }
            else
            {
                Debug.WriteLine("CANT CREATE ACCELEROMETER");
            }
        }


        private void TimerCallback(object state)
        {
            RecordService.GetInstance().AddAccelerometerPoint(_offset, _maxValue);

            _maxValue = 0;

            //Debug.WriteLine("callback");
        }

        private double gX, gY, gZ;

        private void AccelSensorOnCurrentValueChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            //gX = args.Reading.AccelerationX / GravityEarth;
            //gY = args.Reading.AccelerationY / GravityEarth;
            //gZ = args.Reading.AccelerationZ / GravityEarth;

            //// gForce will be close to 1 when there is no movement.
            //double accA = Math.Abs(Math.Sqrt(gX * gX + gY * gY + gZ * gZ) - 1);

            //if (accA > _maxValue)
            //{
            //    _maxValue = accA;
            //    _offset = DateTimeOffset.Now;
            //}

            double accX = args.Reading.AccelerationX;

            double accY = args.Reading.AccelerationY;

            double accZ = args.Reading.AccelerationZ;

            double f = Math.Abs(Math.Sqrt(accX * accX + accY * accY + accZ * accZ) - 1);

            if (f > _maxValue)
            {
                _maxValue = f;
                _offset = DateTimeOffset.Now;

                StateService.Instance.AccValue = f;
            }
        }


        public async void Stop()
        {
            //if (!Accelerometer.IsSupported)
            //{
            //    Debug.WriteLine("SENSOR NOT SUPPORTED");
            //    return;
            //}

            //await FlushBuffer();

            //_currentTrackId = 0;

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            if (_accelSensor != null)
            {
                _accelSensor.ReadingChanged -= AccelSensorOnCurrentValueChanged;

                _accelSensor = null;
            }

            StateService.Instance.AccValue = null;
        }

        // private async void AccelSensorOnCurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> args)
        //{
        //var res = Math.Sqrt(args.SensorReading.Acceleration.X * args.SensorReading.Acceleration.X +
        //    args.SensorReading.Acceleration.Y * args.SensorReading.Acceleration.Y +
        //    args.SensorReading.Acceleration.Z * args.SensorReading.Acceleration.Z);
        ////Debug.WriteLine("{0} {1}", args.SensorReading.Timestamp, res);

        //_accelerometerReadings.Add(new DbTrackPit()
        //{
        //    TimeStamp = args.SensorReading.Timestamp,
        //    PitValue = (float)res,
        //    TrackId = _currentTrackId
        //});

        //if (_accelerometerReadings.Count == AppConstant.SensorCacheCount)
        //{
        //    await FlushBuffer();
        //}
        //}

        //private async Task FlushBuffer()
        //{
        //    if (_accelerometerReadings != null)
        //    {
        //        if (_accelerometerReadings.Any())
        //        {
        //            var recordsCurrent = _accelerometerReadings;

        //            _accelerometerReadings = new List<DbTrackPit>(AppConstant.SensorCacheCount);

        //            await new DbStorageService().TackPitInsert(recordsCurrent);

        //            recordsCurrent.Clear();
        //        }
        //    }
        //}
    }
}
