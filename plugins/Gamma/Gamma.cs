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
        public float CurrentGamma { get; private set; }
        public Gamma(string screenDeviceName)
        {
            createdDC = WinApi.CreateDC(null, screenDeviceName, null, IntPtr.Zero);
        }

        public bool Set(float gamma)
        {
            if (_orginalRamp == null && !TrySetDefaultRamp())
                return false;

            if (gamma <= 5 && gamma >= 0)
            {
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

            return false;
        }

        public bool Restore()
        {
            if (_orginalRamp == null)
                return false;
            
            var ramp = _orginalRamp.Value;
            return WinApi.SetDeviceGammaRamp(createdDC, ref ramp);
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
