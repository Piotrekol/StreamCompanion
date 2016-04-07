using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface ISettingsProvider : ISettings
    {
        string SettingGroup { get; }
        void Free();
        UserControl GetUiSettings();

    }
}
