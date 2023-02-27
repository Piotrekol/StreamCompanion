namespace osu_StreamCompanion.Code.Modules.Logger
{
    partial class LoggerSettingsUserControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_logVerbosity = new System.Windows.Forms.ComboBox();
            this.checkBox_consoleLogger = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 15);
            this.label2.TabIndex = 28;
            this.label2.Text = "Log verbosity:";
            // 
            // comboBox_logVerbosity
            // 
            this.comboBox_logVerbosity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_logVerbosity.FormattingEnabled = true;
            this.comboBox_logVerbosity.Location = new System.Drawing.Point(97, 3);
            this.comboBox_logVerbosity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox_logVerbosity.Name = "comboBox_logVerbosity";
            this.comboBox_logVerbosity.Size = new System.Drawing.Size(162, 23);
            this.comboBox_logVerbosity.TabIndex = 27;
            // 
            // checkBox_consoleLogger
            // 
            this.checkBox_consoleLogger.AutoSize = true;
            this.checkBox_consoleLogger.Location = new System.Drawing.Point(8, 35);
            this.checkBox_consoleLogger.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox_consoleLogger.Name = "checkBox_consoleLogger";
            this.checkBox_consoleLogger.Size = new System.Drawing.Size(106, 19);
            this.checkBox_consoleLogger.TabIndex = 29;
            this.checkBox_consoleLogger.Text = "Console logger";
            this.checkBox_consoleLogger.UseVisualStyleBackColor = true;
            // 
            // LoggerSettingsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_consoleLogger);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_logVerbosity);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "LoggerSettingsUserControl";
            this.Size = new System.Drawing.Size(358, 53);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_logVerbosity;
        private System.Windows.Forms.CheckBox checkBox_consoleLogger;
    }
}
