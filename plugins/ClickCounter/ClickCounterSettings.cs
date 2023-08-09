using System;
using System.Linq;
using System.Windows.Forms;

namespace ClickCounter
{
    public partial class ClickCounterSettings : UserControl
    {
        private readonly Configuration _configuration;
        private KeyboardCounterKeyClick keyboardCounterKeyClick;
        private KeyboardListener keyboardListener;
        public ClickCounterSettings(Configuration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
            checkBox_enableKeyboardHook.Checked = _configuration.KeyboardEnabled;
            dataGridView1.DataSource = _configuration.KeyEntries;
        }

        private void button_AddKey_Click(object sender, EventArgs e)
        {
            if (keyboardCounterKeyClick == null)
                keyboardCounterKeyClick = new KeyboardCounterKeyClick();
            keyboardCounterKeyClick.Location = new System.Drawing.Point(this.Location.X + 20, this.Location.Y + 20);
            keyboardListener = new KeyboardListener(true);
            keyboardListener.KeyDown += Kb_KeyDown;
            keyboardCounterKeyClick.Closing += KeyboardCounterKeyClick_Closing;
            keyboardCounterKeyClick.ShowDialog();

        }

        private void KeyboardCounterKeyClick_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keyboardListener.KeyDown -= Kb_KeyDown;
        }

        private void Kb_KeyDown(object sender, RawKeyEventArgs __args)
        {
            BeginInvoke((Configuration localConfig, RawKeyEventArgs args) =>
            {
                keyboardCounterKeyClick.Close();
                keyboardCounterKeyClick.Dispose();
                keyboardCounterKeyClick = null;
                var keyExists = localConfig.KeyEntries.Any(k => k.Code == args.VKCode);
                if (keyExists)
                    return;

                localConfig.KeyEntries.Add(new KeyEntry
                {
                    Code = args.VKCode,
                    Name = args.Key.ToString()
                });
            }, _configuration, __args);
        }

        private void button_RemoveKey_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
            {
                var keyEntry = (KeyEntry)item.DataBoundItem;
                if (keyEntry.Code == (int)MouseMessages.WM_LBUTTONDOWN || keyEntry.Code == (int)MouseMessages.WM_RBUTTONDOWN)
                    continue;

                _configuration.KeyEntries.RemoveAt(item.Index);
            }
        }
    }
}
