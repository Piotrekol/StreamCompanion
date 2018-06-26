using System.Windows.Forms;

namespace ClickCounter
{
    public partial class KeyClickFrm : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        public KeyClickFrm()
        {
            InitializeComponent();
        }
    }
}
