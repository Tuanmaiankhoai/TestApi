using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.Converters.LikePost;
using BaiTestPost.Payload.DataRequests.LikePost;
using BaiTestPost.Payload.DataResponses.LikePost;
using BaiTestPost.Payload.DataResponses.PostCollection;
using BaiTestPost.Payload.Responses;
using BaiTestPost.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace BaiTestPost.Services.Implement
{
    public class LikePostService : IUserLikePostService
    {
        private readonly AppDbContext _dbContext;
        private readonly ResponseObject<DataLikePost> _response;
        private readonly LikePostConverter _likepostConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LikePostService(AppDbContext dbContext, ResponseObject<DataLikePost> response, LikePostConverter postConverter, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _response = response;
            _likepostConverter = postConverter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataLikePost>> CreateLikeOrUnlikePost(Request_LikePost request)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataLikePost>
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
                    return new ResponseObject<DataLikePost>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị Ban!",
                        Data = null
                    };
                }
                if (userlogin.IsLocked == true)
                {
                    return new ResponseObject<DataLikePost>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                        Data = null
                    };
                }
                
                var post = await _dbContext.posts.SingleOrDefaultAsync(x => x.Id == request.PostId);
                if (post == null)
                {
                    return _response.responseError(StatusCodes.Status400BadRequest, "Post Không tồn tại", null);
                }
                var userLikePost = _dbContext.userLikePosts.FirstOrDefault(x => x.UserId == request.UserId && x.PostId == request.PostId);
                if (userLikePost == null)
                {
                    var likePost = new UserLikePost
                    {
                        UserId = idUser,
                        PostId = request.PostId,
                        LikeTime = DateTime.Now,
                        Unlike = false

                    };
                    await _dbContext.userLikePosts.AddAsync(likePost);
                    await _dbContext.SaveChangesAsync();
                    post.NumberOfLikes += 1;
                    await _dbContext.SaveChangesAsync();
                     return _response.ResponseAccess("Like Thành công!", _likepostConverter.EntityDTO(likePost));
                }
                else
                {
                    if (userLikePost.Unlike == false)
                    {
                        userLikePost.Unlike = true;
                        await _dbContext.SaveChangesAsync();
                        post.NumberOfLikes -= 1;
                        await _dbContext.SaveChangesAsync();
                        return _response.ResponseAccess("UnLike Thành công!", _likepostConverter.EntityDTO(userLikePost));
                    }
                    userLikePost.Unlike = false;
                    await _dbContext.SaveChangesAsync();
                    post.NumberOfLikes += 1;
                    await _dbContext.SaveChangesAsync();
                    return _response.ResponseAccess("Like Thành công!", _likepostConverter.EntityDTO(userLikePost));
                }
                
               
            }
            catch(Exception ex)
            {
                return _response.responseError(StatusCodes.Status400BadRequest, "Lỗi trong quá trình like!"+ex.Message, null);
            }
        }

        public async Task<PageResult<DataLikePost>> GetAll(int pageNumber, int pageSize)
        {
            
            var query = _dbContext.userLikePosts.Where(x => x.Unlike == false).ToList();
            var queryResult = query.Select(x => _likepostConverter.EntityDTO(x)).AsQueryable();
            var result = Pagination.GetPagedData(queryResult, pageNumber, pageSize);
            return result;

        }

    }
}
