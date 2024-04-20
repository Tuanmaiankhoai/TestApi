namespace BaiTestPost.Entities
{
    public class Post:BaseId
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int UserId { get; set; } // Foreign Key
        public User User { get; set; } // Navigation Property
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public int? PostStatusId { get; set; } // Foreign Key
        public PostStatus? PostStatus { get; set; } // Navigation Property
        public bool? IsDeleted { get; set; } = false;
        public DateTime? RemoveAt { get; set; }
        public bool? IsActive { get; set; } = true;
       
    }
}
