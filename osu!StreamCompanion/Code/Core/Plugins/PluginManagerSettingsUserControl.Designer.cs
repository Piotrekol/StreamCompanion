namespace osu_StreamCompanion.Code.Core.Plugins
{
    partial class PluginManagerSettingsUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            dataGridView_plugins = new System.Windows.Forms.DataGridView();
            nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            enabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            enabledForcefullyDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            pluginEntryBindingSource = new System.Windows.Forms.BindingSource(components);
            pluginEntryUserControl = new PluginEntryUserControl();
            ((System.ComponentModel.ISupportInitialize)dataGridView_plugins).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pluginEntryBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dataGridView_plugins
            // 
            dataGridView_plugins.AllowUserToAddRows = false;
            dataGridView_plugins.AllowUserToDeleteRows = false;
            dataGridView_plugins.AllowUserToResizeRows = false;
            dataGridView_plugins.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            dataGridView_plugins.AutoGenerateColumns = false;
            dataGridView_plugins.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_plugins.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridView_plugins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_plugins.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { nameDataGridViewTextBoxColumn, enabledDataGridViewCheckBoxColumn, enabledForcefullyDataGridViewCheckBoxColumn });
            dataGridView_plugins.DataSource = pluginEntryBindingSource;
            dataGridView_plugins.Location = new System.Drawing.Point(4, 0);
            dataGridView_plugins.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dataGridView_plugins.Name = "dataGridView_plugins";
            dataGridView_plugins.RowHeadersVisible = false;
            dataGridView_plugins.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView_plugins.ShowEditingIcon = false;
            dataGridView_plugins.Size = new System.Drawing.Size(433, 532);
            dataGridView_plugins.TabIndex = 1;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            nameDataGridViewTextBoxColumn.HeaderText = "Name";
            nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // enabledDataGridViewCheckBoxColumn
            // 
            enabledDataGridViewCheckBoxColumn.DataPropertyName = "Enabled";
            enabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            enabledDataGridViewCheckBoxColumn.Name = "enabledDataGridViewCheckBoxColumn";
            // 
            // enabledForcefullyDataGridViewCheckBoxColumn
            // 
            enabledForcefullyDataGridViewCheckBoxColumn.DataPropertyName = "EnabledForcefully";
            enabledForcefullyDataGridViewCheckBoxColumn.HeaderText = "EnabledForcefully";
            enabledForcefullyDataGridViewCheckBoxColumn.Name = "enabledForcefullyDataGridViewCheckBoxColumn";
            enabledForcefullyDataGridViewCheckBoxColumn.ReadOnly = true;
            enabledForcefullyDataGridViewCheckBoxColumn.Visible = false;
            // 
            // pluginEntryBindingSource
            // 
            pluginEntryBindingSource.DataSource = typeof(LocalPluginEntry);
            // 
            // pluginEntryUserControl
            // 
            pluginEntryUserControl.Location = new System.Drawing.Point(444, 0);
            pluginEntryUserControl.Name = "pluginEntryUserControl";
            pluginEntryUserControl.Size = new System.Drawing.Size(344, 532);
            pluginEntryUserControl.TabIndex = 2;
            pluginEntryUserControl.LocalPluginEntry = null;
            // 
            // PluginManagerSettingsUserControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(pluginEntryUserControl);
            Controls.Add(dataGridView_plugins);
            Name = "PluginManagerSettingsUserControl";
            Size = new System.Drawing.Size(791, 535);
            ((System.ComponentModel.ISupportInitialize)dataGridView_plugins).EndInit();
            ((System.ComponentModel.ISupportInitialize)pluginEntryBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_plugins;
        private System.Windows.Forms.BindingSource pluginEntryBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledForcefullyDataGridViewCheckBoxColumn;
        private PluginEntryUserControl pluginEntryUserControl;
    }
}
