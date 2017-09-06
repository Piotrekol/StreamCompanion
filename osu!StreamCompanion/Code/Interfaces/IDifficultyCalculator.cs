using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Modules.ModsHandler;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface IDifficultyCalculator
    {
        Beatmap ApplyMods(Beatmap map, EMods mods);
    }
}