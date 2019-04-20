using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using System;
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
        private Task UpdateTask;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public CommandsPreviewSettings()
        {
            InitializeComponent();

            UpdateTask = new Task(UpdateLiveTokens);
            UpdateTask.Start();
        }

        public new void Dispose()
        {
            _cts.Cancel();
            base.Dispose();
        }
        private void UpdateLiveTokens()
        {
            try
            {
                while (true)
                {
                    if (liveTokens != null && this.IsHandleCreated)

                        BeginInvoke((MethodInvoker)(() => { ProcessReplacements(liveTokens, 5, 35); }));


                    _cts.Token.ThrowIfCancellationRequested();

                    Thread.Sleep(11);
                }
            }
            catch (Exception)
            {
            }
        }
        public void Clear()
        {

        }

        private List<KeyValuePair<string, Token>> liveTokens = null;
        public void Add(Dictionary<string, Token> replacements)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => { Add(replacements); }));
                return;
            }


            var normal = replacements.Where(t => t.Value.Type == TokenType.Normal);
            var live = replacements.Where(t => t.Value.Type == TokenType.Live).ToList();

            var size = AddHeader("Live tokens (avaliable only when playing or watching)", 20);
            size.Height += 25;
            size += ProcessReplacements(live, 5, size.Height);

            size.Height += 5;
            size += AddHeader("Regular tokens", size.Height);
            size += ProcessReplacements(normal, 5, size.Height);

            liveTokens = live;
            label_ListedNum.Text = replacements.Count.ToString();
        }

        private Size ProcessReplacements(IEnumerable<KeyValuePair<string, Token>> replacements, int xPosition = 30, int yPosition = 30)
        {
            var replacementsCopy = replacements.ToDictionary(k => k.Key, v => v.Value);
            var startYPosition = yPosition;

            List<int> lineWidths = new List<int>();
            //first iteration - create labels and buttons
            Size buttonRect = new Size();
            foreach (var tokenKv in replacementsCopy)
            {
                buttonRect = AddEditTextBox(tokenKv.Value, tokenKv.Key, xPosition, yPosition);

                string labelName = "L" + tokenKv.Key;
                var labelRect = AddLabel(labelName, tokenKv.Key, buttonRect.Width + xPosition, yPosition + 2);
                lineWidths.Add(labelRect.Width + buttonRect.Width);

                yPosition += Math.Max(labelRect.Height, buttonRect.Height);
                AddSeparator(tokenKv.Key, yPosition);
            }

            var maxLabelWidth = lineWidths.Count > 0 ? lineWidths.Max() : 0;

            yPosition = startYPosition;

            //second iteration - create value fields 
            foreach (var tokenKv in replacementsCopy)
            {
                string labelName = "V" + tokenKv.Key;
                var labelRect = AddLabel(labelName, tokenKv.Value.FormatedValue, maxLabelWidth + 10, yPosition + 2);
                lineWidths.Add(labelRect.Width);
                yPosition += Math.Max(labelRect.Height, buttonRect.Height);
            }


            return new Size(lineWidths.Count > 0 ? lineWidths.Max() : 0, yPosition - startYPosition);
        }

        private (Size Size, T Control, bool AlreadyExisted) AddControl<T>(string name, string text, int xPosition, int yPosition, bool autosize = true, int width = 20, int height = 20) where T : Control, new()
        {
            T control;
            bool alreadyExisted = false;
            if (p.Controls.ContainsKey(name))
            {
                control = (T)p.Controls[name];
                alreadyExisted = true;
            }
            else
            {
                control = new T
                {
                    Location = new Point(xPosition, yPosition),
                    Name = name,
                    Size = new Size(width, height),
                    AutoSize = autosize,
                };
                p.Controls.Add(control);
            }

            if (control.Text != text)
            {
                control.Text = text;
            }

            return (control.Size, control, alreadyExisted);
        }

        private Size AddEditTextBox(Token token, string name, int xPosition, int yPosition)
        {
            var ret = AddControl<TextBox>($"T{name}", token.Format, xPosition, yPosition);
            if (!ret.AlreadyExisted)
            {
                ret.Control.Size = new Size(100, 20);
                ret.Control.TextChanged += (sender, args) =>
                {
                    token.Format = ret.Control.Text;
                    p.Controls["V" + name].Text = token.FormatedValue;
                };
            }

            return ret.Control.Size;
        }

        private Size AddLabel(string name, string text, int xPosition, int yPosition)
        {
            return AddControl<Label>(name, text, xPosition, yPosition).Size;
        }

        private Size AddHeader(string headerText, int yPosition, int width = 500)
        {
            var controlName = "L" + headerText;
            Label label;
            Size separatorSize;
            if (p.Controls.ContainsKey(controlName))
            {
                label = (Label)p.Controls[controlName];
                label.Width = width;
                separatorSize = p.Controls[$"S{controlName}"].Size;
            }
            else
            {
                label = new Label
                {
                    Location = new Point(2, yPosition),
                    Name = controlName,
                    Text = headerText,
                    Size = new Size(width, 13)
                };
                p.Controls.Add(label);
                separatorSize = AddSeparator(controlName, yPosition + label.Height + 1);
            }

            return new Size(label.Width, label.Height + separatorSize.Height);
        }

        private Size AddSeparator(string name, int yPos)
        {
            var ret = AddControl<Label>($"S{name}", "", 2, yPos, false, 660, 2);
            if (!ret.AlreadyExisted)
            {
                ret.Control.BorderStyle = BorderStyle.Fixed3D;
            }

            return ret.Control.Size;
        }



    }
}
