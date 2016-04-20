using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class FileFormating
    {
        [DisplayName("Name")]
        public string Filename { get; set; }
        [DisplayName("Pattern")]
        public string Pattern { get; set; }
        [Editable(false)]
        [DisplayName("Event")]
        public string SaveEventStr
        {
            get
            {
                switch (SaveEvent)
                {
                    case 1:
                        return "Listening";
                    case 2:
                        return "Playing";
                    case 8:
                        return "Watching";
                    case 16:
                        return "Editing";
                    case 27:
                        return "All";
                    default:
                        return "NEVER";
                }
            }
            set { }
        }
        [Browsable(false)]
        public int SaveEvent { get; set; }
    }
}