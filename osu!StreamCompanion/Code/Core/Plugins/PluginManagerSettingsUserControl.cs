using StreamCompanionTypes.Interfaces.Services;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public partial class PluginManagerSettingsUserControl : UserControl
    {
        private readonly PluginManagerConfiguration _configuration;
        private readonly Delegates.Restart _restart;
        private static bool RestartPending = false;

        public PluginManagerSettingsUserControl(PluginManagerConfiguration configuration, Delegates.Restart restart)
        {
            InitializeComponent();
            _configuration = configuration;
            _restart = restart;
            var list = _configuration.Plugins.ToList();
            list.Sort(new LocalPluginComparer());
            dataGridView_plugins.DataSource = list;
            dataGridView_plugins.DataBindingComplete += DataGridView_plugins_DataBindingComplete;
            dataGridView_plugins.RowEnter += DataGridView_plugins_RowEnter;
            pluginEntryUserControl.SettingChanged += PluginEntryUserControl_SettingChanged;
            panel_restart.Visible = RestartPending;
        }

        private void PluginEntryUserControl_SettingChanged(object sender, System.EventArgs e)
        {
            dataGridView_plugins.Refresh();
            panel_restart.Visible = RestartPending = true;
        }

        private void DataGridView_plugins_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            pluginEntryUserControl.LocalPluginEntry = (LocalPluginEntry)dataGridView_plugins.Rows[e.RowIndex].DataBoundItem;
        }

        private void DataGridView_plugins_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dataGridView_plugins.Rows.Count; i++)
            {
                var pluginEntry = (LocalPluginEntry)dataGridView_plugins.Rows[i].DataBoundItem;
                var row = dataGridView_plugins.Rows[i];
                if (pluginEntry.EnabledForcefully)
                {
                    row.ReadOnly = true;
                    var cell = row.Cells[1];
                    cell.Style.BackColor = Color.LightGray;
                    cell.Style.ForeColor = Color.DarkGray;
                    cell.ToolTipText = "This Plugin can not be disabled as it is either required by StreamCompanion or another plugin that is enabled";
                }
            }
        }

        private void button_restart_Click(object sender, System.EventArgs e)
        {
            _restart("Applying plugin changes");
        }
    }
}
