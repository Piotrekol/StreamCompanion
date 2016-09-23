using System;
using System.IO;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Misc
{
    class FileChecker
    {
        private string[] _requiredFiles = new string[0];
        private string caption = "StreamCompanion Error";
        private string errorMessage =
            "It seems that you are missing one or more of the files that are required to run this software." + Environment.NewLine +
            "Place all downloaded files in a single folder then try running it again." + Environment.NewLine +
            "Couldn't find: \"{0}\"" + Environment.NewLine +
            "StreamCompanion will now exit.";

        public FileChecker()
        {
            foreach (var requiredFile in _requiredFiles)
            {
                if (!File.Exists(requiredFile))
                {
                    MessageBox.Show(string.Format(errorMessage, requiredFile), caption, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Program.SafeQuit();
                }
            }
        }
    }
}
