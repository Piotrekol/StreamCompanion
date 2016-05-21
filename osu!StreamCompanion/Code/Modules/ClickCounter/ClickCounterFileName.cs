using System;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.ClickCounter
{
    public partial class ClickCounterFileName : Form
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
        public ClickCounterFileName()
        {
            InitializeComponent();
            this.AcceptButton = button1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!FilenameIsValid(this.textBox_FileName.Text))
                this.textBox_FileName.Text = "";
            this.Close();
        }

        private bool FilenameIsValid(string fileName)
        {
            System.IO.FileInfo fi = null;
            try
            {
                fi = new System.IO.FileInfo(fileName);
            }
            catch (ArgumentException) { }
            catch (System.IO.PathTooLongException) { }
            catch (NotSupportedException) { }

            return !ReferenceEquals(fi, null);
        }
    }
}
