using System.ComponentModel.DataAnnotations;

namespace BaiTestPost.Enum.Email
{
    public class Validate
    {
        public static bool IsValidEmail(string email)
        {
            var checkEmail = new EmailAddressAttribute();
            return checkEmail.IsValid(email);
        }
    }
}
