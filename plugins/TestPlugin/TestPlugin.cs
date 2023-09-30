using StreamCompanion.Common;

namespace TestPlugin;

[SCPluginDependency("OsuMemoryEventSource", "1.0.0")]
[SCPlugin("Test plugin", "Plugin for testing..", "TEST", null)]
public class TestPlugin : IPlugin
{
    public TestPlugin(ILogger logger)
    {
        logger.Log("Hi", LogLevel.Information);
        var tokenSetter = Tokens.CreateTokenSetter("Test Plugin");
        _ = Task.Run(async () =>
        {
            var testToken = tokenSetter("test", "test", TokenType.Live | TokenType.Hidden);

            await testToken.ConvertToLiveToken(() => "updated test", CancellationToken.None);
        });
    }
}
