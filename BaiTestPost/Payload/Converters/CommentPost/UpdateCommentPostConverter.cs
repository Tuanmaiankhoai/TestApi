using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataResponses.CommentPost;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BaiTestPost.Payload.Converters.CommentPost
{
    public class UpdateCommentPostConverter
    {
        private readonly AppDbContext _dbContext;
        public UpdateCommentPostConverter(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Data_CommentPostUpdate EntityDTO(UserCommentPost request)
        {
            return new Data_CommentPostUpdate
            {
                PostName = _dbContext.posts.SingleOrDefault(x => x.Id == request.PostId).Title,
                UserName = _dbContext.users.SingleOrDefault(x => x.Id == request.UserId).Username,
                Content = request.Content,
                NumberOfLikes = request.NumberOfLikes,
                UpdateAt =request.UpdateAt,

            };
        }
    }
}
