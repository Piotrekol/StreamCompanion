
namespace Gamma
{
    partial class GammaSettings
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.checkBox_enabled = new System.Windows.Forms.CheckBox();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.comboBox_display = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(0, 31);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 25;
            this.dataGridView.Size = new System.Drawing.Size(653, 287);
            this.dataGridView.TabIndex = 1;
            // 
            // checkBox_enabled
            // 
            this.checkBox_enabled.AutoSize = true;
            this.checkBox_enabled.Location = new System.Drawing.Point(2, 5);
            this.checkBox_enabled.Name = "checkBox_enabled";
            this.checkBox_enabled.Size = new System.Drawing.Size(68, 19);
            this.checkBox_enabled.TabIndex = 0;
            this.checkBox_enabled.Text = "Enabled";
            this.checkBox_enabled.UseVisualStyleBackColor = true;
            this.checkBox_enabled.CheckedChanged += new System.EventHandler(this.checkBox_enabled_CheckedChanged);
            // 
            // textBox_description
            // 
            this.textBox_description.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_description.Location = new System.Drawing.Point(0, 319);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.ReadOnly = true;
            this.textBox_description.Size = new System.Drawing.Size(653, 82);
            this.textBox_description.TabIndex = 5;
            this.textBox_description.Text = "text set in code\r\ntext set in code\r\ntext set in code\r\ntext set in code\r\ntext set " +
    "in code";
            // 
            // comboBox_display
            // 
            this.comboBox_display.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_display.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_display.FormattingEnabled = true;
            this.comboBox_display.Location = new System.Drawing.Point(364, 5);
            this.comboBox_display.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox_display.Name = "comboBox_display";
            this.comboBox_display.Size = new System.Drawing.Size(289, 23);
            this.comboBox_display.TabIndex = 2;
            this.comboBox_display.SelectedIndexChanged += new System.EventHandler(this.comboBox_display_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(309, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "Display:";
            // 
            // GammaSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_display);
            this.Controls.Add(this.textBox_description);
            this.Controls.Add(this.checkBox_enabled);
            this.Controls.Add(this.dataGridView);
            this.Name = "GammaSettings";
            this.Size = new System.Drawing.Size(653, 430);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.CheckBox checkBox_enabled;
        private System.Windows.Forms.TextBox textBox_description;
        private System.Windows.Forms.ComboBox comboBox_display;
        private System.Windows.Forms.Label label1;
    }
}
