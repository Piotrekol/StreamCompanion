using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace osuOverlayLoader
{
    public sealed class DllInjector
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

        DllInjector() { }

        public (DllInjectionResult InjectionResult, int errorCode) Inject(string sProcName, string sDllPath)
        {
            if (!File.Exists(sDllPath))
            {
                return (DllInjectionResult.DllNotFound, 1);
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
                return (DllInjectionResult.GameProcessNotFound, 2);
            }

            var loadedResult = IsAlreadyLoaded(sProcName, Path.GetFileName(sDllPath));

            if (loadedResult.LoadedResult == LoadedResult.Loaded)
                return (DllInjectionResult.Success, 0);

            if (loadedResult.LoadedResult == LoadedResult.Error)
                return (DllInjectionResult.InjectionFailed, -1);

            var injectionResult = bInject(_procId, sDllPath);
            if (!injectionResult.Success)
            {
                return (DllInjectionResult.InjectionFailed, injectionResult.ErrorCode);
            }

            return (DllInjectionResult.Success, 0);
        }
        public (LoadedResult LoadedResult, int ErrorCode) IsAlreadyLoaded(string procName, string dllName)
        {
            try
            {
                var processes = Process.GetProcessesByName(procName);
                Process osuProcess = null;
                foreach (var p in processes)
                {
                    if (p.ProcessName == procName)
                    {
                        osuProcess = p;
                        break;
                    }
                }

                if (osuProcess != null)
                {
                    foreach (ProcessModule pm in osuProcess.Modules)
                    {
                        if (pm.ModuleName == dllName)
                            return (LoadedResult.Loaded, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (LoadedResult.Error, 0);
            }

            return (LoadedResult.NotLoaded, 0);

        }
        (bool Success, int ErrorCode) bInject(uint pToBeInjected, string sDllPath)
        {
            IntPtr hndProc = OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, pToBeInjected);

            if (hndProc == INTPTR_ZERO)
            {
                return (false, 3);
            }

            IntPtr lpLLAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

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

            if (CreateRemoteThread(hndProc, (IntPtr)null, INTPTR_ZERO, lpLLAddress, lpAddress, 0, (IntPtr)null) == INTPTR_ZERO)
            {
                return (false, 7);
            }

            CloseHandle(hndProc);

            return (true, 0);
        }
    }

}