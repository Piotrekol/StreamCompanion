using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Misc
{
    public partial class Field : UserControl
    {
        private string _labelText;
        private bool _isPassword;

        public string LabelText
        {
            get { return _labelText; }
            set
            {
                label.Text = value;
                textBox.Location = new Point(label.Size.Width+5, textBox.Location.Y);
                _labelText = value;
            }
        }

        public bool IsPassword
        {
            get { return _isPassword; }
            set
            {
                textBox.PasswordChar = value ? '*' : '\0';
                _isPassword = value;
            }
        }

        public Field()
        {
            Start("Default text", false);
        }
        public Field(string text, bool isPassword)
        {
            Start(text, isPassword);
        }

        protected void Start(string text, bool isPassword)
        {
            InitializeComponent();

            AutoSize = true;
            Text = text;
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;
            label.TextAlign = ContentAlignment.MiddleLeft;
            textBox.Anchor = AnchorStyles.Left;

            this.IsPassword = isPassword;
        }

    }
}
