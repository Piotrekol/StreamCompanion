
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
            checkBox_enable = new System.Windows.Forms.CheckBox();
            textBox_overlayUrl = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            numericUpDown_CanvasWidth = new System.Windows.Forms.NumericUpDown();
            label3 = new System.Windows.Forms.Label();
            numericUpDown_CanvasHeight = new System.Windows.Forms.NumericUpDown();
            numericUpDown_positionY = new System.Windows.Forms.NumericUpDown();
            label4 = new System.Windows.Forms.Label();
            numericUpDown_positionX = new System.Windows.Forms.NumericUpDown();
            label5 = new System.Windows.Forms.Label();
            numericUpDown_scale = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            panel_form = new System.Windows.Forms.Panel();
            button_addTab = new System.Windows.Forms.Button();
            listBox_tabs = new System.Windows.Forms.ListBox();
            button_remove = new System.Windows.Forms.Button();
            panel_content = new System.Windows.Forms.Panel();
            button_toggleBorders = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_CanvasWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_CanvasHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_positionY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_positionX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_scale).BeginInit();
            panel_form.SuspendLayout();
            panel_content.SuspendLayout();
            SuspendLayout();
            // 
            // checkBox_enable
            // 
            checkBox_enable.AutoSize = true;
            checkBox_enable.Location = new System.Drawing.Point(17, 12);
            checkBox_enable.Name = "checkBox_enable";
            checkBox_enable.Size = new System.Drawing.Size(299, 19);
            checkBox_enable.TabIndex = 0;
            checkBox_enable.Text = "Enable browser ingame overlay (Requires SC restart)";
            checkBox_enable.UseVisualStyleBackColor = true;
            checkBox_enable.CheckedChanged += checkBox_enable_CheckedChanged;
            // 
            // textBox_overlayUrl
            // 
            textBox_overlayUrl.Location = new System.Drawing.Point(82, 3);
            textBox_overlayUrl.Name = "textBox_overlayUrl";
            textBox_overlayUrl.Size = new System.Drawing.Size(520, 23);
            textBox_overlayUrl.TabIndex = 1;
            textBox_overlayUrl.TextChanged += textBox_overlayUrl_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(2, 6);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(74, 15);
            label2.TabIndex = 4;
            label2.Text = "Overlay URL:";
            // 
            // numericUpDown_CanvasWidth
            // 
            numericUpDown_CanvasWidth.Location = new System.Drawing.Point(132, 35);
            numericUpDown_CanvasWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown_CanvasWidth.Name = "numericUpDown_CanvasWidth";
            numericUpDown_CanvasWidth.Size = new System.Drawing.Size(79, 23);
            numericUpDown_CanvasWidth.TabIndex = 5;
            numericUpDown_CanvasWidth.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(2, 37);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(70, 15);
            label3.TabIndex = 6;
            label3.Text = "Canvas size:";
            // 
            // numericUpDown_CanvasHeight
            // 
            numericUpDown_CanvasHeight.Location = new System.Drawing.Point(270, 35);
            numericUpDown_CanvasHeight.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown_CanvasHeight.Name = "numericUpDown_CanvasHeight";
            numericUpDown_CanvasHeight.Size = new System.Drawing.Size(79, 23);
            numericUpDown_CanvasHeight.TabIndex = 7;
            numericUpDown_CanvasHeight.ValueChanged += numericUpDown_ValueChanged;
            // 
            // numericUpDown_positionY
            // 
            numericUpDown_positionY.Location = new System.Drawing.Point(270, 67);
            numericUpDown_positionY.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown_positionY.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numericUpDown_positionY.Name = "numericUpDown_positionY";
            numericUpDown_positionY.Size = new System.Drawing.Size(79, 23);
            numericUpDown_positionY.TabIndex = 10;
            numericUpDown_positionY.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(19, 69);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 15);
            label4.TabIndex = 9;
            label4.Text = "Position:";
            // 
            // numericUpDown_positionX
            // 
            numericUpDown_positionX.Location = new System.Drawing.Point(132, 67);
            numericUpDown_positionX.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown_positionX.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numericUpDown_positionX.Name = "numericUpDown_positionX";
            numericUpDown_positionX.Size = new System.Drawing.Size(79, 23);
            numericUpDown_positionX.TabIndex = 8;
            numericUpDown_positionX.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(35, 98);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(37, 15);
            label5.TabIndex = 12;
            label5.Text = "Scale:";
            // 
            // numericUpDown_scale
            // 
            numericUpDown_scale.DecimalPlaces = 3;
            numericUpDown_scale.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            numericUpDown_scale.Location = new System.Drawing.Point(132, 96);
            numericUpDown_scale.Name = "numericUpDown_scale";
            numericUpDown_scale.Size = new System.Drawing.Size(79, 23);
            numericUpDown_scale.TabIndex = 11;
            numericUpDown_scale.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(84, 37);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 15);
            label1.TabIndex = 13;
            label1.Text = "Width:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(217, 37);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(46, 15);
            label6.TabIndex = 14;
            label6.Text = "Height:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(109, 69);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(17, 15);
            label7.TabIndex = 15;
            label7.Text = "X:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(246, 69);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(17, 15);
            label8.TabIndex = 16;
            label8.Text = "Y:";
            // 
            // panel_form
            // 
            panel_form.Controls.Add(label2);
            panel_form.Controls.Add(label8);
            panel_form.Controls.Add(textBox_overlayUrl);
            panel_form.Controls.Add(label7);
            panel_form.Controls.Add(numericUpDown_CanvasWidth);
            panel_form.Controls.Add(label6);
            panel_form.Controls.Add(label3);
            panel_form.Controls.Add(label1);
            panel_form.Controls.Add(numericUpDown_CanvasHeight);
            panel_form.Controls.Add(label5);
            panel_form.Controls.Add(numericUpDown_positionX);
            panel_form.Controls.Add(numericUpDown_scale);
            panel_form.Controls.Add(label4);
            panel_form.Controls.Add(numericUpDown_positionY);
            panel_form.Location = new System.Drawing.Point(10, 317);
            panel_form.Name = "panel_form";
            panel_form.Size = new System.Drawing.Size(687, 160);
            panel_form.TabIndex = 17;
            // 
            // button_addTab
            // 
            button_addTab.Location = new System.Drawing.Point(10, 276);
            button_addTab.Name = "button_addTab";
            button_addTab.Size = new System.Drawing.Size(75, 23);
            button_addTab.TabIndex = 19;
            button_addTab.Text = "Add tab";
            button_addTab.UseVisualStyleBackColor = true;
            button_addTab.Click += button_addTab_Click;
            // 
            // listBox_tabs
            // 
            listBox_tabs.FormattingEnabled = true;
            listBox_tabs.ItemHeight = 15;
            listBox_tabs.Location = new System.Drawing.Point(10, 9);
            listBox_tabs.Name = "listBox_tabs";
            listBox_tabs.Size = new System.Drawing.Size(602, 259);
            listBox_tabs.TabIndex = 20;
            listBox_tabs.SelectedIndexChanged += listBox_tabs_SelectedIndexChanged;
            // 
            // button_remove
            // 
            button_remove.Location = new System.Drawing.Point(91, 276);
            button_remove.Name = "button_remove";
            button_remove.Size = new System.Drawing.Size(130, 23);
            button_remove.TabIndex = 21;
            button_remove.Text = "Remove selected tab";
            button_remove.UseVisualStyleBackColor = true;
            button_remove.Click += button_remove_Click;
            // 
            // panel_content
            // 
            panel_content.Controls.Add(button_toggleBorders);
            panel_content.Controls.Add(listBox_tabs);
            panel_content.Controls.Add(button_remove);
            panel_content.Controls.Add(panel_form);
            panel_content.Controls.Add(button_addTab);
            panel_content.Location = new System.Drawing.Point(17, 37);
            panel_content.Name = "panel_content";
            panel_content.Size = new System.Drawing.Size(707, 486);
            panel_content.TabIndex = 22;
            // 
            // button_toggleBorders
            // 
            button_toggleBorders.Location = new System.Drawing.Point(466, 276);
            button_toggleBorders.Name = "button_toggleBorders";
            button_toggleBorders.Size = new System.Drawing.Size(146, 23);
            button_toggleBorders.TabIndex = 22;
            button_toggleBorders.Text = "Toggle all borders";
            button_toggleBorders.UseVisualStyleBackColor = true;
            button_toggleBorders.Click += button_toggleBorders_Click;
            // 
            // BrowserOverlaySettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel_content);
            Controls.Add(checkBox_enable);
            Name = "BrowserOverlaySettings";
            Size = new System.Drawing.Size(770, 572);
            ((System.ComponentModel.ISupportInitialize)numericUpDown_CanvasWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_CanvasHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_positionY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_positionX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_scale).EndInit();
            panel_form.ResumeLayout(false);
            panel_form.PerformLayout();
            panel_content.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel_form;
        private System.Windows.Forms.Button button_addTab;
        private System.Windows.Forms.ListBox listBox_tabs;
        private System.Windows.Forms.Button button_remove;
        private System.Windows.Forms.Panel panel_content;
        private System.Windows.Forms.Button button_toggleBorders;
    }
}
