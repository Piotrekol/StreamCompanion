namespace ScGui
{
    partial class SettingsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tvSettings = new System.Windows.Forms.TreeView();
            this.pSettingTab = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // tvSettings
            // 
            this.tvSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvSettings.FullRowSelect = true;
            this.tvSettings.Location = new System.Drawing.Point(3, 3);
            this.tvSettings.Name = "tvSettings";
            this.tvSettings.ShowLines = false;
            this.tvSettings.ShowNodeToolTips = true;
            this.tvSettings.ShowPlusMinus = false;
            this.tvSettings.ShowRootLines = false;
            this.tvSettings.Size = new System.Drawing.Size(189, 595);
            this.tvSettings.TabIndex = 1;
            this.tvSettings.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvSettings_BeforeCollapse);
            this.tvSettings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSettings_AfterSelect);
            // 
            // pSettingTab
            // 
            this.pSettingTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pSettingTab.Location = new System.Drawing.Point(195, 3);
            this.pSettingTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pSettingTab.Name = "pSettingTab";
            this.pSettingTab.Size = new System.Drawing.Size(769, 595);
            this.pSettingTab.TabIndex = 2;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 601);
            this.Controls.Add(this.pSettingTab);
            this.Controls.Add(this.tvSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(780, 640);
            this.Name = "SettingsForm";
            this.Text = "StreamCompanion Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TreeView tvSettings;
        private System.Windows.Forms.Panel pSettingTab;
    }
}