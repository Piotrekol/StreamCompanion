namespace WebSocketDataSender
{
    partial class WebSocketSettings
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
            this.checkBox_EnableWebSocketOutput = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_EnableWebSocketOutput
            // 
            this.checkBox_EnableWebSocketOutput.AutoSize = true;
            this.checkBox_EnableWebSocketOutput.Location = new System.Drawing.Point(4, 2);
            this.checkBox_EnableWebSocketOutput.Name = "checkBox_EnableWebSocketOutput";
            this.checkBox_EnableWebSocketOutput.Size = new System.Drawing.Size(122, 17);
            this.checkBox_EnableWebSocketOutput.TabIndex = 1;
            this.checkBox_EnableWebSocketOutput.Text = "Enable Web overlay";
            this.checkBox_EnableWebSocketOutput.UseVisualStyleBackColor = true;
            // 
            // WebSocketSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_EnableWebSocketOutput);
            this.Name = "WebSocketSettings";
            this.Size = new System.Drawing.Size(591, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_EnableWebSocketOutput;
    }
}
