using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface ISettingsGetter
    {
        void SetSettingsListHandle(List<ISettingsProvider> settingsList);
    }
}