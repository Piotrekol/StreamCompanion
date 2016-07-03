using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{

    public partial class MapDataParserSettings : UserControl
    {
        private readonly string _errorFilenameExists = "Pattern with that file name already exists" + Environment.NewLine + "Pattern not added";
        private readonly string _errorHorriblyWrong = "Something went horribly wrong " + Environment.NewLine + "Pattern not added";

        private BindingList<FileFormating> _patternDictionary;

        private readonly Dictionary<int, int> _statusToSelection = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _selectionToStatus = new Dictionary<int, int>();

        private Dictionary<string, string> _PreviewReplacementDict;


        public MapDataParserSettings(ref BindingList<FileFormating> patternDictionary)
        {
            InitializeComponent();

            _patternDictionary = patternDictionary;
            var source = new BindingSource(_patternDictionary, null);
            dataGridView.DataSource = source;

            var _saveEvents = new Dictionary<int, string>();
            _saveEvents.Add(0, "Never");
            _saveEvents.Add(1, "Listening");
            _saveEvents.Add(2, "Playing");
            _saveEvents.Add(8, "Watching");
            _saveEvents.Add(16, "Editing");
            _saveEvents.Add(27, "All");

            _statusToSelection.Add(0, 0);
            _statusToSelection.Add(1, 1);
            _statusToSelection.Add(2, 2);
            _statusToSelection.Add(8, 3);
            _statusToSelection.Add(16, 4);
            _statusToSelection.Add(27, 5);

            _selectionToStatus.Add(0, 0);
            _selectionToStatus.Add(1, 1);
            _selectionToStatus.Add(2, 2);
            _selectionToStatus.Add(3, 8);
            _selectionToStatus.Add(4, 16);
            _selectionToStatus.Add(5, 27);



            foreach (var ev in _saveEvents)
            {
                comboBox_saveEvent.Items.Add(ev.Value);
            }
        }


        private void UserError(string msg)
        {
            MessageBox.Show(msg, "Info");
        }
        private bool AddFormat(string fileName, string formating, int saveEvent, bool suppressErrors = false)
        {
            lock (_patternDictionary)
            {
                if (FileNameAlreadyExists(fileName))
                {
                    if (!suppressErrors)
                        UserError(_errorFilenameExists);
                }
                else
                {
                    _patternDictionary.Add(new FileFormating
                    {
                        Filename = fileName,
                        Pattern = formating,
                        SaveEvent = saveEvent
                    });
                    return true;
                }
            }
            return false;
        }


        private bool FileNameAlreadyExists(string filename, string excl = "", int ignoreId = -1)
        {
            for (int i = 0; i < _patternDictionary.Count; i++)
            {
                string currentFileName = _patternDictionary[i].Filename;
                if (string.IsNullOrEmpty(currentFileName)) continue;
                if (currentFileName != excl && currentFileName == filename && i != ignoreId)
                    return true;
            }
            return false;
        }


        private void LoadPattern(int idx)
        {
            lock (_patternDictionary)
            {
                if (idx < _patternDictionary.Count)
                {
                    textBox_FileName.Text = _patternDictionary[idx].Filename;
                    textBox_Formating.Text = _patternDictionary[idx].Pattern;
                    comboBox_saveEvent.SelectedIndex = _statusToSelection[_patternDictionary[idx].SaveEvent == 0 ? 27 : _patternDictionary[idx].SaveEvent];
                    UpdatePreview(idx);
                }
            }
        }

        private void UpdatePreview(int idx)
        {
            if (_PreviewReplacementDict != null)
                textBox_Preview.Text = FormatMapString(textBox_Formating.Text, _PreviewReplacementDict);
            else
                textBox_Preview.Text = "Change map in osu!";
        }

        private void button_AddPattern_Click(object sender, EventArgs e)
        {
            string fileName = textBox_FileName.Text;
            string formating = textBox_Formating.Text;
            int Event = _selectionToStatus[comboBox_saveEvent.SelectedIndex];

            if (fileName != string.Empty && formating != string.Empty)
            {
                AddFormat(fileName, formating, Event);
            }
            else
            {
                UserError("Fill all fields first.");
            }

        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                LoadPattern(dataGridView.SelectedRows[0].Index);
            }
        }

        private void button_EditPattern_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                string fileName = textBox_FileName.Text;
                string formating = textBox_Formating.Text;
                int Event = _selectionToStatus[comboBox_saveEvent.SelectedIndex];
                int id = dataGridView.SelectedRows[0].Index;

                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(formating))
                {
                    UserError("Fill all fields first.");
                    return;
                }
                if (FileNameAlreadyExists(fileName, "", id))
                {
                    UserError(_errorFilenameExists);
                    return;
                }
                if (id == -1)
                    UserError(_errorHorriblyWrong);
                else
                {
                    lock (_patternDictionary)
                    {
                        _patternDictionary[id] = new FileFormating
                        {
                            Filename = fileName,
                            Pattern = formating,
                            SaveEvent = Event
                        };
                    }
                }
            }
        }


        private void button_Reset_Click(object sender, EventArgs e)
        {
            var result =
                MessageBox.Show(
                    "All of your edited/added files will be deleted." + Environment.NewLine + "ARE YOU SURE?",
                    "Info", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
                return;

            lock (_patternDictionary)
            {
                _patternDictionary.Clear();
            }
            AddDefault();
        }

        public void AddDefault()
        {
            AddFormat("np_listening.txt", "Listening: !ArtistRoman! - !TitleRoman!", 1, true);
            AddFormat("np_playing.txt", "Playing: !ArtistRoman! - !TitleRoman! [!DiffName!] CS:!cs! AR:!ar! OD:!od! HP:!hp!", 2, true);
            AddFormat("np_playing_details.txt", "CS:!cs! AR:!ar! OD:!od! HP:!hp!", 2, true);
            AddFormat("np_playing_DL.txt", "!dl!", 2, true);
        }

        private void button_RemovePattern_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                int id = dataGridView.SelectedRows[0].Index;
                lock (_patternDictionary)
                {
                    _patternDictionary.RemoveAt(id);
                }
            }
        }

        public string FormatMapString(string toFormat, Dictionary<string, string> replacements)
        {
            foreach (var r in replacements)
            {
                toFormat = toFormat.Replace(r.Key, r.Value);
            }
            return toFormat;
        }
        public void SetPreviewDict(Dictionary<string, string> replacementDict)
        {
            _PreviewReplacementDict = replacementDict;
            if (dataGridView.SelectedRows.Count == 1)
                if (this.IsHandleCreated)
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        UpdatePreview(dataGridView.SelectedRows[0].Index);
                    }));
        }

        private void button_OpenDirectory(object sender, EventArgs e)
        {
            //AppDomain.CurrentDomain.BaseDirectory + @"/Files/"
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"/Files/");
        }
    }
}
