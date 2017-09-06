using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface ISettingsProvider : ISettings
    {
        string SettingGroup { get; }
        void Free();
        UserControl GetUiSettings();

    }
}
