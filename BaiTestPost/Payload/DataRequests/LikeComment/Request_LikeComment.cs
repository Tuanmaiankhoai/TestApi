using BaiTestPost.Entities;

namespace BaiTestPost.Payload.DataRequests.LikeComment
{
    public class Request_LikeComment
    {
        public int UserId { get; set; } 
        public int UserCommentPostId { get; set; } 

    }
}
