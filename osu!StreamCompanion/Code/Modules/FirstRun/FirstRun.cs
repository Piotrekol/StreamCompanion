using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Core;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public class FirstRun : IModule, ISettings, IMsnGetter
    {
        private ISettingsHandler _settings;
        private FirstRunFrm _setupFrm;
        private Action _getOsuDirectory;

        public FirstRun(Action getOsuDirectory)
        {
            _getOsuDirectory = getOsuDirectory;
        }

        public bool Started { get; set; }

        public bool CompletedSuccesfully { get; set; }


        public void Start(ILogger logger)
        {
            Started = true;
            _setupFrm = new FirstRunFrm(_settings);
            _setupFrm.Closing += _setupFrm_Closing;
            _setupFrm.StartSetup();
        }

        private void _setupFrm_Closing(object sender, EventArgs e)
        {
            CompletedSuccesfully = _setupFrm.CompletedSuccesfully;
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }

        public void nothing() { }
        public void SetNewMsnString(Dictionary<string, string> osuStatus)
        {
            if (_setupFrm != null && !_setupFrm.IsDisposed)
            {
                _getOsuDirectory();
                _getOsuDirectory = nothing;
                _setupFrm.GotMsn(string.Format("{0} {1} - {2} ", osuStatus["status"], osuStatus["artist"], osuStatus["title"]));
            }
        }
    }
}