using StreamCompanionTypes.DataTypes;

namespace FileMapDataSender
{
    public interface IFileMapDataSender
    {
        void Save(string fileName, string content);
        void SetNewMap(MapSearchResult map);
    }
}