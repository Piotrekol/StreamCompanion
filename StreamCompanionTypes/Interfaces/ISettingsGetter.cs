using System.Collections.Generic;

namespace StreamCompanionTypes.Interfaces
{
    public interface ISettingsGetter
    {
        void SetSettingsListHandle(List<ISettingsProvider> settingsList);
    }
}