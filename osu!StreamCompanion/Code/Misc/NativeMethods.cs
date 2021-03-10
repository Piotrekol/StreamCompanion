using System;
using System.Runtime.InteropServices;

namespace osu_StreamCompanion.Code.Misc
{
    internal static class NativeMethods
    {
        [DllImport("kernel32")]
        public static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        public const int WM_SETREDRAW = 11;
    }
}