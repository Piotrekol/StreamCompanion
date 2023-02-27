namespace OsuMemoryEventSource
{
    partial class MemoryDataFinderSettings
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
            this.checkBox_enableSmoothPp = new System.Windows.Forms.CheckBox();
            this.checkBox_saveLiveTokensToDisk = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_enableSmoothPp
            // 
            this.checkBox_enableSmoothPp.AutoSize = true;
            this.checkBox_enableSmoothPp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkBox_enableSmoothPp.Location = new System.Drawing.Point(4, 3);
            this.checkBox_enableSmoothPp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_enableSmoothPp.Name = "checkBox_enableSmoothPp";
            this.checkBox_enableSmoothPp.Size = new System.Drawing.Size(184, 17);
            this.checkBox_enableSmoothPp.TabIndex = 7;
            this.checkBox_enableSmoothPp.Text = "Enable smooth pp value changes";
            this.checkBox_enableSmoothPp.UseVisualStyleBackColor = true;
            this.checkBox_enableSmoothPp.CheckedChanged += new System.EventHandler(this.checkBox_enableSmoothPp_CheckedChanged);
            // 
            // checkBox_saveLiveTokensToDisk
            // 
            this.checkBox_saveLiveTokensToDisk.AutoSize = true;
            this.checkBox_saveLiveTokensToDisk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkBox_saveLiveTokensToDisk.Location = new System.Drawing.Point(4, 29);
            this.checkBox_saveLiveTokensToDisk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_saveLiveTokensToDisk.Name = "checkBox_saveLiveTokensToDisk";
            this.checkBox_saveLiveTokensToDisk.Size = new System.Drawing.Size(361, 17);
            this.checkBox_saveLiveTokensToDisk.TabIndex = 8;
            this.checkBox_saveLiveTokensToDisk.Text = "Output live tokens to text files on disk (SLOW, NOT RECOMMENDED)";
            this.checkBox_saveLiveTokensToDisk.UseVisualStyleBackColor = true;
            this.checkBox_saveLiveTokensToDisk.CheckedChanged += new System.EventHandler(this.checkBox_saveLiveTokensToDisk_CheckedChanged);
            // 
            // MemoryDataFinderSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_saveLiveTokensToDisk);
            this.Controls.Add(this.checkBox_enableSmoothPp);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MemoryDataFinderSettings";
            this.Size = new System.Drawing.Size(690, 54);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkBox_enableSmoothPp;
        private System.Windows.Forms.CheckBox checkBox_saveLiveTokensToDisk;
    }
}
