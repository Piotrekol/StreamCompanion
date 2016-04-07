namespace osu_StreamCompanion.Code.Modules.SCGUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.UpdateText = new System.Windows.Forms.Label();
            this.button_OpenSettings = new System.Windows.Forms.Button();
            this.button_About = new System.Windows.Forms.Button();
            this.BeatmapsLoaded = new System.Windows.Forms.Label();
            this.NowPlaying = new System.Windows.Forms.Label();
            this.exit_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UpdateText
            // 
            this.UpdateText.AutoSize = true;
            this.UpdateText.BackColor = System.Drawing.Color.Transparent;
            this.UpdateText.ForeColor = System.Drawing.Color.Black;
            this.UpdateText.Location = new System.Drawing.Point(176, 9);
            this.UpdateText.Name = "UpdateText";
            this.UpdateText.Size = new System.Drawing.Size(75, 13);
            this.UpdateText.TabIndex = 1;
            this.UpdateText.Text = "_UpdateText_";
            // 
            // button_OpenSettings
            // 
            this.button_OpenSettings.Location = new System.Drawing.Point(49, 4);
            this.button_OpenSettings.Name = "button_OpenSettings";
            this.button_OpenSettings.Size = new System.Drawing.Size(54, 23);
            this.button_OpenSettings.TabIndex = 2;
            this.button_OpenSettings.Text = "Settings";
            this.button_OpenSettings.UseVisualStyleBackColor = true;
            // 
            // button_About
            // 
            this.button_About.Location = new System.Drawing.Point(109, 4);
            this.button_About.Name = "button_About";
            this.button_About.Size = new System.Drawing.Size(54, 23);
            this.button_About.TabIndex = 3;
            this.button_About.Text = "About";
            this.button_About.UseVisualStyleBackColor = true;
            this.button_About.Click += new System.EventHandler(this.button_About_Click);
            // 
            // BeatmapsLoaded
            // 
            this.BeatmapsLoaded.AutoSize = true;
            this.BeatmapsLoaded.BackColor = System.Drawing.Color.Transparent;
            this.BeatmapsLoaded.ForeColor = System.Drawing.Color.Beige;
            this.BeatmapsLoaded.Location = new System.Drawing.Point(76, 30);
            this.BeatmapsLoaded.Name = "BeatmapsLoaded";
            this.BeatmapsLoaded.Size = new System.Drawing.Size(102, 13);
            this.BeatmapsLoaded.TabIndex = 4;
            this.BeatmapsLoaded.Text = "_BeatmapsLoaded_";
            // 
            // NowPlaying
            // 
            this.NowPlaying.AutoSize = true;
            this.NowPlaying.BackColor = System.Drawing.Color.Transparent;
            this.NowPlaying.ForeColor = System.Drawing.Color.Beige;
            this.NowPlaying.Location = new System.Drawing.Point(20, 105);
            this.NowPlaying.MaximumSize = new System.Drawing.Size(618, 26);
            this.NowPlaying.MinimumSize = new System.Drawing.Size(618, 26);
            this.NowPlaying.Name = "NowPlaying";
            this.NowPlaying.Size = new System.Drawing.Size(618, 26);
            this.NowPlaying.TabIndex = 7;
            this.NowPlaying.Text = resources.GetString("NowPlaying.Text");
            this.NowPlaying.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exit_button
            // 
            this.exit_button.Location = new System.Drawing.Point(588, 4);
            this.exit_button.Name = "exit_button";
            this.exit_button.Size = new System.Drawing.Size(59, 23);
            this.exit_button.TabIndex = 8;
            this.exit_button.Text = "Exit";
            this.exit_button.UseVisualStyleBackColor = true;
            this.exit_button.Click += new System.EventHandler(this.exit_button_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::osu_StreamCompanion.Properties.Resources.main_BG;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(659, 171);
            this.Controls.Add(this.exit_button);
            this.Controls.Add(this.NowPlaying);
            this.Controls.Add(this.BeatmapsLoaded);
            this.Controls.Add(this.button_About);
            this.Controls.Add(this.button_OpenSettings);
            this.Controls.Add(this.UpdateText);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "osu!StreamCompanion by Piotrekol";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public System.Windows.Forms.Label UpdateText;
        private System.Windows.Forms.Button button_About;
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public System.Windows.Forms.Label BeatmapsLoaded;
        private System.Windows.Forms.Button exit_button;
        public System.Windows.Forms.Button button_OpenSettings;
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public System.Windows.Forms.Label NowPlaying;
    }
}

