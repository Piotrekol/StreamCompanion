using StreamCompanionTypes.Interfaces.Services;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using System.Text;
using StreamCompanionTypes.Enums;

namespace FileMapDataSender
{
    public class FileMapManager
    {
        Dictionary<string, MapContainer> _files = new Dictionary<string, MapContainer>();
        private ILogger _logger;
        private readonly object _lockingObject = new object();

        public FileMapManager(ILogger logger)
        {
            _logger = logger;
        }

        private class MapContainer
        {
            public MemoryMappedFile File { get; set; }
            public bool ASCIIonly = false;
            private readonly object _lockingObject = new object();

            public void Write(string data)
            {
                lock (_lockingObject)
                {
                    byte[] bytes;
                    if (ASCIIonly)
                        bytes = Encoding.ASCII.GetBytes(data.Replace("\\n", "\n"));
                    else
                        bytes = Encoding.Unicode.GetBytes(data);

                    using (var a = File.CreateViewStream())
                    {
                        var contentSize = bytes.LongLength > a.Capacity - 2
                            ? (int)a.Capacity - 2
                            : bytes.Length;

                        a.Write(bytes, 0, contentSize);
                        a.WriteByte(0);
                        a.WriteByte(0);
                    }
                }
            }
        }
        [SupportedOSPlatform("windows")]
        private MapContainer GetFile(string pipeName)
        {
            lock (_lockingObject)
            {
                if (_files.ContainsKey(pipeName))
                    return _files[pipeName];

                MapContainer f = new MapContainer() { File = MemoryMappedFile.CreateOrOpen(pipeName, 2 * 1024 * 1024) };
                if (pipeName == "Sc-ingamePatterns" || pipeName== "Sc-webOverlayConfiguration" || pipeName.StartsWith("conf-") || pipeName.StartsWith("value-"))
                    f.ASCIIonly = true;
                _files.Add(pipeName, f);
                return f;
            }
        }

        [SupportedOSPlatform("windows")]
        public void Write(string name, string value)
        {
            try
            {
                var file = GetFile(name);
                file.Write(value);
            }
            catch (IOException ex)
            {
                _logger.Log(ex, LogLevel.Warning);
            }
        }
    }
}