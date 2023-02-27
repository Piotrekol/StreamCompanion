namespace Overlay.Common.Loader
{
    public enum DllInjectionResult
    {
        Success = 0,
        DllNotFound = 10,
        GameProcessNotFound = 11,
        InjectionFailed = 12,
        Timeout = 13,
        Cancelled = 14,
        HelperProcessFailed = 15,
    }
}