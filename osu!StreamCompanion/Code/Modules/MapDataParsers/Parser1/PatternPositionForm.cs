using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class PatternPositionForm : Form
    {
        public PatternPositionForm()
        {
            InitializeComponent();
        }

        public int X => this.patternPosition.X;
        public int Y => this.patternPosition.Y;
        
    }
}
