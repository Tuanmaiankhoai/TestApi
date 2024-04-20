using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataResponses.User;

namespace BaiTestPost.Payload.Converters.UserConverter
{
    public class UserConverter
    {
        private readonly AppDbContext _context;
        public UserConverter(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        public DataResponse_User EntityDTO(User user)
        {
            return new DataResponse_User
            {
                Username = user.Username,
                RoleName = _context.roles.SingleOrDefault(x => x.Id == user.RoleId).Name,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                Avatar = user.Avatar,
                StatusUser = _context.userStatuses.SingleOrDefault(x => x.Id == user.UserStatusId).Name
            };
        }

    }
}
