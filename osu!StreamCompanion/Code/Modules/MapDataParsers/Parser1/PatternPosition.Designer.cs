namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    partial class PatternPosition
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
            this.panel_OsuScreen = new System.Windows.Forms.Panel();
            this.label_text = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ui_osuWidth = new System.Windows.Forms.NumericUpDown();
            this.ui_osuHeight = new System.Windows.Forms.NumericUpDown();
            this.ui_textY = new System.Windows.Forms.NumericUpDown();
            this.ui_textX = new System.Windows.Forms.NumericUpDown();
            this.panel_OsuScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ui_osuWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui_osuHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui_textY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui_textX)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_OsuScreen
            // 
            this.panel_OsuScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_OsuScreen.Controls.Add(this.label_text);
            this.panel_OsuScreen.Location = new System.Drawing.Point(25, 14);
            this.panel_OsuScreen.Name = "panel_OsuScreen";
            this.panel_OsuScreen.Size = new System.Drawing.Size(320, 180);
            this.panel_OsuScreen.TabIndex = 0;
            this.panel_OsuScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_OsuScreen_MouseMove);
            // 
            // label_text
            // 
            this.label_text.AutoSize = true;
            this.label_text.CausesValidation = false;
            this.label_text.Location = new System.Drawing.Point(102, 84);
            this.label_text.Name = "label_text";
            this.label_text.Size = new System.Drawing.Size(98, 13);
            this.label_text.TabIndex = 0;
            this.label_text.Text = "Drag me to position";
            this.label_text.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_text_MouseMove);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "osu! width:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "osu! height:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 242);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "text X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(128, 242);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "text Y:";
            // 
            // ui_osuWidth
            // 
            this.ui_osuWidth.Location = new System.Drawing.Point(25, 213);
            this.ui_osuWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.ui_osuWidth.Name = "ui_osuWidth";
            this.ui_osuWidth.Size = new System.Drawing.Size(88, 20);
            this.ui_osuWidth.TabIndex = 9;
            this.ui_osuWidth.ValueChanged += new System.EventHandler(this.OsuRectChanged);
            // 
            // ui_osuHeight
            // 
            this.ui_osuHeight.Location = new System.Drawing.Point(131, 213);
            this.ui_osuHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.ui_osuHeight.Name = "ui_osuHeight";
            this.ui_osuHeight.Size = new System.Drawing.Size(88, 20);
            this.ui_osuHeight.TabIndex = 10;
            // 
            // ui_textY
            // 
            this.ui_textY.Location = new System.Drawing.Point(131, 258);
            this.ui_textY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.ui_textY.Name = "ui_textY";
            this.ui_textY.Size = new System.Drawing.Size(88, 20);
            this.ui_textY.TabIndex = 12;
            this.ui_textY.ValueChanged += new System.EventHandler(this.TextPositionValueChanged);
            // 
            // ui_textX
            // 
            this.ui_textX.Location = new System.Drawing.Point(25, 258);
            this.ui_textX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.ui_textX.Name = "ui_textX";
            this.ui_textX.Size = new System.Drawing.Size(88, 20);
            this.ui_textX.TabIndex = 11;
            this.ui_textX.ValueChanged += new System.EventHandler(this.TextPositionValueChanged);
            // 
            // PatternPosition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ui_textY);
            this.Controls.Add(this.ui_textX);
            this.Controls.Add(this.ui_osuHeight);
            this.Controls.Add(this.ui_osuWidth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel_OsuScreen);
            this.Name = "PatternPosition";
            this.Size = new System.Drawing.Size(370, 367);
            this.panel_OsuScreen.ResumeLayout(false);
            this.panel_OsuScreen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ui_osuWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui_osuHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui_textY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui_textX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel_OsuScreen;
        private System.Windows.Forms.Label label_text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown ui_osuWidth;
        private System.Windows.Forms.NumericUpDown ui_osuHeight;
        private System.Windows.Forms.NumericUpDown ui_textY;
        private System.Windows.Forms.NumericUpDown ui_textX;
    }
}
