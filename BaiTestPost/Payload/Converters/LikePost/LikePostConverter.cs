using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataRequests.LikePost;
using BaiTestPost.Payload.DataResponses.LikePost;

namespace BaiTestPost.Payload.Converters.LikePost
{
    public class LikePostConverter
    {
        private readonly AppDbContext _dbContext;
        public LikePostConverter(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public DataLikePost EntityDTO(UserLikePost request)
        {
            return new DataLikePost
            {
                UserName= _dbContext.users.SingleOrDefault(x=>x.Id== request.UserId).Username,
                PostName=_dbContext.posts.SingleOrDefault(x=>x.Id== request.PostId).Title,
                LikeTime=request.LikeTime,
            };
        }
    }
}
