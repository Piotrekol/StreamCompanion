namespace osu_StreamCompanion.Code.Modules.IrcBot
{
    partial class IrcBotSettings
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
            this.checkBox_enableTwitchBot = new System.Windows.Forms.CheckBox();
            this.panel_botOptions = new System.Windows.Forms.Panel();
            this.label_configureCommands = new System.Windows.Forms.Label();
            this.field_Channel = new osu_StreamCompanion.Code.Misc.Field();
            this.field_password = new osu_StreamCompanion.Code.Misc.Field();
            this.field_username = new osu_StreamCompanion.Code.Misc.Field();
            this.button_reconnect = new System.Windows.Forms.Button();
            this.panel_botOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox_enableTwitchBot
            // 
            this.checkBox_enableTwitchBot.AutoSize = true;
            this.checkBox_enableTwitchBot.Location = new System.Drawing.Point(4, 4);
            this.checkBox_enableTwitchBot.Name = "checkBox_enableTwitchBot";
            this.checkBox_enableTwitchBot.Size = new System.Drawing.Size(112, 17);
            this.checkBox_enableTwitchBot.TabIndex = 0;
            this.checkBox_enableTwitchBot.Text = "Enable Twitch bot";
            this.checkBox_enableTwitchBot.UseVisualStyleBackColor = true;
            // 
            // panel_botOptions
            // 
            this.panel_botOptions.Controls.Add(this.button_reconnect);
            this.panel_botOptions.Controls.Add(this.label_configureCommands);
            this.panel_botOptions.Controls.Add(this.field_Channel);
            this.panel_botOptions.Controls.Add(this.field_password);
            this.panel_botOptions.Controls.Add(this.field_username);
            this.panel_botOptions.Enabled = false;
            this.panel_botOptions.Location = new System.Drawing.Point(4, 28);
            this.panel_botOptions.Name = "panel_botOptions";
            this.panel_botOptions.Size = new System.Drawing.Size(605, 368);
            this.panel_botOptions.TabIndex = 1;
            // 
            // label_configureCommands
            // 
            this.label_configureCommands.AutoSize = true;
            this.label_configureCommands.Location = new System.Drawing.Point(3, 123);
            this.label_configureCommands.Name = "label_configureCommands";
            this.label_configureCommands.Size = new System.Drawing.Size(298, 13);
            this.label_configureCommands.TabIndex = 3;
            this.label_configureCommands.Text = "Add and configure your bot commands in \"Map formating\" tab\r\n";
            // 
            // field_Channel
            // 
            this.field_Channel.AutoSize = true;
            this.field_Channel.IsPassword = false;
            this.field_Channel.LabelText = "Channel name:    ";
            this.field_Channel.Location = new System.Drawing.Point(4, 65);
            this.field_Channel.Name = "field_Channel";
            this.field_Channel.Size = new System.Drawing.Size(268, 24);
            this.field_Channel.TabIndex = 2;
            // 
            // field_password
            // 
            this.field_password.AutoSize = true;
            this.field_password.IsPassword = true;
            this.field_password.LabelText = "Twitch Oauth:     ";
            this.field_password.Location = new System.Drawing.Point(4, 34);
            this.field_password.Name = "field_password";
            this.field_password.Size = new System.Drawing.Size(267, 24);
            this.field_password.TabIndex = 1;
            // 
            // field_username
            // 
            this.field_username.AutoSize = true;
            this.field_username.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.field_username.IsPassword = false;
            this.field_username.LabelText = "Twitch username:";
            this.field_username.Location = new System.Drawing.Point(3, 3);
            this.field_username.Name = "field_username";
            this.field_username.Size = new System.Drawing.Size(269, 24);
            this.field_username.TabIndex = 0;
            // 
            // button_reconnect
            // 
            this.button_reconnect.Location = new System.Drawing.Point(101, 95);
            this.button_reconnect.Name = "button_reconnect";
            this.button_reconnect.Size = new System.Drawing.Size(75, 23);
            this.button_reconnect.TabIndex = 4;
            this.button_reconnect.Text = "Reconnect";
            this.button_reconnect.UseVisualStyleBackColor = true;
            // 
            // IrcBotSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel_botOptions);
            this.Controls.Add(this.checkBox_enableTwitchBot);
            this.Name = "IrcBotSettings";
            this.Size = new System.Drawing.Size(612, 399);
            this.panel_botOptions.ResumeLayout(false);
            this.panel_botOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_enableTwitchBot;
        private System.Windows.Forms.Panel panel_botOptions;
        private Misc.Field field_username;
        private Misc.Field field_password;
        private Misc.Field field_Channel;
        private System.Windows.Forms.Label label_configureCommands;
        public System.Windows.Forms.Button button_reconnect;
    }
}
