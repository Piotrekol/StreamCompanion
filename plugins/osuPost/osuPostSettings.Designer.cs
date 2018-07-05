namespace osuPost
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
            this.panel_settings = new System.Windows.Forms.Panel();
            this.panel_advanced = new System.Windows.Forms.Panel();
            this.button_resetEndpoint = new System.Windows.Forms.Button();
            this.textBox_endpointUrl = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox_advanced = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel_settings.SuspendLayout();
            this.panel_advanced.SuspendLayout();
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
            // panel_settings
            // 
            this.panel_settings.Controls.Add(this.panel_advanced);
            this.panel_settings.Controls.Add(this.checkBox_advanced);
            this.panel_settings.Controls.Add(this.label2);
            this.panel_settings.Controls.Add(this.label1);
            this.panel_settings.Controls.Add(this.textBox_userId);
            this.panel_settings.Controls.Add(this.textBox_userPassword);
            this.panel_settings.Location = new System.Drawing.Point(4, 27);
            this.panel_settings.Name = "panel_settings";
            this.panel_settings.Size = new System.Drawing.Size(482, 138);
            this.panel_settings.TabIndex = 11;
            // 
            // panel_advanced
            // 
            this.panel_advanced.Controls.Add(this.button_resetEndpoint);
            this.panel_advanced.Controls.Add(this.textBox_endpointUrl);
            this.panel_advanced.Controls.Add(this.label6);
            this.panel_advanced.Location = new System.Drawing.Point(238, 31);
            this.panel_advanced.Name = "panel_advanced";
            this.panel_advanced.Size = new System.Drawing.Size(241, 100);
            this.panel_advanced.TabIndex = 13;
            this.panel_advanced.Visible = false;
            // 
            // button_resetEndpoint
            // 
            this.button_resetEndpoint.Location = new System.Drawing.Point(6, 52);
            this.button_resetEndpoint.Name = "button_resetEndpoint";
            this.button_resetEndpoint.Size = new System.Drawing.Size(75, 23);
            this.button_resetEndpoint.TabIndex = 2;
            this.button_resetEndpoint.Text = "Reset";
            this.button_resetEndpoint.UseVisualStyleBackColor = true;
            this.button_resetEndpoint.Click += new System.EventHandler(this.button_resetEndpoint_Click);
            // 
            // textBox_endpointUrl
            // 
            this.textBox_endpointUrl.Location = new System.Drawing.Point(6, 28);
            this.textBox_endpointUrl.Name = "textBox_endpointUrl";
            this.textBox_endpointUrl.Size = new System.Drawing.Size(232, 20);
            this.textBox_endpointUrl.TabIndex = 1;
            this.textBox_endpointUrl.TextChanged += new System.EventHandler(this.textBox_endpointUrl_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Endpoint url:";
            // 
            // checkBox_advanced
            // 
            this.checkBox_advanced.AutoSize = true;
            this.checkBox_advanced.Location = new System.Drawing.Point(238, 8);
            this.checkBox_advanced.Name = "checkBox_advanced";
            this.checkBox_advanced.Size = new System.Drawing.Size(75, 17);
            this.checkBox_advanced.TabIndex = 12;
            this.checkBox_advanced.Text = "Advanced";
            this.checkBox_advanced.UseVisualStyleBackColor = true;
            this.checkBox_advanced.CheckedChanged += new System.EventHandler(this.checkBox_advanced_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(75, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "or";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(91, 120);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(28, 13);
            this.linkLabel2.TabIndex = 15;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "here";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(49, 120);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(28, 13);
            this.linkLabel1.TabIndex = 14;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "here";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Check";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "You don\'t know what osu!Post is?";
            // 
            // osuPostSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel_settings);
            this.Controls.Add(this.checkBox_osuPostEnabled);
            this.Name = "osuPostSettings";
            this.Size = new System.Drawing.Size(489, 168);
            this.panel_settings.ResumeLayout(false);
            this.panel_settings.PerformLayout();
            this.panel_advanced.ResumeLayout(false);
            this.panel_advanced.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_userId;
        private System.Windows.Forms.TextBox textBox_userPassword;
        public System.Windows.Forms.CheckBox checkBox_osuPostEnabled;
        private System.Windows.Forms.Panel panel_settings;
        public System.Windows.Forms.CheckBox checkBox_advanced;
        private System.Windows.Forms.Panel panel_advanced;
        private System.Windows.Forms.TextBox textBox_endpointUrl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_resetEndpoint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}
