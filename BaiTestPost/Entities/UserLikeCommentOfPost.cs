namespace BaiTestPost.Entities
{
    public class UserLikeCommentOfPost:BaseId
    {
        public int UserId { get; set; } // Foreign Key
        public  User User { get; set; } // Navigation Property
        public int UserCommentPostId { get; set; } // Foreign Key
        public  UserCommentPost UserCommentPost { get; set; } // Navigation Property
        public DateTime LikeTime { get; set; }
        public bool Unlike { get; set; } = false;
    }
}
