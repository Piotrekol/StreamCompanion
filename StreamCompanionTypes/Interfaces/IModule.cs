namespace StreamCompanionTypes.Interfaces
{
    public interface IModule
    {
        bool Started { get; set; }
        void Start(ILogger logger);
    }
}
