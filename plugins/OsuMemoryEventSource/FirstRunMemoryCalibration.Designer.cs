namespace OsuMemoryEventSource
{
    partial class FirstRunMemoryCalibration
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label_msnString = new System.Windows.Forms.Label();
            this.label_beatmapDL = new System.Windows.Forms.Label();
            this.label_CalibrationResult = new System.Windows.Forms.Label();
            this.linkLabel_mapDL = new System.Windows.Forms.LinkLabel();
            this.button_Next = new System.Windows.Forms.Button();
            this.button_anotherMap = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label_memoryStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label_msnString
            // 
            this.label_msnString.Location = new System.Drawing.Point(137, 3);
            this.label_msnString.Name = "label_msnString";
            this.label_msnString.Size = new System.Drawing.Size(251, 45);
            this.label_msnString.TabIndex = 10;
            this.label_msnString.Text = "We\'ll now perform small test to make sure that memory pooling and !mods! command " +
    "works correctly on your system.";
            // 
            // label_beatmapDL
            // 
            this.label_beatmapDL.Location = new System.Drawing.Point(136, 48);
            this.label_beatmapDL.Name = "label_beatmapDL";
            this.label_beatmapDL.Size = new System.Drawing.Size(251, 53);
            this.label_beatmapDL.TabIndex = 11;
            this.label_beatmapDL.Text = "Enable following mods:\r\n$\r\n$";
            // 
            // label_CalibrationResult
            // 
            this.label_CalibrationResult.Location = new System.Drawing.Point(3, 142);
            this.label_CalibrationResult.Name = "label_CalibrationResult";
            this.label_CalibrationResult.Size = new System.Drawing.Size(385, 30);
            this.label_CalibrationResult.TabIndex = 12;
            this.label_CalibrationResult.Text = "Waiting...";
            // 
            // linkLabel_mapDL
            // 
            this.linkLabel_mapDL.Location = new System.Drawing.Point(137, 105);
            this.linkLabel_mapDL.Name = "linkLabel_mapDL";
            this.linkLabel_mapDL.Size = new System.Drawing.Size(250, 31);
            this.linkLabel_mapDL.TabIndex = 13;
            this.linkLabel_mapDL.TabStop = true;
            this.linkLabel_mapDL.Text = "Click here for map download";
            this.linkLabel_mapDL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_mapDL_LinkClicked);
            // 
            // button_Next
            // 
            this.button_Next.Location = new System.Drawing.Point(72, 222);
            this.button_Next.Name = "button_Next";
            this.button_Next.Size = new System.Drawing.Size(254, 28);
            this.button_Next.TabIndex = 14;
            this.button_Next.Text = "Next";
            this.button_Next.UseVisualStyleBackColor = true;
            this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
            // 
            // button_anotherMap
            // 
            this.button_anotherMap.Location = new System.Drawing.Point(309, 104);
            this.button_anotherMap.Name = "button_anotherMap";
            this.button_anotherMap.Size = new System.Drawing.Size(78, 23);
            this.button_anotherMap.TabIndex = 15;
            this.button_anotherMap.Text = "No";
            this.button_anotherMap.UseVisualStyleBackColor = true;
            this.button_anotherMap.Click += new System.EventHandler(this.button_anotherMap_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(385, 29);
            this.label1.TabIndex = 16;
            this.label1.Text = "You can skip this step, but you won\'t be able to get data like live pp or hit cou" +
    "nts";
            // 
            // label_memoryStatus
            // 
            this.label_memoryStatus.Location = new System.Drawing.Point(3, 174);
            this.label_memoryStatus.Name = "label_memoryStatus";
            this.label_memoryStatus.Size = new System.Drawing.Size(385, 17);
            this.label_memoryStatus.TabIndex = 17;
            this.label_memoryStatus.Text = "----";
            // 
            // FirstRunMemoryCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_memoryStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_anotherMap);
            this.Controls.Add(this.button_Next);
            this.Controls.Add(this.linkLabel_mapDL);
            this.Controls.Add(this.label_CalibrationResult);
            this.Controls.Add(this.label_beatmapDL);
            this.Controls.Add(this.label_msnString);
            this.Controls.Add(this.pictureBox1);
            this.Name = "FirstRunMemoryCalibration";
            this.Size = new System.Drawing.Size(399, 253);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label label_msnString;
        public System.Windows.Forms.Label label_beatmapDL;
        public System.Windows.Forms.Label label_CalibrationResult;
        private System.Windows.Forms.LinkLabel linkLabel_mapDL;
        public System.Windows.Forms.Button button_Next;
        private System.Windows.Forms.Button button_anotherMap;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label_memoryStatus;
    }
}
