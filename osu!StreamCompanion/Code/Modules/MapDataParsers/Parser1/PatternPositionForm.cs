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
