namespace BaiTestPost.Entities
{
    public class ConfirmEmail:BaseId
    {
        public int UserId { get; set; } // Foreign Key
        public User User { get; set; } // Navigation Property
        public DateTime ExpiredTime { get; set; }
        public string ConfirmCode { get; set; }
        public bool Confirmed { get; set; }
    }
}
