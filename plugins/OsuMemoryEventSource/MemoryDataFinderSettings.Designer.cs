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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBox_EnableMemoryPooling = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_EnableMemoryFinder
            // 
            this.checkBox_EnableMemoryFinder.AutoSize = true;
            this.checkBox_EnableMemoryFinder.Location = new System.Drawing.Point(3, 36);
            this.checkBox_EnableMemoryFinder.Name = "checkBox_EnableMemoryFinder";
            this.checkBox_EnableMemoryFinder.Size = new System.Drawing.Size(163, 17);
            this.checkBox_EnableMemoryFinder.TabIndex = 0;
            this.checkBox_EnableMemoryFinder.Text = "Enable memory data scanner";
            this.checkBox_EnableMemoryFinder.UseVisualStyleBackColor = true;
            this.checkBox_EnableMemoryFinder.CheckedChanged += new System.EventHandler(this.checkBox_EnableMemoryFinder_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(428, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Option below gives accurate map search results when playing and adds !mods! comma" +
    "nd\r\n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(244, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "This doesn\'t work on Stable (fallback) osu! version";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 71);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(310, 39);
            this.label9.TabIndex = 5;
            this.label9.Text = "This removes any need for MSN,\r\neverything will be manually grabbed from memory o" +
    "nce in a while\r\nProvides accurate difficulty outputs anywhere in osu!";
            // 
            // checkBox_EnableMemoryPooling
            // 
            this.checkBox_EnableMemoryPooling.AutoSize = true;
            this.checkBox_EnableMemoryPooling.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBox_EnableMemoryPooling.Location = new System.Drawing.Point(3, 113);
            this.checkBox_EnableMemoryPooling.Name = "checkBox_EnableMemoryPooling";
            this.checkBox_EnableMemoryPooling.Size = new System.Drawing.Size(135, 17);
            this.checkBox_EnableMemoryPooling.TabIndex = 6;
            this.checkBox_EnableMemoryPooling.Text = "Enable memory pooling";
            this.checkBox_EnableMemoryPooling.UseVisualStyleBackColor = true;
            this.checkBox_EnableMemoryPooling.CheckedChanged += new System.EventHandler(this.checkBox_EnableMemoryPooling_CheckedChanged);
            // 
            // MemoryDataFinderSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_EnableMemoryPooling);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox_EnableMemoryFinder);
            this.Name = "MemoryDataFinderSettings";
            this.Size = new System.Drawing.Size(591, 138);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_EnableMemoryFinder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBox_EnableMemoryPooling;
    }
}
