using System.Numerics;

namespace BaiTestPost.Entities
{
    public class User:BaseId
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; } // Foreign Key
        public Role? Role { get; set; } // Navigation Property
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public bool? IsLocked { get; set; }
        public int? UserStatusId { get; set; } // Foreign Key
        public UserStatus? UserStatus { get; set; } // Navigation Property
        public bool? IsActive { get; set; } = true;

    }
}
