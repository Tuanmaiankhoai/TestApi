using BaiTestPost.Entities;

namespace BaiTestPost.Payload.DataResponses.Post
{
    public class DataResponse_Post
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? RemoveAt { get; set; }
        public string UserName{ get; set; } // Foreign Key
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public string PostStatusName { get; set; } // Foreign Key
        
    }
}
