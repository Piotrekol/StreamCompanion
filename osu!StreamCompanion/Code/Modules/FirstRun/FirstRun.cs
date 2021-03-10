using System;
using System.Collections.Generic;
using System.Linq;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public sealed class FirstRun : IModule, IDisposable
    {
        private ISettings _settings;
        private FirstRunFrm _setupFrm;
        public ILogger _logger { get; set; }
        private Lazy<List<IFirstRunControlProvider>> _firstRunControlProviders;
        public FirstRun(ILogger logger, ISettings settings, Lazy<List<IFirstRunControlProvider>> firstRunControlProviders)
        {
            _logger = logger;
            _settings = settings;
            _firstRunControlProviders = firstRunControlProviders;
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

            return (shouldForceFirstRun || _settings.Get<bool>(SettingNames.Instance.FirstRun)) && _firstRunControlProviders.Value.Count != 0;
        }
        private IEnumerable<IFirstRunControl> GetControls()
        {
            foreach (var controlProvider in _firstRunControlProviders.Value)
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

        public void Dispose()
        {
            _setupFrm?.Dispose();
        }
    }
}