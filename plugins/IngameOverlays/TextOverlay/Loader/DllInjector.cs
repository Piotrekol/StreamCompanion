using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace osuOverlay.Loader
{
    public class DllInjector
    {

        public enum LoadedResult
        {
            Loaded,
            NotLoaded,
            Error
        }
        static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        const UInt32 WAIT_ABANDONED = 0x00000080;
        const UInt32 WAIT_OBJECT_0 = 0x00000000;
        const UInt32 WAIT_TIMEOUT = 0x00000102;

        static DllInjector _instance;

        public static DllInjector GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DllInjector();
                }
                return _instance;
            }
        }

        protected DllInjector() { }

        public (DllInjectionResult InjectionResult, int errorCode, int Win32Error) Inject(string sProcName, string sDllPath)
        {
            if (!File.Exists(sDllPath))
            {
                return (DllInjectionResult.DllNotFound, 1, 0);
            }

            uint _procId = 0;

            Process[] _procs = Process.GetProcesses();
            for (int i = 0; i < _procs.Length; i++)
            {
                if (_procs[i].ProcessName == sProcName)
                {
                    _procId = (uint)_procs[i].Id;
                    break;
                }
            }

            if (_procId == 0)
            {
                return (DllInjectionResult.GameProcessNotFound, 2, 0);
            }

            var loadedResult = IsAlreadyLoaded(sProcName, Path.GetFileName(sDllPath));
            if (loadedResult.LoadedResult == LoadedResult.Loaded)
                return (DllInjectionResult.Success, 0, 0);

            if (loadedResult.LoadedResult == LoadedResult.Error)
                return (DllInjectionResult.InjectionFailed, -1, 0);

            var libAddress = GetLoadLibraryAAddress();
            if (libAddress == IntPtr.Zero)
                return (DllInjectionResult.HelperProcessFailed, -2, 0);

            var injectionResult = BInject(_procId, sDllPath, libAddress);
            if (!injectionResult.Success)
            {
                return (DllInjectionResult.InjectionFailed, injectionResult.ErrorCode, Marshal.GetLastWin32Error());
            }

            return (DllInjectionResult.Success, 0, 0);
        }

        private IntPtr GetLoadLibraryAAddress()
        {
            var file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Plugins", "Dlls", "X32ProcessOverlayHelper.exe");
            var process = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = file,
                Arguments = "proc LoadLibraryA"
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
                return IntPtr.Zero;

            var rawAddress = process.StandardOutput.ReadToEnd().Trim();
            return new IntPtr(Convert.ToInt32(rawAddress, 16));
        }

        public (LoadedResult LoadedResult, int ErrorCode) IsAlreadyLoaded(string procName, string dllName)
        {
            try
            {
                var moduleFound = ListModules(procName).Any(moduleName => moduleName == dllName);
                if (moduleFound)
                    return (LoadedResult.Loaded, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (LoadedResult.Error, 0);
            }

            return (LoadedResult.NotLoaded, 0);
        }

        public IEnumerable<string> ListModules(string procName)
        {
            var processes = Process.GetProcessesByName(procName);
            var process = processes.FirstOrDefault(p => p.ProcessName == procName);
            if (process != null)
            {
                foreach (var pm in process.X32BitModules())
                    yield return Path.GetFileName(pm.szExePath);
            }
        }

        protected virtual (bool Success, int ErrorCode) BInject(uint pToBeInjected, string sDllPath, IntPtr? LoadLibraryAAddress = null)
        {
            IntPtr hndProc = OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, pToBeInjected);

            if (hndProc == INTPTR_ZERO)
            {
                return (false, 3);
            }

            IntPtr lpLLAddress = LoadLibraryAAddress ?? GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (lpLLAddress == INTPTR_ZERO)
            {
                return (false, 4);
            }

            IntPtr lpAddress = VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)sDllPath.Length, (0x1000 | 0x2000), 0X40);

            if (lpAddress == INTPTR_ZERO)
            {
                return (false, 5);
            }

            byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);

            if (WriteProcessMemory(hndProc, lpAddress, bytes, (uint)bytes.Length, 0) == 0)
            {
                return (false, 6);
            }

            IntPtr threadHandle;
            if ((threadHandle = CreateRemoteThread(hndProc, (IntPtr)null, INTPTR_ZERO, lpLLAddress, lpAddress, 0, (IntPtr)null)) == INTPTR_ZERO)
            {
                return (false, 7);
            }

            var objectResult = WaitForSingleObject(threadHandle, 3000);
            if (objectResult == WAIT_ABANDONED || objectResult == WAIT_TIMEOUT)
                return (false, 100 + (int)objectResult);

            CloseHandle(hndProc);

            return (true, 0);
        }
    }
}