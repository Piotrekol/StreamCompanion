using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Enums;

namespace TestPlugin;

[PluginDependency("OsuMemoryEventSource", "1.0.0")]
public class TestPlugin : IPlugin
{
    public TestPlugin(ILogger logger)
    {
        logger.Log("Hi", LogLevel.Information);
    }

    public string Description => "Plugin for testing..";

    public string Name => "TestPlugin";

    public string Author => "";

    public string Url => "";
}
