using System;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Core.Savers
{
    public class MainSaver : ISaver
    {
        private ISaver _saver;
        private ILogger _logger;

        public string SaveDirectory
        {
            get { return _saver.SaveDirectory; }
            set { }
        }


        public MainSaver(ILogger logger, TextSaver saver)
        {
            _logger = logger;
            _saver = saver;
        }
        public void ChangeSaver(ISaver saver)
        {
            if (_saver is IDisposable)
                ((IDisposable)_saver).Dispose();
            _saver = null;
            _saver = saver;
        }

        public void Save(string directory, string content)
        {
            try
            {
                _saver.Save(directory, content);
            }
            catch (System.IO.IOException e)
            {
                _logger.Log("EXCEPTION: {0}" + Environment.NewLine + "{1}", LogLevel.Basic, e.Message, e.StackTrace);
            }
        }

        public void append(string directory, string content)
        {
            try
            {
                _saver.append(directory, content);
            }
            catch (System.IO.IOException e)
            {
                _logger.Log("EXCEPTION: {0}" + Environment.NewLine + "{1}", LogLevel.Basic, e.Message, e.StackTrace);
            }
        }
    }
}
