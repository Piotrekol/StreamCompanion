namespace osu_StreamCompanion.Code.Windows
{
    partial class Error
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label_exitText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(518, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "Whoops! This error was sent to Piotrekol.";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(16, 57);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(515, 189);
            this.textBox1.TabIndex = 1;
            // 
            // label_exitText
            // 
            this.label_exitText.AutoSize = true;
            this.label_exitText.Location = new System.Drawing.Point(12, 262);
            this.label_exitText.Name = "label_exitText";
            this.label_exitText.Size = new System.Drawing.Size(155, 13);
            this.label_exitText.TabIndex = 2;
            this.label_exitText.Text = "StreamCompanion will now exit.";
            // 
            // Error
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 284);
            this.Controls.Add(this.label_exitText);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "Error";
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label_exitText;
    }
}