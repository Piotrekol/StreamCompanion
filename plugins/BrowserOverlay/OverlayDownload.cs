using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserOverlay
{
    public partial class OverlayDownload : Form
    {
        public OverlayDownload()
        {
            InitializeComponent();
        }

        public void SetStatus(string status)
        {
            label_status.Text = status;
        }

        public void SetProgress(int progress)
        {
            progressBar_downloadProgress.Value = progress;
        }
    }
}
