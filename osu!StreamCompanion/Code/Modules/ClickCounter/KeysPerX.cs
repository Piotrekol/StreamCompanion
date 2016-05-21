using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace osu_StreamCompanion.Code.Modules.ClickCounter
{
    public partial class KeysPerX : Form
    {
        public KeysPerX()
        {
            InitializeComponent();
            _timer.Elapsed += Timer_Elapsed;
            _timer.SynchronizingObject = this.label1;
        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void KeysPerX_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void SetKeys(string keys)
        {
            label1.Text = keys;
        }
        private readonly Timer _timer = new Timer();

        public void AddToKeys()
        {
            lock (this)
            {
                _pressedInCurrentLoop++;
            }
        }

        public void Stop()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                this.Hide();
            }
        }

        public void Start(int timeSpan = 5)
        {
            if (_timer.Enabled)
                return;
            this.Show();
            _timeRange = timeSpan;//in seconds
            _loops = _timeRange * 2; //times 2 because we probe each 500ms
            _timer.Interval = 500;
            _initialCount = 1;
            _pressedInCurrentLoop = 0;
            _pressedInHistory = 0;
            _pressHistory.Clear();
            _loopNr = 0;
            for (int i = 0; i < _loops; i++)
            {
                _pressHistory.Add(0);
            }
           _timer.Start();
            
        }
        private int _initialCount = 1;
        private int _timeRange;
        private int _loops;
        private int _pressedInCurrentLoop;
        private int _pressedInHistory;
        private int _loopNr;
        readonly List<int> _pressHistory = new List<int>();
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _pressedInHistory -= _pressHistory[_loopNr];
            _pressHistory[_loopNr] = _pressedInCurrentLoop;
            _pressedInHistory += _pressedInCurrentLoop;
            var result = (float)_pressedInHistory / _initialCount;
            _pressedInCurrentLoop = 0;
            SetKeys(result.ToString(CultureInfo.InvariantCulture));
            _loopNr++;
            if (_loopNr == _loops)
                _loopNr = 0;
            if (_initialCount < _timeRange)
                _initialCount++;
        }

        
    }
}
