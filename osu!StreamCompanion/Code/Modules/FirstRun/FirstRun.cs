using System;
using System.Collections.Generic;
using System.Linq;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public class FirstRun : IModule
    {
        private ISettingsHandler _settings;
        private FirstRunFrm _setupFrm;
        public ILogger _logger { get; set; }
        private List<IFirstRunControlProvider> _firstRunControlProviders;
        public FirstRun(ILogger logger, ISettingsHandler settings, IEnumerable<IFirstRunControlProvider> firstRunControlProviders)
        {
            _logger = logger;
            _settings = settings;
            _firstRunControlProviders = firstRunControlProviders.ToList();
            _logger.Log(">loaded {0} plugins for firstRun setup", LogLevel.Advanced, _firstRunControlProviders.Count.ToString());
            Start(_logger);
        }

        public bool Started { get; set; }

        public bool CompletedSuccesfully { get; set; }

        private bool ShouldRun()
        {

            bool shouldForceFirstRun;
            var lastVersionStr = _settings.Get<string>(SettingNames.Instance.LastRunVersion);
            if (lastVersionStr == SettingNames.Instance.LastRunVersion.Default<string>())
            {
                shouldForceFirstRun = true;
            }
            else
            {
                try
                {
                    var lastVersion = Helpers.Helpers.GetDateFromVersionString(lastVersionStr);
                    var versionToResetOn = Helpers.Helpers.GetDateFromVersionString("v180209.13");
                    shouldForceFirstRun = lastVersion < versionToResetOn;
                }
                catch (Exception e)
                {
                    if (e is FormatException || e is ArgumentNullException)
                        shouldForceFirstRun = true;
                    else
                        throw;
                }
            }

            return _firstRunControlProviders.Count != 0 && (shouldForceFirstRun || _settings.Get<bool>(SettingNames.Instance.FirstRun));
        }
        private IEnumerable<FirstRunUserControl> GetControls()
        {
            foreach (var controlProvider in _firstRunControlProviders)
            {
                foreach (var control in controlProvider.GetFirstRunUserControls())
                {
                    yield return control;
                }
            }
        }

        public void Start(ILogger logger)
        {
            _logger = logger;
            Started = true;

            if (!ShouldRun())
            {
                if (_firstRunControlProviders.Count == 0)
                {
                    _logger.Log(">Did not find any first run controls!", LogLevel.Advanced);
                }

                CompletedSuccesfully = true;
                return;
            }
            _setupFrm = new FirstRunFrm(GetControls().ToList());
            _setupFrm.Closing += _setupFrm_Closing;
            _setupFrm.StartSetup();

            if (!CompletedSuccesfully)
                Program.SafeQuit();
        }

        private void _setupFrm_Closing(object sender, EventArgs e)
        {
            CompletedSuccesfully = _setupFrm.CompletedSuccesfully;
        }
    }
}