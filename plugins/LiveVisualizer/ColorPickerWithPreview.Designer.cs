namespace LiveVisualizer
{
    partial class ColorPickerWithPreview
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
            this.panel_colorPreview = new System.Windows.Forms.Panel();
            this.Label = new System.Windows.Forms.Label();
            this.button_change = new System.Windows.Forms.Button();
            this.numericUpDown_alpha = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_alpha)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_colorPreview
            // 
            this.panel_colorPreview.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel_colorPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panel_colorPreview.Location = new System.Drawing.Point(84, 0);
            this.panel_colorPreview.Name = "panel_colorPreview";
            this.panel_colorPreview.Size = new System.Drawing.Size(25, 25);
            this.panel_colorPreview.TabIndex = 0;
            this.panel_colorPreview.Click += new System.EventHandler(this.panel_ColorPreview_Click);
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(3, 6);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(64, 13);
            this.Label.TabIndex = 1;
            this.Label.Text = "Default_text";
            // 
            // button_change
            // 
            this.button_change.Location = new System.Drawing.Point(3, 1);
            this.button_change.Name = "button_change";
            this.button_change.Size = new System.Drawing.Size(75, 23);
            this.button_change.TabIndex = 2;
            this.button_change.Text = "Change";
            this.button_change.UseVisualStyleBackColor = true;
            this.button_change.Click += new System.EventHandler(this.button_Click);
            // 
            // numericUpDown_alpha
            // 
            this.numericUpDown_alpha.Location = new System.Drawing.Point(154, 3);
            this.numericUpDown_alpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown_alpha.Name = "numericUpDown_alpha";
            this.numericUpDown_alpha.Size = new System.Drawing.Size(53, 20);
            this.numericUpDown_alpha.TabIndex = 3;
            this.numericUpDown_alpha.ValueChanged += new System.EventHandler(this.numericUpDown_alpha_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(113, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Alpha:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDown_alpha);
            this.panel1.Controls.Add(this.panel_colorPreview);
            this.panel1.Controls.Add(this.button_change);
            this.panel1.Location = new System.Drawing.Point(297, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(210, 26);
            this.panel1.TabIndex = 5;
            // 
            // ColorPickerWithPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Label);
            this.Controls.Add(this.panel1);
            this.Name = "ColorPickerWithPreview";
            this.Size = new System.Drawing.Size(507, 26);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_alpha)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel_colorPreview;
        private System.Windows.Forms.Button button_change;
        public System.Windows.Forms.Label Label;
        private System.Windows.Forms.NumericUpDown numericUpDown_alpha;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}
