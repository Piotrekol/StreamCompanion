namespace StreamCompanionTypes.Interfaces
{
    /// <summary>
    /// Defines a class that should get initalized in StreamCompanion <para/>
    /// All plugins are required to implement <see cref="IPlugin"/> in order to get loaded <para/>
    /// All dependencies defined in plugin class constructor will get injected.
    /// </summary>
    public interface IPlugin 
    {
        string Description { get; }
        string Name { get; }
        string Author { get; }
        string Url { get; }
        string UpdateUrl { get; }
    }
}