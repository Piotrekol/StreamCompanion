using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface IFirstRunControlProvider
    {
        List<FirstRunUserControl> GetFirstRunUserControls();
    }
}