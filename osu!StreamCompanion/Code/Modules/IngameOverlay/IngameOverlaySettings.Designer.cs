namespace osu_StreamCompanion.Code.Modules.IngameOverlay
{
    partial class IngameOverlaySettings
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
            this.checkBox_ingameOverlay = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_ingameOverlay
            // 
            this.checkBox_ingameOverlay.AutoSize = true;
            this.checkBox_ingameOverlay.Location = new System.Drawing.Point(4, 4);
            this.checkBox_ingameOverlay.Name = "checkBox_ingameOverlay";
            this.checkBox_ingameOverlay.Size = new System.Drawing.Size(233, 17);
            this.checkBox_ingameOverlay.TabIndex = 0;
            this.checkBox_ingameOverlay.Text = "Enable ingame overlay (Requires SC restart)";
            this.checkBox_ingameOverlay.UseVisualStyleBackColor = true;
            // 
            // IngameOverlaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_ingameOverlay);
            this.Name = "IngameOverlaySettings";
            this.Size = new System.Drawing.Size(259, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_ingameOverlay;
    }
}
