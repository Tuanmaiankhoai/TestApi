namespace BaiTestPost.Payload.DataResponses.CommentPost
{
    public class Data_CommentPostCreate
    {
        public string PostName { get; set; } // Foreign Key
        public string UserName { get; set; } // Foreign Key
        public string Content { get; set; }
        public int NumberOfLikes { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
