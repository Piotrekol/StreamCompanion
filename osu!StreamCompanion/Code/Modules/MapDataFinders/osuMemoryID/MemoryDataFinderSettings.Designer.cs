namespace osu_StreamCompanion.Code.Modules.MapDataFinders.osuMemoryID
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
            this.SuspendLayout();
            // 
            // checkBox_EnableMemoryFinder
            // 
            this.checkBox_EnableMemoryFinder.AutoSize = true;
            this.checkBox_EnableMemoryFinder.Location = new System.Drawing.Point(4, 42);
            this.checkBox_EnableMemoryFinder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_EnableMemoryFinder.Name = "checkBox_EnableMemoryFinder";
            this.checkBox_EnableMemoryFinder.Size = new System.Drawing.Size(179, 19);
            this.checkBox_EnableMemoryFinder.TabIndex = 0;
            this.checkBox_EnableMemoryFinder.Text = "Enable memory data scanner";
            this.checkBox_EnableMemoryFinder.UseVisualStyleBackColor = true;
            this.checkBox_EnableMemoryFinder.CheckedChanged += new System.EventHandler(this.checkBox_EnableMemoryFinder_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(480, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Option below gives accurate map search results when playing and adds !mods! comma" +
    "nd\r\n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(269, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "This doesn\'t work on Stable (fallback) osu! version";
            // 
            // MemoryDataFinderSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox_EnableMemoryFinder);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MemoryDataFinderSettings";
            this.Size = new System.Drawing.Size(690, 61);
            this.Load += new System.EventHandler(this.MemoryDataFinderSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_EnableMemoryFinder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}
