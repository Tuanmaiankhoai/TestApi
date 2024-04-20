using BaiTestPost.Entities;

namespace BaiTestPost.Payload.DataResponses.LikeComment
{
    public class Data_LikeComment
    {
        public string UserName { get; set; } 
        public string UserCommentContent { get; set; } 
        public DateTime LikeTime { get; set; }
    }
}
