namespace StreamCompanionTypes.Interfaces
{
    /// <summary>
    /// Defines a class that should get initalized in StreamCompanion <para/>
    /// All plugins are required to implement <see cref="IPlugin"/> in order to get loaded <para/>
    /// There is NO guaranted order of with handlers for other interfaces(like <see cref="ISettingsGetter"/> or <see cref="ISqliteUser"/>) will get called <para/>
    /// Only guarantee is that <see cref="IModule.Start"/> (with <see cref="IPlugin"/> implements) will get called as last
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