namespace osu_StreamCompanion.Code.Modules.TokensPreview
{
    partial class TokensPreviewSettings
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
            this.p = new System.Windows.Forms.Panel();
            this.label_ListedNum = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.p.SuspendLayout();
            this.SuspendLayout();
            // 
            // p
            // 
            this.p.AutoScroll = true;
            this.p.Controls.Add(this.label2);
            this.p.Controls.Add(this.label_ListedNum);
            this.p.Controls.Add(this.label1);
            this.p.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p.Location = new System.Drawing.Point(0, 0);
            this.p.Name = "p";
            this.p.Size = new System.Drawing.Size(612, 399);
            this.p.TabIndex = 0;
            // 
            // label_ListedNum
            // 
            this.label_ListedNum.AutoSize = true;
            this.label_ListedNum.Location = new System.Drawing.Point(48, 4);
            this.label_ListedNum.Name = "label_ListedNum";
            this.label_ListedNum.Size = new System.Drawing.Size(13, 13);
            this.label_ListedNum.TabIndex = 1;
            this.label_ListedNum.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Listed:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(98, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Change map in osu to populate this list";
            // 
            // CommandsPreviewSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.p);
            this.Name = "TokensPreviewSettings";
            this.Size = new System.Drawing.Size(612, 399);
            this.p.ResumeLayout(false);
            this.p.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel p;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_ListedNum;
        private System.Windows.Forms.Label label2;
    }
}
