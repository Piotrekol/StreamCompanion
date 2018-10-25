namespace ModsHandler
{
    partial class ModParserSettings
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
            this.label4 = new System.Windows.Forms.Label();
            this.radioButton_shortMods = new System.Windows.Forms.RadioButton();
            this.radioButton_longMods = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Mods = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Use long or short mods names?";
            // 
            // radioButton_shortMods
            // 
            this.radioButton_shortMods.AutoSize = true;
            this.radioButton_shortMods.Location = new System.Drawing.Point(143, 55);
            this.radioButton_shortMods.Name = "radioButton_shortMods";
            this.radioButton_shortMods.Size = new System.Drawing.Size(62, 17);
            this.radioButton_shortMods.TabIndex = 14;
            this.radioButton_shortMods.TabStop = true;
            this.radioButton_shortMods.Text = "HR, DT";
            this.radioButton_shortMods.UseVisualStyleBackColor = true;
            this.radioButton_shortMods.CheckedChanged += new System.EventHandler(this.radioButton_longMods_CheckedChanged);
            // 
            // radioButton_longMods
            // 
            this.radioButton_longMods.AutoSize = true;
            this.radioButton_longMods.Location = new System.Drawing.Point(3, 55);
            this.radioButton_longMods.Name = "radioButton_longMods";
            this.radioButton_longMods.Size = new System.Drawing.Size(134, 17);
            this.radioButton_longMods.TabIndex = 13;
            this.radioButton_longMods.TabStop = true;
            this.radioButton_longMods.Text = "HardRock,DoubleTime";
            this.radioButton_longMods.UseVisualStyleBackColor = true;
            this.radioButton_longMods.CheckedChanged += new System.EventHandler(this.radioButton_longMods_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(271, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "What should be displayed when there is no mod active?";
            // 
            // textBox_Mods
            // 
            this.textBox_Mods.Location = new System.Drawing.Point(3, 16);
            this.textBox_Mods.Name = "textBox_Mods";
            this.textBox_Mods.Size = new System.Drawing.Size(143, 20);
            this.textBox_Mods.TabIndex = 11;
            this.textBox_Mods.TextChanged += new System.EventHandler(this.textBox_Mods_TextChanged);
            // 
            // ModParserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.radioButton_shortMods);
            this.Controls.Add(this.radioButton_longMods);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Mods);
            this.Name = "ModParserSettings";
            this.Size = new System.Drawing.Size(591, 77);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButton_shortMods;
        private System.Windows.Forms.RadioButton radioButton_longMods;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Mods;
    }
}
