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
            this.button_save.Location = new System.Drawing.Point(6, 101);
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
            this.textBox_formating.Size = new System.Drawing.Size(466, 20);
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
            this.textBox_preview.Size = new System.Drawing.Size(466, 20);
            this.textBox_preview.TabIndex = 9;
            // 
            // button_addNew
            // 
            this.button_addNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_addNew.Location = new System.Drawing.Point(87, 101);
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
            this.button_delete.Location = new System.Drawing.Point(168, 101);
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
            this.comboBox_saveEvent.Location = new System.Drawing.Point(322, 6);
            this.comboBox_saveEvent.Name = "comboBox_saveEvent";
            this.comboBox_saveEvent.Size = new System.Drawing.Size(121, 21);
            this.comboBox_saveEvent.TabIndex = 14;
            // 
            // label_warning
            // 
            this.label_warning.AutoSize = true;
            this.label_warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_warning.ForeColor = System.Drawing.Color.Crimson;
            this.label_warning.Location = new System.Drawing.Point(3, 81);
            this.label_warning.Name = "label_warning";
            this.label_warning.Size = new System.Drawing.Size(300, 13);
            this.label_warning.TabIndex = 15;
            this.label_warning.Text = "Detected live tokens. This pattern will save only when playing!";
            this.label_warning.Visible = false;
            // 
            // PatternEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Name = "PatternEdit";
            this.Size = new System.Drawing.Size(585, 127);
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
    }
}
