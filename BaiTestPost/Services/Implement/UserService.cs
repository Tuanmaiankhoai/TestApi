using Azure.Core;
using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Enum.Email;
using BaiTestPost.Payload.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Cryptography;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Text;
using BaiTestPost.Handler.Image;
using BaiTestPost.Services.Interface;
using BaiTestPost.Payload.Converters.UserConverter;
using BaiTestPost.Payload.DataRequests.User;
using BaiTestPost.Payload.DataResponses.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace BaiTestPost.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponse_User> _response_User;
        private readonly ResponseObject<DataResponse_Token> _response_Token;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserConverter _userConverter;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(UserConverter userConverter, AppDbContext context, ResponseObject<DataResponse_User> response_User, ResponseObject<DataResponse_Token> response_Token, IConfiguration configuration, IHttpContextAccessor httpContextAccessor , IWebHostEnvironment webHostEnvironment)
        {
            _userConverter = userConverter;
            _context = context;
            _response_User = response_User;
            _response_Token = response_Token;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        #region//Đăng ký
        private string UploadImageAsync(IFormFile imageFile)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadFiles", "Avatars");
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
        public async Task<ResponseObject<DataResponse_User>> resgister(Request_Register request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName)
                || string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Password)
                || string.IsNullOrWhiteSpace(request.Email)
                )
            {
                return _response_User.responseError(StatusCodes.Status400BadRequest, "Hãy nhập đầy đủ thông tin!", null);
            }
            if (_context.users.Any(x => x.Email.Equals(request.Email)))
            {
                return _response_User.responseError(StatusCodes.Status400BadRequest, "Email đã tồn tại!", null);
            }
            if (_context.users.Any(x => x.Username.Equals(request.Username)))
            {
                return _response_User.responseError(StatusCodes.Status400BadRequest, "Tài khoản đã tồn tại!", null);
            }
            if (!Validate.IsValidEmail(request.Email))
            {
                return _response_User.responseError(StatusCodes.Status400BadRequest, "Định dạng Email không hợp lệ!", null);
            }

            var user = new User();
            user.FullName = request.FullName;
            user.Username = request.Username;
            user.Password = BCryptNet.HashPassword(request.Password);
            user.Email = request.Email;
            user.Avatar = UploadImageAsync(request.Avatar);
            user.DateOfBirth = request.DateOfBirth;
            user.RoleId = 3;
            user.UserStatusId = 3;
            user.IsActive = true;
            user.IsLocked = false;
            _context.users.Add(user);
            _context.SaveChanges();
            return _response_User.ResponseAccess("Đăng kí tài khoản thành công", _userConverter.EntityDTO(user));
        }
        #endregion

        #region//Đăng nhập
        public ResponseObject<DataResponse_Token> Login(Request_Login request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                return _response_Token.responseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin!", null);
            }

            var user = _context.users.SingleOrDefault(x => x.Username.Equals(request.UserName));
            if (user == null)
            {
                return _response_Token.responseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại!", null);
            }

            bool checkPass = BCryptNet.Verify(request.Password, user.Password);
            if (!checkPass)
            {
                return _response_Token.responseError(StatusCodes.Status400BadRequest, "Mật khẩu không chính xác!", null);
            }

            try
            {
                var accessToken = GenerateAccessToken(user);
                return _response_Token.ResponseAccess("Đăng nhập thành công", accessToken);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return _response_Token.responseError(StatusCodes.Status500InternalServerError, "Lỗi trong quá trình đăng nhập!", null);
            }
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            var token = _context.refreshTokens.FirstOrDefault(x => x.Token == refreshToken);
            if (token != null)
            {
                _context.refreshTokens.Remove(token);
                _context.SaveChanges();
            }
        }


        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var item = RandomNumberGenerator.Create())
            {
                item.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        public DataResponse_Token GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var sercretKeyByte = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value);
            var role = _context.roles.SingleOrDefault(x => x.Id == user.RoleId);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Email",user.Email),
                    new Claim(ClaimTypes.Role, role.Code),
                }),
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(sercretKeyByte), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            RefreshToken rf = new RefreshToken()
            {
                Token = refreshToken,
                ExpiredTime = DateTime.Now.AddDays(1),
                UserId = user.Id,
            };
            _context.refreshTokens.Add(rf);
            _context.SaveChanges();


            user.UserStatusId = 1;
            _context.users.Update(user);
            _context.SaveChanges();
            DataResponse_Token result = new DataResponse_Token
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                StatusUser = _context.userStatuses.SingleOrDefault(x => x.Id == user.UserStatusId).Name
            };
            return result;

        }

        public DataResponse_Token RenewAccessToken(Request_RenewAccess request)
        {
            var refreshToken = _context.refreshTokens.SingleOrDefault(x => x.Token.Equals(request.RefreshToken));
            var user = _context.users.SingleOrDefault(x => x.Id == refreshToken.UserId);
            if (refreshToken.ExpiredTime > DateTime.Now)
            {
                throw new ArgumentException("ToKen da het han");
            }
            return GenerateAccessToken(user);
        }

        #endregion

        #region//Đăng xuất
        public ResponseObject<DataResponse_Token> Logout(string refreshToken)
        {
            /*
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.User.Identity.IsAuthenticated)
            {
                var userIdLogin = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdLogin != null && int.TryParse(userIdLogin.Value, out int userId))
                {
                    var refreshToken = _context.refreshTokens.SingleOrDefault(x => x.UserId == userId);

                    if (refreshToken != null)
                    {
                        _context.refreshTokens.Remove(refreshToken);
                        _context.SaveChanges();
                    }
                    httpContext.Session.Remove("AccessToken");
                    return _response_Token.ResponseAccess("Đăng xuất thành công", null);
                }
                return _response_Token.responseError(StatusCodes.Status401Unauthorized, "Có lỗi xảy ra khi đăng xuất", null);
            }
            return _response_Token.responseError(StatusCodes.Status401Unauthorized, "Tài khoản chưa xác thực", null);
            */
            try
            {
                RevokeRefreshToken(refreshToken);
            
                return _response_Token.ResponseAccess("Đăng xuất thành công", null);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return _response_Token.responseError(StatusCodes.Status500InternalServerError, "Lỗi trong quá trình đăng xuất!", null);
            }
        }
        #endregion

        #region//Lấy toàn bộ danh sách User
        public ResponseObject<IQueryable<DataResponse_User>> GetAll()
        {
            var currenUser = _httpContextAccessor.HttpContext.User;
            if (!currenUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<IQueryable<DataResponse_User>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Tai khoan chua xac thuc",
                    Data = null
                };
            }
            if (currenUser.IsInRole("Admin"))
            {
                var user = _context.users.ToList().Select(x => _userConverter.EntityDTO(x)).AsQueryable();
                if (!user.Any())
                {
                    return new ResponseObject<IQueryable<DataResponse_User>>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Danh sach trong",
                        Data = null
                    };

                }
                return new ResponseObject<IQueryable<DataResponse_User>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Thực hiện thao tác thành công!",
                    Data = user
                };

            }
            return new ResponseObject<IQueryable<DataResponse_User>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Bạn không đủ quyền hạn để sử dụng chức năng này!",
                Data = null
            };
        }
        #endregion

        #region //Sửa quyền hạn (chỉ Admin)
        public ResponseObject<DataResponse_User> UpdateRole(int UserId, int RoleIDUpdate)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return _response_User.responseError(StatusCodes.Status400BadRequest, "Người dùng chưa được xác thực!", null);
            }
            if (currentUser.IsInRole("Admin"))
            {
                var user = _context.users.SingleOrDefault(x => x.Id == UserId);
                if (user == null)
                {
                    return _response_User.responseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }
                var role = _context.roles.SingleOrDefault(role => role.Id == RoleIDUpdate);
                if (role == null)
                {
                    return _response_User.responseError(StatusCodes.Status404NotFound, "Quyền hạn này không tồn tại", null);
                }
                user.RoleId = role.Id;
                _context.users.Update(user);
                _context.SaveChanges();
                return _response_User.ResponseAccess("Sửa role người dùng thành công!", _userConverter.EntityDTO(user));
            }
            else
            {
                return _response_User.responseError(StatusCodes.Status401Unauthorized, "Bạn không có quyền hạn để dùng chức năng này!", null);
            }
        }
        #endregion

        #region//Sửa,Xóa người dùng(Admin)
        public async Task<ResponseObject<DataResponse_User>> UpdateUser(int id, Request_Register request)
        {
            {
                var currentUser = _httpContextAccessor.HttpContext.User;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return _response_User.responseError(StatusCodes.Status400BadRequest, "Người dùng chưa được xác thực!", null);
                }
                var user = _context.users.SingleOrDefault(x => x.Id == id);
                if (user == null)
                {
                    return _response_User.responseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }
                if (currentUser.IsInRole("Admin"))
                {

                    if (string.IsNullOrWhiteSpace(request.FullName)
                        || string.IsNullOrWhiteSpace(request.Username)
                        || string.IsNullOrWhiteSpace(request.Password)
                        || string.IsNullOrWhiteSpace(request.Email)
                        )
                    {
                        return _response_User.responseError(StatusCodes.Status400BadRequest, "Hãy nhập đầy đủ thông tin!", null);
                    }
                    if (_context.users.Any(x => x.Email.Equals(request.Email)))
                    {
                        return _response_User.responseError(StatusCodes.Status400BadRequest, "Email đã tồn tại!", null);
                    }
                    if (_context.users.Any(x => x.Username.Equals(request.Username)))
                    {
                        return _response_User.responseError(StatusCodes.Status400BadRequest, "Tài khoản đã tồn tại!", null);
                    }
                    if (!Validate.IsValidEmail(request.Email))
                    {
                        return _response_User.responseError(StatusCodes.Status400BadRequest, "Định dạng Email không hợp lệ!", null);
                    }
                    user.FullName = request.FullName;
                    user.Email = request.Email;
                    user.DateOfBirth = request.DateOfBirth;
                    user.Avatar = UploadImageAsync(request.Avatar);
                    user.Password = BCryptNet.HashPassword(request.Password);
                    user.Username = request.Username;
                    _context.users.Update(user);
                    _context.SaveChanges();
                    return _response_User.ResponseAccess("Sửa thông tin người dùng thành công!", _userConverter.EntityDTO(user));
                }
                return _response_User.responseError(StatusCodes.Status401Unauthorized, "Bạn không có quyền hạn để dùng chức năng này!", null);
            }
        }

        //Xóa bài viết và các bảng liên quan khi xóa người dùng
        private void deletePost(int id)
        {
            //Gỡ bài viết
            var posts = _context.posts.Where(x => x.UserId == id).ToList();
            for (int i = 0; i < posts.Count(); i++)
            {
                //Xóa likePost
                var LikePost = _context.userLikePosts.Where(c => c.PostId == posts[i].Id);
                _context.userLikePosts.RemoveRange(LikePost);
                //Xóa toàn bộ Likecoment của coment trong bài viết
                var ComentPost = _context.userCommentPosts.Where(c => c.PostId == posts[i].Id).ToList();
                for (int j = 0; j < ComentPost.Count(); j++)
                {
                    var likeComent = _context.userLikeCommentOfPosts.Where(c => c.UserCommentPostId == ComentPost[j].Id);
                    _context.userLikeCommentOfPosts.RemoveRange(likeComent);
                }
                //Xóa toàn bộ coment trong bài viết
                _context.userCommentPosts.RemoveRange(ComentPost);
            }
            _context.posts.RemoveRange(posts);
            _context.SaveChanges();
        }
        //Xóa vĩnh viễn người dùng (chỉ admin)
        public ResponseObject<IQueryable<DataResponse_User>> DeleteUserVV(int id)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<IQueryable<DataResponse_User>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực!",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                var user = _context.users.Find(id);
                if (user != null)
                {
                    //Hướng xử lý là xóa Bảng Post trước khi xóa user nhưng vì bảng post sẽ tự bị xóa
                    //khi xóa user nên phải xóa những thứ dính tới post trước(PostCollection, ReportPost,likePost,comentPost)
                    // Bước 1: xóa PostCollection, ReportPost,likePost
                    // Bước 2: Xóa bảng likeComent trước khi xóa bảng comentPost
                    // => xong những thứ dính tới Post
                    // bây giờ Xóa những thứ dính tới user(Likecoment, likePost,comentPost)-(Không cần xóa bảng report vì xóa chung ở trên rồi)
                    // Bước 3: xóa like post
                    // Bước 4 : Xóa likeComent trước khi xóa comentPost
                    // Bước 5 : xóa user (những bảng dính tới user không nhắc đến ở trên sẽ tự động bị xóa)

                    // xóa bảng User like coment
                    var UlikeComent = _context.userLikeCommentOfPosts.Where(k => k.UserId == id);
                    _context.userLikeCommentOfPosts.RemoveRange(UlikeComent);
                    //Xóa bảng PostCollection
                    var Collection = _context.collections.Where(x => x.UserId == id).ToList();
                    for (int i = 0; i < Collection.Count(); i++)
                    {
                        var postCollection = _context.postCollections.Where(x => x.CollectionId == Collection[i].Id);
                        _context.postCollections.RemoveRange(postCollection);
                    }
                    //Xóa bảng Report (bài report của user và user bị report)
                    var report = _context.reports.Where(x => x.UserReportId == id || x.UserReportedId == id);
                    _context.reports.RemoveRange(report);
                    //Xóa những thứ dính tới post và xóa post
                    deletePost(id);
                    //Xóa bảng like coment của các Coment của user
                    var ComentPost = _context.userCommentPosts.Where(m => m.UserId == id).ToList();
                    for (int j = 0; j < ComentPost.Count(); j++)
                    {
                        var likeComent = _context.userLikeCommentOfPosts.Where(c => c.UserCommentPostId == ComentPost[j].Id);
                        _context.userLikeCommentOfPosts.RemoveRange(likeComent);
                    }
                    //xóa coment
                    _context.userCommentPosts.RemoveRange(ComentPost);
                    //xóa bảng RelationShip
                    var follow = _context.relationships.Where(x => x.FollowerId == id || x.FollowingId == id);
                    _context.relationships.RemoveRange(follow);
                    //xóa bảng like post (user like post)
                    var likePost = _context.userLikePosts.Where(x => x.UserId == id);
                    _context.userLikePosts.RemoveRange(likePost);
                    //xóa người dùng()
                    _context.users.Remove(user);
                    _context.SaveChanges();
                    var listUser = _context.users.ToList().Select(x => _userConverter.EntityDTO(x)).AsQueryable();
                    return new ResponseObject<IQueryable<DataResponse_User>>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Xóa tài khoản thành công!",
                        Data = listUser
                    };
                }
                return new ResponseObject<IQueryable<DataResponse_User>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không tồn tại!",
                    Data = null
                };
            }
            return new ResponseObject<IQueryable<DataResponse_User>>
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = "Bạn không đủ quyền hạn để dùng chức năng này!",
                Data = null
            };
        
        }
        #endregion

        #region// ban-lock User
        public ResponseObject<DataResponse_User> LockOrUnlockAccount()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<DataResponse_User>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực!",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            if (user.IsLocked== true)
            {
                user.IsLocked = false;
                _context.SaveChanges();
                return new ResponseObject<DataResponse_User>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Mở khóa tài khoản thành công!",
                    Data = _userConverter.EntityDTO(user)
                };
            }
            else
            {

                user.IsLocked = true;
                _context.SaveChanges();
                return new ResponseObject<DataResponse_User>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Khóa tài khoản thành công!",
                    Data = _userConverter.EntityDTO(user)
                };
            }
        }
        //Ban tài khoản(Admin)
        public ResponseObject<DataResponse_User> BanAccount(int iduser)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<DataResponse_User>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực!",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                var user = _context.users.FirstOrDefault(x => x.Id == iduser && x.IsActive == false);
                if (user == null)
                {
                    return new ResponseObject<DataResponse_User>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài khoản này đã bị ban!",
                        Data = null
                    };
                }
                user.IsActive = false;
                return new ResponseObject<DataResponse_User>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Ban tài khoản thành công!",
                    Data = _userConverter.EntityDTO(user)
                };
            }
            return new ResponseObject<DataResponse_User>
            {
                Status = StatusCodes.Status200OK,
                Message = "Bạn không đủ quyền để thực hiện chức năng này!",
                Data = null
            };
        }
        #endregion


    }
}
