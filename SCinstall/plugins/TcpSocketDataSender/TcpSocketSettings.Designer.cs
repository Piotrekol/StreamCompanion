namespace TcpSocketDataSender
{
    partial class TcpSocketSettings
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
            this.checkBox_EnableTcpOutput = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_EnableTcpOutput
            // 
            this.checkBox_EnableTcpOutput.AutoSize = true;
            this.checkBox_EnableTcpOutput.Location = new System.Drawing.Point(4, 2);
            this.checkBox_EnableTcpOutput.Name = "checkBox_EnableTcpOutput";
            this.checkBox_EnableTcpOutput.Size = new System.Drawing.Size(169, 17);
            this.checkBox_EnableTcpOutput.TabIndex = 0;
            this.checkBox_EnableTcpOutput.Text = "Enable TCP output of patterns";
            this.checkBox_EnableTcpOutput.UseVisualStyleBackColor = true;
            // 
            // TcpSocketSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_EnableTcpOutput);
            this.Name = "TcpSocketSettings";
            this.Size = new System.Drawing.Size(591, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_EnableTcpOutput;
    }
}
