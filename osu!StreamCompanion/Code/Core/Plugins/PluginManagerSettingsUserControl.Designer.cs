using StreamCompanionTypes.DataTypes;
using System.Windows.Forms;

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
            dataGridView_plugins = new DataGridView();
            typeNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            enabledDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            metadataDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            typeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            enabledForcefullyDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            pluginDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            pluginEntryBindingSource = new BindingSource(components);
            pluginEntryUserControl = new PluginEntryUserControl();
            panel_restart = new Panel();
            button_restart = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView_plugins).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pluginEntryBindingSource).BeginInit();
            panel_restart.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView_plugins
            // 
            dataGridView_plugins.AllowUserToAddRows = false;
            dataGridView_plugins.AllowUserToDeleteRows = false;
            dataGridView_plugins.AllowUserToResizeRows = false;
            dataGridView_plugins.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dataGridView_plugins.AutoGenerateColumns = false;
            dataGridView_plugins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_plugins.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridView_plugins.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_plugins.Columns.AddRange(new DataGridViewColumn[] { typeNameDataGridViewTextBoxColumn, enabledDataGridViewCheckBoxColumn, metadataDataGridViewTextBoxColumn, typeDataGridViewTextBoxColumn, enabledForcefullyDataGridViewCheckBoxColumn, pluginDataGridViewTextBoxColumn });
            dataGridView_plugins.DataSource = pluginEntryBindingSource;
            dataGridView_plugins.Location = new System.Drawing.Point(4, 0);
            dataGridView_plugins.Margin = new Padding(4, 3, 4, 3);
            dataGridView_plugins.Name = "dataGridView_plugins";
            dataGridView_plugins.RowHeadersVisible = false;
            dataGridView_plugins.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_plugins.ShowEditingIcon = false;
            dataGridView_plugins.Size = new System.Drawing.Size(433, 535);
            dataGridView_plugins.TabIndex = 1;
            // 
            // typeNameDataGridViewTextBoxColumn
            // 
            typeNameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            typeNameDataGridViewTextBoxColumn.HeaderText = "Name";
            typeNameDataGridViewTextBoxColumn.Name = "typeNameDataGridViewTextBoxColumn";
            typeNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // enabledDataGridViewCheckBoxColumn
            // 
            enabledDataGridViewCheckBoxColumn.DataPropertyName = "Enabled";
            enabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            enabledDataGridViewCheckBoxColumn.Name = "enabledDataGridViewCheckBoxColumn";
            enabledDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // metadataDataGridViewTextBoxColumn
            // 
            metadataDataGridViewTextBoxColumn.DataPropertyName = "Metadata";
            metadataDataGridViewTextBoxColumn.HeaderText = "Metadata";
            metadataDataGridViewTextBoxColumn.Name = "metadataDataGridViewTextBoxColumn";
            metadataDataGridViewTextBoxColumn.Visible = false;
            // 
            // typeDataGridViewTextBoxColumn
            // 
            typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
            typeDataGridViewTextBoxColumn.HeaderText = "Type";
            typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            typeDataGridViewTextBoxColumn.Visible = false;
            // 
            // enabledForcefullyDataGridViewCheckBoxColumn
            // 
            enabledForcefullyDataGridViewCheckBoxColumn.DataPropertyName = "EnabledForcefully";
            enabledForcefullyDataGridViewCheckBoxColumn.HeaderText = "EnabledForcefully";
            enabledForcefullyDataGridViewCheckBoxColumn.Name = "enabledForcefullyDataGridViewCheckBoxColumn";
            enabledForcefullyDataGridViewCheckBoxColumn.Visible = false;
            // 
            // pluginDataGridViewTextBoxColumn
            // 
            pluginDataGridViewTextBoxColumn.DataPropertyName = "Plugin";
            pluginDataGridViewTextBoxColumn.HeaderText = "Plugin";
            pluginDataGridViewTextBoxColumn.Name = "pluginDataGridViewTextBoxColumn";
            pluginDataGridViewTextBoxColumn.Visible = false;
            // 
            // pluginEntryBindingSource
            // 
            pluginEntryBindingSource.DataSource = typeof(LocalPluginEntry);
            // 
            // pluginEntryUserControl
            // 
            pluginEntryUserControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pluginEntryUserControl.LocalPluginEntry = null;
            pluginEntryUserControl.Location = new System.Drawing.Point(444, 0);
            pluginEntryUserControl.Name = "pluginEntryUserControl";
            pluginEntryUserControl.Size = new System.Drawing.Size(344, 535);
            pluginEntryUserControl.TabIndex = 2;
            // 
            // panel_restart
            // 
            panel_restart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel_restart.Controls.Add(button_restart);
            panel_restart.Controls.Add(label1);
            panel_restart.Location = new System.Drawing.Point(444, 479);
            panel_restart.Name = "panel_restart";
            panel_restart.Size = new System.Drawing.Size(344, 56);
            panel_restart.TabIndex = 3;
            panel_restart.Visible = false;
            // 
            // button_restart
            // 
            button_restart.Location = new System.Drawing.Point(3, 28);
            button_restart.Name = "button_restart";
            button_restart.Size = new System.Drawing.Size(75, 23);
            button_restart.TabIndex = 1;
            button_restart.Text = "Restart";
            button_restart.UseVisualStyleBackColor = true;
            button_restart.Click += button_restart_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(217, 25);
            label1.TabIndex = 0;
            label1.Text = "Restart to apply changes";
            // 
            // PluginManagerSettingsUserControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_restart);
            Controls.Add(pluginEntryUserControl);
            Controls.Add(dataGridView_plugins);
            Name = "PluginManagerSettingsUserControl";
            Size = new System.Drawing.Size(791, 535);
            ((System.ComponentModel.ISupportInitialize)dataGridView_plugins).EndInit();
            ((System.ComponentModel.ISupportInitialize)pluginEntryBindingSource).EndInit();
            panel_restart.ResumeLayout(false);
            panel_restart.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView_plugins;
        private PluginEntryUserControl pluginEntryUserControl;
        private BindingSource pluginEntryBindingSource;
        private DataGridViewTextBoxColumn typeNameDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn metadataDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn enabledForcefullyDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn pluginDataGridViewTextBoxColumn;
        private Panel panel_restart;
        private Button button_restart;
        private Label label1;
    }
}
