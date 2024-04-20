using BaiTestPost.Enum;

namespace BaiTestPost.Payload.DataResponses.ReportType
{
    public class Data_ReportType
    {
        public int ReportId { get; set; }
        public int PostId { get; set; }
        public string TitlePost { get; set; }
        public string ReporterName { get; set; }
        public string ReportedUserName { get; set; }
        public ReportTypeEnum ReportType { get; set; }
        public string ReportingReason { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
