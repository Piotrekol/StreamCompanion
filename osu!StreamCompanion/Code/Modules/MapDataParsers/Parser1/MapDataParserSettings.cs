using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    //TODO: rewrite this... mess (use databindings?, simplify logic)

    public partial class MapDataParserSettings : UserControl
    {
        private readonly string _errorFilenameExists = "Pattern with that file name already exists" + Environment.NewLine + "Pattern not added";
        private readonly string _errorHorriblyWrong = "Something went horribly wrong " + Environment.NewLine + "Pattern not added";

        private List<FileFormating> _patternDictionary;
        private Dictionary<int, string> _saveEvents = new Dictionary<int, string>();
        //Conversion table from OsuStatus Id to corresponding Index in ComboBox selection.
        private readonly Dictionary<int, int> _statusToSelection = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _selectionToStatus = new Dictionary<int, int>();

        public event EventHandler dictionaryUpdated;
        private Dictionary<string, string> _replacementDict;


        public MapDataParserSettings(ref List<FileFormating> patternDictionary)
        {
            InitializeComponent();

            _patternDictionary = patternDictionary;

            _saveEvents.Add(1, "Listening");
            _saveEvents.Add(2, "Playing");
            _saveEvents.Add(8, "Watching");
            _saveEvents.Add(16, "Editing");
            _saveEvents.Add(27, "All");

            _statusToSelection.Add(1, 0);
            _statusToSelection.Add(2, 1);
            _statusToSelection.Add(8, 2);
            _statusToSelection.Add(16, 3);
            _statusToSelection.Add(27, 4);

            _selectionToStatus.Add(0, 1);
            _selectionToStatus.Add(1, 2);
            _selectionToStatus.Add(2, 8);
            _selectionToStatus.Add(3, 16);
            _selectionToStatus.Add(4, 27);



            foreach (var ev in _saveEvents)
            {
                comboBox_saveEvent.Items.Add(ev.Value);
            }
            UpdatePatterns();
            if (dataGridView.Rows.Count > 0)
            {
                LoadPattern(0);
            }
        }

        private void UpdatePatterns()
        {
            int currentlySelected = -1;
            if (dataGridView.SelectedRows.Count == 1)
            {
                currentlySelected = dataGridView.SelectedRows[0].Index;
            }
            this.dataGridView.Rows.Clear();
            lock (_patternDictionary)
            {
                foreach (var pattern in _patternDictionary)
                {
                    this.dataGridView.Rows.Add(pattern.Filename, pattern.Pattern, _saveEvents[pattern.SaveEvent]);
                }
            }
            if (currentlySelected > -1)
                if (dataGridView.Rows.Count > currentlySelected)
                    dataGridView.CurrentCell = dataGridView.Rows[currentlySelected].Cells[0];
                else if (dataGridView.Rows.Count > 0)
                {
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[0];
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
                    OnDictionaryUpdated();
                    return true;
                }



            }
            return false;
        }


        private bool FileNameAlreadyExists(string filename, string excl = "")
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                string currentName = dataGridView.Rows[i].Cells[0].Value.ToString().ToLower();
                if (currentName != excl && currentName == filename)
                    return true;
            }
            return false;
        }


        private void LoadPattern(int idx)
        {
            lock (_patternDictionary)
            {
                textBox_FileName.Text = _patternDictionary[idx].Filename;
                textBox_Formating.Text = _patternDictionary[idx].Pattern;
                comboBox_saveEvent.SelectedIndex = _statusToSelection[_patternDictionary[idx].SaveEvent];
                UpdatePreview(idx);
            }
        }

        private void UpdatePreview(int idx)
        {
            if (_replacementDict != null)
                textBox_Preview.Text = FormatMapString(textBox_Formating.Text, _replacementDict);
            else
                textBox_Preview.Text = "Change map in osu!";
        }

        private void button_AddPattern_Click(object sender, EventArgs e)
        {
            string fileName = textBox_FileName.Text;
            string formating = textBox_Formating.Text;
            int Event = _selectionToStatus[comboBox_saveEvent.SelectedIndex];
            //int Event = _statusToSelection.Select(cv => cv).First(cv => cv.Value == comboBox_saveEvent.SelectedIndex).Key;

            if (fileName != string.Empty && formating != string.Empty)
            {
                if (AddFormat(fileName, formating, Event))
                    UpdatePatterns();
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
                //int Event = _statusToSelection.Select(cv => cv).First(cv => cv.Value == comboBox_saveEvent.SelectedIndex).Key;
                int id = dataGridView.SelectedRows[0].Index;

                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(formating))
                    UserError("Fill all fields first.");

                if (id == -1)
                    UserError(_errorHorriblyWrong);
                else
                {
                    lock (_patternDictionary)
                    {
                        _patternDictionary.RemoveAt(id);
                        _patternDictionary.Add(new FileFormating
                        {
                            Filename = fileName,
                            Pattern = formating,
                            SaveEvent = Event
                        });
                    }
                    OnDictionaryUpdated();
                    UpdatePatterns();
                }
            }
        }

        protected virtual void OnDictionaryUpdated()
        {
            dictionaryUpdated?.Invoke(this, EventArgs.Empty);
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
            dataGridView.Rows.Clear();
            AddDefault();
            UpdatePatterns();
        }

        public void AddDefault()
        {
            AddFormat("np_listening.txt", "Listening: !ArtistRoman! - !TitleRoman!", 1,true);
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
                OnDictionaryUpdated();
                UpdatePatterns();
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
        public void SetTestDict(Dictionary<string, string> replacementDict)
        {
            _replacementDict = replacementDict;
            if (dataGridView.SelectedRows.Count == 1)
                if (this.IsHandleCreated)
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        UpdatePreview(dataGridView.SelectedRows[0].Index);
                    }));
        }

        private void textBox_Formating_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                UpdatePreview(dataGridView.SelectedRows[0].Index);
            }
        }

        private void button_OpenDirectory(object sender, EventArgs e)
        {
            //AppDomain.CurrentDomain.BaseDirectory + @"/Files/"
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"/Files/");
        }
    }
}
