namespace osu_StreamCompanion.Code.Modules.osuPost
{
    partial class osuPostSettings
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
            this.checkBox_osuPostEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_userId = new System.Windows.Forms.TextBox();
            this.textBox_userPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel_settings = new System.Windows.Forms.Panel();
            this.panel_settings.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox_osuPostEnabled
            // 
            this.checkBox_osuPostEnabled.AutoSize = true;
            this.checkBox_osuPostEnabled.Location = new System.Drawing.Point(4, 4);
            this.checkBox_osuPostEnabled.Name = "checkBox_osuPostEnabled";
            this.checkBox_osuPostEnabled.Size = new System.Drawing.Size(103, 17);
            this.checkBox_osuPostEnabled.TabIndex = 0;
            this.checkBox_osuPostEnabled.Text = "Enable osu!Post";
            this.checkBox_osuPostEnabled.UseVisualStyleBackColor = true;
            this.checkBox_osuPostEnabled.CheckedChanged += new System.EventHandler(this.checkBox_osuPostEnabled_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "User ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "User Key:";
            // 
            // textBox_userId
            // 
            this.textBox_userId.Location = new System.Drawing.Point(60, 5);
            this.textBox_userId.Name = "textBox_userId";
            this.textBox_userId.PasswordChar = '*';
            this.textBox_userId.Size = new System.Drawing.Size(162, 20);
            this.textBox_userId.TabIndex = 3;
            this.textBox_userId.TextChanged += new System.EventHandler(this.textBox_login_TextChanged);
            // 
            // textBox_userPassword
            // 
            this.textBox_userPassword.Location = new System.Drawing.Point(60, 31);
            this.textBox_userPassword.Name = "textBox_userPassword";
            this.textBox_userPassword.PasswordChar = '*';
            this.textBox_userPassword.Size = new System.Drawing.Size(162, 20);
            this.textBox_userPassword.TabIndex = 4;
            this.textBox_userPassword.TextChanged += new System.EventHandler(this.textBox_password_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "You don\'t know what osu!Post is?";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Check";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(46, 83);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(28, 13);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "here";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(88, 83);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(28, 13);
            this.linkLabel2.TabIndex = 9;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "here";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(72, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "or";
            // 
            // panel_settings
            // 
            this.panel_settings.Controls.Add(this.label2);
            this.panel_settings.Controls.Add(this.label5);
            this.panel_settings.Controls.Add(this.label1);
            this.panel_settings.Controls.Add(this.linkLabel2);
            this.panel_settings.Controls.Add(this.textBox_userId);
            this.panel_settings.Controls.Add(this.linkLabel1);
            this.panel_settings.Controls.Add(this.textBox_userPassword);
            this.panel_settings.Controls.Add(this.label4);
            this.panel_settings.Controls.Add(this.label3);
            this.panel_settings.Location = new System.Drawing.Point(4, 27);
            this.panel_settings.Name = "panel_settings";
            this.panel_settings.Size = new System.Drawing.Size(244, 105);
            this.panel_settings.TabIndex = 11;
            // 
            // osuPostSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel_settings);
            this.Controls.Add(this.checkBox_osuPostEnabled);
            this.Name = "osuPostSettings";
            this.Size = new System.Drawing.Size(489, 168);
            this.panel_settings.ResumeLayout(false);
            this.panel_settings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_userId;
        private System.Windows.Forms.TextBox textBox_userPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox checkBox_osuPostEnabled;
        private System.Windows.Forms.Panel panel_settings;
    }
}
