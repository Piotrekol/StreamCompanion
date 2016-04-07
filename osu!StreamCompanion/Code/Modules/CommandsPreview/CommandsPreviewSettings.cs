using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.CommandsPreview
{
    public partial class CommandsPreviewSettings : UserControl
    {
        private int _numberOfElements;

        public CommandsPreviewSettings()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            
        }

        public void Add(Dictionary<string, string> replacements)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => { Add(replacements); }));
                return;
            }
            foreach (var replacement in replacements)
            {
                string key = "T" + replacement.Key.Replace("!", "");
                if (p.Controls.ContainsKey(key))
                {
                    p.Controls[key].Text = replacement.Value;
                }
                else
                {
                    AddSet(replacement.Key,replacement.Value);
                    label_ListedNum.Text = _numberOfElements.ToString();
                }


            }
        }
        private void AddSet(string key, string value)
        {
            if (Controls.ContainsKey("L" + key.Replace("!", "")))
            {
                return;
            }
            Label label = new Label
            {
                Location = new Point(10, p.Controls[p.Controls.Count - 2].Location.Y + 15),
                Name = "L" + key.Replace("!", ""),
                Text = key,
                Size = new Size(90, 13)
            };
            p.Controls.Add(label);
            Label label2 = new Label
            {
                Location = new Point(105, p.Controls[p.Controls.Count - 1].Location.Y),
                Name = "T" + key.Replace("!", ""),
                Text = value,
                Size = new Size(480, 13)
            };
            p.Controls.Add(label2);
            AddSeparator(p.Controls[p.Controls.Count - 1].Location.Y + 13);
            
            _numberOfElements++;
        }
        private void AddSeparator(int yPos)
        {
            var label3 = new Label
            {
                Location = new Point(14, yPos),
                Name = "separator" + yPos,
                BorderStyle = BorderStyle.Fixed3D,
                Size = new Size(560, 2),
                Text = ""
            };
            p.Controls.Add(label3);
        }



    }
}
