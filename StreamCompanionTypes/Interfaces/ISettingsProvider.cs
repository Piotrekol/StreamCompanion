using System.Windows.Forms;

namespace StreamCompanionTypes.Interfaces
{
    public interface ISettingsProvider
    {
        string SettingGroup { get; }
        void Free();
        //TODO: change this from UserControl to object to not require referencing winForms when plugin doesn't use it
        UserControl GetUiSettings();

    }
}
