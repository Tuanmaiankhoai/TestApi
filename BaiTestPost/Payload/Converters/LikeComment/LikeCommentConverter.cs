using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataResponses.LikeComment;

namespace BaiTestPost.Payload.Converters.LikeComment
{
    public class LikeCommentConverter
    {
        private readonly AppDbContext _dbContext;
        public LikeCommentConverter(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Data_LikeComment EntityDTO(UserLikeCommentOfPost request)
        {
            return new Data_LikeComment
            {
                UserName = _dbContext.users.SingleOrDefault(x => x.Id == request.UserId).Username,
                LikeTime = request.LikeTime,
                UserCommentContent = _dbContext.userCommentPosts.SingleOrDefault(x => x.Id == request.UserCommentPostId).Content,
            };
        }
    }
}
