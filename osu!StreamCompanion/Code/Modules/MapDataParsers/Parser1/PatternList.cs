using System;
using System.ComponentModel;
using System.Windows.Forms;
using StreamCompanionTypes.DataTypes;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class PatternList : UserControl
    {
        public EventHandler<OutputPattern> SelectedItemChanged;

        public bool InGameOverlayIsAvailable;

        public PatternList()
        {
            InitializeComponent();
            dataGridView.SelectionChanged += DataGridViewOnSelectionChanged;
        }

        private void DataGridViewOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            OutputPattern item = null;
            if (dataGridView.SelectedRows.Count > 0)
                item = (OutputPattern)dataGridView.SelectedRows[0].DataBoundItem;
            SelectedItemChanged?.Invoke(sender, item);
        }

        public void SetPatterns(BindingList<OutputPattern> patterns)
        {
            dataGridView.DataSource = patterns;
            dataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (!InGameOverlayIsAvailable && dataGridView?.Columns["ShowInOsu"] != null)
                dataGridView.Columns["ShowInOsu"].Visible = false;

            if (dataGridView.Columns.Contains("Replacements") && dataGridView.Columns["Replacements"] != null)
                dataGridView.Columns["Replacements"].Visible = false;

        }
    }
}
