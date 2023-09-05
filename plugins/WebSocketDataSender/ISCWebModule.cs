using EmbedIO;
using System.Collections.Generic;

namespace WebSocketDataSender
{
    public interface ISCWebModule
    {
        List<(string Description, IWebModule module)> GetModules();
    }
}
