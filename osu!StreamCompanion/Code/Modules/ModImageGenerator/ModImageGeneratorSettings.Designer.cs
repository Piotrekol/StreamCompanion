namespace osu_StreamCompanion.Code.Modules.ModImageGenerator
{
    partial class ModImageGeneratorSettings
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
            this.pictureBox_preview = new System.Windows.Forms.PictureBox();
            this.checkBox_enable = new System.Windows.Forms.CheckBox();
            this.nUd_ImageWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_enabled = new System.Windows.Forms.Panel();
            this.textBox_PreviewMods = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton_DrawFromRightToLeft = new System.Windows.Forms.RadioButton();
            this.radioButton_DrawFromLeftToRight = new System.Windows.Forms.RadioButton();
            this.panel_RadioButton_drawSide = new System.Windows.Forms.Panel();
            this.radioButton_DrawOnRight = new System.Windows.Forms.RadioButton();
            this.radioButton_DrawOnLeft = new System.Windows.Forms.RadioButton();
            this.nUd_Opacity = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nUd_Spacing = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nUd_ModImageHeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nUd_ModImageWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_ImageWidth)).BeginInit();
            this.panel_enabled.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel_RadioButton_drawSide.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_Opacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_Spacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_ModImageHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_ModImageWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_preview
            // 
            this.pictureBox_preview.Location = new System.Drawing.Point(3, 133);
            this.pictureBox_preview.Name = "pictureBox_preview";
            this.pictureBox_preview.Size = new System.Drawing.Size(625, 90);
            this.pictureBox_preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_preview.TabIndex = 0;
            this.pictureBox_preview.TabStop = false;
            // 
            // checkBox_enable
            // 
            this.checkBox_enable.AutoSize = true;
            this.checkBox_enable.Location = new System.Drawing.Point(6, 4);
            this.checkBox_enable.Name = "checkBox_enable";
            this.checkBox_enable.Size = new System.Drawing.Size(179, 17);
            this.checkBox_enable.TabIndex = 1;
            this.checkBox_enable.Text = "Enable generating of mod Image";
            this.checkBox_enable.UseVisualStyleBackColor = true;
            this.checkBox_enable.CheckedChanged += new System.EventHandler(this.checkBox_enable_CheckedChanged);
            // 
            // nUd_ImageWidth
            // 
            this.nUd_ImageWidth.Location = new System.Drawing.Point(126, 3);
            this.nUd_ImageWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUd_ImageWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_ImageWidth.Name = "nUd_ImageWidth";
            this.nUd_ImageWidth.Size = new System.Drawing.Size(65, 20);
            this.nUd_ImageWidth.TabIndex = 2;
            this.nUd_ImageWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_ImageWidth.ValueChanged += new System.EventHandler(this.nUd_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Generated image width";
            // 
            // panel_enabled
            // 
            this.panel_enabled.Controls.Add(this.textBox_PreviewMods);
            this.panel_enabled.Controls.Add(this.label6);
            this.panel_enabled.Controls.Add(this.panel1);
            this.panel_enabled.Controls.Add(this.panel_RadioButton_drawSide);
            this.panel_enabled.Controls.Add(this.nUd_Opacity);
            this.panel_enabled.Controls.Add(this.label5);
            this.panel_enabled.Controls.Add(this.nUd_Spacing);
            this.panel_enabled.Controls.Add(this.label4);
            this.panel_enabled.Controls.Add(this.nUd_ModImageHeight);
            this.panel_enabled.Controls.Add(this.label3);
            this.panel_enabled.Controls.Add(this.nUd_ModImageWidth);
            this.panel_enabled.Controls.Add(this.label2);
            this.panel_enabled.Controls.Add(this.nUd_ImageWidth);
            this.panel_enabled.Controls.Add(this.label1);
            this.panel_enabled.Controls.Add(this.pictureBox_preview);
            this.panel_enabled.Location = new System.Drawing.Point(6, 27);
            this.panel_enabled.Name = "panel_enabled";
            this.panel_enabled.Size = new System.Drawing.Size(631, 229);
            this.panel_enabled.TabIndex = 4;
            // 
            // textBox_PreviewMods
            // 
            this.textBox_PreviewMods.Location = new System.Drawing.Point(325, 109);
            this.textBox_PreviewMods.Name = "textBox_PreviewMods";
            this.textBox_PreviewMods.Size = new System.Drawing.Size(242, 20);
            this.textBox_PreviewMods.TabIndex = 15;
            this.textBox_PreviewMods.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(249, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Preview mods";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton_DrawFromRightToLeft);
            this.panel1.Controls.Add(this.radioButton_DrawFromLeftToRight);
            this.panel1.Location = new System.Drawing.Point(248, 73);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(319, 29);
            this.panel1.TabIndex = 13;
            // 
            // radioButton_DrawFromRightToLeft
            // 
            this.radioButton_DrawFromRightToLeft.AutoSize = true;
            this.radioButton_DrawFromRightToLeft.Location = new System.Drawing.Point(158, 4);
            this.radioButton_DrawFromRightToLeft.Name = "radioButton_DrawFromRightToLeft";
            this.radioButton_DrawFromRightToLeft.Size = new System.Drawing.Size(142, 17);
            this.radioButton_DrawFromRightToLeft.TabIndex = 1;
            this.radioButton_DrawFromRightToLeft.TabStop = true;
            this.radioButton_DrawFromRightToLeft.Text = "<--| Draw from right to left";
            this.radioButton_DrawFromRightToLeft.UseVisualStyleBackColor = true;
            this.radioButton_DrawFromRightToLeft.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton_DrawFromLeftToRight
            // 
            this.radioButton_DrawFromLeftToRight.AutoSize = true;
            this.radioButton_DrawFromLeftToRight.Location = new System.Drawing.Point(4, 4);
            this.radioButton_DrawFromLeftToRight.Name = "radioButton_DrawFromLeftToRight";
            this.radioButton_DrawFromLeftToRight.Size = new System.Drawing.Size(142, 17);
            this.radioButton_DrawFromLeftToRight.TabIndex = 0;
            this.radioButton_DrawFromLeftToRight.TabStop = true;
            this.radioButton_DrawFromLeftToRight.Text = "|--> Draw from left to right";
            this.radioButton_DrawFromLeftToRight.UseVisualStyleBackColor = true;
            this.radioButton_DrawFromLeftToRight.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // panel_RadioButton_drawSide
            // 
            this.panel_RadioButton_drawSide.Controls.Add(this.radioButton_DrawOnRight);
            this.panel_RadioButton_drawSide.Controls.Add(this.radioButton_DrawOnLeft);
            this.panel_RadioButton_drawSide.Location = new System.Drawing.Point(248, 46);
            this.panel_RadioButton_drawSide.Name = "panel_RadioButton_drawSide";
            this.panel_RadioButton_drawSide.Size = new System.Drawing.Size(319, 29);
            this.panel_RadioButton_drawSide.TabIndex = 12;
            // 
            // radioButton_DrawOnRight
            // 
            this.radioButton_DrawOnRight.AutoSize = true;
            this.radioButton_DrawOnRight.Location = new System.Drawing.Point(158, 4);
            this.radioButton_DrawOnRight.Name = "radioButton_DrawOnRight";
            this.radioButton_DrawOnRight.Size = new System.Drawing.Size(154, 17);
            this.radioButton_DrawOnRight.TabIndex = 1;
            this.radioButton_DrawOnRight.TabStop = true;
            this.radioButton_DrawOnRight.Text = "Draw on right side of Image";
            this.radioButton_DrawOnRight.UseVisualStyleBackColor = true;
            this.radioButton_DrawOnRight.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton_DrawOnLeft
            // 
            this.radioButton_DrawOnLeft.AutoSize = true;
            this.radioButton_DrawOnLeft.Location = new System.Drawing.Point(4, 4);
            this.radioButton_DrawOnLeft.Name = "radioButton_DrawOnLeft";
            this.radioButton_DrawOnLeft.Size = new System.Drawing.Size(148, 17);
            this.radioButton_DrawOnLeft.TabIndex = 0;
            this.radioButton_DrawOnLeft.TabStop = true;
            this.radioButton_DrawOnLeft.Text = "Draw on left side of Image";
            this.radioButton_DrawOnLeft.UseVisualStyleBackColor = true;
            this.radioButton_DrawOnLeft.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // nUd_Opacity
            // 
            this.nUd_Opacity.Location = new System.Drawing.Point(126, 107);
            this.nUd_Opacity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_Opacity.Name = "nUd_Opacity";
            this.nUd_Opacity.Size = new System.Drawing.Size(65, 20);
            this.nUd_Opacity.TabIndex = 10;
            this.nUd_Opacity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_Opacity.ValueChanged += new System.EventHandler(this.nUd_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Image opacity";
            // 
            // nUd_Spacing
            // 
            this.nUd_Spacing.Location = new System.Drawing.Point(126, 81);
            this.nUd_Spacing.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUd_Spacing.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nUd_Spacing.Name = "nUd_Spacing";
            this.nUd_Spacing.Size = new System.Drawing.Size(65, 20);
            this.nUd_Spacing.TabIndex = 8;
            this.nUd_Spacing.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_Spacing.ValueChanged += new System.EventHandler(this.nUd_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Mod Images spacing";
            // 
            // nUd_ModImageHeight
            // 
            this.nUd_ModImageHeight.Location = new System.Drawing.Point(126, 55);
            this.nUd_ModImageHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUd_ModImageHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_ModImageHeight.Name = "nUd_ModImageHeight";
            this.nUd_ModImageHeight.Size = new System.Drawing.Size(65, 20);
            this.nUd_ModImageHeight.TabIndex = 6;
            this.nUd_ModImageHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_ModImageHeight.ValueChanged += new System.EventHandler(this.nUd_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Mod images height";
            // 
            // nUd_ModImageWidth
            // 
            this.nUd_ModImageWidth.Location = new System.Drawing.Point(126, 29);
            this.nUd_ModImageWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUd_ModImageWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_ModImageWidth.Name = "nUd_ModImageWidth";
            this.nUd_ModImageWidth.Size = new System.Drawing.Size(65, 20);
            this.nUd_ModImageWidth.TabIndex = 4;
            this.nUd_ModImageWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_ModImageWidth.ValueChanged += new System.EventHandler(this.nUd_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Mod images width";
            // 
            // ModImageGeneratorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel_enabled);
            this.Controls.Add(this.checkBox_enable);
            this.Name = "ModImageGeneratorSettings";
            this.Size = new System.Drawing.Size(640, 263);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_ImageWidth)).EndInit();
            this.panel_enabled.ResumeLayout(false);
            this.panel_enabled.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel_RadioButton_drawSide.ResumeLayout(false);
            this.panel_RadioButton_drawSide.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_Opacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_Spacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_ModImageHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_ModImageWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBox_preview;
        private System.Windows.Forms.CheckBox checkBox_enable;
        private System.Windows.Forms.NumericUpDown nUd_ImageWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_enabled;
        private System.Windows.Forms.NumericUpDown nUd_Spacing;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nUd_ModImageHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUd_ModImageWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUd_Opacity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel_RadioButton_drawSide;
        private System.Windows.Forms.RadioButton radioButton_DrawOnRight;
        private System.Windows.Forms.RadioButton radioButton_DrawOnLeft;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton_DrawFromRightToLeft;
        private System.Windows.Forms.RadioButton radioButton_DrawFromLeftToRight;
        private System.Windows.Forms.TextBox textBox_PreviewMods;
        private System.Windows.Forms.Label label6;
    }
}
