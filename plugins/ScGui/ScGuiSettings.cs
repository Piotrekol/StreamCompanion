using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces.Services;

namespace ScGui
{
    public partial class ScGuiSettings : UserControl
    {
        private readonly ISettings _settings;

        public ScGuiSettings(ISettings settings)
        {
            InitializeComponent();
            _settings = settings;

            this.comboBox_Theme.DataSource = new[] {"Light", "Dark"};
            this.comboBox_Theme.SelectedItem = _settings.Get<string>(MainWindowPlugin.Theme);
            this.comboBox_Theme.SelectedValueChanged += (_, __) =>
                _settings.Add(MainWindowPlugin.Theme.Name, comboBox_Theme.SelectedItem.ToString(), true);
        }
    }
}
