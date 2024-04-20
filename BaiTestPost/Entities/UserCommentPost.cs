namespace BaiTestPost.Entities
{
    public class UserCommentPost:BaseId
    {
        public int PostId { get; set; } // Foreign Key
        public  Post Post { get; set; } // Navigation Property
        public int UserId { get; set; } // Foreign Key
        public  User User { get; set; } // Navigation Property
        public string Content { get; set; }
        public int NumberOfLikes { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime RemoveAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

    }
}
