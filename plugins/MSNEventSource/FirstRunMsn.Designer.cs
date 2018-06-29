namespace MSNEventSource
{
    partial class FirstRunMsn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstRunMsn));
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.panel1.Controls.Add(this.label_Description2);
            this.panel1.Controls.Add(this.label_Description1);
            this.panel1.Controls.Add(this.label_Title);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(399, 242);
            this.panel1.TabIndex = 1;
            // 
            // label_Description2
            // 
            this.label_Description2.Location = new System.Drawing.Point(3, 134);
            this.label_Description2.Name = "label_Description2";
            this.label_Description2.Size = new System.Drawing.Size(388, 76);
            this.label_Description2.TabIndex = 4;
            this.label_Description2.Text = resources.GetString("label_Description2.Text");
            // 
            // label_Description1
            // 
            this.label_Description1.Location = new System.Drawing.Point(137, 78);
            this.label_Description1.Name = "label_Description1";
            this.label_Description1.Size = new System.Drawing.Size(254, 53);
            this.label_Description1.TabIndex = 3;
            this.label_Description1.Text = "First, We will need you to start your osu! and enable one specific option that al" +
    "lows StreamCompanion to work";
            // 
            // label_Title
            // 
            this.label_Title.Location = new System.Drawing.Point(137, 3);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(254, 67);
            this.label_Title.TabIndex = 2;
            this.label_Title.Text = "Welcome!\r\n\r\nAs this is first time you\'re running StreamCompanion we\'ll setup some" +
    " basic options";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Phase1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "FirstRunMsn";
            this.Size = new System.Drawing.Size(399, 242);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Description1;
        private System.Windows.Forms.Label label_Description2;
        private System.Windows.Forms.Panel panel1;
    }
}
