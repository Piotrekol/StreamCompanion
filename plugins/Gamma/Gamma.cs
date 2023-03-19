using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gamma
{
    [SupportedOSPlatform("windows")]
    public class Gamma : IDisposable
    {
        private IntPtr createdDC;
        private WinApi.RAMP? _orginalRamp;
        public double CurrentGamma { get; private set; } = double.NaN;
        public string ScreenDeviceName { get; private set; }
        public Gamma(string screenDeviceName)
        {
            ScreenDeviceName = screenDeviceName;
            createdDC = WinApi.CreateDC(null, screenDeviceName, null, IntPtr.Zero);
        }

        public bool ScreenIsValid()
            => createdDC != IntPtr.Zero;

        public bool Set(int userGamma)
        {
            if (_orginalRamp == null && !TrySetDefaultRamp())
                return false;

            var gamma = UserValueToGamma(userGamma);
            WinApi.RAMP ramp = new WinApi.RAMP();
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];
            for (int i = 1; i < 256; i++)
            {
                var iArrayValue = Math.Pow((i + 1) / 256.0, gamma) * 65535 + 0.5;
                if (iArrayValue > 65535)
                    iArrayValue = 65535;
                ramp.Red[i] = ramp.Blue[i] = ramp.Green[i] = (ushort)iArrayValue;
            }

            CurrentGamma = gamma;
            return WinApi.SetDeviceGammaRamp(createdDC, ref ramp);
        }

        public bool Restore()
        {
            if (_orginalRamp == null)
                return false;

            var ramp = _orginalRamp.Value;
            var restored = WinApi.SetDeviceGammaRamp(createdDC, ref ramp);
            if (restored)
                CurrentGamma = float.NaN;

            return restored;
        }

        public void Dispose()
        {
            Restore();
            if (createdDC != IntPtr.Zero)
                WinApi.DeleteDC(createdDC);
        }

        private bool TrySetDefaultRamp()
        {
            var ramp = new WinApi.RAMP();
            if (!WinApi.GetDeviceGammaRamp(createdDC, ref ramp))
                return false;

            _orginalRamp = ramp;
            return true;
        }

        private static double Map(double value, double range1Start, double range1End, double range2Start, double range2End)
        {
            return (value - range1Start) / (range1End - range1Start) * (range2End - range2Start) + range2Start;
        }

        //user range 0 - 100
        //actual gamma range: 0.228 - 4.46
        public static double UserValueToGamma(int value) => Math.Clamp(Map(value, 100, 0, 0.228, 4.46), 0.228, 4.46);
        public static int GammaToUserValue(float value) => Math.Clamp((int)Map(value, 0.228, 4.46, 100, 0), 0, 100);

        private class WinApi
        {
            [DllImport("gdi32.dll")]
            public static extern bool SetDeviceGammaRamp(IntPtr hdc, ref RAMP lpRamp);

            [DllImport("gdi32.dll")]
            public static extern bool GetDeviceGammaRamp(IntPtr hdc, ref RAMP lpRamp);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice,
                string lpszOutput, IntPtr lpInitData);
            [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
            public static extern bool DeleteDC([In] IntPtr hdc);
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct RAMP
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
                public UInt16[] Red;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
                public UInt16[] Green;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
                public UInt16[] Blue;
            }
        }
    }
}
