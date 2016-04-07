using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface ISettingsGetter
    {
        void SetSettingsListHandle(List<ISettingsProvider> settingsList);
    }
}