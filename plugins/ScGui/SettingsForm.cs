using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Sources;

namespace ScGui
{
    public partial class SettingsForm : Form
    {
        private readonly List<ISettingsSource> _settingsList;
        private List<(ISettingsSource settingsSource, UserControl Control)> _settingsInUse = new List<(ISettingsSource settingsSource, UserControl Control)>();
        private class UserControlPostion
        {
            public int TabNumber = -1;
            public int StartWidth = 0;
            public int StartHeight;
        }

        private readonly Dictionary<string, UserControlPostion> _groupControlPostions = new Dictionary<string, UserControlPostion>();
        public SettingsForm(List<ISettingsSource> settingsList)
        {
            _settingsList = settingsList;
            InitializeComponent();
            //add predefined tabs
            AddTab("General");
            AddTab("Click counter");
            AddTab("Map matching");

            //add tabs
            foreach (var settingGroupName in _settingsList.Select(x => x.SettingGroup).Distinct())
            {
                if (!_groupControlPostions.ContainsKey(settingGroupName))
                    AddTab(settingGroupName);
            }

            PrepareCurrentTab();
        }

        private void PrepareCurrentTab()
        {
            var tabNumber = this.tabControl.SelectedIndex;
            var groupName = tabControl.TabPages[this.tabControl.SelectedIndex].Text;
            foreach (var setting in _settingsList.Where(s => s.SettingGroup == groupName))
            {
                var control = (UserControl)setting.GetUiSettings();
                if (control == null)
                    continue;

                //set proper control postion
                control.Location = new Point(_groupControlPostions[setting.SettingGroup].StartWidth, _groupControlPostions[setting.SettingGroup].StartHeight);
                
                try
                {
                    //add control
                    tabControl.TabPages[tabNumber].Controls.Add(control);
                }
                catch (Win32Exception)
                {
                    MessageBox.Show(
                        $"Failed to initialize one of the controls in current tab.{Environment.NewLine}Move settings window to your primary monitor.{Environment.NewLine}If that doesn't work, review your windows DPI settings.",
                        "StreamCompanion - settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    FreeControlsInUse();
                    setting.Free();
                    return;
                }

                //change start postion for next control in that group
                _groupControlPostions[setting.SettingGroup].StartHeight += control.Height;
                if (setting.SettingGroup == "Tokens Preview")
                    tabControl.TabPages[tabNumber].Controls[0].Dock = DockStyle.Fill;

                _settingsInUse.Add((setting, control));
            }
        }

        private int AddTab(string groupName)
        {
            int tabNumber = _groupControlPostions.Count;
            _groupControlPostions.Add(groupName, new UserControlPostion() { TabNumber = tabNumber });
            tabControl.TabPages.Add(groupName);
            return tabNumber;
        }

        private void FreeControlsInUse()
        {
            foreach (var setting in _settingsInUse)
            {
                _groupControlPostions[setting.settingsSource.SettingGroup].StartHeight -= setting.Control.Height;

                setting.settingsSource.Free();
            }
            _settingsInUse.Clear();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FreeControlsInUse();
        }

        private void tabControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            FreeControlsInUse();
            PrepareCurrentTab();
        }
    }
}
