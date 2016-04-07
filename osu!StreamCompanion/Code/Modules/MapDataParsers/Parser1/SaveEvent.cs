namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class SaveEvent
    {
        public SaveEvent(int saveEnum, string name)
        {
            SaveEnum = saveEnum;
            Name = name;
        }
        public int SaveEnum { get; set; }
        public string Name { get; set; }
    }
}
