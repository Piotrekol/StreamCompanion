using System;
using System.IO;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Core.Savers
{
    class FileSaver : ISaver
    {
        private readonly string _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files"+Path.DirectorySeparatorChar);
        public string SaveDirectory
        {
            get { return _saveDirectory; }
            set { }
        }
        public FileSaver()
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }


        public void Save(string directory, string content)
        {
            lock (this)
            {
                File.WriteAllText(SaveDirectory + directory, content);
            }
        }

        public void append(string directory, string content)
        {
            lock (this)
            {
                if (!File.Exists(SaveDirectory + directory))
                    File.WriteAllText(SaveDirectory + directory, content);
                else
                    File.AppendAllText(SaveDirectory + directory, content);
            }
        }
    }
}
