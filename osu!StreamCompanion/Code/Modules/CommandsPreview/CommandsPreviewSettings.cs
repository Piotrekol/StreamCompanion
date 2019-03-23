using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.CommandsPreview
{
    public partial class CommandsPreviewSettings : UserControl
    {
        private int _numberOfElements;

        public CommandsPreviewSettings()
        {
            InitializeComponent();

            new Task(UpdateLiveTokens).Start();

        }

        private void UpdateLiveTokens()
        {
            while (true)
            {
                if (liveTokens != null)
                    try
                    {
                        BeginInvoke((MethodInvoker) (() => { ProcessReplacements(liveTokens, 200); }));
                    }
                    catch
                    {
                        return;
                    }



                Thread.Sleep(11);
            }
        }
        public void Clear()
        {

        }

        private List<KeyValuePair<string, Token>> liveTokens = null;
        public void Add(Tokens replacements)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => { Add(replacements); }));
                return;
            }


            var normal = replacements.Where(t => t.Value.Type == TokenType.Normal);
            var live = replacements.Where(t => t.Value.Type == TokenType.Live).ToList();

            AddHeader("Live tokens (avaliable only when playing or watching)");
            ProcessReplacements(live, 200);

            AddHeader("Regular tokens");
            ProcessReplacements(normal);

            liveTokens = live;
        }

        private void ProcessReplacements(IEnumerable<KeyValuePair<string, Token>> replacements, int valueStartX = 105)
        {

            foreach (var replacement in replacements)
            {
                var value = replacement.Value.FormatedValue;

                string key = "T" + replacement.Key.Replace("!", "");
                if (p.Controls.ContainsKey(key))
                {
                    p.Controls[key].Text = value;
                }
                else
                {
                    AddSet($"!{replacement.Key}!", value, valueStartX);
                    label_ListedNum.Text = _numberOfElements.ToString();
                }
            }
        }

        private void AddHeader(string headerText)
        {
            if (p.Controls.ContainsKey("L" + headerText))
                return;

            Label label = new Label
            {
                Location = new Point(10, p.Controls[p.Controls.Count - 1].Location.Y + 15),
                Name = "L" + headerText.Replace("!", ""),
                Text = headerText,
                Size = new Size(500, 13)
            };
            p.Controls.Add(label);
            AddSeparator(p.Controls[p.Controls.Count - 1].Location.Y + 13);

            //label = new Label
            //{
            //    Location = new Point(10, p.Controls[p.Controls.Count - 1].Location.Y + 15),
            //    Name = "LSpacer" + headerText.Replace("!", ""),
            //    Text = string.Empty,
            //    Size = new Size(200, 13)
            //};
            //p.Controls.Add(label);
        }
        private void AddSet(string key, string value, int valueStartX)
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
                Size = new Size(valueStartX - 10, 13)
            };
            p.Controls.Add(label);
            Label label2 = new Label
            {
                Location = new Point(valueStartX, p.Controls[p.Controls.Count - 1].Location.Y),
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
