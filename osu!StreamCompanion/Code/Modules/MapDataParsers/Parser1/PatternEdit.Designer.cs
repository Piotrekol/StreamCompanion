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
            label_commandName = new System.Windows.Forms.Label();
            textBox_FileName = new System.Windows.Forms.TextBox();
            button_save = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            textBox_formating = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            textBox_preview = new System.Windows.Forms.TextBox();
            button_addNew = new System.Windows.Forms.Button();
            button_delete = new System.Windows.Forms.Button();
            comboBox_saveEvent = new System.Windows.Forms.ComboBox();
            label_warning = new System.Windows.Forms.Label();
            checkBox_ShowIngame = new System.Windows.Forms.CheckBox();
            numericUpDown_XPosition = new System.Windows.Forms.NumericUpDown();
            numericUpDown_YPosition = new System.Windows.Forms.NumericUpDown();
            panel_showInOsu = new System.Windows.Forms.Panel();
            comboBox_font = new System.Windows.Forms.ComboBox();
            comboBox_align = new System.Windows.Forms.ComboBox();
            label9 = new System.Windows.Forms.Label();
            numericUpDown_colorAlpha = new System.Windows.Forms.NumericUpDown();
            label7 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            numericUpDown_fontSize = new System.Windows.Forms.NumericUpDown();
            label3 = new System.Windows.Forms.Label();
            label_TestText = new System.Windows.Forms.Label();
            panel_ColorPreview = new System.Windows.Forms.Panel();
            label8 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label_statusInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_XPosition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_YPosition).BeginInit();
            panel_showInOsu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_colorAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_fontSize).BeginInit();
            SuspendLayout();
            // 
            // label_commandName
            // 
            label_commandName.AutoSize = true;
            label_commandName.Location = new System.Drawing.Point(4, 10);
            label_commandName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_commandName.Name = "label_commandName";
            label_commandName.Size = new System.Drawing.Size(123, 15);
            label_commandName.TabIndex = 5;
            label_commandName.Text = "File/Command name:";
            // 
            // textBox_FileName
            // 
            textBox_FileName.Location = new System.Drawing.Point(135, 7);
            textBox_FileName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox_FileName.Name = "textBox_FileName";
            textBox_FileName.Size = new System.Drawing.Size(233, 23);
            textBox_FileName.TabIndex = 4;
            textBox_FileName.KeyUp += textBox_FileName_KeyUp;
            // 
            // button_save
            // 
            button_save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button_save.Location = new System.Drawing.Point(7, 198);
            button_save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button_save.Name = "button_save";
            button_save.Size = new System.Drawing.Size(88, 27);
            button_save.TabIndex = 6;
            button_save.Text = "Save";
            button_save.UseVisualStyleBackColor = true;
            button_save.Click += button_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(63, 40);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 15);
            label1.TabIndex = 8;
            label1.Text = "Formating:";
            // 
            // textBox_formating
            // 
            textBox_formating.Location = new System.Drawing.Point(135, 37);
            textBox_formating.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox_formating.Name = "textBox_formating";
            textBox_formating.Size = new System.Drawing.Size(607, 23);
            textBox_formating.TabIndex = 7;
            textBox_formating.TextChanged += textBox_formating_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(72, 70);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(51, 15);
            label2.TabIndex = 10;
            label2.Text = "Preview:";
            // 
            // textBox_preview
            // 
            textBox_preview.Location = new System.Drawing.Point(135, 67);
            textBox_preview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox_preview.Name = "textBox_preview";
            textBox_preview.ReadOnly = true;
            textBox_preview.Size = new System.Drawing.Size(607, 23);
            textBox_preview.TabIndex = 9;
            // 
            // button_addNew
            // 
            button_addNew.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button_addNew.Location = new System.Drawing.Point(102, 198);
            button_addNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button_addNew.Name = "button_addNew";
            button_addNew.Size = new System.Drawing.Size(88, 27);
            button_addNew.TabIndex = 11;
            button_addNew.Text = "Add new";
            button_addNew.UseVisualStyleBackColor = true;
            button_addNew.Click += button_Click;
            // 
            // button_delete
            // 
            button_delete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button_delete.Location = new System.Drawing.Point(196, 198);
            button_delete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button_delete.Name = "button_delete";
            button_delete.Size = new System.Drawing.Size(88, 27);
            button_delete.TabIndex = 12;
            button_delete.Text = "Delete";
            button_delete.UseVisualStyleBackColor = true;
            button_delete.Click += button_Click;
            // 
            // comboBox_saveEvent
            // 
            comboBox_saveEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox_saveEvent.FormattingEnabled = true;
            comboBox_saveEvent.Location = new System.Drawing.Point(538, 7);
            comboBox_saveEvent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBox_saveEvent.Name = "comboBox_saveEvent";
            comboBox_saveEvent.Size = new System.Drawing.Size(205, 23);
            comboBox_saveEvent.TabIndex = 14;
            // 
            // label_warning
            // 
            label_warning.AutoSize = true;
            label_warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label_warning.ForeColor = System.Drawing.Color.Crimson;
            label_warning.Location = new System.Drawing.Point(4, 179);
            label_warning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_warning.Name = "label_warning";
            label_warning.Size = new System.Drawing.Size(316, 13);
            label_warning.TabIndex = 15;
            label_warning.Text = "Detected live tokens. Use OBS plugin or ingame overlay to read it";
            label_warning.Visible = false;
            // 
            // checkBox_ShowIngame
            // 
            checkBox_ShowIngame.AutoSize = true;
            checkBox_ShowIngame.Location = new System.Drawing.Point(5, 96);
            checkBox_ShowIngame.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox_ShowIngame.Name = "checkBox_ShowIngame";
            checkBox_ShowIngame.Size = new System.Drawing.Size(93, 19);
            checkBox_ShowIngame.TabIndex = 16;
            checkBox_ShowIngame.Text = "Show in osu!";
            checkBox_ShowIngame.UseVisualStyleBackColor = true;
            checkBox_ShowIngame.CheckedChanged += checkBox_ShowIngame_CheckedChanged;
            // 
            // numericUpDown_XPosition
            // 
            numericUpDown_XPosition.Location = new System.Drawing.Point(30, 5);
            numericUpDown_XPosition.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDown_XPosition.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericUpDown_XPosition.Name = "numericUpDown_XPosition";
            numericUpDown_XPosition.Size = new System.Drawing.Size(48, 23);
            numericUpDown_XPosition.TabIndex = 18;
            // 
            // numericUpDown_YPosition
            // 
            numericUpDown_YPosition.Location = new System.Drawing.Point(107, 5);
            numericUpDown_YPosition.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDown_YPosition.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericUpDown_YPosition.Name = "numericUpDown_YPosition";
            numericUpDown_YPosition.Size = new System.Drawing.Size(48, 23);
            numericUpDown_YPosition.TabIndex = 19;
            // 
            // panel_showInOsu
            // 
            panel_showInOsu.Controls.Add(comboBox_font);
            panel_showInOsu.Controls.Add(comboBox_align);
            panel_showInOsu.Controls.Add(label9);
            panel_showInOsu.Controls.Add(numericUpDown_colorAlpha);
            panel_showInOsu.Controls.Add(label7);
            panel_showInOsu.Controls.Add(label6);
            panel_showInOsu.Controls.Add(label4);
            panel_showInOsu.Controls.Add(numericUpDown_fontSize);
            panel_showInOsu.Controls.Add(label3);
            panel_showInOsu.Controls.Add(label_TestText);
            panel_showInOsu.Controls.Add(panel_ColorPreview);
            panel_showInOsu.Controls.Add(numericUpDown_XPosition);
            panel_showInOsu.Controls.Add(numericUpDown_YPosition);
            panel_showInOsu.Controls.Add(label8);
            panel_showInOsu.Location = new System.Drawing.Point(108, 93);
            panel_showInOsu.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel_showInOsu.Name = "panel_showInOsu";
            panel_showInOsu.Size = new System.Drawing.Size(636, 62);
            panel_showInOsu.TabIndex = 23;
            panel_showInOsu.Visible = false;
            // 
            // comboBox_font
            // 
            comboBox_font.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox_font.FormattingEnabled = true;
            comboBox_font.Location = new System.Drawing.Point(208, 32);
            comboBox_font.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBox_font.Name = "comboBox_font";
            comboBox_font.Size = new System.Drawing.Size(259, 23);
            comboBox_font.TabIndex = 24;
            comboBox_font.SelectedIndexChanged += comboBox_font_SelectedIndexChanged;
            // 
            // comboBox_align
            // 
            comboBox_align.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox_align.FormattingEnabled = true;
            comboBox_align.Location = new System.Drawing.Point(208, 3);
            comboBox_align.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBox_align.Name = "comboBox_align";
            comboBox_align.Size = new System.Drawing.Size(102, 23);
            comboBox_align.TabIndex = 31;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(162, 7);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(38, 15);
            label9.TabIndex = 32;
            label9.Text = "Align:";
            // 
            // numericUpDown_colorAlpha
            // 
            numericUpDown_colorAlpha.Location = new System.Drawing.Point(107, 32);
            numericUpDown_colorAlpha.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDown_colorAlpha.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numericUpDown_colorAlpha.Name = "numericUpDown_colorAlpha";
            numericUpDown_colorAlpha.Size = new System.Drawing.Size(48, 23);
            numericUpDown_colorAlpha.TabIndex = 33;
            numericUpDown_colorAlpha.ValueChanged += numericUpDown_colorAlpha_ValueChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(86, 9);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(16, 15);
            label7.TabIndex = 32;
            label7.Text = "y:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(8, 9);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(16, 15);
            label6.TabIndex = 31;
            label6.Text = "x:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(309, 7);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(56, 15);
            label4.TabIndex = 29;
            label4.Text = "Font size:";
            // 
            // numericUpDown_fontSize
            // 
            numericUpDown_fontSize.Location = new System.Drawing.Point(377, 5);
            numericUpDown_fontSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDown_fontSize.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDown_fontSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown_fontSize.Name = "numericUpDown_fontSize";
            numericUpDown_fontSize.Size = new System.Drawing.Size(91, 23);
            numericUpDown_fontSize.TabIndex = 28;
            numericUpDown_fontSize.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(162, 38);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(34, 15);
            label3.TabIndex = 27;
            label3.Text = "Font:";
            // 
            // label_TestText
            // 
            label_TestText.AutoSize = true;
            label_TestText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label_TestText.Location = new System.Drawing.Point(523, 20);
            label_TestText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_TestText.Name = "label_TestText";
            label_TestText.Size = new System.Drawing.Size(87, 17);
            label_TestText.TabIndex = 25;
            label_TestText.Text = "test 123,4pp";
            // 
            // panel_ColorPreview
            // 
            panel_ColorPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            panel_ColorPreview.Location = new System.Drawing.Point(12, 32);
            panel_ColorPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel_ColorPreview.Name = "panel_ColorPreview";
            panel_ColorPreview.Size = new System.Drawing.Size(44, 23);
            panel_ColorPreview.TabIndex = 24;
            panel_ColorPreview.Click += panel_ColorPreview_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(62, 37);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(41, 15);
            label8.TabIndex = 34;
            label8.Text = "Alpha:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(455, 10);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(66, 15);
            label5.TabIndex = 30;
            label5.Text = "Save event:";
            // 
            // label_statusInfo
            // 
            label_statusInfo.AutoSize = true;
            label_statusInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label_statusInfo.ForeColor = System.Drawing.Color.SaddleBrown;
            label_statusInfo.Location = new System.Drawing.Point(4, 159);
            label_statusInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_statusInfo.Name = "label_statusInfo";
            label_statusInfo.Size = new System.Drawing.Size(55, 13);
            label_statusInfo.TabIndex = 31;
            label_statusInfo.Text = "status info";
            label_statusInfo.Visible = false;
            // 
            // PatternEdit
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(label_statusInfo);
            Controls.Add(label5);
            Controls.Add(checkBox_ShowIngame);
            Controls.Add(label_warning);
            Controls.Add(comboBox_saveEvent);
            Controls.Add(button_delete);
            Controls.Add(button_addNew);
            Controls.Add(label2);
            Controls.Add(textBox_preview);
            Controls.Add(label1);
            Controls.Add(textBox_formating);
            Controls.Add(button_save);
            Controls.Add(label_commandName);
            Controls.Add(textBox_FileName);
            Controls.Add(panel_showInOsu);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "PatternEdit";
            Size = new System.Drawing.Size(748, 228);
            ((System.ComponentModel.ISupportInitialize)numericUpDown_XPosition).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_YPosition).EndInit();
            panel_showInOsu.ResumeLayout(false);
            panel_showInOsu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_colorAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_fontSize).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
