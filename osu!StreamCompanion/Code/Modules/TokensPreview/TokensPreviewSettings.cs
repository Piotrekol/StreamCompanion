using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanion.Common.Extensions;

namespace osu_StreamCompanion.Code.Modules.TokensPreview
{
    public partial class TokensPreviewSettings : UserControl
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public TokensPreviewSettings()
        {
            InitializeComponent();

            _ = UpdateLiveTokens(_cts.Token);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cts.TryCancel();
                _cts.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private async Task UpdateLiveTokens(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    if (liveTokens != null && this.IsHandleCreated)
                        Invoke((MethodInvoker)(() => { ProcessReplacements(liveTokens, 5, 35); }));

                    token.ThrowIfCancellationRequested();
                    await Task.Delay(50, token);
                }
            }
            catch
            {
                //ignored
                //Sometimes throws after exiting setting tab
            }
        }

        private List<KeyValuePair<string, IToken>> liveTokens = null;
        public void Add(Dictionary<string, IToken> replacements)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker) (() => { Add(replacements); }));
                return;
            }

            var notHidden = replacements.Where(x => (x.Value.Type & TokenType.Hidden) == 0).ToList();
            var normal = notHidden.Where(t => t.Value.Type == TokenType.Normal).OrderBy(x => x.Value.Name).ToList();
            var live = notHidden.Where(t => t.Value.Type == TokenType.Live).OrderBy(x => x.Value.Name).ToList();

            DrawingControl.SuspendDrawing(this);
            try
            {
                var currentRect = new Size(0, 25);
                currentRect += AddHeader("Live tokens (not always avaliable, not saved to files on disk)", currentRect.Height);
                currentRect += DrawTokens(live, currentRect.Height);
                currentRect.Height += 10;
                currentRect += AddHeader("Regular tokens", currentRect.Height);
                currentRect += DrawTokens(normal, currentRect.Height);

                liveTokens = live;
                label_ListedNum.Text = notHidden.Count.ToString();
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                DrawingControl.ResumeDrawing(this);
            }
        }

        private Size DrawTokens(IEnumerable<KeyValuePair<string, IToken>> tokens, int startHeight)
        {
            var size = new Size(0, 0);
            var tokensByPlugin = tokens.GroupBy(kv => kv.Value.PluginName).OrderBy(k => k.Key);
            foreach (var tokensGroup in tokensByPlugin)
            {
                size.Height += 2;
                size += DrawSection($"plugin: {tokensGroup.Key}", tokensGroup.ToList(), size.Height + startHeight);
            }

            return size;
        }

        private Size DrawSection(string sectionName, IEnumerable<KeyValuePair<string, IToken>> tokens, int startHeight)
        {
            var size = AddHeader(sectionName, startHeight);
            size.Height += 2;
            size += ProcessReplacements(tokens, 5, startHeight + size.Height);
            return size;
        }

        private Size ProcessReplacements(IEnumerable<KeyValuePair<string, IToken>> replacements, int xPosition = 30, int yPosition = 30)
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
                var value = tokenKv.Value.FormatedValue;
                if (value.Length > 150)
                    value = value.Substring(0, 150)+"...(Preview truncated)";
                var labelRect = AddLabel(labelName, value, maxLabelWidth + 10, yPosition + 2);
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
                p.VerticalScroll.Value = 0;
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

        private Size AddEditTextBox(IToken token, string name, int xPosition, int yPosition)
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
                p.VerticalScroll.Value = 0;
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
                ret.Control.Anchor = AnchorStyles.Left | AnchorStyles.Right | ret.Control.Anchor;
            }

            return ret.Control.Size;
        }



    }
}
