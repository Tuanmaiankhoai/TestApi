using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.Converters.Relationship;
using BaiTestPost.Payload.DataResponses.Relationship;
using BaiTestPost.Payload.Responses;
using BaiTestPost.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BaiTestPost.Services.Implement
{
    public class RelationshipService : IRelationshipService
    {
        private readonly AppDbContext _Context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly RelationshipConverter _Converter;
        public RelationshipService(AppDbContext context, IHttpContextAccessor contextAccessor, RelationshipConverter Converter)
        {
            _Context = context;
            _contextAccessor = contextAccessor;
            _Converter = Converter;
        }
        public ResponseObject<Data_Relationship> FollowingUser(int idUserWantFollow)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var IDUser = int.Parse(claim.Value);
            var user = _Context.users.FirstOrDefault(x => x.Id == IDUser);
            if (user.IsActive==false)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị Ban!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                    Data = null
                };
            }
            if (IDUser == idUserWantFollow)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không thể folow chính mình!",
                    Data = null
                };
            }
            if (!_Context.users.Any(x => x.Id == idUserWantFollow))
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy người dùng này!",
                    Data = null
                };
            }
            Relationship relationship = new Relationship
            {
                FollowerId = IDUser,
                FollowingId = idUserWantFollow
            };
            _Context.relationships.Add(relationship);
            _Context.SaveChanges();
            return new ResponseObject<Data_Relationship>
            {
                Status = StatusCodes.Status200OK,
                Message = "Folow thành công!",
                Data = _Converter.FollowToDTO(IDUser)
            };
        }

        public ResponseObject<Data_Relationship> GetRelationShipOfUser()
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var IDUser = int.Parse(claim.Value);
            var user = _Context.users.FirstOrDefault(x => x.Id == IDUser);
            if (user.IsActive==false)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị Ban!",
                    Data = null
                };
            }
            if (user.IsLocked== true)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                    Data = null
                };
            }
            return new ResponseObject<Data_Relationship>
            {
                Status = StatusCodes.Status200OK,
                Message = "Thực hiện thao tác thành công",
                Data = _Converter.FollowToDTO(IDUser)
            };
        }
        public ResponseObject<Data_Relationship> UnFollow(int idUserWantUnFollow)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var IDUser = int.Parse(claim.Value);
            var user = _Context.users.FirstOrDefault(x => x.Id == IDUser);
            if (user.IsActive== false)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị Ban!",
                    Data = null
                };
            }
            if (user.IsLocked == true)
            {
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị khóa, vui lòng mở khóa để có thể sử dụng chức năng!",
                    Data = null
                };
            }
            if (_Context.users.Any(y => y.Id == idUserWantUnFollow))
            {
                var relationship = _Context.relationships.FirstOrDefault(x => x.FollowerId == IDUser && x.FollowingId == idUserWantUnFollow);
                if (relationship == null)
                {
                    return new ResponseObject<Data_Relationship>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bạn chưa follow người này",
                        Data = null
                    };
                }
                _Context.Remove(relationship);
                _Context.SaveChanges();
                return new ResponseObject<Data_Relationship>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Thực hiện thao tác thành công",
                    Data = _Converter.FollowToDTO(IDUser)
                };
            }
            return new ResponseObject<Data_Relationship>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Người dùng bạn muốn bỏ theo dõi không tồn tại",
                Data = null
            };
        }

    }
    
}
