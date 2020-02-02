namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    partial class ParserSettings
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
            this.checkBox_disableDiskSaving = new System.Windows.Forms.CheckBox();
            this.patternEdit = new osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1.PatternEdit();
            this.patternList = new osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1.PatternList();
            this.button_reset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox_disableDiskSaving
            // 
            this.checkBox_disableDiskSaving.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_disableDiskSaving.AutoSize = true;
            this.checkBox_disableDiskSaving.Location = new System.Drawing.Point(246, 413);
            this.checkBox_disableDiskSaving.Name = "checkBox_disableDiskSaving";
            this.checkBox_disableDiskSaving.Size = new System.Drawing.Size(153, 17);
            this.checkBox_disableDiskSaving.TabIndex = 2;
            this.checkBox_disableDiskSaving.Text = "Disable saving files on disk";
            this.checkBox_disableDiskSaving.UseVisualStyleBackColor = true;
            // 
            // patternEdit
            // 
            this.patternEdit.Current = null;
            this.patternEdit.InGameOverlayIsAvailable = false;
            this.patternEdit.Location = new System.Drawing.Point(0, 223);
            this.patternEdit.Name = "patternEdit";
            this.patternEdit.Size = new System.Drawing.Size(601, 213);
            this.patternEdit.TabIndex = 1;
            // 
            // patternList
            // 
            this.patternList.Location = new System.Drawing.Point(0, 0);
            this.patternList.Name = "patternList";
            this.patternList.Size = new System.Drawing.Size(640, 226);
            this.patternList.TabIndex = 0;
            // 
            // button_reset
            // 
            this.button_reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_reset.Location = new System.Drawing.Point(534, 407);
            this.button_reset.Name = "button_reset";
            this.button_reset.Size = new System.Drawing.Size(93, 23);
            this.button_reset.TabIndex = 3;
            this.button_reset.Text = "Reset patterns";
            this.button_reset.UseVisualStyleBackColor = true;
            this.button_reset.Click += new System.EventHandler(this.button_reset_Click);
            // 
            // ParserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_reset);
            this.Controls.Add(this.patternList);
            this.Controls.Add(this.checkBox_disableDiskSaving);
            this.Controls.Add(this.patternEdit);
            this.Name = "ParserSettings";
            this.Size = new System.Drawing.Size(640, 436);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PatternList patternList;
        private PatternEdit patternEdit;
        private System.Windows.Forms.CheckBox checkBox_disableDiskSaving;
        private System.Windows.Forms.Button button_reset;
    }
}
