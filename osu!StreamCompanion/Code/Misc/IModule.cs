using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Misc
{
    internal interface IModule
    {
        bool Started { get; set; }
        void Start(ILogger logger);
    }
}