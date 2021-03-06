namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    partial class PatternEdit
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
            this.label_commandName = new System.Windows.Forms.Label();
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.button_save = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_formating = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_preview = new System.Windows.Forms.TextBox();
            this.button_addNew = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.comboBox_saveEvent = new System.Windows.Forms.ComboBox();
            this.label_warning = new System.Windows.Forms.Label();
            this.checkBox_ShowIngame = new System.Windows.Forms.CheckBox();
            this.numericUpDown_XPosition = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_YPosition = new System.Windows.Forms.NumericUpDown();
            this.panel_showInOsu = new System.Windows.Forms.Panel();
            this.comboBox_font = new System.Windows.Forms.ComboBox();
            this.comboBox_align = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown_colorAlpha = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_fontSize = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label_TestText = new System.Windows.Forms.Label();
            this.panel_ColorPreview = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_statusInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_XPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_YPosition)).BeginInit();
            this.panel_showInOsu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_colorAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label_commandName
            // 
            this.label_commandName.AutoSize = true;
            this.label_commandName.Location = new System.Drawing.Point(3, 9);
            this.label_commandName.Name = "label_commandName";
            this.label_commandName.Size = new System.Drawing.Size(107, 13);
            this.label_commandName.TabIndex = 5;
            this.label_commandName.Text = "File/Command name:";
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.Location = new System.Drawing.Point(116, 6);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.Size = new System.Drawing.Size(200, 20);
            this.textBox_FileName.TabIndex = 4;
            this.textBox_FileName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_FileName_KeyPress);
            // 
            // button_save
            // 
            this.button_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_save.Location = new System.Drawing.Point(6, 172);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 6;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Formating:";
            // 
            // textBox_formating
            // 
            this.textBox_formating.Location = new System.Drawing.Point(116, 32);
            this.textBox_formating.Name = "textBox_formating";
            this.textBox_formating.Size = new System.Drawing.Size(521, 20);
            this.textBox_formating.TabIndex = 7;
            this.textBox_formating.TextChanged += new System.EventHandler(this.textBox_formating_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Preview:";
            // 
            // textBox_preview
            // 
            this.textBox_preview.Location = new System.Drawing.Point(116, 58);
            this.textBox_preview.Name = "textBox_preview";
            this.textBox_preview.ReadOnly = true;
            this.textBox_preview.Size = new System.Drawing.Size(521, 20);
            this.textBox_preview.TabIndex = 9;
            // 
            // button_addNew
            // 
            this.button_addNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_addNew.Location = new System.Drawing.Point(87, 172);
            this.button_addNew.Name = "button_addNew";
            this.button_addNew.Size = new System.Drawing.Size(75, 23);
            this.button_addNew.TabIndex = 11;
            this.button_addNew.Text = "Add new";
            this.button_addNew.UseVisualStyleBackColor = true;
            this.button_addNew.Click += new System.EventHandler(this.button_Click);
            // 
            // button_delete
            // 
            this.button_delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_delete.Location = new System.Drawing.Point(168, 172);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(75, 23);
            this.button_delete.TabIndex = 12;
            this.button_delete.Text = "Delete";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_Click);
            // 
            // comboBox_saveEvent
            // 
            this.comboBox_saveEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_saveEvent.FormattingEnabled = true;
            this.comboBox_saveEvent.Location = new System.Drawing.Point(461, 6);
            this.comboBox_saveEvent.Name = "comboBox_saveEvent";
            this.comboBox_saveEvent.Size = new System.Drawing.Size(176, 21);
            this.comboBox_saveEvent.TabIndex = 14;
            // 
            // label_warning
            // 
            this.label_warning.AutoSize = true;
            this.label_warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_warning.ForeColor = System.Drawing.Color.Crimson;
            this.label_warning.Location = new System.Drawing.Point(3, 155);
            this.label_warning.Name = "label_warning";
            this.label_warning.Size = new System.Drawing.Size(316, 13);
            this.label_warning.TabIndex = 15;
            this.label_warning.Text = "Detected live tokens. Use OBS plugin or ingame overlay to read it";
            this.label_warning.Visible = false;
            // 
            // checkBox_ShowIngame
            // 
            this.checkBox_ShowIngame.AutoSize = true;
            this.checkBox_ShowIngame.Location = new System.Drawing.Point(4, 83);
            this.checkBox_ShowIngame.Name = "checkBox_ShowIngame";
            this.checkBox_ShowIngame.Size = new System.Drawing.Size(87, 17);
            this.checkBox_ShowIngame.TabIndex = 16;
            this.checkBox_ShowIngame.Text = "Show in osu!";
            this.checkBox_ShowIngame.UseVisualStyleBackColor = true;
            this.checkBox_ShowIngame.CheckedChanged += new System.EventHandler(this.checkBox_ShowIngame_CheckedChanged);
            // 
            // numericUpDown_XPosition
            // 
            this.numericUpDown_XPosition.Location = new System.Drawing.Point(26, 4);
            this.numericUpDown_XPosition.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_XPosition.Name = "numericUpDown_XPosition";
            this.numericUpDown_XPosition.Size = new System.Drawing.Size(41, 20);
            this.numericUpDown_XPosition.TabIndex = 18;
            // 
            // numericUpDown_YPosition
            // 
            this.numericUpDown_YPosition.Location = new System.Drawing.Point(92, 4);
            this.numericUpDown_YPosition.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_YPosition.Name = "numericUpDown_YPosition";
            this.numericUpDown_YPosition.Size = new System.Drawing.Size(41, 20);
            this.numericUpDown_YPosition.TabIndex = 19;
            // 
            // panel_showInOsu
            // 
            this.panel_showInOsu.Controls.Add(this.comboBox_font);
            this.panel_showInOsu.Controls.Add(this.comboBox_align);
            this.panel_showInOsu.Controls.Add(this.label9);
            this.panel_showInOsu.Controls.Add(this.numericUpDown_colorAlpha);
            this.panel_showInOsu.Controls.Add(this.label7);
            this.panel_showInOsu.Controls.Add(this.label6);
            this.panel_showInOsu.Controls.Add(this.label4);
            this.panel_showInOsu.Controls.Add(this.numericUpDown_fontSize);
            this.panel_showInOsu.Controls.Add(this.label3);
            this.panel_showInOsu.Controls.Add(this.label_TestText);
            this.panel_showInOsu.Controls.Add(this.panel_ColorPreview);
            this.panel_showInOsu.Controls.Add(this.numericUpDown_XPosition);
            this.panel_showInOsu.Controls.Add(this.numericUpDown_YPosition);
            this.panel_showInOsu.Controls.Add(this.label8);
            this.panel_showInOsu.Location = new System.Drawing.Point(93, 81);
            this.panel_showInOsu.Name = "panel_showInOsu";
            this.panel_showInOsu.Size = new System.Drawing.Size(545, 54);
            this.panel_showInOsu.TabIndex = 23;
            this.panel_showInOsu.Visible = false;
            // 
            // comboBox_font
            // 
            this.comboBox_font.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_font.FormattingEnabled = true;
            this.comboBox_font.Location = new System.Drawing.Point(178, 28);
            this.comboBox_font.Name = "comboBox_font";
            this.comboBox_font.Size = new System.Drawing.Size(223, 21);
            this.comboBox_font.TabIndex = 24;
            this.comboBox_font.SelectedIndexChanged += new System.EventHandler(this.comboBox_font_SelectedIndexChanged);
            // 
            // comboBox_align
            // 
            this.comboBox_align.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_align.FormattingEnabled = true;
            this.comboBox_align.Location = new System.Drawing.Point(178, 3);
            this.comboBox_align.Name = "comboBox_align";
            this.comboBox_align.Size = new System.Drawing.Size(88, 21);
            this.comboBox_align.TabIndex = 31;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(139, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "Align:";
            // 
            // numericUpDown_colorAlpha
            // 
            this.numericUpDown_colorAlpha.Location = new System.Drawing.Point(92, 28);
            this.numericUpDown_colorAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown_colorAlpha.Name = "numericUpDown_colorAlpha";
            this.numericUpDown_colorAlpha.Size = new System.Drawing.Size(41, 20);
            this.numericUpDown_colorAlpha.TabIndex = 33;
            this.numericUpDown_colorAlpha.ValueChanged += new System.EventHandler(this.numericUpDown_colorAlpha_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(74, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "y:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "x:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(265, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Font size:";
            // 
            // numericUpDown_fontSize
            // 
            this.numericUpDown_fontSize.Location = new System.Drawing.Point(323, 4);
            this.numericUpDown_fontSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_fontSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_fontSize.Name = "numericUpDown_fontSize";
            this.numericUpDown_fontSize.Size = new System.Drawing.Size(78, 20);
            this.numericUpDown_fontSize.TabIndex = 28;
            this.numericUpDown_fontSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(139, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Font:";
            // 
            // label_TestText
            // 
            this.label_TestText.AutoSize = true;
            this.label_TestText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_TestText.Location = new System.Drawing.Point(448, 17);
            this.label_TestText.Name = "label_TestText";
            this.label_TestText.Size = new System.Drawing.Size(57, 17);
            this.label_TestText.TabIndex = 25;
            this.label_TestText.Text = "test 123,4pp";
            // 
            // panel_ColorPreview
            // 
            this.panel_ColorPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panel_ColorPreview.Location = new System.Drawing.Point(10, 28);
            this.panel_ColorPreview.Name = "panel_ColorPreview";
            this.panel_ColorPreview.Size = new System.Drawing.Size(38, 20);
            this.panel_ColorPreview.TabIndex = 24;
            this.panel_ColorPreview.Click += new System.EventHandler(this.panel_ColorPreview_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(53, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Alpha:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(390, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Save event:";
            // 
            // label_statusInfo
            // 
            this.label_statusInfo.AutoSize = true;
            this.label_statusInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_statusInfo.ForeColor = System.Drawing.Color.SaddleBrown;
            this.label_statusInfo.Location = new System.Drawing.Point(3, 138);
            this.label_statusInfo.Name = "label_statusInfo";
            this.label_statusInfo.Size = new System.Drawing.Size(55, 13);
            this.label_statusInfo.TabIndex = 31;
            this.label_statusInfo.Text = "status info";
            this.label_statusInfo.Visible = false;
            // 
            // PatternEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_statusInfo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox_ShowIngame);
            this.Controls.Add(this.label_warning);
            this.Controls.Add(this.comboBox_saveEvent);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.button_addNew);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_preview);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_formating);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.label_commandName);
            this.Controls.Add(this.textBox_FileName);
            this.Controls.Add(this.panel_showInOsu);
            this.Name = "PatternEdit";
            this.Size = new System.Drawing.Size(641, 198);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_XPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_YPosition)).EndInit();
            this.panel_showInOsu.ResumeLayout(false);
            this.panel_showInOsu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_colorAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fontSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_commandName;
        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_formating;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_preview;
        private System.Windows.Forms.Button button_addNew;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.ComboBox comboBox_saveEvent;
        private System.Windows.Forms.Label label_warning;
        private System.Windows.Forms.CheckBox checkBox_ShowIngame;
        private System.Windows.Forms.NumericUpDown numericUpDown_XPosition;
        private System.Windows.Forms.NumericUpDown numericUpDown_YPosition;
        private System.Windows.Forms.Panel panel_showInOsu;
        private System.Windows.Forms.Panel panel_ColorPreview;
        private System.Windows.Forms.ComboBox comboBox_font;
        private System.Windows.Forms.Label label_TestText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown_fontSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown_colorAlpha;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_align;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label_statusInfo;
    }
}
