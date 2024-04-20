namespace BaiTestPost.Entities
{
    public class RefreshToken:BaseId
    {
        public string Token { get; set; }
        public DateTime ExpiredTime { get; set; }
        public int UserId { get; set; } // Foreign Key
        public  User User { get; set; } // Navigation Property

    }
}
