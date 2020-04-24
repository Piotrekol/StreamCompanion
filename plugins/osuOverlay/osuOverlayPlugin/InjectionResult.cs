namespace osuOverlay
{
    internal enum InjectionResult
    {
        Success = 0,
        DllNotFound = 10,
        GameProcessNotFound = 11,
        InjectionFailed = 12,
        Timeout = 13,
    }
}