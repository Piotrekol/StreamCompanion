using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Modules.Logger
{
    public partial class LoggerSettingsUserControl : UserControl
    {
        private readonly ISettings settings;
        private readonly SettingNames _names = SettingNames.Instance;
        public Dictionary<string, LogLevel> LogLevels = new Dictionary<string, LogLevel>
        {
            { "Disabled", LogLevel.Disabled },
            { "Basic", LogLevel.Basic },
            { "Advanced", LogLevel.Advanced },
            { "Debug", LogLevel.Debug },
            { "Error", LogLevel.Error }
        };

        public LoggerSettingsUserControl(ISettings settings)
        {
            this.settings = settings;
            InitializeComponent();
            this.comboBox_logVerbosity.DataSource = LogLevels.Select(v => v.Key).ToList();
            var logLevel = settings.Get<int>(_names.LogLevel);
            this.comboBox_logVerbosity.SelectedItem = LogLevels.FirstOrDefault(x => x.Value.GetHashCode() == logLevel).Key;
            this.checkBox_consoleLogger.Checked = settings.Get<bool>(_names.Console);

            this.comboBox_logVerbosity.SelectedIndexChanged += ComboBox_logVerbosityOnSelectedIndexChanged;
            this.checkBox_consoleLogger.CheckedChanged += CheckBox_consoleLoggerOnCheckedChanged;
        }

        private void CheckBox_consoleLoggerOnCheckedChanged(object sender, EventArgs e)
        {
            settings.Add(_names.Console.Name, checkBox_consoleLogger.Checked, true);
        }

        private void ComboBox_logVerbosityOnSelectedIndexChanged(object sender, EventArgs e)
        {
            settings.Add(_names.LogLevel.Name, LogLevels.First(x => x.Key == comboBox_logVerbosity.SelectedItem.ToString()).Value.GetHashCode());
        }
    }
}
