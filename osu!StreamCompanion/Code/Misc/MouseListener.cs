using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace osu_StreamCompanion.Code.Misc
{
    public class MouseListener : IDisposable
    {
        private LowLevelMouseProc _proc;
        private IntPtr _hookHandle;

        public event EventHandler OnLeftMouseDown;
        public event EventHandler OnRightMouseDown;
        
        ~MouseListener()
        {
            Dispose();
        }

        public void Hook()
        {
            _proc = MouseHookCallback;
            _hookHandle = SetMouseHook(_proc);
        }
        public void UnHook()
        {
            UnhookWindowsHookEx(_hookHandle);
        }
        public void Dispose()
        {
            UnHook();
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0) return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
            switch ((MouseMessages)wParam)
            {
                case MouseMessages.WM_LBUTTONDOWN:
                    OnLeftMouseDown?.Invoke(this, null);
                    break;
                case MouseMessages.WM_RBUTTONDOWN:
                    OnRightMouseDown?.Invoke(this, null);
                    break;
            }
            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }
        
        private IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_MOUSE_LL = 14;
        //private const int WH_MOUSE = 7;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            //WM_LBUTTONUP = 0x0202,
            //WM_MOUSEMOVE = 0x0200,
            //WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            //WM_RBUTTONUP = 0x0205
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    }

}
