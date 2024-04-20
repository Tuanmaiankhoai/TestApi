using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataResponses.ReportType;

namespace BaiTestPost.Payload.Converters.ReportTypeConverter
{
    public class ReportTypeConverter
    {
        private readonly AppDbContext _context;
        public ReportTypeConverter(AppDbContext context)
        {
            _context = context;
        }
        public Data_ReportType ReportToDTO(Report report)
        {
            return new Data_ReportType
            {
                ReportId = report.Id,
                PostId = report.PostId,
                TitlePost = _context.posts.SingleOrDefault(x => x.Id == report.PostId).Title,
                ReporterName = _context.users.SingleOrDefault(y => y.Id == report.UserReportedId).FullName,
                ReportedUserName = _context.users.SingleOrDefault(c => c.Id == report.UserReportId).FullName,
                ReportType = (Enum.ReportTypeEnum)report.ReportType,
                ReportingReason = report.ReportingReason,
                CreateAt = report.CreateAt
            };
        }
    }
}
