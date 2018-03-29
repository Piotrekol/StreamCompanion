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
            this.patternList = new osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1.PatternList();
            this.patternEdit = new osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1.PatternEdit();
            this.checkBox_disableDiskSaving = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // patternList
            // 
            this.patternList.Location = new System.Drawing.Point(0, 0);
            this.patternList.Name = "patternList";
            this.patternList.Size = new System.Drawing.Size(601, 253);
            this.patternList.TabIndex = 0;
            // 
            // patternEdit
            // 
            this.patternEdit.Current = null;
            this.patternEdit.Location = new System.Drawing.Point(0, 257);
            this.patternEdit.Name = "patternEdit";
            this.patternEdit.Size = new System.Drawing.Size(601, 125);
            this.patternEdit.TabIndex = 1;
            // 
            // checkBox_disableDiskSaving
            // 
            this.checkBox_disableDiskSaving.AutoSize = true;
            this.checkBox_disableDiskSaving.Location = new System.Drawing.Point(247, 359);
            this.checkBox_disableDiskSaving.Name = "checkBox_disableDiskSaving";
            this.checkBox_disableDiskSaving.Size = new System.Drawing.Size(153, 17);
            this.checkBox_disableDiskSaving.TabIndex = 2;
            this.checkBox_disableDiskSaving.Text = "Disable saving files on disk";
            this.checkBox_disableDiskSaving.UseVisualStyleBackColor = true;
            // 
            // ParserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_disableDiskSaving);
            this.Controls.Add(this.patternEdit);
            this.Controls.Add(this.patternList);
            this.Name = "ParserSettings";
            this.Size = new System.Drawing.Size(601, 383);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PatternList patternList;
        private PatternEdit patternEdit;
        private System.Windows.Forms.CheckBox checkBox_disableDiskSaving;
    }
}
