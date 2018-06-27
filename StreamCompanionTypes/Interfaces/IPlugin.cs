namespace StreamCompanionTypes.Interfaces
{
    /// <summary>
    /// Defines a class that should get initalized in StreamCompanion <para/>
    /// All plugins are required to implement <see cref="IPlugin"/> in order to get loaded
    /// </summary>
    public interface IPlugin : IModule
    {
        string Description { get; }
        string Name { get; }
        string Author { get; }
        string Url { get; }
        string UpdateUrl { get; }
    }
}