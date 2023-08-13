using System.Drawing;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public partial class PluginManagerSettingsUserControl : UserControl
    {
        private readonly PluginManagerConfiguration _configuration;

        public PluginManagerSettingsUserControl(PluginManagerConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
            dataGridView_plugins.DataSource = _configuration.Plugins;
            dataGridView_plugins.DataBindingComplete += DataGridView_plugins_DataBindingComplete;
            dataGridView_plugins.RowEnter += DataGridView_plugins_RowEnter;
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
    }
}
