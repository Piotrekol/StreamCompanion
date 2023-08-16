namespace TestPlugin;

[SCPluginDependency("OsuMemoryEventSource", "1.0.0")]
[SCPlugin("Test plugin", "Plugin for testing..", "TEST", null)]
public class TestPlugin : IPlugin
{
    public TestPlugin(ILogger logger)
    {
        logger.Log("Hi", LogLevel.Information);
    }
}
