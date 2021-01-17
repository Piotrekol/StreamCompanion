using System.Diagnostics;

namespace StreamCompanion.Common
{
    public static class ProcessExt
    {
        public static Process OpenUrl(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            return Process.Start(psi);
        }
    }
}