using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.Converters.LikeComment;
using BaiTestPost.Payload.DataRequests.LikeComment;
using BaiTestPost.Payload.DataResponses.LikeComment;
using BaiTestPost.Payload.DataResponses.LikePost;
using BaiTestPost.Payload.Responses;
using BaiTestPost.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BaiTestPost.Services.Implement
{
    public class LikeCommentService : IUserLikeCommentPostService
    {
        private readonly AppDbContext _dbContext;
        private readonly ResponseObject<Data_LikeComment> _responseLike;
        private readonly LikeCommentConverter _likeCommentConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LikeCommentService(AppDbContext dbContext, ResponseObject<Data_LikeComment> responseObject, LikeCommentConverter likeCommentConverter, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _responseLike = responseObject;
            _likeCommentConverter = likeCommentConverter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<Data_LikeComment>> CreateLikeOrUnlikeComment(Request_LikeComment request)
        {

            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_LikeComment>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực!",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var userlogin = _dbContext.users.FirstOrDefault(x => x.Id == idUser);
            if (userlogin.IsActive == false)
            {
                return new ResponseObject<Data_LikeComment>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị Ban!",
                    Data = null
                };
            }
            if (userlogin.IsLocked == true)
            {
                return new ResponseObject<Data_LikeComment>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                    Data = null
                };
            }

           var commentId = await _dbContext.userCommentPosts.SingleOrDefaultAsync(x => x.Id == request.UserCommentPostId);
            if (commentId == null || commentId.IsActive == false)
            {
                return _responseLike.responseError(StatusCodes.Status400BadRequest, "Comment không tồn tại", null);
            }
            var likeComment = await _dbContext.userLikeCommentOfPosts.SingleOrDefaultAsync(x => x.UserCommentPostId== request.UserCommentPostId && x.UserId==request.UserId);
            if (likeComment == null)
            {
                var like = new UserLikeCommentOfPost
                {
                    UserId = idUser,
                    UserCommentPostId = request.UserCommentPostId,
                    LikeTime = DateTime.Now,
                    Unlike = false
                };
                await _dbContext.userLikeCommentOfPosts.AddAsync(like);
                await _dbContext.SaveChangesAsync();
                commentId.NumberOfLikes += 1;
                _dbContext.userCommentPosts.Update(commentId);
                await _dbContext.SaveChangesAsync();
                return _responseLike.ResponseAccess("Like comment thành công! ", _likeCommentConverter.EntityDTO(like));
            }
            else
            {
                if (likeComment.Unlike == false)
                {
                    likeComment.Unlike = true;
                    await _dbContext.SaveChangesAsync();
                    commentId.NumberOfLikes -= 1;
                    _dbContext.userCommentPosts.Update(commentId);
                    await _dbContext.SaveChangesAsync();
                    return _responseLike.ResponseAccess("Unlike comment thành công!", _likeCommentConverter.EntityDTO(likeComment));
                }
                likeComment.Unlike = false;
                await _dbContext.SaveChangesAsync();
                commentId.NumberOfLikes += 1;
                _dbContext.userCommentPosts.Update(commentId);
                await _dbContext.SaveChangesAsync();
                return _responseLike.ResponseAccess("like comment thành công!", _likeCommentConverter.EntityDTO(likeComment));
            }

        }

        public Task<PageResult<Data_LikeComment>> GetAllLikeComment(int commentId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
