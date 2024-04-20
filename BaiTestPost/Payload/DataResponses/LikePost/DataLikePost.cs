namespace BaiTestPost.Payload.DataResponses.LikePost
{
    public class DataLikePost
    {
        public string UserName { get; set; } // Foreign Key
        public string PostName { get; set; } // Foreign Key
        public DateTime LikeTime { get; set; }

    }
}
