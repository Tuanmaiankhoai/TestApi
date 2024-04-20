using BaiTestPost.Enum;
using BaiTestPost.Enum.Repost;

namespace BaiTestPost.Payload.DataRequests.ReportType
{
    public class Request_ReportType
    {
        public int PostId { get; set; }
        public ReportTypeEnum ReportType { get; set; } //Kiểu báo cáo
        public string ReportingReason { get; set; } //Lý do báo cáo
    }
}
