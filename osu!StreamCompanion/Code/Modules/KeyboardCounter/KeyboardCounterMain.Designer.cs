namespace osu_StreamCompanion.Code.Modules.KeyboardCounter
{
    partial class KeyboardCounterMain
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_K2T = new System.Windows.Forms.Label();
            this.button_KeyCounterReset = new System.Windows.Forms.Button();
            this.label_K1T = new System.Windows.Forms.Label();
            this.label_K2 = new System.Windows.Forms.Label();
            this.label_K1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_K2T);
            this.groupBox1.Controls.Add(this.button_KeyCounterReset);
            this.groupBox1.Controls.Add(this.label_K1T);
            this.groupBox1.Controls.Add(this.label_K2);
            this.groupBox1.Controls.Add(this.label_K1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(93, 73);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "KeyCounter";
            // 
            // label_K2T
            // 
            this.label_K2T.AutoSize = true;
            this.label_K2T.Location = new System.Drawing.Point(41, 53);
            this.label_K2T.Name = "label_K2T";
            this.label_K2T.Size = new System.Drawing.Size(10, 13);
            this.label_K2T.TabIndex = 1;
            this.label_K2T.Text = "-";
            // 
            // button_KeyCounterReset
            // 
            this.button_KeyCounterReset.Location = new System.Drawing.Point(3, 16);
            this.button_KeyCounterReset.Margin = new System.Windows.Forms.Padding(0);
            this.button_KeyCounterReset.Name = "button_KeyCounterReset";
            this.button_KeyCounterReset.Size = new System.Drawing.Size(48, 24);
            this.button_KeyCounterReset.TabIndex = 2;
            this.button_KeyCounterReset.Text = "Reset";
            this.button_KeyCounterReset.UseVisualStyleBackColor = true;
            // 
            // label_K1T
            // 
            this.label_K1T.AutoSize = true;
            this.label_K1T.Location = new System.Drawing.Point(41, 40);
            this.label_K1T.Name = "label_K1T";
            this.label_K1T.Size = new System.Drawing.Size(10, 13);
            this.label_K1T.TabIndex = 1;
            this.label_K1T.Text = "-";
            // 
            // label_K2
            // 
            this.label_K2.AutoSize = true;
            this.label_K2.Location = new System.Drawing.Point(0, 53);
            this.label_K2.Name = "label_K2";
            this.label_K2.Size = new System.Drawing.Size(23, 13);
            this.label_K2.TabIndex = 1;
            this.label_K2.Text = "K2:";
            // 
            // label_K1
            // 
            this.label_K1.AutoSize = true;
            this.label_K1.Location = new System.Drawing.Point(0, 40);
            this.label_K1.Name = "label_K1";
            this.label_K1.Size = new System.Drawing.Size(23, 13);
            this.label_K1.TabIndex = 1;
            this.label_K1.Text = "K1:";
            // 
            // KeyboardCounterMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "KeyboardCounterMain";
            this.Size = new System.Drawing.Size(99, 77);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_K2;
        private System.Windows.Forms.Label label_K1;
        public System.Windows.Forms.Label label_K1T;
        public System.Windows.Forms.Label label_K2T;
        public System.Windows.Forms.Button button_KeyCounterReset;
    }
}
