namespace osuOverlayLoader
{
    public class InjectionResult
    {
        public DllInjectionResult ResultCode { get; }
        public int ErrorCode { get; }
        public int Win32ErrorCode { get; }
        public string Result { get; }

        public InjectionResult(DllInjectionResult resultCode, int ErrorCode, int Win32ErrorCode, string Result)
        {
            this.ResultCode = resultCode;
            this.ErrorCode = ErrorCode;
            this.Win32ErrorCode = Win32ErrorCode;
            this.Result = Result;
        }

        public override string ToString() => $"{ResultCode},{ErrorCode},{Win32ErrorCode},{Result}";
    }
}