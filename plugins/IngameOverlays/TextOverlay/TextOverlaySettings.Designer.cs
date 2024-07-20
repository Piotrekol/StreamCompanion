namespace TextOverlay
{
    partial class TextOverlaySettings
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
            checkBox_ingameOverlay = new System.Windows.Forms.CheckBox();
            checkBox_noOsuRestartCheck = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // checkBox_ingameOverlay
            // 
            checkBox_ingameOverlay.AutoSize = true;
            checkBox_ingameOverlay.Location = new System.Drawing.Point(9, 10);
            checkBox_ingameOverlay.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            checkBox_ingameOverlay.Name = "checkBox_ingameOverlay";
            checkBox_ingameOverlay.Size = new System.Drawing.Size(481, 34);
            checkBox_ingameOverlay.TabIndex = 0;
            checkBox_ingameOverlay.Text = "Enable text ingame overlay (Requires SC restart)";
            checkBox_ingameOverlay.UseVisualStyleBackColor = true;
            // 
            // checkBox_noOsuRestartCheck
            // 
            checkBox_noOsuRestartCheck.AutoSize = true;
            checkBox_noOsuRestartCheck.Location = new System.Drawing.Point(9, 47);
            checkBox_noOsuRestartCheck.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            checkBox_noOsuRestartCheck.Name = "checkBox_noOsuRestartCheck";
            checkBox_noOsuRestartCheck.Size = new System.Drawing.Size(514, 34);
            checkBox_noOsuRestartCheck.TabIndex = 24;
            checkBox_noOsuRestartCheck.Text = "Don't require restarting osu! before starting overlay";
            checkBox_noOsuRestartCheck.UseVisualStyleBackColor = true;
            // 
            // TextOverlaySettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(checkBox_noOsuRestartCheck);
            Controls.Add(checkBox_ingameOverlay);
            Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            Name = "TextOverlaySettings";
            Size = new System.Drawing.Size(801, 195);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_ingameOverlay;
        private System.Windows.Forms.CheckBox checkBox_noOsuRestartCheck;
    }
}
