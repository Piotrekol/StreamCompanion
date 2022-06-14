using System;
using System.Drawing;

namespace ScGui
{
    public static class Helpers
    {
        public static Bitmap GetStreamCompanionLogo()
        {
            if (!OperatingSystem.IsWindows())
                return null;

            return new Bitmap(
                System.Reflection.Assembly.GetEntryAssembly().
                    GetManifestResourceStream("osu_StreamCompanion.Resources.logo_256x256.png"));
        }
    }
}