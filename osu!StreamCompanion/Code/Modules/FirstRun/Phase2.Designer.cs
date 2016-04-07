namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    partial class Phase2
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_msnString = new System.Windows.Forms.Label();
            this.button_end = new System.Windows.Forms.Button();
            this.label_osuDirectory = new System.Windows.Forms.Label();
            this.label_Description2 = new System.Windows.Forms.Label();
            this.label_Description1 = new System.Windows.Forms.Label();
            this.label_Title = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_msnString);
            this.panel1.Controls.Add(this.button_end);
            this.panel1.Controls.Add(this.label_osuDirectory);
            this.panel1.Controls.Add(this.label_Description2);
            this.panel1.Controls.Add(this.label_Description1);
            this.panel1.Controls.Add(this.label_Title);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(399, 242);
            this.panel1.TabIndex = 2;
            // 
            // label_msnString
            // 
            this.label_msnString.Location = new System.Drawing.Point(140, 27);
            this.label_msnString.Name = "label_msnString";
            this.label_msnString.Size = new System.Drawing.Size(251, 51);
            this.label_msnString.TabIndex = 9;
            this.label_msnString.Text = "-\r\n-\r\n-";
            // 
            // button_end
            // 
            this.button_end.Location = new System.Drawing.Point(72, 211);
            this.button_end.Name = "button_end";
            this.button_end.Size = new System.Drawing.Size(254, 28);
            this.button_end.TabIndex = 8;
            this.button_end.Text = "End setup";
            this.button_end.UseVisualStyleBackColor = true;
            // 
            // label_osuDirectory
            // 
            this.label_osuDirectory.Location = new System.Drawing.Point(137, 98);
            this.label_osuDirectory.Name = "label_osuDirectory";
            this.label_osuDirectory.Size = new System.Drawing.Size(254, 33);
            this.label_osuDirectory.TabIndex = 6;
            this.label_osuDirectory.Text = "-\r\n-";
            // 
            // label_Description2
            // 
            this.label_Description2.Location = new System.Drawing.Point(3, 134);
            this.label_Description2.Name = "label_Description2";
            this.label_Description2.Size = new System.Drawing.Size(388, 24);
            this.label_Description2.TabIndex = 4;
            this.label_Description2.Text = "You can now close this setup.";
            // 
            // label_Description1
            // 
            this.label_Description1.Location = new System.Drawing.Point(137, 78);
            this.label_Description1.Name = "label_Description1";
            this.label_Description1.Size = new System.Drawing.Size(254, 20);
            this.label_Description1.TabIndex = 3;
            this.label_Description1.Text = "Your osu location:";
            // 
            // label_Title
            // 
            this.label_Title.Location = new System.Drawing.Point(137, 3);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(254, 20);
            this.label_Title.TabIndex = 2;
            this.label_Title.Text = "Got information from osu!:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::osu_StreamCompanion.Properties.Resources.logo_256x256;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Phase2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Phase2";
            this.Size = new System.Drawing.Size(399, 242);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_Description2;
        private System.Windows.Forms.Label label_Description1;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label label_osuDirectory;
        public System.Windows.Forms.Button button_end;
        public System.Windows.Forms.Label label_msnString;
    }
}
