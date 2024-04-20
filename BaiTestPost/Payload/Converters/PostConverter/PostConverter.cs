using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataResponses.Post;
using BaiTestPost.Payload.DataResponses.User;

namespace BaiTestPost.Payload.Converters.PostConverter
{
    public class PostConverter
    {
        private readonly AppDbContext _context;
        public PostConverter(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        public DataResponse_Post EntityDTO(Post post)
        {
            return new DataResponse_Post
            {
                ImageUrl=post.ImageUrl,
                Title=post.Title,
                Description=post.Description,
                UserName= _context.users.SingleOrDefault(x=>x.Id==post.UserId).FullName,
                CreateAt =post.CreateAt,
                UpdateAt=post.UpdateAt,
                RemoveAt=post.RemoveAt,
                NumberOfComments=post.NumberOfComments,
                NumberOfLikes=post.NumberOfLikes,
                PostStatusName= _context.postStatuses.SingleOrDefault(x=>x.Id== post.PostStatusId).Name,
            };
        }

    }
}
