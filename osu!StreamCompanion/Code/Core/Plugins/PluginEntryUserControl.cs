using StreamCompanion.Common;
using StreamCompanionTypes.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public partial class PluginEntryUserControl : UserControl
    {
        private Size _minimumElementSize = new Size(100, 20);

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
                _pluginEntry = value;
                PopulatePluginDetails(value?.Metadata);
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

        private void PopulatePluginDetails(IPluginMetadata pluginMetadata)
        {
            flowLayoutPanel1.Controls.Clear();
            SetButtonStates();

            if (pluginMetadata == null)
            {
                flowLayoutPanel1.Controls.Add(GetLabel($"Plugin is missing {nameof(SCPluginAttribute)} on its class!"));
                flowLayoutPanel1.Controls.Add(GetLabel($"TODO: pending plugin repo/metadata impl."));
                flowLayoutPanel1.Controls.Add(_buttonPanel);
                return;
            }

            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(GetLabel($"Name: {pluginMetadata.Name}"));
            flowLayoutPanel1.Controls.Add(GetLabel($"Authors: {pluginMetadata.Authors}"));
            flowLayoutPanel1.Controls.Add(GetLabel($"Description: {pluginMetadata.Description}"));
            flowLayoutPanel1.Controls.Add(GetLinkButton($"Project page", pluginMetadata.ProjectURL));
            flowLayoutPanel1.Controls.Add(GetLinkButton($"Wiki page", pluginMetadata.WikiUrl));
            if (_pluginEntry.EnabledForcefully)
            {
                flowLayoutPanel1.Controls.Add(GetLabel($"Required by: {string.Join(", ", _pluginEntry.EnabledForcefullyByPlugins.Select(p => p.Name))}"));
            }

            flowLayoutPanel1.Controls.Add(_buttonPanel);
        }

        private void SetButtonStates()
        {
            if (_pluginEntry == null)
            {
                _disableButton.Enabled = false;
                _enableButton.Enabled = false;
            }
            else
            {
                _disableButton.Enabled = !_pluginEntry.EnabledForcefully && _pluginEntry.Enabled;
                _enableButton.Enabled = !_pluginEntry.Enabled;
            }
        }

        private Label GetLabel(string text)
        {
            return new Label { Text = text, AutoSize = true, UseMnemonic = false, MinimumSize = _minimumElementSize };
        }

        private Button GetLinkButton(string text, string url)
        {
            var button = new Button { Text = text, AutoSize = true, UseMnemonic = false, MinimumSize = _minimumElementSize, Tag = url };
            if (url == null)
            {
                button.Enabled = false;
            }
            else
            {
                button.Click += Button_Click;
            }

            return button;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Tag is not string url)
                return;

            ProcessExt.OpenUrl(url);
        }
    }
}
