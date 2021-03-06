
namespace BrowserOverlay
{
    partial class BrowserOverlaySettings
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
            this.checkBox_enable = new System.Windows.Forms.CheckBox();
            this.textBox_overlayUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_CanvasWidth = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_CanvasHeight = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_positionY = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_positionX = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_scale = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CanvasWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CanvasHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_positionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_positionX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_scale)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox_enable
            // 
            this.checkBox_enable.AutoSize = true;
            this.checkBox_enable.Location = new System.Drawing.Point(17, 12);
            this.checkBox_enable.Name = "checkBox_enable";
            this.checkBox_enable.Size = new System.Drawing.Size(124, 19);
            this.checkBox_enable.TabIndex = 0;
            this.checkBox_enable.Text = "Enable osu overlay";
            this.checkBox_enable.UseVisualStyleBackColor = true;
            // 
            // textBox_overlayUrl
            // 
            this.textBox_overlayUrl.Location = new System.Drawing.Point(97, 47);
            this.textBox_overlayUrl.Name = "textBox_overlayUrl";
            this.textBox_overlayUrl.Size = new System.Drawing.Size(280, 23);
            this.textBox_overlayUrl.TabIndex = 1;
            this.textBox_overlayUrl.TextChanged += new System.EventHandler(this.textBox_overlayUrl_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Overlay URL:";
            // 
            // numericUpDown_CanvasWidth
            // 
            this.numericUpDown_CanvasWidth.Location = new System.Drawing.Point(97, 79);
            this.numericUpDown_CanvasWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_CanvasWidth.Name = "numericUpDown_CanvasWidth";
            this.numericUpDown_CanvasWidth.Size = new System.Drawing.Size(79, 23);
            this.numericUpDown_CanvasWidth.TabIndex = 5;
            this.numericUpDown_CanvasWidth.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Canvas size:";
            // 
            // numericUpDown_CanvasHeight
            // 
            this.numericUpDown_CanvasHeight.Location = new System.Drawing.Point(182, 79);
            this.numericUpDown_CanvasHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_CanvasHeight.Name = "numericUpDown_CanvasHeight";
            this.numericUpDown_CanvasHeight.Size = new System.Drawing.Size(79, 23);
            this.numericUpDown_CanvasHeight.TabIndex = 7;
            this.numericUpDown_CanvasHeight.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDown_positionY
            // 
            this.numericUpDown_positionY.Location = new System.Drawing.Point(182, 111);
            this.numericUpDown_positionY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_positionY.Name = "numericUpDown_positionY";
            this.numericUpDown_positionY.Size = new System.Drawing.Size(79, 23);
            this.numericUpDown_positionY.TabIndex = 10;
            this.numericUpDown_positionY.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Position:";
            // 
            // numericUpDown_positionX
            // 
            this.numericUpDown_positionX.Location = new System.Drawing.Point(97, 111);
            this.numericUpDown_positionX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_positionX.Name = "numericUpDown_positionX";
            this.numericUpDown_positionX.Size = new System.Drawing.Size(79, 23);
            this.numericUpDown_positionX.TabIndex = 8;
            this.numericUpDown_positionX.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Scale:";
            // 
            // numericUpDown_scale
            // 
            this.numericUpDown_scale.DecimalPlaces = 3;
            this.numericUpDown_scale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numericUpDown_scale.Location = new System.Drawing.Point(97, 140);
            this.numericUpDown_scale.Name = "numericUpDown_scale";
            this.numericUpDown_scale.Size = new System.Drawing.Size(79, 23);
            this.numericUpDown_scale.TabIndex = 11;
            this.numericUpDown_scale.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // BrowserOverlaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDown_scale);
            this.Controls.Add(this.numericUpDown_positionY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDown_positionX);
            this.Controls.Add(this.numericUpDown_CanvasHeight);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown_CanvasWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_overlayUrl);
            this.Controls.Add(this.checkBox_enable);
            this.Name = "BrowserOverlaySettings";
            this.Size = new System.Drawing.Size(463, 340);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CanvasWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CanvasHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_positionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_positionX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_scale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_enable;
        private System.Windows.Forms.TextBox textBox_overlayUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_CanvasWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown_CanvasHeight;
        private System.Windows.Forms.NumericUpDown numericUpDown_positionY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown_positionX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown_scale;
    }
}
