using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace MobileSRC.MobileRemote
{
    internal interface IGSensor : IDisposable
    {
        AccelerationVector GetAccelerationVector();
        bool IsEnabled
        {
            get;
        }
    }

    internal struct AccelerationVector
    {
        public double X, Y, Z;

        public AccelerationVector(double x, double y, double z, double scaleFactor)
        {
            X = x * scaleFactor;
            Y = y * scaleFactor;
            Z = z * scaleFactor;
        }
    }

    internal class GSensor : IGSensor
    {
        private struct HTCGSensorData
        {
            public short TiltX;          // From -1000 to 1000 (about), 0 is flat
            public short TiltY;          // From -1000 to 1000 (about), 0 is flat
            public short TiltZ;          // From -1000 to 1000 (about), 0 = Straight up, -1000 = Flat, 1000 = Upside down
            public short Unknown1;       // Always zero
            public int AngleY;           // From 0 to 359
            public int AngleX;           // From 0 to 359
            public int Unknown2;         // Bit field?
        };

        private enum HTCSensor : uint
        {
            Something = 0,
            GSensor = 1,
            Light = 2,
            Another = 3,
        }

        [DllImport("HTCSensorSDK")]
        private extern static IntPtr HTCSensorGetDataOutput(IntPtr handle, out HTCGSensorData sensorData);

        [DllImport("HTCSensorSDK")]
        private extern static IntPtr HTCSensorOpen(HTCSensor sensor);

        [DllImport("HTCSensorSDK")]
        private extern static void HTCSensorClose(IntPtr handle);

        private IntPtr _hGSensor = IntPtr.Zero;

        public GSensor()
        {
            try
            {
                _hGSensor = HTCSensorOpen(HTCSensor.GSensor);
            }
            catch
            {
                _hGSensor = IntPtr.Zero;
            }
                
        }

        public bool IsEnabled
        {
            get { return _hGSensor != IntPtr.Zero; }
        }

        public void Dispose()
        {
            if (IntPtr.Zero != _hGSensor)
            {
                HTCSensorClose(_hGSensor);
                _hGSensor = IntPtr.Zero;
            }
        }

        private HTCGSensorData GetRawSensorData()
        {
            HTCGSensorData data;
            HTCSensorGetDataOutput(_hGSensor, out data);
            return data;
        }

        public AccelerationVector GetAccelerationVector()
        {
            HTCGSensorData data = GetRawSensorData();
            double htcScaleFactor = 1.0 / 1000.0 * 9.8;

            // Depending on our orientation, this needs to behave differently
            int orientation = Convert.ToInt32(Microsoft.WindowsMobile.Status.SystemState.GetValue(Microsoft.WindowsMobile.Status.SystemProperty.DisplayRotation));

            switch (orientation)
            {
                case 90:
                    {
                        return new AccelerationVector(-data.TiltY, data.TiltX, data.TiltZ, htcScaleFactor);
                    }
                case 180:
                    {
                        return new AccelerationVector(-data.TiltX, -data.TiltY, data.TiltZ, htcScaleFactor);
                    }
                case 270:
                    {
                        return new AccelerationVector(data.TiltY, -data.TiltX, data.TiltZ, htcScaleFactor);
                    }
                default:
                    {
                        return new AccelerationVector(data.TiltX, data.TiltY, data.TiltZ, htcScaleFactor);
                    }
            }
        }
    }
}
