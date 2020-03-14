using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Helpers;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Savers
{
    public class TextSaver : ISaver
    {
        public static readonly ConfigEntry SaveDirectoryConfigEntry = new ConfigEntry("FileSaveDirectory", null);
        private string _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files" + Path.DirectorySeparatorChar);
        private readonly object _lockingObject = new object();
        public string SaveDirectory
        {
            get { return _saveDirectory; }
            set { }
        }

        public TextSaver(ISettings settings)
        {
            var saveDirectory = settings.Get<string>(SaveDirectoryConfigEntry);
            if (saveDirectory != SaveDirectoryConfigEntry.Default<string>()
                && !string.IsNullOrWhiteSpace(saveDirectory))
            {
                _saveDirectory = saveDirectory;
                var driveList = DriveInfo.GetDrives().ToList();
                var saveDirectoryInfo = new DirectoryInfo(_saveDirectory);
                foreach (var drive in driveList)
                {
                    if ((drive.DriveType == DriveType.Network || drive.DriveType == DriveType.Removable)
                        && saveDirectoryInfo.Root.FullName == drive.RootDirectory.FullName)
                    {
                        MessageBox.Show("Please note that saving patterns on network shares might cause unexpected behaviours(like SC updating with 30s+ delay) - especially if connection is slow, and is not really supported.",
                            "osu!StreamCompanion - network or removable media information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            try
            {
                if (!Directory.Exists(SaveDirectory))
                {
                    Directory.CreateDirectory(SaveDirectory);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new NonLoggableException(ex, "Could not create folder due to insuffisient premissions" +
                    Environment.NewLine + "Please move this exectuable into a non-system folder");
            }
        }

        public void Save(string directory, string content)
        {
            lock (_lockingObject)
            {
                File.WriteAllText(Path.Combine(SaveDirectory, directory), content);
            }
        }

        public void append(string directory, string content)
        {
            lock (_lockingObject)
            {
                var path = Path.Combine(SaveDirectory, directory);
                if (!File.Exists(path))
                    File.WriteAllText(path, content);
                else
                    File.AppendAllText(path, content);
            }
        }
    }
}
