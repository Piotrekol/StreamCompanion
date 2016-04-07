namespace osu_StreamCompanion.Code.Modules.osuPathReslover
{
    partial class OsuPathResolverSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_osuDir = new System.Windows.Forms.TextBox();
            this.button_AutoDetect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "osu! Directory:";
            // 
            // textBox_osuDir
            // 
            this.textBox_osuDir.Location = new System.Drawing.Point(81, 3);
            this.textBox_osuDir.Name = "textBox_osuDir";
            this.textBox_osuDir.Size = new System.Drawing.Size(209, 20);
            this.textBox_osuDir.TabIndex = 1;
            // 
            // button_AutoDetect
            // 
            this.button_AutoDetect.Location = new System.Drawing.Point(297, 2);
            this.button_AutoDetect.Name = "button_AutoDetect";
            this.button_AutoDetect.Size = new System.Drawing.Size(75, 23);
            this.button_AutoDetect.TabIndex = 2;
            this.button_AutoDetect.Text = "Auto detect";
            this.button_AutoDetect.UseVisualStyleBackColor = true;
            // 
            // OsuPathResolverSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_AutoDetect);
            this.Controls.Add(this.textBox_osuDir);
            this.Controls.Add(this.label1);
            this.Name = "OsuPathResolverSettings";
            this.Size = new System.Drawing.Size(392, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox textBox_osuDir;
        public System.Windows.Forms.Button button_AutoDetect;
    }
}
