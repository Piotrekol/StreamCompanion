using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface IMapDataStorer
    {
        void StartMassStoring();
        void EndMassStoring();
        void StoreBeatmap(Beatmap beatmap);

    }
}