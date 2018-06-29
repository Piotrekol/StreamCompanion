using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Core;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public class FirstRun : IModule, ISettings
    {
        private readonly List<FirstRunUserControl> _firstRunControls;
        private ISettingsHandler _settings;
        private FirstRunFrm _setupFrm;

        public FirstRun(List<FirstRunUserControl> firstRunControls)
        {
            _firstRunControls = firstRunControls;
        }

        public bool Started { get; set; }

        public bool CompletedSuccesfully { get; set; }


        public void Start(ILogger logger)
        {
            Started = true;
            if (_firstRunControls.Count == 0)
            {
                CompletedSuccesfully = true;
                return;
            }
            _setupFrm = new FirstRunFrm(_firstRunControls);
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
    }
}