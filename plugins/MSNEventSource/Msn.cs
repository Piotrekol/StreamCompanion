using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace MSNEventSource
{
    public class Msn : IDisposable, IPlugin, IOsuEventSource, IFirstRunControlProvider
    {
        private IntPtr m_hwnd;
        private Dictionary<string, string> _osuStatus = new Dictionary<string, string>();
        private static WNDCLASS lpWndClass;
        private ILogger _logger;
        private const string lpClassName = "MsnMsgrUIManager";
        public bool Suspend { get; set; }
        private string _lastMsnString = "";

        public string Description { get; } = "Provides basic osu! events using old MSN communication that still exists in osu!";
        public string Name { get; } = nameof(Msn);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public EventHandler<IMapSearchArgs> NewOsuEvent { get; set; }

        private static WndProc WndProcc;

        public Msn(ILogger logger)
        {
            _logger = logger;
            if (WndProcc != null)
                return;
            WndProcc = CustomWndProc;
            lpWndClass = new WNDCLASS
            {
                lpszClassName = lpClassName,
                lpfnWndProc = WndProcc
            };

            ushort num = RegisterClassW(ref lpWndClass);
            int num2 = Marshal.GetLastWin32Error();
            if ((num == 0) && (num2 != 0x582))
            {
                throw new Exception("Could not register window class");
            }
            this.m_hwnd = CreateWindowExW(0, lpClassName, string.Empty, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        protected virtual void OnMsnStringChanged()
        {
            if (Suspend)
                return;
            Task.Factory.StartNew<int>(() =>
            {
                _firstRunUserControl?.GotMsn(string.Format("{0} - {1}", _osuStatus["title"], _osuStatus["artist"]));

                var args = CreateArgs(_osuStatus);

                if(args != null)
                    NewOsuEvent?.Invoke(this, args);

                return 1;
            });
        }

        private MapSearchArgs CreateArgs(Dictionary<string, string> osuStatus)
        {
            OsuStatus status = osuStatus["status"] == "Listening" ? OsuStatus.Listening
                : osuStatus["status"] == "Playing" ? OsuStatus.Playing
                    : osuStatus["status"] == "Watching" ? OsuStatus.Watching
                        : osuStatus["status"] == "Editing" ? OsuStatus.Editing
                            : OsuStatus.Null;

            osuStatus["raw"] = string.Format("{0} - {1}", osuStatus["title"], osuStatus["artist"]);
            bool isFalsePlay;
            lock (this)
            {
                isFalsePlay = IsFalsePlay(osuStatus["raw"], status, _lastMsnString);
            }
            if (isFalsePlay)
            {
                _logger?.Log(">ignoring second MSN string...", LogLevel.Advanced);
            }
            else
            {
                _lastMsnString = osuStatus["raw"];
                _logger?.Log("", LogLevel.Advanced);
                string result = ">Got ";
                foreach (var v in osuStatus)
                {
                    if (v.Key != "raw") result = result + $"{v.Key}: \"{v.Value}\" ";
                }
                _logger?.Log(result, LogLevel.Basic);

                var searchArgs = new MapSearchArgs("Msn", OsuEventType.MapChange)
                {
                    Artist = osuStatus["artist"] ?? "",
                    Title = osuStatus["title"] ?? "",
                    Diff = osuStatus["diff"] ?? "",
                    Raw = osuStatus["raw"] ?? "",
                    Status = status,

                };
                return searchArgs;
            }
            return null;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateWindowExW(uint dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        private IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == 0x4a)
            {
                COPYDATASTRUCT copydatastruct =
                    (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));

                var ptr = copydatastruct.lpData;
                if (ptr != IntPtr.Zero)
                {
                    string str = Marshal.PtrToStringUni(ptr, copydatastruct.cbData / 2);
                    string[] separator = new string[] { @"\0" };
                    string[] sourceArray = str.Split(separator, StringSplitOptions.None);
                    if (sourceArray.Length > 8)
                    {
                        _osuStatus["artist"] = sourceArray[5];
                        _osuStatus["title"] = sourceArray[4];
                        _osuStatus["diff"] = sourceArray[7];
                        _osuStatus["status"] = sourceArray[3].Split(new[] { ' ' }, 2)[0];

                        OnMsnStringChanged();
                    }
                }
            }
            return DefWindowProcW(hWnd, msg, wParam, lParam);

        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyWindow(IntPtr hWnd);
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.m_hwnd != IntPtr.Zero)
            {
                DestroyWindow(this.m_hwnd);
                this.m_hwnd = IntPtr.Zero;
            }

        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern ushort RegisterClassW([In] ref WNDCLASS lpWndClass);

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WNDCLASS
        {
            public uint style;
            public Msn.WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszClassName;
        }

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


        #region MSN double-send fix
        public class MapArgs
        {
            public string MapName;
            public OsuStatus MapAction;
        }
        //osu! MSN double-send detection
        private readonly string[] _lastListened = new string[2];
        bool IsFalsePlay(string msnString, OsuStatus msnStatus, string lastMapString)
        {
            lock (_lastListened)
            {
                // if we're listening to a song AND it's not already in the first place of our Queue
                if (msnStatus == OsuStatus.Listening && msnString != _lastListened[0])
                {
                    //first process our last listened song "Queue" 
                    _lastListened[1] = _lastListened[0];
                    _lastListened[0] = msnString;
                }
                //we have to be playing for bug to occour...
                if (msnStatus != OsuStatus.Playing)
                    return false;
                //if same string is sent 2 times in a row
                if (msnString == lastMapString)
                {
                    //this is where it gets checked for actual bug- Map gets duplicated only when we just switched from another song
                    //so check if we switched by checking if last listened song has changed 
                    if (_lastListened[0] != _lastListened[1])
                    {
                        //to avoid marking another plays(Retrys) as False- we "break" our Queue until we change song.
                        _lastListened[1] = _lastListened[0];
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion //MSN FIX

        private FirstRunMsn _firstRunUserControl = null;
        public List<IFirstRunControl> GetFirstRunUserControls()
        {
            _firstRunUserControl = new FirstRunMsn();
            return new List<IFirstRunControl> { _firstRunUserControl };
        }
        
    }
}
