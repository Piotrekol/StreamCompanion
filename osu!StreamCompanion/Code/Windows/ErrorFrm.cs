﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using StreamCompanion.Common;

namespace osu_StreamCompanion.Code.Windows
{
    public partial class Error : Form
    {
        private readonly string errorTemplate = "Steps to reproduce this error(if known):" + Environment.NewLine +
                                       "" + Environment.NewLine +
                                       "" + Environment.NewLine +
                                       "" + Environment.NewLine +
                                       "~~" + Environment.NewLine +
                                       "Error:" + Environment.NewLine +
                                       "[code]" + Environment.NewLine +
                                       "{0}" + Environment.NewLine +
                                       "[/code]" + Environment.NewLine;
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public Error(string Message, string exitText)
        {
            InitializeComponent();
            this.textBox1.Text = Message;

            if(!string.IsNullOrEmpty(exitText))
                this.label_exitText.Text = exitText;
        }

        private void button_message_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Format(errorTemplate, textBox1.Text));
            ProcessExt.OpenUrl(@"https://osu.ppy.sh/forum/ucp.php?i=pm&mode=compose&u=304520");
        }
    }
}
