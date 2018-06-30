using CollectionManager.DataTypes;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace StreamCompanionTypes.Interfaces
{
    public interface IDifficultyCalculator
    {
        Beatmap ApplyMods(Beatmap map, Mods mods);
    }
}