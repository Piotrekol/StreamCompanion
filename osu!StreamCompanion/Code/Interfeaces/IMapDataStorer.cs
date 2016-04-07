using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface IMapDataStorer
    {
        void StartMassStoring();
        void EndMassStoring();
        void StoreBeatmap(Beatmap beatmap);

    }
}