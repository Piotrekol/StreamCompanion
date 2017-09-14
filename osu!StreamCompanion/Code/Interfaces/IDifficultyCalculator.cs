using CollectionManager.DataTypes;
using Beatmap = osu_StreamCompanion.Code.Core.DataTypes.Beatmap;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface IDifficultyCalculator
    {
        Beatmap ApplyMods(Beatmap map, Mods mods);
    }
}