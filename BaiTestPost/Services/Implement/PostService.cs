using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Handler.Image;
using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.Converters.PostConverter;
using BaiTestPost.Payload.Converters.ReportTypeConverter;
using BaiTestPost.Payload.Converters.UserConverter;
using BaiTestPost.Payload.DataRequests.Post;
using BaiTestPost.Payload.DataRequests.ReportType;
using BaiTestPost.Payload.DataResponses.Post;
using BaiTestPost.Payload.DataResponses.ReportType;
using BaiTestPost.Payload.Responses;
using BaiTestPost.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BaiTestPost.Services.Implement
{
    public class PostService : IPostService
    {

        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponse_Post> _responseObject;
        private readonly PostConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ReportTypeConverter _typeConverter;
        public PostService(AppDbContext dbContext, ResponseObject<DataResponse_Post> responseObject,PostConverter converter, 
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, ReportTypeConverter typeConverter)
        {
            _context = dbContext;
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _typeConverter = typeConverter;
        }
        //Thêm ảnh image
        private string UploadImageAsync(IFormFile imageFile)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadFiles", "ImagePosts");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(uploadPath, imageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.CopyToAsync(stream);
            }

            return imageName;
        }
        #region //Thêm 1 bài Post
        public async Task<ResponseObject<DataResponse_Post>> CreatePost(Request_CreatePost request)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<DataResponse_Post>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực!",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var userlogin = _context.users.FirstOrDefault(x => x.Id == idUser);
            if (userlogin.IsActive == false)
            {
                return new ResponseObject<DataResponse_Post>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị Ban!",
                    Data = null
                };
            }
            if (userlogin.IsLocked == true)
            {
                return new ResponseObject<DataResponse_Post>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                    Data = null
                };
            }
            if (string.IsNullOrEmpty(request.Title)
               || string.IsNullOrEmpty(request.Description)
               || request.ImageUrl == null )
            {
                return new ResponseObject<DataResponse_Post>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Vui lòng điền đầy đủ thông tin!",
                    Data = null
                };
            }
            string urlImage = UploadImageAsync(request.ImageUrl);
            Post post = new Post
            {
                ImageUrl =  urlImage,
                Title = request.Title,
                Description = request.Description,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                UserId = idUser,
                NumberOfLikes = 0,
                NumberOfComments = 0,
                PostStatusId = 2,
                IsDeleted = false,
                //RemoveAt = null,
                IsActive = true
            };
            _context.posts.Add(post);
            _context.SaveChanges();
            return new ResponseObject<DataResponse_Post>
            {
                Status = StatusCodes.Status200OK,
                Message = "Đăng bài thành công!",
                Data = _converter.EntityDTO(post)
            };
        }
        #endregion
        
        #region //Sửa bài Post
        public async Task<ResponseObject<DataResponse_Post>> UpdatePost(Request_UpdatePost request)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponse_Post>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người dùng chưa xác thực!",
                        Data = null
                    };
                }
                var claim = currentUser.FindFirst("ID");
                var idUser = int.Parse(claim.Value);
                var userlogin = _context.users.FirstOrDefault(x => x.Id == idUser);
                if (userlogin.IsActive==false)
                {
                    return new ResponseObject<DataResponse_Post>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị Ban!",
                        Data = null
                    };
                }
                if (userlogin.IsLocked==true)
                {
                    return new ResponseObject<DataResponse_Post>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                        Data = null
                    };
                }
                var post = await _context.posts.SingleOrDefaultAsync(x => x.Id == request.PostId);
                if (post == null)
                {
                    return _responseObject.responseError(StatusCodes.Status400BadRequest, "Không tìm thấy bài đăng",null);
                }
                var count = _context.userLikePosts.Count(x => x.Unlike == false && x.PostId==request.PostId);
                var countComment = _context.userCommentPosts.Count(x => x.IsActive == true && x.PostId==request.PostId);

                string urlImage = UploadImageAsync(request.ImageUrl);
                // Cập nhật thông tin bài đăng dựa trên request
                post.ImageUrl = urlImage;
                post.Title = request.Title;
                post.Description = request.Description;
                post.UpdateAt = DateTime.Now;
                post.NumberOfLikes = count;
                post.NumberOfComments = countComment;
                _context.posts.Update(post);
                await _context.SaveChangesAsync();

                return _responseObject.ResponseAccess("Cập nhật bài đăng thành công", _converter.EntityDTO(post));
            }
            catch (Exception ex)
            {
                return _responseObject.responseError(StatusCodes.Status400BadRequest,  "Đã xảy ra lỗi khi cập nhật bài đăng: ",null);
            }
        }
        #endregion

        #region //Xóa bài Post
        public async Task<string> DeletePost(int PostId)
        {
            try
            {

                var post = await _context.posts.SingleOrDefaultAsync(x => x.Id == PostId);
                if(post == null)
                {
                    return "Post không tồn tại";
                }
                post.IsActive = false;

               // _context.posts.Remove(post);
                post.RemoveAt = DateTime.Now;
                _context.posts.Update(post);
                await _context.SaveChangesAsync();
                return "Xóa Post thành công";
            }
            catch (Exception ex)
            {
                return "Đã xảy ra lỗi khi xóa bài đăng ";
            }

        }
        #endregion

        #region//Lấy danh sách các bài Post
        public async Task<PageResult<DataResponse_Post>> GetAll(int pageNumber, int pageSize)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return null;
            }
            if (currentUser.IsInRole("Admin"))
            {                
                var query = _context.posts.ToList();
                var queryResult = query.Select(x => _converter.EntityDTO(x)).AsQueryable();
                var result = Pagination.GetPagedData(queryResult, pageNumber, pageSize);
                return result;
            }
            if (currentUser.IsInRole("User"))
            {
                var query = _context.posts.Where(x => x.IsActive == true).ToList();
                var queryResult = query.Select(x => _converter.EntityDTO(x)).AsQueryable();
                var result = Pagination.GetPagedData(queryResult, pageNumber, pageSize);
                return result;
            }
            //var query =  _context.posts.Select(x => _converter.EntityDTO(x)).AsQueryable();
            //var result = Pagination.GetPagedData(query,pageNumber, pageSize);
            return null;
        }
        #endregion

        #region//Report bài viết

        public ResponseObject<Data_ReportType> ReportPost(Request_ReportType request)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_ReportType>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Bạn chưa xác thực hoặc chưa đăng nhập",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<Data_ReportType>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<Data_ReportType>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            var Post = _context.posts.SingleOrDefault(x => x.Id == request.PostId && x.IsActive==true);
            if (Post == null)
            {
                return new ResponseObject<Data_ReportType>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Bài viết đã bị ẩn hoặc đã bị gỡ xuống!",
                    Data = null
                };
            }
            Report report = new Report
            {
                PostId = request.PostId,
                UserReportId = Post.UserId,
                UserReportedId = idUser,
                ReportType = (Enum.Repost.ReportType)request.ReportType,
                ReportingReason = request.ReportingReason,
                CreateAt = DateTime.UtcNow,
            };
            _context.reports.Add(report);
            _context.SaveChanges();
            return new ResponseObject<Data_ReportType>
            {
                Status = StatusCodes.Status200OK,
                Message = "Báo cáo bài viết thành công!",
                Data = _typeConverter.ReportToDTO(report)
            };
        }

        public ResponseObject<IEnumerable<Data_ReportType>> GetAllReport()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<IEnumerable<Data_ReportType>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Bạn chưa xác thực hoặc chưa đăng nhập",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                var Report = _context.reports.ToList()
                            .Select(x => _typeConverter.ReportToDTO(x));
                if (Report == null)
                {
                    return new ResponseObject<IEnumerable<Data_ReportType>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Không có báo cáo nào!",
                        Data = null
                    };
                }
                return new ResponseObject<IEnumerable<Data_ReportType>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy ra danh sách thành công!",
                    Data = Report
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<IEnumerable<Data_ReportType>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<IEnumerable<Data_ReportType>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            var Report2 = _context.reports.ToList()
                          .Where(x => x.Id == idUser)
                          .Select(x => _typeConverter.ReportToDTO(x));
            if (Report2 == null)
            {
                return new ResponseObject<IEnumerable<Data_ReportType>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không có báo cáo nào!",
                    Data = null
                };
            }
            return new ResponseObject<IEnumerable<Data_ReportType>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy ra danh sách thành công!",
                Data = Report2
            };
        }
        #endregion
    }
}
