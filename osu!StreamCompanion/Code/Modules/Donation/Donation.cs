using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.Donation
{
    class Donation : IModule, ISettingsProvider
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
        public void SetSettingsHandle(ISettingsHandler settings)
        {
        }

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
