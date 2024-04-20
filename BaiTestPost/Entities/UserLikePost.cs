using System.Data;

namespace BaiTestPost.Entities
{
    public class UserLikePost:BaseId
    {
        public int UserId { get; set; } // Foreign Key
        public  User User { get; set; } // Navigation Property
        public int PostId { get; set; } // Foreign Key
        public  Post Post { get; set; } // Navigation Property
        public DateTime LikeTime { get; set; }
        public bool Unlike { get; set; } = false;
    }
}
