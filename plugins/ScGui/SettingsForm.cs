using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Sources;

namespace ScGui
{
    public partial class SettingsForm : Form
    {
        private readonly List<ISettingsSource> _settingsList;

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

            //add UserControls and tabs
            foreach (var setting in _settingsList)
            {
                int tabNumber;
                var control = (UserControl)setting.GetUiSettings();
                if(control==null)
                    continue;
                //If group tab doesn't exist - create it
                if (!_groupControlPostions.ContainsKey(setting.SettingGroup))
                {
                    tabNumber = AddTab(setting.SettingGroup);
                }
                else
                    tabNumber = _groupControlPostions[setting.SettingGroup].TabNumber;

                //get control to add
                //set proper control postion
                control.Location = new Point(_groupControlPostions[setting.SettingGroup].StartWidth, _groupControlPostions[setting.SettingGroup].StartHeight);
                //add control
                tabControl.TabPages[tabNumber].Controls.Add(control);
                //change start postion for next control in that group
                _groupControlPostions[setting.SettingGroup].StartHeight += control.Height;
                if (setting.SettingGroup == "Tokens Preview")
                    tabControl.TabPages[tabNumber].Controls[0].Dock = DockStyle.Fill;
            }
        }

        private int AddTab(string groupName)
        {
            int tabNumber = _groupControlPostions.Count;
            _groupControlPostions.Add(groupName, new UserControlPostion() { TabNumber = tabNumber });
            tabControl.TabPages.Add(groupName);
            return tabNumber;
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var setting in _settingsList)
            {
               setting.Free();
            }
        }
    }
}
