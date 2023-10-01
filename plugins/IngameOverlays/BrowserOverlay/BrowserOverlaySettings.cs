using StreamCompanion.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace BrowserOverlay
{
    public partial class BrowserOverlaySettings : UserControl
    {
        private readonly Configuration _configuration;
        private readonly List<WebOverlay> _webOverlays;
        public EventHandler<(string SettingName, object Value)?> SettingUpdated;
        private bool _init = true;
        private BindingList<OverlayTab> OverlayTabs;
        private OverlayTab CurrentOverlayTab = null;
        private bool globalBordersEnabled = false;
        private WebOverlay _dummyWebOverlay = new WebOverlay { RelativePath = "Custom url" };

        public BrowserOverlaySettings(Configuration configuration, List<WebOverlay> webOverlays)
        {
            InitializeComponent();
            _configuration = configuration;
            _webOverlays = webOverlays;
            comboBox_localOverlays.DataSource = _webOverlays.Prepend(_dummyWebOverlay).ToList();

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
            UpdateWebOverlayElements(CurrentOverlayTab.Url);

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
            listBox_tabs.SelectedItem = OverlayTabs.AddNew();
            listBox_tabs_SelectedIndexChanged(listBox_tabs, null);
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
            if (CurrentOverlayTab != null)
                CurrentOverlayTab.Border = false;

            CurrentOverlayTab = (OverlayTab)listBox_tabs.SelectedItem;
            if (CurrentOverlayTab == null)
                return;

            CurrentOverlayTab.Border = true;

            panel_form.Enabled = CurrentOverlayTab != null;
            if (panel_form.Enabled)
                PopulateForm(CurrentOverlayTab);

            OnSettingUpdated();
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
            UpdateWebOverlayElements(overlayTab.Url);

            _init = false;
        }

        private void UpdateWebOverlayElements(string newUrl)
        {
            var webOverlay = _webOverlays.FirstOrDefault(o => o.URL == newUrl);
            if (webOverlay != null)
            {
                comboBox_localOverlays.SelectedItem = webOverlay;
                if (webOverlay.RecommendedSettings != null)
                {
                    groupBox_recommendedSettings.Visible = true;
                    label_recommendedSettings.Text = webOverlay.RecommendedSettings.ToUserReadableString();
                }
                else
                {
                    groupBox_recommendedSettings.Visible = false;
                }
            }
            else
            {
                comboBox_localOverlays.SelectedItem = _dummyWebOverlay;
                groupBox_recommendedSettings.Visible = false;
            }
        }

        protected virtual void OnSettingUpdated()
        {
            SettingUpdated?.Invoke(this, null);
        }
        protected virtual void OnSettingUpdated(string settingName, object value)
        {
            SettingUpdated?.Invoke(this, (settingName, value));
        }

        private void button_toggleBorders_Click(object sender, EventArgs e)
        {
            globalBordersEnabled = !globalBordersEnabled;
            foreach (var tab in _configuration.OverlayTabs)
            {
                tab.Border = globalBordersEnabled;
            }

            OnSettingUpdated();
        }

        private void comboBox_localOverlays_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init)
                return;

            var webOverlay = (WebOverlay)comboBox_localOverlays.SelectedItem;
            if (webOverlay != _dummyWebOverlay)
                textBox_overlayUrl.Text = webOverlay.URL;

            SettingValueUpdated();
        }

        private void button_applyRecommendedSettings_Click(object sender, EventArgs e)
        {
            var webOverlay = (WebOverlay)comboBox_localOverlays.SelectedItem;

            numericUpDown_CanvasHeight.Value = webOverlay.RecommendedSettings.Height;
            numericUpDown_CanvasWidth.Value = webOverlay.RecommendedSettings.Width;
            SettingValueUpdated();
        }
    }
}
