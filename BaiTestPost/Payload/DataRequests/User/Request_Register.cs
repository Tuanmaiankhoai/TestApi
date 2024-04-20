using BaiTestPost.Entities;
using System.ComponentModel.DataAnnotations;

namespace BaiTestPost.Payload.DataRequests.User
{
    public class Request_Register
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile Avatar { get; set; }


    }
}
