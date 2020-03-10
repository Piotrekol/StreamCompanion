using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Modules.Donation
{
    class Donation : IModule, ISettingsSource
    {
        public bool Started { get; set; }

        public Donation(ILogger logger)
        {
            Start(logger);
        }

        public void Start(ILogger logger)
        {
            Started = true;
        }

        public string SettingGroup { get; } = "General";

        public void Free()
        {
            donationSettings.Dispose();
        }

        private DonationSettings donationSettings;
        public object GetUiSettings()
        {
            if (donationSettings == null || donationSettings.IsDisposed)
            {
                donationSettings = new DonationSettings();
            }
            return donationSettings;
        }
    }
}
