namespace BaiTestPost.Entities
{
    public class Relationship:BaseId
    {
        public int FollowerId { get; set; } // Foreign Key
        public  User Follower { get; set; } // Navigation Property
        public int FollowingId { get; set; } // Foreign Key
        public  User Following { get; set; } // Navigation Property
    }
}
