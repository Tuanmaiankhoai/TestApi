using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataRequests.CommentPost;
using BaiTestPost.Payload.DataResponses.CommentPost;
using BaiTestPost.Payload.Responses;


namespace BaiTestPost.Payload.Converters.CommentPost
{
    public class CommentPostConverter
    {
        private readonly AppDbContext _dbContext;
        public CommentPostConverter(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Data_CommentPostCreate EntityDTO(UserCommentPost request)
        {
            return new Data_CommentPostCreate
            {
                PostName = _dbContext.posts.SingleOrDefault(x => x.Id == request.PostId).Title,
                UserName = _dbContext.users.SingleOrDefault(x => x.Id == request.UserId).Username,
                Content = request.Content,
                NumberOfLikes = request.NumberOfLikes,
                CreateAt = request.CreateAt,

            };
        }
    }
}
