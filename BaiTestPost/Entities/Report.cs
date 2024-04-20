using BaiTestPost.Enum.Repost;

namespace BaiTestPost.Entities
{
    public class Report:BaseId
    {
        public int PostId { get; set; } // Foreign Key
        public  Post Post { get; set; } // Navigation Property
        public int UserReportId { get; set; } // Foreign Key
        public User UserReport { get; set; } // Navigation Property
        public int UserReportedId { get; set; } // Foreign Key
        public  User UserReported { get; set; } // Navigation Property
        public  ReportType ReportType { get; set; }
        public string ReportingReason { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
