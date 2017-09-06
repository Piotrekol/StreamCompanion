using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Interfaces;

namespace osu_StreamCompanion.Code.Modules.Donation
{
    class Donation:IModule,ISettingsProvider
    {
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
        }

        public string SettingGroup { get; } = "General";
        public void SetSettingsHandle(Settings settings)
        {
        }

        public void Free()
        {
            donationSettings.Dispose();
        }

        private DonationSettings donationSettings;
        public UserControl GetUiSettings()
        {
            if (donationSettings == null || donationSettings.IsDisposed)
            {
                donationSettings = new DonationSettings();
            }
            return donationSettings;
        }
    }
}
