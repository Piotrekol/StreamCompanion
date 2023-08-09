namespace ClickCounter
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
            dataGridView1 = new System.Windows.Forms.DataGridView();
            button_AddKey = new System.Windows.Forms.Button();
            button_RemoveKey = new System.Windows.Forms.Button();
            checkBox_ResetOnRestart = new System.Windows.Forms.CheckBox();
            checkBox_EnableKPX = new System.Windows.Forms.CheckBox();
            checkBox_enableMouseHook = new System.Windows.Forms.CheckBox();
            checkBox_resetOnPlay = new System.Windows.Forms.CheckBox();
            checkBox_enableKeyboardHook = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new System.Drawing.Point(5, 5);
            dataGridView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.Size = new System.Drawing.Size(472, 265);
            dataGridView1.TabIndex = 0;
            // 
            // button_AddKey
            // 
            button_AddKey.Location = new System.Drawing.Point(484, 6);
            button_AddKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button_AddKey.Name = "button_AddKey";
            button_AddKey.Size = new System.Drawing.Size(88, 27);
            button_AddKey.TabIndex = 1;
            button_AddKey.Text = "Add key";
            button_AddKey.UseVisualStyleBackColor = true;
            button_AddKey.Click += button_AddKey_Click;
            // 
            // button_RemoveKey
            // 
            button_RemoveKey.Location = new System.Drawing.Point(484, 39);
            button_RemoveKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button_RemoveKey.Name = "button_RemoveKey";
            button_RemoveKey.Size = new System.Drawing.Size(88, 27);
            button_RemoveKey.TabIndex = 2;
            button_RemoveKey.Text = "Remove key";
            button_RemoveKey.UseVisualStyleBackColor = true;
            button_RemoveKey.Click += button_RemoveKey_Click;
            // 
            // checkBox_ResetOnRestart
            // 
            checkBox_ResetOnRestart.AutoSize = true;
            checkBox_ResetOnRestart.Location = new System.Drawing.Point(5, 321);
            checkBox_ResetOnRestart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox_ResetOnRestart.Name = "checkBox_ResetOnRestart";
            checkBox_ResetOnRestart.Size = new System.Drawing.Size(167, 19);
            checkBox_ResetOnRestart.TabIndex = 3;
            checkBox_ResetOnRestart.Text = "Reset key counts on restart";
            checkBox_ResetOnRestart.UseVisualStyleBackColor = true;
            // 
            // checkBox_EnableKPX
            // 
            checkBox_EnableKPX.AutoSize = true;
            checkBox_EnableKPX.Enabled = false;
            checkBox_EnableKPX.Location = new System.Drawing.Point(5, 365);
            checkBox_EnableKPX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox_EnableKPX.Name = "checkBox_EnableKPX";
            checkBox_EnableKPX.Size = new System.Drawing.Size(177, 19);
            checkBox_EnableKPX.TabIndex = 4;
            checkBox_EnableKPX.Text = "Enable averange key counter";
            checkBox_EnableKPX.UseVisualStyleBackColor = true;
            // 
            // checkBox_enableMouseHook
            // 
            checkBox_enableMouseHook.AutoSize = true;
            checkBox_enableMouseHook.Location = new System.Drawing.Point(5, 277);
            checkBox_enableMouseHook.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox_enableMouseHook.Name = "checkBox_enableMouseHook";
            checkBox_enableMouseHook.Size = new System.Drawing.Size(201, 19);
            checkBox_enableMouseHook.TabIndex = 5;
            checkBox_enableMouseHook.Text = "Enable tracking of mouse presses";
            checkBox_enableMouseHook.UseVisualStyleBackColor = true;
            // 
            // checkBox_resetOnPlay
            // 
            checkBox_resetOnPlay.AutoSize = true;
            checkBox_resetOnPlay.Location = new System.Drawing.Point(5, 343);
            checkBox_resetOnPlay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox_resetOnPlay.Name = "checkBox_resetOnPlay";
            checkBox_resetOnPlay.Size = new System.Drawing.Size(184, 19);
            checkBox_resetOnPlay.TabIndex = 8;
            checkBox_resetOnPlay.Text = "Reset key counts on each play";
            checkBox_resetOnPlay.UseVisualStyleBackColor = true;
            // 
            // checkBox_enableKeyboardHook
            // 
            checkBox_enableKeyboardHook.AutoSize = true;
            checkBox_enableKeyboardHook.Location = new System.Drawing.Point(5, 299);
            checkBox_enableKeyboardHook.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox_enableKeyboardHook.Name = "checkBox_enableKeyboardHook";
            checkBox_enableKeyboardHook.Size = new System.Drawing.Size(214, 19);
            checkBox_enableKeyboardHook.TabIndex = 9;
            checkBox_enableKeyboardHook.Text = "Enable tracking of keyboard presses";
            checkBox_enableKeyboardHook.UseVisualStyleBackColor = true;
            // 
            // ClickCounterSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(checkBox_enableKeyboardHook);
            Controls.Add(checkBox_resetOnPlay);
            Controls.Add(checkBox_enableMouseHook);
            Controls.Add(checkBox_EnableKPX);
            Controls.Add(checkBox_ResetOnRestart);
            Controls.Add(button_RemoveKey);
            Controls.Add(button_AddKey);
            Controls.Add(dataGridView1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ClickCounterSettings";
            Size = new System.Drawing.Size(575, 410);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_AddKey;
        private System.Windows.Forms.Button button_RemoveKey;
        public System.Windows.Forms.CheckBox checkBox_ResetOnRestart;
        public System.Windows.Forms.CheckBox checkBox_EnableKPX;
        public System.Windows.Forms.CheckBox checkBox_enableMouseHook;
        public System.Windows.Forms.CheckBox checkBox_resetOnPlay;
        public System.Windows.Forms.CheckBox checkBox_enableKeyboardHook;
    }
}
