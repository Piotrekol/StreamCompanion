namespace StreamCompanionTypes.Interfaces
{
    public interface IPlugin : IModule
    {
        string Description { get; }
        string Name { get; }
        string Author { get; }
        string Url { get; }
        string UpdateUrl { get; }
    }
}