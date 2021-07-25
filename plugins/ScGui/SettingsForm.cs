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
        private List<(ISettingsSource settingsSource, UserControl Control)> _settingsInUse = new();
        private class UserControlPostion
        {
            public int StartWidth = 0;
            public int StartHeight;
        }

        private readonly Dictionary<string, UserControlPostion> _groupControlPostions = new();
        public SettingsForm(List<ISettingsSource> settingsList)
        {
            _settingsList = settingsList;
            InitializeComponent();
            //add predefined tabs
            AddTab("General", "Basic StreamCompanion settings");
            AddTab("Click counter", "Keyboard and mouse clicks tracker");

            //add tabs
            foreach (var settingGroupName in _settingsList.Select(x => new { x, x.SettingGroup }).Distinct())
            {
                var description = settingGroupName.x is IPlugin plugin
                    ? plugin.Description
                    : string.Empty;
                if (!_groupControlPostions.ContainsKey(settingGroupName.SettingGroup))
                    AddTab(settingGroupName.SettingGroup, description);
            }

            tvSettings.SelectedNode = tvSettings.Nodes[0];
            tvSettings.ExpandAll();
            PrepareCurrentTab();
        }

        private void PrepareCurrentTab()
        {
            var groupName = (string)tvSettings.SelectedNode.Tag;
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
                    pSettingTab.Controls.Add(control);
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
                    pSettingTab.Controls[0].Dock = DockStyle.Fill;

                _settingsInUse.Add((setting, control));
            }

            if (pSettingTab.Controls.Count == 0 && tvSettings.SelectedNode.Nodes.Count > 0)
            {
                tvSettings.SelectedNode = tvSettings.SelectedNode.Nodes[0];
            }
        }

        private void AddTab(string fullTabPath, string description, string tabPathRemaining = null, TreeNodeCollection parentNode = null)
        {
            if (string.IsNullOrEmpty(fullTabPath))
                throw new ArgumentNullException("Settings tab name can't be empty");

            tabPathRemaining ??= fullTabPath;
            string tabName, pathUntilNow;
            if (tabPathRemaining.Contains("__"))
            {
                var splitterIndex = tabPathRemaining.IndexOf("__", StringComparison.InvariantCulture);
                tabName = tabPathRemaining.Substring(0, splitterIndex);
                pathUntilNow = tabPathRemaining.Substring(0, splitterIndex);
                tabPathRemaining = tabPathRemaining.Substring(splitterIndex + 2);
            }
            else
            {
                tabName = tabPathRemaining;
                pathUntilNow = fullTabPath;
                tabPathRemaining = string.Empty;
            }

            if (!_groupControlPostions.ContainsKey(pathUntilNow))
                _groupControlPostions.Add(pathUntilNow, new());

            var mainNode = parentNode ?? tvSettings.Nodes;
            if (!mainNode.ContainsKey(tabName))
            {
                var node = mainNode.Add(pathUntilNow, tabName);
                node.Tag = pathUntilNow;
                node.ToolTipText = description;
            }

            if (tabPathRemaining.Length > 0)
                AddTab(fullTabPath, description, tabPathRemaining.TrimStart('_'), mainNode[pathUntilNow].Nodes);
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

        private void tvSettings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FreeControlsInUse();
            PrepareCurrentTab();
        }

        private void tvSettings_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
