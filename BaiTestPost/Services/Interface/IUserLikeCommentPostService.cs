using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.DataRequests.LikeComment;
using BaiTestPost.Payload.DataResponses.CommentPost;
using BaiTestPost.Payload.DataResponses.LikeComment;
using BaiTestPost.Payload.Responses;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaiTestPost.Services.Interface
{
    public interface IUserLikeCommentPostService
    {
        Task<ResponseObject<Data_LikeComment>> CreateLikeOrUnlikeComment(Request_LikeComment request);
        Task<PageResult<Data_LikeComment>> GetAllLikeComment(int commentId, int pageNumber, int pageSize);
    }
}
