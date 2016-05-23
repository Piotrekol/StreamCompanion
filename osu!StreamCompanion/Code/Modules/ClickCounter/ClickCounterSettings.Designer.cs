namespace osu_StreamCompanion.Code.Modules.ClickCounter
{
    partial class ClickCounterSettings
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_AddKey = new System.Windows.Forms.Button();
            this.button_RemoveKey = new System.Windows.Forms.Button();
            this.checkBox_ResetOnRestart = new System.Windows.Forms.CheckBox();
            this.checkBox_EnableKPX = new System.Windows.Forms.CheckBox();
            this.checkBox_enableMouseHook = new System.Windows.Forms.CheckBox();
            this.groupBox_Mouse = new System.Windows.Forms.GroupBox();
            this.label_MouseLeft = new System.Windows.Forms.Label();
            this.label_MouseRight = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox_Mouse.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Key,
            this.Count,
            this.FileName});
            this.dataGridView1.Location = new System.Drawing.Point(4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(405, 230);
            this.dataGridView1.TabIndex = 0;
            // 
            // Key
            // 
            this.Key.HeaderText = "Key";
            this.Key.Name = "Key";
            this.Key.ReadOnly = true;
            // 
            // Count
            // 
            this.Count.HeaderText = "Count";
            this.Count.Name = "Count";
            this.Count.ReadOnly = true;
            // 
            // FileName
            // 
            this.FileName.HeaderText = "File name";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            // 
            // button_AddKey
            // 
            this.button_AddKey.Location = new System.Drawing.Point(415, 5);
            this.button_AddKey.Name = "button_AddKey";
            this.button_AddKey.Size = new System.Drawing.Size(75, 23);
            this.button_AddKey.TabIndex = 1;
            this.button_AddKey.Text = "Add key";
            this.button_AddKey.UseVisualStyleBackColor = true;
            this.button_AddKey.Click += new System.EventHandler(this.button_AddKey_Click);
            // 
            // button_RemoveKey
            // 
            this.button_RemoveKey.Location = new System.Drawing.Point(415, 34);
            this.button_RemoveKey.Name = "button_RemoveKey";
            this.button_RemoveKey.Size = new System.Drawing.Size(75, 23);
            this.button_RemoveKey.TabIndex = 2;
            this.button_RemoveKey.Text = "Remove key";
            this.button_RemoveKey.UseVisualStyleBackColor = true;
            this.button_RemoveKey.Click += new System.EventHandler(this.button_RemoveKey_Click);
            // 
            // checkBox_ResetOnRestart
            // 
            this.checkBox_ResetOnRestart.AutoSize = true;
            this.checkBox_ResetOnRestart.Location = new System.Drawing.Point(4, 260);
            this.checkBox_ResetOnRestart.Name = "checkBox_ResetOnRestart";
            this.checkBox_ResetOnRestart.Size = new System.Drawing.Size(156, 17);
            this.checkBox_ResetOnRestart.TabIndex = 3;
            this.checkBox_ResetOnRestart.Text = "Reset key counts on restart";
            this.checkBox_ResetOnRestart.UseVisualStyleBackColor = true;
            // 
            // checkBox_EnableKPX
            // 
            this.checkBox_EnableKPX.AutoSize = true;
            this.checkBox_EnableKPX.Enabled = false;
            this.checkBox_EnableKPX.Location = new System.Drawing.Point(4, 280);
            this.checkBox_EnableKPX.Name = "checkBox_EnableKPX";
            this.checkBox_EnableKPX.Size = new System.Drawing.Size(166, 17);
            this.checkBox_EnableKPX.TabIndex = 4;
            this.checkBox_EnableKPX.Text = "Enable averange key counter";
            this.checkBox_EnableKPX.UseVisualStyleBackColor = true;
            // 
            // checkBox_enableMouseHook
            // 
            this.checkBox_enableMouseHook.AutoSize = true;
            this.checkBox_enableMouseHook.Location = new System.Drawing.Point(4, 240);
            this.checkBox_enableMouseHook.Name = "checkBox_enableMouseHook";
            this.checkBox_enableMouseHook.Size = new System.Drawing.Size(219, 17);
            this.checkBox_enableMouseHook.TabIndex = 5;
            this.checkBox_enableMouseHook.Text = "Enable tracking of Mouse button presses";
            this.checkBox_enableMouseHook.UseVisualStyleBackColor = true;
            // 
            // groupBox_Mouse
            // 
            this.groupBox_Mouse.Controls.Add(this.label_MouseLeft);
            this.groupBox_Mouse.Controls.Add(this.label_MouseRight);
            this.groupBox_Mouse.Controls.Add(this.label2);
            this.groupBox_Mouse.Controls.Add(this.label1);
            this.groupBox_Mouse.Location = new System.Drawing.Point(230, 241);
            this.groupBox_Mouse.Name = "groupBox_Mouse";
            this.groupBox_Mouse.Size = new System.Drawing.Size(179, 70);
            this.groupBox_Mouse.TabIndex = 6;
            this.groupBox_Mouse.TabStop = false;
            this.groupBox_Mouse.Text = "Mouse counter";
            // 
            // label_MouseLeft
            // 
            this.label_MouseLeft.AutoSize = true;
            this.label_MouseLeft.Location = new System.Drawing.Point(41, 40);
            this.label_MouseLeft.Name = "label_MouseLeft";
            this.label_MouseLeft.Size = new System.Drawing.Size(13, 13);
            this.label_MouseLeft.TabIndex = 3;
            this.label_MouseLeft.Text = "0";
            // 
            // label_MouseRight
            // 
            this.label_MouseRight.AutoSize = true;
            this.label_MouseRight.Location = new System.Drawing.Point(41, 19);
            this.label_MouseRight.Name = "label_MouseRight";
            this.label_MouseRight.Size = new System.Drawing.Size(13, 13);
            this.label_MouseRight.TabIndex = 2;
            this.label_MouseRight.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Left:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Right:";
            // 
            // ClickCounterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_Mouse);
            this.Controls.Add(this.checkBox_enableMouseHook);
            this.Controls.Add(this.checkBox_EnableKPX);
            this.Controls.Add(this.checkBox_ResetOnRestart);
            this.Controls.Add(this.button_RemoveKey);
            this.Controls.Add(this.button_AddKey);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ClickCounterSettings";
            this.Size = new System.Drawing.Size(493, 324);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox_Mouse.ResumeLayout(false);
            this.groupBox_Mouse.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_AddKey;
        private System.Windows.Forms.Button button_RemoveKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn Count;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        public System.Windows.Forms.CheckBox checkBox_ResetOnRestart;
        public System.Windows.Forms.CheckBox checkBox_EnableKPX;
        public System.Windows.Forms.CheckBox checkBox_enableMouseHook;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox_Mouse;
        private System.Windows.Forms.Label label_MouseLeft;
        private System.Windows.Forms.Label label_MouseRight;
    }
}
