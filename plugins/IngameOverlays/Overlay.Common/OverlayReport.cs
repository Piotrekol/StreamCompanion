namespace Overlay.Common
{
    public class OverlayReport
    {
        public ReportType ReportType;
        public string Message;

        public OverlayReport(ReportType reportType, string message)
        {
            ReportType = reportType;
            Message = message;
        }
    }
}
