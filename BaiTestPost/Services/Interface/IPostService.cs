using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.DataRequests.Post;
using BaiTestPost.Payload.DataRequests.ReportType;
using BaiTestPost.Payload.DataResponses.Post;
using BaiTestPost.Payload.DataResponses.ReportType;
using BaiTestPost.Payload.Responses;

namespace BaiTestPost.Services.Interface
{
    public interface IPostService
    {
        Task<ResponseObject<DataResponse_Post>> CreatePost(Request_CreatePost request);
        Task<ResponseObject<DataResponse_Post>> UpdatePost(Request_UpdatePost request);
        Task<string> DeletePost(int PostId);
        Task<PageResult<DataResponse_Post>> GetAll(int pageNumber, int pageSize);
        ResponseObject<Data_ReportType> ReportPost(Request_ReportType request);
        ResponseObject<IEnumerable<Data_ReportType>> GetAllReport();
    }
}
