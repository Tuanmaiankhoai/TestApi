using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.DataRequests.CommentPost;
using BaiTestPost.Payload.DataResponses.CommentPost;
using BaiTestPost.Payload.Responses;

namespace BaiTestPost.Services.Interface
{
    public interface IUserCommentPost
    {
        Task<ResponseObject<Data_CommentPostCreate>> CreateComment(Request_CommentPost request);
        Task<ResponseObject<Data_CommentPostUpdate>> UpdateComment(int commentId, Request_UpdateComment request);
        Task<string> RemoveComment(int postId,int IdComment);
       // Task<PageResult<Data_CommentPostCreate>> GetAllComment(int PostId, int pageNumber, int pageSize);
    }
}
