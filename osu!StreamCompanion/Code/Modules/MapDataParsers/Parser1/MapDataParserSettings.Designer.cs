namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    partial class MapDataParserSettings
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.textBox_Formating = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_EditPattern = new System.Windows.Forms.Button();
            this.button_RemovePattern = new System.Windows.Forms.Button();
            this.textBox_Preview = new System.Windows.Forms.TextBox();
            this.label_Preview = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_saveEvent = new System.Windows.Forms.ComboBox();
            this.button_AddPattern = new System.Windows.Forms.Button();
            this.button_Reset = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowEditingIcon = false;
            this.dataGridView.Size = new System.Drawing.Size(595, 247);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.Location = new System.Drawing.Point(3, 272);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.Size = new System.Drawing.Size(165, 20);
            this.textBox_FileName.TabIndex = 2;
            // 
            // textBox_Formating
            // 
            this.textBox_Formating.Location = new System.Drawing.Point(3, 310);
            this.textBox_Formating.Name = "textBox_Formating";
            this.textBox_Formating.Size = new System.Drawing.Size(496, 20);
            this.textBox_Formating.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 253);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "File/Command name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 294);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Formating:";
            // 
            // button_EditPattern
            // 
            this.button_EditPattern.Location = new System.Drawing.Point(87, 377);
            this.button_EditPattern.Name = "button_EditPattern";
            this.button_EditPattern.Size = new System.Drawing.Size(59, 23);
            this.button_EditPattern.TabIndex = 4;
            this.button_EditPattern.Text = "Save";
            this.button_EditPattern.UseVisualStyleBackColor = true;
            this.button_EditPattern.Click += new System.EventHandler(this.button_EditPattern_Click);
            // 
            // button_RemovePattern
            // 
            this.button_RemovePattern.Location = new System.Drawing.Point(152, 377);
            this.button_RemovePattern.Name = "button_RemovePattern";
            this.button_RemovePattern.Size = new System.Drawing.Size(59, 23);
            this.button_RemovePattern.TabIndex = 4;
            this.button_RemovePattern.Text = "Delete";
            this.button_RemovePattern.UseVisualStyleBackColor = true;
            this.button_RemovePattern.Click += new System.EventHandler(this.button_RemovePattern_Click);
            // 
            // textBox_Preview
            // 
            this.textBox_Preview.Location = new System.Drawing.Point(3, 352);
            this.textBox_Preview.Name = "textBox_Preview";
            this.textBox_Preview.Size = new System.Drawing.Size(496, 20);
            this.textBox_Preview.TabIndex = 2;
            // 
            // label_Preview
            // 
            this.label_Preview.AutoSize = true;
            this.label_Preview.Location = new System.Drawing.Point(3, 336);
            this.label_Preview.Name = "label_Preview";
            this.label_Preview.Size = new System.Drawing.Size(48, 13);
            this.label_Preview.TabIndex = 3;
            this.label_Preview.Text = "Preview:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Saved when:";
            // 
            // comboBox_saveEvent
            // 
            this.comboBox_saveEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_saveEvent.FormattingEnabled = true;
            this.comboBox_saveEvent.Location = new System.Drawing.Point(189, 272);
            this.comboBox_saveEvent.Name = "comboBox_saveEvent";
            this.comboBox_saveEvent.Size = new System.Drawing.Size(81, 21);
            this.comboBox_saveEvent.TabIndex = 6;
            // 
            // button_AddPattern
            // 
            this.button_AddPattern.Location = new System.Drawing.Point(6, 377);
            this.button_AddPattern.Name = "button_AddPattern";
            this.button_AddPattern.Size = new System.Drawing.Size(75, 23);
            this.button_AddPattern.TabIndex = 7;
            this.button_AddPattern.Text = "Add as new";
            this.button_AddPattern.UseVisualStyleBackColor = true;
            this.button_AddPattern.Click += new System.EventHandler(this.button_AddPattern_Click);
            // 
            // button_Reset
            // 
            this.button_Reset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_Reset.Location = new System.Drawing.Point(523, 377);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(75, 23);
            this.button_Reset.TabIndex = 4;
            this.button_Reset.Text = "RESET";
            this.button_Reset.UseVisualStyleBackColor = true;
            this.button_Reset.Click += new System.EventHandler(this.button_Reset_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(406, 377);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Open save directory";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button_OpenDirectory);
            // 
            // MapDataParserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_AddPattern);
            this.Controls.Add(this.comboBox_saveEvent);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_Reset);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_RemovePattern);
            this.Controls.Add(this.button_EditPattern);
            this.Controls.Add(this.label_Preview);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Preview);
            this.Controls.Add(this.textBox_Formating);
            this.Controls.Add(this.textBox_FileName);
            this.Controls.Add(this.dataGridView);
            this.Name = "MapDataParserSettings";
            this.Size = new System.Drawing.Size(601, 409);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.TextBox textBox_Formating;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_EditPattern;
        private System.Windows.Forms.Button button_RemovePattern;
        private System.Windows.Forms.TextBox textBox_Preview;
        private System.Windows.Forms.Label label_Preview;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_saveEvent;
        private System.Windows.Forms.Button button_AddPattern;
        private System.Windows.Forms.Button button_Reset;
        private System.Windows.Forms.Button button1;
    }
}
