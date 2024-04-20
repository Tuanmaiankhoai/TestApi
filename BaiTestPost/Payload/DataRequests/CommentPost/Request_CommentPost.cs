namespace BaiTestPost.Payload.DataRequests.CommentPost
{
    public class Request_CommentPost
    {
        public int UserId { get; set; } // Foreign Key
        public int PostId { get; set; } // Foreign Key        
        public string Content { get; set; }

    }
}

