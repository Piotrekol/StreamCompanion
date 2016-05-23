using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.ClickCounter
{
    public partial class ClickCounterSettings : UserControl
    {
        private Settings _settings;
        private KeyboardCounterKeyClick keyboardCounterKeyClick;
        public event EventHandler KeysChanged;
        public ClickCounterSettings(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            RefreshDataGrid();
            groupBox_Mouse.Visible = checkBox_enableMouseHook.Checked;
            checkBox_enableMouseHook.CheckedChanged += CheckBox_enableMouseHook_CheckedChanged;
        }

        private void CheckBox_enableMouseHook_CheckedChanged(object sender, EventArgs e)
        {
            groupBox_Mouse.Visible = checkBox_enableMouseHook.Checked;
        }

        public void RefreshDataGrid()
        {
            this.dataGridView1.Rows.Clear();
            var keys = _settings.Get("keyList");
            var keysSaves = _settings.Get("keyNames");
            var keysCounts = _settings.Geti("keyCounts");
            
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == string.Empty) continue;
                System.Windows.Input.Key key = KeyInterop.KeyFromVirtualKey(Convert.ToInt32(keys[i]));

                this.dataGridView1.Rows.Add(key, keysCounts[i], keysSaves[i]);
            }
        }

        private KeyboardListener keyboardListener;
        private void button_AddKey_Click(object sender, EventArgs e)
        {
            if (keyboardCounterKeyClick == null)
                keyboardCounterKeyClick = new KeyboardCounterKeyClick();
            keyboardCounterKeyClick.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            keyboardListener = new KeyboardListener(true);
            keyboardListener.KeyDown += Kb_KeyDown;
            keyboardCounterKeyClick.Closing += KeyboardCounterKeyClick_Closing;
            keyboardCounterKeyClick.ShowDialog();

        }

        private void KeyboardCounterKeyClick_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keyboardListener.KeyDown -= Kb_KeyDown;
        }

        private void Kb_KeyDown(object sender, RawKeyEventArgs args)
        {
            var keys = _settings.Get("keyList");

            bool exists = false;
            foreach (var key in keys)
            {
                if (key.Contains(args.VKCode.ToString()))
                {
                    exists = true;
                    break;
                }
            }


            BeginInvoke((MethodInvoker)(() =>
           {
               keyboardCounterKeyClick.Close();
               keyboardCounterKeyClick.Dispose();
               keyboardCounterKeyClick = null;
               if (!exists)
               {
                   //get file name to save
                   var filenameFrm = new ClickCounterFileName();
                   filenameFrm.ShowDialog();
                   string filename = filenameFrm.textBox_FileName.Text;
                   filenameFrm.Dispose();

                   //check if filename is valid
                   string invalidChars = new string(Path.GetInvalidFileNameChars());
                   foreach (var invalidChar in invalidChars)
                   {
                       if (filename.Contains(invalidChar))
                           return;
                   }


                   if (filename.Trim() != string.Empty)
                   {
                       if (!filename.EndsWith(".txt"))
                           filename += ".txt";

                       AddKey(args.VKCode, filename.Trim());
                       this.dataGridView1.Rows.Add(args.Key, "0", filename.Trim());
                   }
               }
           }));
        }

        private void AddKey(int key, string filename)
        {
            var currentKeys = _settings.Get("keyList");
            var currentKeysSaves = _settings.Get("keyNames");
            var currentKeyCounts = _settings.Geti("keyCounts");

            currentKeys.Add(key.ToString());
            currentKeysSaves.Add(filename);
            currentKeyCounts.Add(0);

            _settings.Add("keyList",currentKeys,true);
            _settings.Add("keyNames", currentKeysSaves,true);
            _settings.Add("keyCounts", currentKeyCounts,true);
            
            OnKeysChanged();
        }

        private void RemoveKey(int keyIndex)
        {

            var keys = _settings.Get("keyList");
            var keysSaves = _settings.Get("keyNames");
            var keysCounts = _settings.Geti("keyCounts");

            keys.RemoveAt(keyIndex);
            keysSaves.RemoveAt(keyIndex);
            keysCounts.RemoveAt(keyIndex);

            _settings.Add("keyList", keys, true);
            _settings.Add("keyNames",keysSaves, true);
            _settings.Add("keyCounts",keysCounts, true);
        }
        private void OnKeysChanged()
        {
            KeysChanged?.Invoke(this, null);
        }

        private void button_RemoveKey_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
            {
                RemoveKey(item.Index);
                dataGridView1.Rows.RemoveAt(item.Index);
            }
        }

        public void SetRightMouseCount(long count)
        {
            label_MouseRight.Text = count.ToString();
        }
        public void SetLeftMouseCount(long count)
        {
            label_MouseLeft.Text = count.ToString();
        }
    }
}
