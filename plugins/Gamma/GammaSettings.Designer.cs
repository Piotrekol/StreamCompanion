
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
            checkBox_enabled = new System.Windows.Forms.CheckBox();
            comboBox_display = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            panel_main = new System.Windows.Forms.Panel();
            label_mapAR = new System.Windows.Forms.Label();
            panel_rangeSettings = new System.Windows.Forms.Panel();
            textBox_gamma = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            trackBar_gamma = new System.Windows.Forms.TrackBar();
            label3 = new System.Windows.Forms.Label();
            numericUpDown_arMax = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            numericUpDown_arMin = new System.Windows.Forms.NumericUpDown();
            listBox_gammaRanges = new System.Windows.Forms.ListBox();
            button_remove = new System.Windows.Forms.Button();
            button_addRange = new System.Windows.Forms.Button();
            panel_main.SuspendLayout();
            panel_rangeSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_gamma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_arMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_arMin).BeginInit();
            SuspendLayout();
            // 
            // checkBox_enabled
            // 
            checkBox_enabled.AutoSize = true;
            checkBox_enabled.Location = new System.Drawing.Point(2, 5);
            checkBox_enabled.Name = "checkBox_enabled";
            checkBox_enabled.Size = new System.Drawing.Size(68, 19);
            checkBox_enabled.TabIndex = 0;
            checkBox_enabled.Text = "Enabled";
            checkBox_enabled.UseVisualStyleBackColor = true;
            checkBox_enabled.CheckedChanged += checkBox_enabled_CheckedChanged;
            // 
            // comboBox_display
            // 
            comboBox_display.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBox_display.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox_display.FormattingEnabled = true;
            comboBox_display.Location = new System.Drawing.Point(364, 5);
            comboBox_display.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBox_display.Name = "comboBox_display";
            comboBox_display.Size = new System.Drawing.Size(360, 23);
            comboBox_display.TabIndex = 2;
            comboBox_display.SelectedIndexChanged += comboBox_display_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(309, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 15);
            label1.TabIndex = 16;
            label1.Text = "Display:";
            // 
            // panel_main
            // 
            panel_main.Controls.Add(panel_rangeSettings);
            panel_main.Controls.Add(listBox_gammaRanges);
            panel_main.Controls.Add(button_remove);
            panel_main.Controls.Add(button_addRange);
            panel_main.Location = new System.Drawing.Point(3, 30);
            panel_main.Name = "panel_main";
            panel_main.Size = new System.Drawing.Size(722, 488);
            panel_main.TabIndex = 17;
            // 
            // label_mapAR
            // 
            label_mapAR.AutoSize = true;
            label_mapAR.Location = new System.Drawing.Point(244, 22);
            label_mapAR.Name = "label_mapAR";
            label_mapAR.Size = new System.Drawing.Size(95, 15);
            label_mapAR.TabIndex = 11;
            label_mapAR.Text = "Current map AR:";
            // 
            // panel_rangeSettings
            // 
            panel_rangeSettings.Controls.Add(label_mapAR);
            panel_rangeSettings.Controls.Add(textBox_gamma);
            panel_rangeSettings.Controls.Add(label4);
            panel_rangeSettings.Controls.Add(trackBar_gamma);
            panel_rangeSettings.Controls.Add(label3);
            panel_rangeSettings.Controls.Add(numericUpDown_arMax);
            panel_rangeSettings.Controls.Add(label2);
            panel_rangeSettings.Controls.Add(numericUpDown_arMin);
            panel_rangeSettings.Location = new System.Drawing.Point(3, 300);
            panel_rangeSettings.Name = "panel_rangeSettings";
            panel_rangeSettings.Size = new System.Drawing.Size(716, 185);
            panel_rangeSettings.TabIndex = 25;
            // 
            // textBox_gamma
            // 
            textBox_gamma.Location = new System.Drawing.Point(455, 51);
            textBox_gamma.Name = "textBox_gamma";
            textBox_gamma.ReadOnly = true;
            textBox_gamma.Size = new System.Drawing.Size(37, 23);
            textBox_gamma.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(32, 54);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(52, 15);
            label4.TabIndex = 9;
            label4.Text = "Gamma:";
            // 
            // trackBar_gamma
            // 
            trackBar_gamma.LargeChange = 10;
            trackBar_gamma.Location = new System.Drawing.Point(90, 49);
            trackBar_gamma.Maximum = 100;
            trackBar_gamma.Name = "trackBar_gamma";
            trackBar_gamma.Size = new System.Drawing.Size(359, 45);
            trackBar_gamma.TabIndex = 8;
            trackBar_gamma.ValueChanged += trackBar_gamma_ValueChanged;
            trackBar_gamma.MouseDown += trackBar_gamma_MouseDown;
            trackBar_gamma.MouseUp += trackBar_gamma_MouseUp;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(158, 22);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(12, 15);
            label3.TabIndex = 7;
            label3.Text = "-";
            // 
            // numericUpDown_arMax
            // 
            numericUpDown_arMax.DecimalPlaces = 2;
            numericUpDown_arMax.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numericUpDown_arMax.Location = new System.Drawing.Point(176, 20);
            numericUpDown_arMax.Maximum = new decimal(new int[] { 1501, 0, 0, 131072 });
            numericUpDown_arMax.Name = "numericUpDown_arMax";
            numericUpDown_arMax.Size = new System.Drawing.Size(62, 23);
            numericUpDown_arMax.TabIndex = 6;
            numericUpDown_arMax.ValueChanged += numericUpDown_arMax_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 22);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(77, 15);
            label2.TabIndex = 5;
            label2.Text = "AR min-max:";
            // 
            // numericUpDown_arMin
            // 
            numericUpDown_arMin.DecimalPlaces = 2;
            numericUpDown_arMin.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numericUpDown_arMin.Location = new System.Drawing.Point(90, 20);
            numericUpDown_arMin.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            numericUpDown_arMin.Name = "numericUpDown_arMin";
            numericUpDown_arMin.Size = new System.Drawing.Size(62, 23);
            numericUpDown_arMin.TabIndex = 0;
            numericUpDown_arMin.ValueChanged += numericUpDown_arMin_ValueChanged;
            // 
            // listBox_gammaRanges
            // 
            listBox_gammaRanges.FormattingEnabled = true;
            listBox_gammaRanges.ItemHeight = 15;
            listBox_gammaRanges.Location = new System.Drawing.Point(3, 4);
            listBox_gammaRanges.Name = "listBox_gammaRanges";
            listBox_gammaRanges.Size = new System.Drawing.Size(716, 259);
            listBox_gammaRanges.TabIndex = 23;
            listBox_gammaRanges.SelectedIndexChanged += listBox_gammaRanges_SelectedIndexChanged;
            // 
            // button_remove
            // 
            button_remove.Location = new System.Drawing.Point(84, 271);
            button_remove.Name = "button_remove";
            button_remove.Size = new System.Drawing.Size(159, 23);
            button_remove.TabIndex = 24;
            button_remove.Text = "Remove selected range";
            button_remove.UseVisualStyleBackColor = true;
            button_remove.Click += button_remove_Click;
            // 
            // button_addRange
            // 
            button_addRange.Location = new System.Drawing.Point(3, 271);
            button_addRange.Name = "button_addRange";
            button_addRange.Size = new System.Drawing.Size(75, 23);
            button_addRange.TabIndex = 22;
            button_addRange.Text = "Add range";
            button_addRange.UseVisualStyleBackColor = true;
            button_addRange.Click += button_addRange_Click;
            // 
            // GammaSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel_main);
            Controls.Add(label1);
            Controls.Add(comboBox_display);
            Controls.Add(checkBox_enabled);
            Name = "GammaSettings";
            Size = new System.Drawing.Size(728, 521);
            panel_main.ResumeLayout(false);
            panel_rangeSettings.ResumeLayout(false);
            panel_rangeSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_gamma).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_arMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_arMin).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.CheckBox checkBox_enabled;
        private System.Windows.Forms.ComboBox comboBox_display;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_main;
        private System.Windows.Forms.ListBox listBox_gammaRanges;
        private System.Windows.Forms.Button button_remove;
        private System.Windows.Forms.Button button_addRange;
        private System.Windows.Forms.Panel panel_rangeSettings;
        private System.Windows.Forms.NumericUpDown numericUpDown_arMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackBar_gamma;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown_arMax;
        private System.Windows.Forms.TextBox textBox_gamma;
        private System.Windows.Forms.Label label_mapAR;
    }
}
