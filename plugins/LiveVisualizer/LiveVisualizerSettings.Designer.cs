namespace LiveVisualizer
{
    partial class LiveVisualizerSettings
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox_hideMapStats = new System.Windows.Forms.CheckBox();
            this.checkBox_hideDiffText = new System.Windows.Forms.CheckBox();
            this.button_miniCounter = new System.Windows.Forms.Button();
            this.button_reset = new System.Windows.Forms.Button();
            this.checkBox_simulatePP = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_chartHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBox_chartColors = new System.Windows.Forms.GroupBox();
            this.button_openInBrowser = new System.Windows.Forms.Button();
            this.button_openFilesLocation = new System.Windows.Forms.Button();
            this.label_webUrl = new System.Windows.Forms.Label();
            this.label_localFiles = new System.Windows.Forms.Label();
            this.color_ppBackground = new LiveVisualizer.ColorPickerWithPreview();
            this.color_hit100Background = new LiveVisualizer.ColorPickerWithPreview();
            this.color_hit50Background = new LiveVisualizer.ColorPickerWithPreview();
            this.color_hitMissBackground = new LiveVisualizer.ColorPickerWithPreview();
            this.color_chartPrimary = new LiveVisualizer.ColorPickerWithPreview();
            this.color_chartProgress = new LiveVisualizer.ColorPickerWithPreview();
            this.color_imageDimming = new LiveVisualizer.ColorPickerWithPreview();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_chartHeight)).BeginInit();
            this.groupBox_chartColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBox_hideMapStats);
            this.panel1.Controls.Add(this.checkBox_hideDiffText);
            this.panel1.Controls.Add(this.button_miniCounter);
            this.panel1.Controls.Add(this.button_reset);
            this.panel1.Controls.Add(this.checkBox_simulatePP);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.numericUpDown_chartHeight);
            this.panel1.Controls.Add(this.groupBox_chartColors);
            this.panel1.Location = new System.Drawing.Point(0, 61);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(650, 244);
            this.panel1.TabIndex = 1;
            // 
            // checkBox_hideMapStats
            // 
            this.checkBox_hideMapStats.AutoSize = true;
            this.checkBox_hideMapStats.Location = new System.Drawing.Point(10, 179);
            this.checkBox_hideMapStats.Name = "checkBox_hideMapStats";
            this.checkBox_hideMapStats.Size = new System.Drawing.Size(96, 17);
            this.checkBox_hideMapStats.TabIndex = 56;
            this.checkBox_hideMapStats.Text = "Hide map stats";
            this.checkBox_hideMapStats.UseVisualStyleBackColor = true;
            // 
            // checkBox_hideDiffText
            // 
            this.checkBox_hideDiffText.AutoSize = true;
            this.checkBox_hideDiffText.Location = new System.Drawing.Point(10, 156);
            this.checkBox_hideDiffText.Name = "checkBox_hideDiffText";
            this.checkBox_hideDiffText.Size = new System.Drawing.Size(197, 17);
            this.checkBox_hideDiffText.TabIndex = 55;
            this.checkBox_hideDiffText.Text = "Hide artist/title/mapper/difficulty text";
            this.checkBox_hideDiffText.UseVisualStyleBackColor = true;
            // 
            // button_miniCounter
            // 
            this.button_miniCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_miniCounter.Location = new System.Drawing.Point(456, 215);
            this.button_miniCounter.Name = "button_miniCounter";
            this.button_miniCounter.Size = new System.Drawing.Size(81, 23);
            this.button_miniCounter.TabIndex = 50;
            this.button_miniCounter.Text = "Mini counter";
            this.button_miniCounter.UseVisualStyleBackColor = true;
            this.button_miniCounter.Click += new System.EventHandler(this.Button_miniCounter_Click);
            // 
            // button_reset
            // 
            this.button_reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_reset.Location = new System.Drawing.Point(543, 215);
            this.button_reset.Name = "button_reset";
            this.button_reset.Size = new System.Drawing.Size(97, 23);
            this.button_reset.TabIndex = 49;
            this.button_reset.Text = "Reset to defaults";
            this.button_reset.UseVisualStyleBackColor = true;
            this.button_reset.Click += new System.EventHandler(this.Button_reset_Click);
            // 
            // checkBox_simulatePP
            // 
            this.checkBox_simulatePP.AutoSize = true;
            this.checkBox_simulatePP.Location = new System.Drawing.Point(10, 133);
            this.checkBox_simulatePP.Name = "checkBox_simulatePP";
            this.checkBox_simulatePP.Size = new System.Drawing.Size(151, 17);
            this.checkBox_simulatePP.TabIndex = 45;
            this.checkBox_simulatePP.Text = "Simulate pp when listening";
            this.checkBox_simulatePP.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 202);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Chart height:";
            // 
            // numericUpDown_chartHeight
            // 
            this.numericUpDown_chartHeight.Location = new System.Drawing.Point(80, 198);
            this.numericUpDown_chartHeight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDown_chartHeight.Name = "numericUpDown_chartHeight";
            this.numericUpDown_chartHeight.Size = new System.Drawing.Size(66, 20);
            this.numericUpDown_chartHeight.TabIndex = 43;
            this.numericUpDown_chartHeight.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // groupBox_chartColors
            // 
            this.groupBox_chartColors.Controls.Add(this.color_ppBackground);
            this.groupBox_chartColors.Controls.Add(this.color_hit100Background);
            this.groupBox_chartColors.Controls.Add(this.color_hit50Background);
            this.groupBox_chartColors.Controls.Add(this.color_hitMissBackground);
            this.groupBox_chartColors.Controls.Add(this.color_chartPrimary);
            this.groupBox_chartColors.Controls.Add(this.color_chartProgress);
            this.groupBox_chartColors.Controls.Add(this.color_imageDimming);
            this.groupBox_chartColors.Location = new System.Drawing.Point(4, 4);
            this.groupBox_chartColors.Name = "groupBox_chartColors";
            this.groupBox_chartColors.Size = new System.Drawing.Size(636, 123);
            this.groupBox_chartColors.TabIndex = 42;
            this.groupBox_chartColors.TabStop = false;
            this.groupBox_chartColors.Text = "Chart colors";
            // 
            // button_openInBrowser
            // 
            this.button_openInBrowser.Location = new System.Drawing.Point(4, 3);
            this.button_openInBrowser.Name = "button_openInBrowser";
            this.button_openInBrowser.Size = new System.Drawing.Size(194, 23);
            this.button_openInBrowser.TabIndex = 3;
            this.button_openInBrowser.Text = "Open overlay in browser";
            this.button_openInBrowser.UseVisualStyleBackColor = true;
            this.button_openInBrowser.Click += new System.EventHandler(this.button_openInBrowser_Click);
            // 
            // button_openFilesLocation
            // 
            this.button_openFilesLocation.Location = new System.Drawing.Point(4, 32);
            this.button_openFilesLocation.Name = "button_openFilesLocation";
            this.button_openFilesLocation.Size = new System.Drawing.Size(194, 23);
            this.button_openFilesLocation.TabIndex = 4;
            this.button_openFilesLocation.Text = "Open overlay files location";
            this.button_openFilesLocation.UseVisualStyleBackColor = true;
            this.button_openFilesLocation.Click += new System.EventHandler(this.button_openFilesLocation_Click);
            // 
            // label_webUrl
            // 
            this.label_webUrl.AutoSize = true;
            this.label_webUrl.Location = new System.Drawing.Point(204, 8);
            this.label_webUrl.Name = "label_webUrl";
            this.label_webUrl.Size = new System.Drawing.Size(52, 13);
            this.label_webUrl.TabIndex = 0;
            this.label_webUrl.Text = "<webUrl>";
            // 
            // label_localFiles
            // 
            this.label_localFiles.AutoSize = true;
            this.label_localFiles.Location = new System.Drawing.Point(204, 37);
            this.label_localFiles.Name = "label_localFiles";
            this.label_localFiles.Size = new System.Drawing.Size(78, 13);
            this.label_localFiles.TabIndex = 5;
            this.label_localFiles.Text = "<filesLocation>";
            // 
            // color_ppBackground
            // 
            this.color_ppBackground.Color = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            // 
            // 
            // 
            this.color_ppBackground.LabelDesigner.AutoSize = true;
            this.color_ppBackground.LabelDesigner.Location = new System.Drawing.Point(3, 6);
            this.color_ppBackground.LabelDesigner.Name = "Label";
            this.color_ppBackground.LabelDesigner.Size = new System.Drawing.Size(81, 13);
            this.color_ppBackground.LabelDesigner.TabIndex = 1;
            this.color_ppBackground.LabelDesigner.Text = "PP background";
            this.color_ppBackground.Location = new System.Drawing.Point(302, 19);
            this.color_ppBackground.Name = "color_ppBackground";
            this.color_ppBackground.Size = new System.Drawing.Size(298, 21);
            this.color_ppBackground.TabIndex = 43;
            // 
            // color_hit100Background
            // 
            this.color_hit100Background.Color = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            // 
            // 
            // 
            this.color_hit100Background.LabelDesigner.AutoSize = true;
            this.color_hit100Background.LabelDesigner.Location = new System.Drawing.Point(3, 6);
            this.color_hit100Background.LabelDesigner.Name = "Label";
            this.color_hit100Background.LabelDesigner.Size = new System.Drawing.Size(96, 13);
            this.color_hit100Background.LabelDesigner.TabIndex = 1;
            this.color_hit100Background.LabelDesigner.Text = "hit100 background";
            this.color_hit100Background.Location = new System.Drawing.Point(302, 43);
            this.color_hit100Background.Name = "color_hit100Background";
            this.color_hit100Background.Size = new System.Drawing.Size(298, 21);
            this.color_hit100Background.TabIndex = 44;
            // 
            // color_hit50Background
            // 
            this.color_hit50Background.Color = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            // 
            // 
            // 
            this.color_hit50Background.LabelDesigner.AutoSize = true;
            this.color_hit50Background.LabelDesigner.Location = new System.Drawing.Point(3, 6);
            this.color_hit50Background.LabelDesigner.Name = "Label";
            this.color_hit50Background.LabelDesigner.Size = new System.Drawing.Size(90, 13);
            this.color_hit50Background.LabelDesigner.TabIndex = 1;
            this.color_hit50Background.LabelDesigner.Text = "hit50 background";
            this.color_hit50Background.Location = new System.Drawing.Point(302, 67);
            this.color_hit50Background.Name = "color_hit50Background";
            this.color_hit50Background.Size = new System.Drawing.Size(298, 21);
            this.color_hit50Background.TabIndex = 45;
            // 
            // color_hitMissBackground
            // 
            this.color_hitMissBackground.Color = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            // 
            // 
            // 
            this.color_hitMissBackground.LabelDesigner.AutoSize = true;
            this.color_hitMissBackground.LabelDesigner.Location = new System.Drawing.Point(3, 6);
            this.color_hitMissBackground.LabelDesigner.Name = "Label";
            this.color_hitMissBackground.LabelDesigner.Size = new System.Drawing.Size(99, 13);
            this.color_hitMissBackground.LabelDesigner.TabIndex = 1;
            this.color_hitMissBackground.LabelDesigner.Text = "hitMiss background";
            this.color_hitMissBackground.Location = new System.Drawing.Point(302, 91);
            this.color_hitMissBackground.Name = "color_hitMissBackground";
            this.color_hitMissBackground.Size = new System.Drawing.Size(298, 21);
            this.color_hitMissBackground.TabIndex = 46;
            // 
            // color_chartPrimary
            // 
            this.color_chartPrimary.Color = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            // 
            // 
            // 
            this.color_chartPrimary.LabelDesigner.AutoSize = true;
            this.color_chartPrimary.LabelDesigner.Location = new System.Drawing.Point(3, 6);
            this.color_chartPrimary.LabelDesigner.Name = "Label";
            this.color_chartPrimary.LabelDesigner.Size = new System.Drawing.Size(68, 13);
            this.color_chartPrimary.LabelDesigner.TabIndex = 1;
            this.color_chartPrimary.LabelDesigner.Text = "Primary chart";
            this.color_chartPrimary.Location = new System.Drawing.Point(6, 19);
            this.color_chartPrimary.Name = "color_chartPrimary";
            this.color_chartPrimary.Size = new System.Drawing.Size(290, 21);
            this.color_chartPrimary.TabIndex = 0;
            // 
            // color_chartProgress
            // 
            this.color_chartProgress.Color = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            // 
            // 
            // 
            this.color_chartProgress.LabelDesigner.AutoSize = true;
            this.color_chartProgress.LabelDesigner.Location = new System.Drawing.Point(3, 6);
            this.color_chartProgress.LabelDesigner.Name = "Label";
            this.color_chartProgress.LabelDesigner.Size = new System.Drawing.Size(75, 13);
            this.color_chartProgress.LabelDesigner.TabIndex = 1;
            this.color_chartProgress.LabelDesigner.Text = "Chart progress";
            this.color_chartProgress.Location = new System.Drawing.Point(6, 43);
            this.color_chartProgress.Name = "color_chartProgress";
            this.color_chartProgress.Size = new System.Drawing.Size(290, 21);
            this.color_chartProgress.TabIndex = 1;
            // 
            // color_imageDimming
            // 
            this.color_imageDimming.Color = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            // 
            // 
            // 
            this.color_imageDimming.LabelDesigner.AutoSize = true;
            this.color_imageDimming.LabelDesigner.Location = new System.Drawing.Point(3, 6);
            this.color_imageDimming.LabelDesigner.Name = "Label";
            this.color_imageDimming.LabelDesigner.Size = new System.Drawing.Size(77, 13);
            this.color_imageDimming.LabelDesigner.TabIndex = 1;
            this.color_imageDimming.LabelDesigner.Text = "Image dimming";
            this.color_imageDimming.Location = new System.Drawing.Point(6, 67);
            this.color_imageDimming.Name = "color_imageDimming";
            this.color_imageDimming.Size = new System.Drawing.Size(290, 21);
            this.color_imageDimming.TabIndex = 40;
            // 
            // LiveVisualizerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_localFiles);
            this.Controls.Add(this.label_webUrl);
            this.Controls.Add(this.button_openFilesLocation);
            this.Controls.Add(this.button_openInBrowser);
            this.Controls.Add(this.panel1);
            this.Name = "LiveVisualizerSettings";
            this.Size = new System.Drawing.Size(650, 305);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_chartHeight)).EndInit();
            this.groupBox_chartColors.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private ColorPickerWithPreview color_chartPrimary;
        private ColorPickerWithPreview color_chartProgress;
        private ColorPickerWithPreview color_imageDimming;
        private System.Windows.Forms.GroupBox groupBox_chartColors;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown_chartHeight;
        private System.Windows.Forms.CheckBox checkBox_simulatePP;
        private ColorPickerWithPreview color_ppBackground;
        private ColorPickerWithPreview color_hit100Background;
        private ColorPickerWithPreview color_hit50Background;
        private ColorPickerWithPreview color_hitMissBackground;
        private System.Windows.Forms.Button button_miniCounter;
        private System.Windows.Forms.Button button_reset;
        private System.Windows.Forms.CheckBox checkBox_hideDiffText;
        private System.Windows.Forms.CheckBox checkBox_hideMapStats;
        private System.Windows.Forms.Button button_openInBrowser;
        private System.Windows.Forms.Button button_openFilesLocation;
        private System.Windows.Forms.Label label_webUrl;
        private System.Windows.Forms.Label label_localFiles;
    }
}
