using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces.Services;

namespace BrowserOverlay
{
    public partial class BrowserOverlaySettings : UserControl
    {
        private readonly Configuration _configuration;
        public EventHandler<(string SettingName, object Value)?> SettingUpdated;
        private bool _init = true;
        private BindingList<OverlayTab> OverlayTabs;
        private OverlayTab CurrentOverlayTab = null;
        public BrowserOverlaySettings(Configuration configuration)
        {
            InitializeComponent();

            _configuration = configuration;
            OverlayTabs = new BindingList<OverlayTab>(_configuration.OverlayTabs);
            listBox_tabs.DataSource = OverlayTabs;

            panel_content.Enabled = checkBox_enable.Checked = configuration.Enabled;
            panel_form.Enabled = OverlayTabs.Any();

            _init = false;
        }

        private void textBox_overlayUrl_TextChanged(object sender, EventArgs e)
        {
            SettingValueUpdated();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            SettingValueUpdated();
        }

        private void SettingValueUpdated()
        {
            if (_init || CurrentOverlayTab == null)
                return;

            CurrentOverlayTab.Canvas.Height = Convert.ToInt32(numericUpDown_CanvasHeight.Value);
            CurrentOverlayTab.Canvas.Width = Convert.ToInt32(numericUpDown_CanvasWidth.Value);
            CurrentOverlayTab.Position.X = Convert.ToInt32(numericUpDown_positionX.Value);
            CurrentOverlayTab.Position.Y = Convert.ToInt32(numericUpDown_positionY.Value);
            CurrentOverlayTab.Url = textBox_overlayUrl.Text;
            CurrentOverlayTab.Scale = numericUpDown_scale.Value;
            OverlayTabs.ResetBindings();
            OnSettingUpdated();
        }

        private void checkBox_enable_CheckedChanged(object sender, EventArgs e)
        {
            if (_init)
                return;

            panel_content.Enabled = _configuration.Enabled = checkBox_enable.Checked;
            OnSettingUpdated("enable", checkBox_enable.Checked);
        }

        private void button_addTab_Click(object sender, EventArgs e)
        {
            OverlayTabs.AddNew();
            OnSettingUpdated();
        }

        private void button_remove_Click(object sender, EventArgs e)
        {
            if (listBox_tabs.SelectedItem == null)
                return;

            OverlayTabs.Remove((OverlayTab)listBox_tabs.SelectedItem);
            SettingUpdated?.Invoke(this, null);
        }

        private void listBox_tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentOverlayTab = (OverlayTab)listBox_tabs.SelectedItem;
            panel_form.Enabled = CurrentOverlayTab != null;
            if (panel_form.Enabled)
                PopulateForm(CurrentOverlayTab);

        }

        private void PopulateForm(OverlayTab overlayTab)
        {
            _init = true;
            numericUpDown_CanvasHeight.Value = overlayTab.Canvas.Height;
            numericUpDown_CanvasWidth.Value = overlayTab.Canvas.Width;
            numericUpDown_positionX.Value = overlayTab.Position.X;
            numericUpDown_positionY.Value = overlayTab.Position.Y;
            textBox_overlayUrl.Text = overlayTab.Url;
            numericUpDown_scale.Value = overlayTab.Scale;
            _init = false;
        }

        protected virtual void OnSettingUpdated()
        {
            SettingUpdated?.Invoke(this, null);
        }
        protected virtual void OnSettingUpdated(string settingName, object value)
        {
            SettingUpdated?.Invoke(this, (settingName, value));
        }
    }
}
