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
            this.patternList = new PatternList();
            this.patternEdit = new PatternEdit();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
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
            this.patternEdit.Location = new System.Drawing.Point(0, 259);
            this.patternEdit.Name = "patternEdit";
            this.patternEdit.Size = new System.Drawing.Size(601, 107);
            this.patternEdit.TabIndex = 1;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(247, 343);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(153, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Disable saving files on disk";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Parser2Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.patternEdit);
            this.Controls.Add(this.patternList);
            this.Name = "ParserSettings";
            this.Size = new System.Drawing.Size(601, 409);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PatternList patternList;
        private PatternEdit patternEdit;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
