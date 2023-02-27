
namespace BrowserOverlay
{
    partial class OverlayDownload
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
            this.progressBar_downloadProgress = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label_status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar_downloadProgress
            // 
            this.progressBar_downloadProgress.Location = new System.Drawing.Point(13, 48);
            this.progressBar_downloadProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressBar_downloadProgress.Name = "progressBar_downloadProgress";
            this.progressBar_downloadProgress.Size = new System.Drawing.Size(368, 27);
            this.progressBar_downloadProgress.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(319, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Browser overlay is downloading missing assets. Please wait.";
            // 
            // label_status
            // 
            this.label_status.Location = new System.Drawing.Point(13, 27);
            this.label_status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(368, 18);
            this.label_status.TabIndex = 4;
            this.label_status.Text = "download status";
            this.label_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OverlayDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 99);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar_downloadProgress);
            this.Name = "OverlayDownload";
            this.Text = "BrowserOverlay - Download";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar_downloadProgress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_status;
    }
}