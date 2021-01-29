namespace WebSocketDataSender.WebOverlay
{
    partial class WebOverlaySettings
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
            this.textBox_remoteAccessUrl = new System.Windows.Forms.TextBox();
            this.button_remoteOverlay = new System.Windows.Forms.Button();
            this.checkBox_hideChartLegend = new System.Windows.Forms.CheckBox();
            this.checkBox_hideMapStats = new System.Windows.Forms.CheckBox();
            this.checkBox_hideDiffText = new System.Windows.Forms.CheckBox();
            this.button_miniCounter = new System.Windows.Forms.Button();
            this.button_reset = new System.Windows.Forms.Button();
            this.checkBox_simulatePP = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_chartHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBox_chartColors = new System.Windows.Forms.GroupBox();
            this.color_ppBackground = new WebSocketDataSender.WebOverlay.ColorPickerWithPreview();
            this.color_hit100Background = new WebSocketDataSender.WebOverlay.ColorPickerWithPreview();
            this.color_hit50Background = new WebSocketDataSender.WebOverlay.ColorPickerWithPreview();
            this.color_hitMissBackground = new WebSocketDataSender.WebOverlay.ColorPickerWithPreview();
            this.color_chartPrimary = new WebSocketDataSender.WebOverlay.ColorPickerWithPreview();
            this.color_chartProgress = new WebSocketDataSender.WebOverlay.ColorPickerWithPreview();
            this.color_imageDimming = new WebSocketDataSender.WebOverlay.ColorPickerWithPreview();
            this.button_openInBrowser = new System.Windows.Forms.Button();
            this.button_openFilesLocation = new System.Windows.Forms.Button();
            this.label_webUrl = new System.Windows.Forms.Label();
            this.label_localFiles = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_chartHeight)).BeginInit();
            this.groupBox_chartColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox_remoteAccessUrl);
            this.panel1.Controls.Add(this.button_remoteOverlay);
            this.panel1.Controls.Add(this.checkBox_hideChartLegend);
            this.panel1.Controls.Add(this.checkBox_hideMapStats);
            this.panel1.Controls.Add(this.checkBox_hideDiffText);
            this.panel1.Controls.Add(this.button_miniCounter);
            this.panel1.Controls.Add(this.button_reset);
            this.panel1.Controls.Add(this.checkBox_simulatePP);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.numericUpDown_chartHeight);
            this.panel1.Controls.Add(this.groupBox_chartColors);
            this.panel1.Location = new System.Drawing.Point(0, 70);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(758, 359);
            this.panel1.TabIndex = 1;
            // 
            // textBox_remoteAccessUrl
            // 
            this.textBox_remoteAccessUrl.Location = new System.Drawing.Point(250, 155);
            this.textBox_remoteAccessUrl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox_remoteAccessUrl.Name = "textBox_remoteAccessUrl";
            this.textBox_remoteAccessUrl.ReadOnly = true;
            this.textBox_remoteAccessUrl.Size = new System.Drawing.Size(235, 23);
            this.textBox_remoteAccessUrl.TabIndex = 59;
            // 
            // button_remoteOverlay
            // 
            this.button_remoteOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_remoteOverlay.Location = new System.Drawing.Point(12, 153);
            this.button_remoteOverlay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_remoteOverlay.Name = "button_remoteOverlay";
            this.button_remoteOverlay.Size = new System.Drawing.Size(230, 27);
            this.button_remoteOverlay.TabIndex = 58;
            this.button_remoteOverlay.Text = "Enable remote access";
            this.button_remoteOverlay.UseVisualStyleBackColor = true;
            this.button_remoteOverlay.Click += new System.EventHandler(this.button_remoteOverlay_Click);
            // 
            // checkBox_hideChartLegend
            // 
            this.checkBox_hideChartLegend.AutoSize = true;
            this.checkBox_hideChartLegend.Location = new System.Drawing.Point(12, 268);
            this.checkBox_hideChartLegend.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_hideChartLegend.Name = "checkBox_hideChartLegend";
            this.checkBox_hideChartLegend.Size = new System.Drawing.Size(120, 19);
            this.checkBox_hideChartLegend.TabIndex = 57;
            this.checkBox_hideChartLegend.Text = "Hide chart legend";
            this.checkBox_hideChartLegend.UseVisualStyleBackColor = true;
            // 
            // checkBox_hideMapStats
            // 
            this.checkBox_hideMapStats.AutoSize = true;
            this.checkBox_hideMapStats.Location = new System.Drawing.Point(12, 241);
            this.checkBox_hideMapStats.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_hideMapStats.Name = "checkBox_hideMapStats";
            this.checkBox_hideMapStats.Size = new System.Drawing.Size(105, 19);
            this.checkBox_hideMapStats.TabIndex = 56;
            this.checkBox_hideMapStats.Text = "Hide map stats";
            this.checkBox_hideMapStats.UseVisualStyleBackColor = true;
            // 
            // checkBox_hideDiffText
            // 
            this.checkBox_hideDiffText.AutoSize = true;
            this.checkBox_hideDiffText.Location = new System.Drawing.Point(12, 215);
            this.checkBox_hideDiffText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_hideDiffText.Name = "checkBox_hideDiffText";
            this.checkBox_hideDiffText.Size = new System.Drawing.Size(226, 19);
            this.checkBox_hideDiffText.TabIndex = 55;
            this.checkBox_hideDiffText.Text = "Hide artist/title/mapper/difficulty text";
            this.checkBox_hideDiffText.UseVisualStyleBackColor = true;
            // 
            // button_miniCounter
            // 
            this.button_miniCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_miniCounter.Location = new System.Drawing.Point(532, 325);
            this.button_miniCounter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_miniCounter.Name = "button_miniCounter";
            this.button_miniCounter.Size = new System.Drawing.Size(94, 27);
            this.button_miniCounter.TabIndex = 50;
            this.button_miniCounter.Text = "Mini counter";
            this.button_miniCounter.UseVisualStyleBackColor = true;
            this.button_miniCounter.Click += new System.EventHandler(this.Button_miniCounter_Click);
            // 
            // button_reset
            // 
            this.button_reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_reset.Location = new System.Drawing.Point(634, 325);
            this.button_reset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_reset.Name = "button_reset";
            this.button_reset.Size = new System.Drawing.Size(113, 27);
            this.button_reset.TabIndex = 49;
            this.button_reset.Text = "Reset to defaults";
            this.button_reset.UseVisualStyleBackColor = true;
            this.button_reset.Click += new System.EventHandler(this.Button_reset_Click);
            // 
            // checkBox_simulatePP
            // 
            this.checkBox_simulatePP.AutoSize = true;
            this.checkBox_simulatePP.Location = new System.Drawing.Point(12, 188);
            this.checkBox_simulatePP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_simulatePP.Name = "checkBox_simulatePP";
            this.checkBox_simulatePP.Size = new System.Drawing.Size(169, 19);
            this.checkBox_simulatePP.TabIndex = 45;
            this.checkBox_simulatePP.Text = "Simulate pp when listening";
            this.checkBox_simulatePP.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 300);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 15);
            this.label6.TabIndex = 44;
            this.label6.Text = "Chart height:";
            // 
            // numericUpDown_chartHeight
            // 
            this.numericUpDown_chartHeight.Location = new System.Drawing.Point(93, 295);
            this.numericUpDown_chartHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown_chartHeight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDown_chartHeight.Name = "numericUpDown_chartHeight";
            this.numericUpDown_chartHeight.Size = new System.Drawing.Size(77, 23);
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
            this.groupBox_chartColors.Location = new System.Drawing.Point(5, 5);
            this.groupBox_chartColors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox_chartColors.Name = "groupBox_chartColors";
            this.groupBox_chartColors.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox_chartColors.Size = new System.Drawing.Size(742, 142);
            this.groupBox_chartColors.TabIndex = 42;
            this.groupBox_chartColors.TabStop = false;
            this.groupBox_chartColors.Text = "Chart colors";
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
            this.color_ppBackground.LabelDesigner.Size = new System.Drawing.Size(88, 15);
            this.color_ppBackground.LabelDesigner.TabIndex = 1;
            this.color_ppBackground.LabelDesigner.Text = "PP background";
            this.color_ppBackground.Location = new System.Drawing.Point(352, 22);
            this.color_ppBackground.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.color_ppBackground.Name = "color_ppBackground";
            this.color_ppBackground.Size = new System.Drawing.Size(348, 25);
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
            this.color_hit100Background.LabelDesigner.Size = new System.Drawing.Size(106, 15);
            this.color_hit100Background.LabelDesigner.TabIndex = 1;
            this.color_hit100Background.LabelDesigner.Text = "hit100 background";
            this.color_hit100Background.Location = new System.Drawing.Point(352, 50);
            this.color_hit100Background.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.color_hit100Background.Name = "color_hit100Background";
            this.color_hit100Background.Size = new System.Drawing.Size(348, 25);
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
            this.color_hit50Background.LabelDesigner.TabIndex = 1;
            this.color_hit50Background.LabelDesigner.Text = "hit50 background";
            this.color_hit50Background.Location = new System.Drawing.Point(352, 77);
            this.color_hit50Background.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.color_hit50Background.Name = "color_hit50Background";
            this.color_hit50Background.Size = new System.Drawing.Size(348, 25);
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
            this.color_hitMissBackground.LabelDesigner.Size = new System.Drawing.Size(112, 15);
            this.color_hitMissBackground.LabelDesigner.TabIndex = 1;
            this.color_hitMissBackground.LabelDesigner.Text = "hitMiss background";
            this.color_hitMissBackground.Location = new System.Drawing.Point(352, 105);
            this.color_hitMissBackground.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.color_hitMissBackground.Name = "color_hitMissBackground";
            this.color_hitMissBackground.Size = new System.Drawing.Size(348, 25);
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
            this.color_chartPrimary.LabelDesigner.Size = new System.Drawing.Size(78, 15);
            this.color_chartPrimary.LabelDesigner.TabIndex = 1;
            this.color_chartPrimary.LabelDesigner.Text = "Primary chart";
            this.color_chartPrimary.Location = new System.Drawing.Point(7, 22);
            this.color_chartPrimary.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.color_chartPrimary.Name = "color_chartPrimary";
            this.color_chartPrimary.Size = new System.Drawing.Size(338, 25);
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
            this.color_chartProgress.LabelDesigner.Size = new System.Drawing.Size(84, 15);
            this.color_chartProgress.LabelDesigner.TabIndex = 1;
            this.color_chartProgress.LabelDesigner.Text = "Chart progress";
            this.color_chartProgress.Location = new System.Drawing.Point(7, 50);
            this.color_chartProgress.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.color_chartProgress.Name = "color_chartProgress";
            this.color_chartProgress.Size = new System.Drawing.Size(338, 25);
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
            this.color_imageDimming.LabelDesigner.Size = new System.Drawing.Size(92, 15);
            this.color_imageDimming.LabelDesigner.TabIndex = 1;
            this.color_imageDimming.LabelDesigner.Text = "Image dimming";
            this.color_imageDimming.Location = new System.Drawing.Point(7, 77);
            this.color_imageDimming.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.color_imageDimming.Name = "color_imageDimming";
            this.color_imageDimming.Size = new System.Drawing.Size(338, 25);
            this.color_imageDimming.TabIndex = 40;
            // 
            // button_openInBrowser
            // 
            this.button_openInBrowser.Location = new System.Drawing.Point(5, 3);
            this.button_openInBrowser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_openInBrowser.Name = "button_openInBrowser";
            this.button_openInBrowser.Size = new System.Drawing.Size(226, 27);
            this.button_openInBrowser.TabIndex = 3;
            this.button_openInBrowser.Text = "Open overlay in browser";
            this.button_openInBrowser.UseVisualStyleBackColor = true;
            this.button_openInBrowser.Click += new System.EventHandler(this.button_openInBrowser_Click);
            // 
            // button_openFilesLocation
            // 
            this.button_openFilesLocation.Location = new System.Drawing.Point(5, 37);
            this.button_openFilesLocation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_openFilesLocation.Name = "button_openFilesLocation";
            this.button_openFilesLocation.Size = new System.Drawing.Size(226, 27);
            this.button_openFilesLocation.TabIndex = 4;
            this.button_openFilesLocation.Text = "Open overlay files location";
            this.button_openFilesLocation.UseVisualStyleBackColor = true;
            this.button_openFilesLocation.Click += new System.EventHandler(this.button_openFilesLocation_Click);
            // 
            // label_webUrl
            // 
            this.label_webUrl.AutoSize = true;
            this.label_webUrl.Location = new System.Drawing.Point(238, 9);
            this.label_webUrl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_webUrl.Name = "label_webUrl";
            this.label_webUrl.Size = new System.Drawing.Size(60, 15);
            this.label_webUrl.TabIndex = 0;
            this.label_webUrl.Text = "<webUrl>";
            // 
            // label_localFiles
            // 
            this.label_localFiles.AutoSize = true;
            this.label_localFiles.Location = new System.Drawing.Point(238, 43);
            this.label_localFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_localFiles.Name = "label_localFiles";
            this.label_localFiles.Size = new System.Drawing.Size(90, 15);
            this.label_localFiles.TabIndex = 5;
            this.label_localFiles.Text = "<filesLocation>";
            // 
            // WebOverlaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_localFiles);
            this.Controls.Add(this.label_webUrl);
            this.Controls.Add(this.button_openFilesLocation);
            this.Controls.Add(this.button_openInBrowser);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "WebOverlaySettings";
            this.Size = new System.Drawing.Size(758, 429);
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
        private System.Windows.Forms.CheckBox checkBox_hideChartLegend;
        private System.Windows.Forms.Button button_remoteOverlay;
        private System.Windows.Forms.TextBox textBox_remoteAccessUrl;
    }
}
