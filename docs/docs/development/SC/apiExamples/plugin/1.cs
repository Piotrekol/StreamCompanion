using StreamCompanionTypes.Interfaces;

namespace newTestPlugin
{
    public class MyPlugin : IPlugin
    {
        public string Description => "my plugin description";
        public string Name => "my plugin name";
        public string Author => "my name";
        public string Url => "Plugin homepage url(github/site)";
    }
}