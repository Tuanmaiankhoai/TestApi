namespace BaiTestPost.Payload.DataResponses.CommentPost
{
    public class Data_CommentPostUpdate
    {
        public string PostName { get; set; } // Foreign Key
        public string UserName { get; set; } // Foreign Key
        public string Content { get; set; }
        public int NumberOfLikes { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
