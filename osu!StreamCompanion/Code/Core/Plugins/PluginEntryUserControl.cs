using System;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public partial class PluginEntryUserControl : UserControl
    {

        private Button _disableButton = new()
        {
            Text = "Disable"
        };
        private Button _enableButton = new()
        {
            Text = "Enable"
        };
        private Panel _buttonPanel;

        private LocalPluginEntry _pluginEntry;
        public LocalPluginEntry LocalPluginEntry
        {
            get => _pluginEntry;
            set
            {
                if (value == null)
                    return;

                _pluginEntry = value;
                Populate();
            }
        }

        public PluginEntryUserControl()
        {
            InitializeComponent();
            _disableButton.Click += _disableButton_Click;
            _enableButton.Click += _EnableButton_Click;
            _buttonPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.LeftToRight,
                Controls = { _disableButton, _enableButton },
            };
        }

        private void _EnableButton_Click(object sender, EventArgs e)
        {
            _pluginEntry.Enabled = true;
            SetButtonStates();
        }

        private void _disableButton_Click(object sender, EventArgs e)
        {
            _pluginEntry.Enabled = false;
            SetButtonStates();
        }


        private void Populate()
        {
            flowLayoutPanel1.Controls.Clear();
            SetButtonStates();
            if (_pluginEntry == null)
            {
                return;
            }
            if (_pluginEntry.Plugin == null)
            {
                flowLayoutPanel1.Controls.Add(GetLabel($"TODO: pending plugin repo/metadata impl."));
                flowLayoutPanel1.Controls.Add(_buttonPanel);
                return;
            }

            var plugin = _pluginEntry.Plugin;
            flowLayoutPanel1.Controls.Add(GetLabel($"Name: {plugin.Name}"));
            flowLayoutPanel1.Controls.Add(GetLabel($"Author: {plugin.Author}"));
            flowLayoutPanel1.Controls.Add(GetLabel($"Description: {plugin.Description}"));
            flowLayoutPanel1.Controls.Add(GetLabel($"Url: {plugin.Url}"));
            flowLayoutPanel1.Controls.Add(_buttonPanel);
        }

        private void SetButtonStates()
        {
            _disableButton.Enabled = !_pluginEntry.EnabledForcefully && _pluginEntry.Enabled;
            _enableButton.Enabled = !_pluginEntry.Enabled;
        }
        private Label GetLabel(string text)
        {
            return new Label { Text = text, AutoSize = true };
        }
    }
}
