namespace osu_StreamCompanion.Code.Modules.Updater
{
    partial class UpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel_downloadProgress = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label_currentVersion = new System.Windows.Forms.Label();
            this.label_newVersion = new System.Windows.Forms.Label();
            this.richTextBox_changelog = new System.Windows.Forms.RichTextBox();
            this.button_update = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.label_downloadProgress = new System.Windows.Forms.Label();
            this.panel_downloadProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 27);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(248, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // panel_downloadProgress
            // 
            this.panel_downloadProgress.Controls.Add(this.label_downloadProgress);
            this.panel_downloadProgress.Controls.Add(this.progressBar1);
            this.panel_downloadProgress.Location = new System.Drawing.Point(170, 58);
            this.panel_downloadProgress.Name = "panel_downloadProgress";
            this.panel_downloadProgress.Size = new System.Drawing.Size(272, 62);
            this.panel_downloadProgress.TabIndex = 1;
            this.panel_downloadProgress.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "There\'s update avaliable for StreamCompanion";
            // 
            // label_currentVersion
            // 
            this.label_currentVersion.AutoSize = true;
            this.label_currentVersion.Location = new System.Drawing.Point(12, 36);
            this.label_currentVersion.Name = "label_currentVersion";
            this.label_currentVersion.Size = new System.Drawing.Size(13, 13);
            this.label_currentVersion.TabIndex = 3;
            this.label_currentVersion.Text = "--";
            // 
            // label_newVersion
            // 
            this.label_newVersion.AutoSize = true;
            this.label_newVersion.Location = new System.Drawing.Point(12, 60);
            this.label_newVersion.Name = "label_newVersion";
            this.label_newVersion.Size = new System.Drawing.Size(13, 13);
            this.label_newVersion.TabIndex = 4;
            this.label_newVersion.Text = "--";
            // 
            // richTextBox_changelog
            // 
            this.richTextBox_changelog.Location = new System.Drawing.Point(0, 126);
            this.richTextBox_changelog.Name = "richTextBox_changelog";
            this.richTextBox_changelog.ReadOnly = true;
            this.richTextBox_changelog.Size = new System.Drawing.Size(454, 143);
            this.richTextBox_changelog.TabIndex = 5;
            this.richTextBox_changelog.Text = "";
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(12, 97);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(73, 23);
            this.button_update.TabIndex = 6;
            this.button_update.Text = "Update";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(91, 97);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(73, 23);
            this.button_cancel.TabIndex = 7;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // label_downloadProgress
            // 
            this.label_downloadProgress.Location = new System.Drawing.Point(12, 2);
            this.label_downloadProgress.Name = "label_downloadProgress";
            this.label_downloadProgress.Size = new System.Drawing.Size(248, 22);
            this.label_downloadProgress.TabIndex = 8;
            this.label_downloadProgress.Text = "Initalizing download...";
            this.label_downloadProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 268);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_update);
            this.Controls.Add(this.richTextBox_changelog);
            this.Controls.Add(this.label_newVersion);
            this.Controls.Add(this.label_currentVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel_downloadProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdateForm";
            this.Text = "StreamCompanion - Update is avaliable!";
            this.TopMost = true;
            this.panel_downloadProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel_downloadProgress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_currentVersion;
        private System.Windows.Forms.Label label_newVersion;
        private System.Windows.Forms.RichTextBox richTextBox_changelog;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Label label_downloadProgress;
    }
}