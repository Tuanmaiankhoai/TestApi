using Azure.Core;
using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.Converters.CommentPost;
using BaiTestPost.Payload.DataRequests.CommentPost;
using BaiTestPost.Payload.DataResponses.CommentPost;
using BaiTestPost.Payload.DataResponses.LikeComment;
using BaiTestPost.Payload.Responses;
using BaiTestPost.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace BaiTestPost.Services.Implement
{
    public class CommentPostService : IUserCommentPost
    {
        private readonly AppDbContext _dbContext;
        private readonly ResponseObject<Data_CommentPostCreate> _responseCreate;
        private readonly ResponseObject<Data_CommentPostUpdate> _responseUpdate;
        private readonly CommentPostConverter _commentPostConverter;
        private readonly UpdateCommentPostConverter _updateCommentPost;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentPostService(AppDbContext dbContext, ResponseObject<Data_CommentPostCreate> response,
            ResponseObject<Data_CommentPostUpdate> responseObject, CommentPostConverter commentPostConverter,
            UpdateCommentPostConverter updateCommentPost, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _responseCreate = response;
            _responseUpdate = responseObject;
            _commentPostConverter = commentPostConverter;
            _updateCommentPost = updateCommentPost;
            _httpContextAccessor = httpContextAccessor;
        }
        #region//Thêm commnet
        public async Task<ResponseObject<Data_CommentPostCreate>> CreateComment(Request_CommentPost request)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<Data_CommentPostCreate>
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
                    return new ResponseObject<Data_CommentPostCreate>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị Ban!",
                        Data = null
                    };
                }
                if (userlogin.IsLocked == true)
                {
                    return new ResponseObject<Data_CommentPostCreate>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                        Data = null
                    };
                }

                var post = await _dbContext.posts.SingleOrDefaultAsync(x => x.Id == request.PostId);
                if (post == null)
                {
                    return _responseCreate.responseError(StatusCodes.Status400BadRequest, "Post Không tồn tại ", null);
                }
                var comment = new UserCommentPost
                {
                    PostId = request.PostId,
                    UserId = idUser,
                    Content = request.Content,
                    NumberOfLikes = 0,
                    IsActive = true,
                    CreateAt = DateTime.Now,
                };
                await _dbContext.userCommentPosts.AddAsync(comment);
                await _dbContext.SaveChangesAsync();
                post.NumberOfComments += 1;
                _dbContext.posts.Update(post);
                await _dbContext.SaveChangesAsync();
                return _responseCreate.ResponseAccess("Thêm comment thành công", _commentPostConverter.EntityDTO(comment));
            }
            catch(Exception ex)
            {
                return _responseCreate.responseError(StatusCodes.Status400BadRequest, "Lỗi trong quá trình thêm", null);
            }
        }

        #endregion

        #region//Update Commnet
        public async Task<ResponseObject<Data_CommentPostUpdate>> UpdateComment(int commentId, Request_UpdateComment request)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<Data_CommentPostUpdate>
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
                    return new ResponseObject<Data_CommentPostUpdate>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị Ban!",
                        Data = null
                    };
                }
                if (userlogin.IsLocked == true)
                {
                    return new ResponseObject<Data_CommentPostUpdate>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                        Data = null
                    };
                }

                var comment = await _dbContext.userCommentPosts.SingleOrDefaultAsync(x => x.Id == commentId);
                if (comment == null || comment.IsActive==false)
                {
                    return _responseUpdate.responseError(StatusCodes.Status400BadRequest, "Comment Không tồn tại ", null);
                }
                comment.Content = request.Content;
                comment.UpdateAt = DateTime.Now;
                _dbContext.userCommentPosts.Update(comment);
                await _dbContext.SaveChangesAsync();
                return _responseUpdate.ResponseAccess("Sửa comment thành công", _updateCommentPost.EntityDTO(comment));
            }
            catch (Exception ex)
            {
                return _responseUpdate.responseError(StatusCodes.Status400BadRequest, "Lỗi trong quá trình thêm", null);
            }
        }
        #endregion

        #region//Xóa Comment
        public async Task<string> RemoveComment(int postId, int IdComment )
        {
            try
            {
                
                var comment = await _dbContext.userCommentPosts.SingleOrDefaultAsync(x => x.Id == IdComment);
                if (comment == null)
                {
                    return null;
                }
                comment.IsActive = false;
                comment.RemoveAt = DateTime.Now;
                _dbContext.userCommentPosts.Update(comment);
                await _dbContext.SaveChangesAsync();

                var post = await _dbContext.posts.SingleOrDefaultAsync(x => x.Id == postId);
                if(postId !=null)
                {
                    if(comment.IsActive== false)
                    {
                        post.NumberOfComments -= 1;
                        await _dbContext.SaveChangesAsync();
                    }    
                }    
                return "Đã xóa comment thành công";
            }
            catch (Exception ex)
            {
                return "Lỗi trong quá trình xóa";
            }
        }
        #endregion
       /* public async Task<PageResult<Data_CommentPostCreate>> GetAllComment(int PostId, int pageNumber, int pageSize)
        {
            var query = _dbContext.userCommentPosts.Where(x=>x.PostId==PostId && x.IsActive==true).ToList();
            var queryVer = query.Select(x => _commentPostConverter.EntityDTO(x)).AsQueryable();
            var result = Pagination.GetPagedData(queryVer, pageNumber, pageSize);
            return result;
        }*/
    }
}
