using BaiTestPost.Entities;
using BaiTestPost.Payload.DataRequests.User;
using BaiTestPost.Payload.DataResponses.User;
using BaiTestPost.Payload.Responses;

namespace BaiTestPost.Services.Interface
{
    public interface IUserService
    {
        Task<ResponseObject<DataResponse_User>> resgister(Request_Register request);
        DataResponse_Token GenerateAccessToken(User user);
        ResponseObject<DataResponse_Token> Login(Request_Login request);
        ResponseObject<DataResponse_Token> Logout(string refreshToken);
        DataResponse_Token RenewAccessToken(Request_RenewAccess request);
        ResponseObject<IQueryable<DataResponse_User>> GetAll();
        Task<ResponseObject<DataResponse_User>> UpdateUser(int id, Request_Register request);
        ResponseObject<DataResponse_User> UpdateRole(int UserId, int RoleID);
        ResponseObject<IQueryable<DataResponse_User>> DeleteUserVV(int id);
        ResponseObject<DataResponse_User> LockOrUnlockAccount();
        ResponseObject<DataResponse_User> BanAccount(int iduser);

    }
}
