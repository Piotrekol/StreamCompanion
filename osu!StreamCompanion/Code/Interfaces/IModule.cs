namespace osu_StreamCompanion.Code.Interfaces
{
    //IModule instances are required to have parameter-less constructor to be loaded.
    public interface IModule
    {
        bool Started { get; set; }
        void Start(ILogger logger);
        
    }
}
