using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.DataRequests.LikePost;
using BaiTestPost.Payload.DataResponses.LikePost;
using BaiTestPost.Payload.DataResponses.Post;
using BaiTestPost.Payload.Responses;

namespace BaiTestPost.Services.Interface
{
    public interface IUserLikePostService
    {
        Task<ResponseObject<DataLikePost>> CreateLikeOrUnlikePost(Request_LikePost request);
        Task<PageResult<DataLikePost>> GetAll(int pageNumber, int pageSize);
    }
}
