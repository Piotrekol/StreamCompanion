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
            this.checkBox_EnableMemoryFinder = new System.Windows.Forms.CheckBox();
            this.checkBox_EnableMemoryPooling = new System.Windows.Forms.CheckBox();
            this.checkBox_enableSmoothPp = new System.Windows.Forms.CheckBox();
            this.checkBox_clearTokensAfterPlay = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_EnableMemoryFinder
            // 
            this.checkBox_EnableMemoryFinder.AutoSize = true;
            this.checkBox_EnableMemoryFinder.Location = new System.Drawing.Point(3, 3);
            this.checkBox_EnableMemoryFinder.Name = "checkBox_EnableMemoryFinder";
            this.checkBox_EnableMemoryFinder.Size = new System.Drawing.Size(368, 17);
            this.checkBox_EnableMemoryFinder.TabIndex = 0;
            this.checkBox_EnableMemoryFinder.Text = "Enable memory data scanner (adds !mods! && accurate map identification)";
            this.checkBox_EnableMemoryFinder.UseVisualStyleBackColor = true;
            this.checkBox_EnableMemoryFinder.CheckedChanged += new System.EventHandler(this.checkBox_EnableMemoryFinder_CheckedChanged);
            // 
            // checkBox_EnableMemoryPooling
            // 
            this.checkBox_EnableMemoryPooling.AutoSize = true;
            this.checkBox_EnableMemoryPooling.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBox_EnableMemoryPooling.Location = new System.Drawing.Point(3, 26);
            this.checkBox_EnableMemoryPooling.Name = "checkBox_EnableMemoryPooling";
            this.checkBox_EnableMemoryPooling.Size = new System.Drawing.Size(467, 17);
            this.checkBox_EnableMemoryPooling.TabIndex = 6;
            this.checkBox_EnableMemoryPooling.Text = "Enable memory pooling (adds live tokens && per-diff detection in song selection)," +
    " replaces MSN";
            this.checkBox_EnableMemoryPooling.UseVisualStyleBackColor = true;
            this.checkBox_EnableMemoryPooling.CheckedChanged += new System.EventHandler(this.checkBox_EnableMemoryPooling_CheckedChanged);
            // 
            // checkBox_enableSmoothPp
            // 
            this.checkBox_enableSmoothPp.AutoSize = true;
            this.checkBox_enableSmoothPp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBox_enableSmoothPp.Location = new System.Drawing.Point(3, 72);
            this.checkBox_enableSmoothPp.Name = "checkBox_enableSmoothPp";
            this.checkBox_enableSmoothPp.Size = new System.Drawing.Size(184, 17);
            this.checkBox_enableSmoothPp.TabIndex = 7;
            this.checkBox_enableSmoothPp.Text = "Enable smooth pp value changes";
            this.checkBox_enableSmoothPp.UseVisualStyleBackColor = true;
            this.checkBox_enableSmoothPp.CheckedChanged += new System.EventHandler(this.checkBox_enableSmoothPp_CheckedChanged);
            // 
            // checkBox_clearTokensAfterPlay
            // 
            this.checkBox_clearTokensAfterPlay.AutoSize = true;
            this.checkBox_clearTokensAfterPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBox_clearTokensAfterPlay.Location = new System.Drawing.Point(3, 49);
            this.checkBox_clearTokensAfterPlay.Name = "checkBox_clearTokensAfterPlay";
            this.checkBox_clearTokensAfterPlay.Size = new System.Drawing.Size(246, 17);
            this.checkBox_clearTokensAfterPlay.TabIndex = 8;
            this.checkBox_clearTokensAfterPlay.Text = "Clear live tokens(acc/pp) after finishing playing";
            this.checkBox_clearTokensAfterPlay.UseVisualStyleBackColor = true;
            this.checkBox_clearTokensAfterPlay.CheckedChanged += new System.EventHandler(this.checkBox_clearTokensAfterPlay_CheckedChanged);
            // 
            // MemoryDataFinderSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_clearTokensAfterPlay);
            this.Controls.Add(this.checkBox_enableSmoothPp);
            this.Controls.Add(this.checkBox_EnableMemoryPooling);
            this.Controls.Add(this.checkBox_EnableMemoryFinder);
            this.Name = "MemoryDataFinderSettings";
            this.Size = new System.Drawing.Size(591, 199);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_EnableMemoryFinder;
        private System.Windows.Forms.CheckBox checkBox_EnableMemoryPooling;
        private System.Windows.Forms.CheckBox checkBox_enableSmoothPp;
        private System.Windows.Forms.CheckBox checkBox_clearTokensAfterPlay;
    }
}
