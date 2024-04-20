
using BaiTestPost.Payload.DataRequests.User;
using BaiTestPost.Services.Implement;
using BaiTestPost.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaiTestPost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRelationshipService _relationship;
        public UserController(IUserService userService, IRelationshipService relationship)
        {
            _userService = userService;
            _relationship = relationship;
        }
        
        #region//Dang ky, dang nhap, su ly nguoi dung
        [HttpPost("/api/auth/Register")]
        public IActionResult Register([FromForm] Request_Register request)
        {
            return Ok(_userService.resgister(request));
        }
        [HttpPost("/api/auth/Login")]
        public IActionResult Login([FromForm] Request_Login login)
        {
            return Ok(_userService.Login(login));
        }
        [HttpPost("/api/auth/Logout")]
        public IActionResult Logout(string refresh)
        {
            return Ok(_userService.Logout(refresh));
        }
        [HttpGet("/api/auth/get-all")]
        public IActionResult GetAll()
        {
            return Ok(_userService.GetAll());
        }

        [HttpPut("/api/auth/update-user")]
        public IActionResult Update([FromForm] int id, [FromForm] Request_Register request)
        {
            return Ok(_userService.UpdateUser(id, request));
        }

        [HttpDelete("api/auth/delete-user")]
        public IActionResult DeleteUser([FromForm] int id)
        {
            return Ok(_userService.DeleteUserVV(id));
        }
        [HttpPut("/api/auth/update-role")]
        public IActionResult UpdateRole([FromForm] int UserID, [FromForm] int RoleID)
        {
            return Ok(_userService.UpdateRole(UserID, RoleID));
        }

        [HttpPost("/api/auth/LookOrUnLook")]
        public IActionResult LookOrUnLook()
        {
            return Ok(_userService.LockOrUnlockAccount());
        }

        [HttpPost("/api/auth/BanAcc")]
        public IActionResult BanAcc([FromForm] int UserId)
        {
            return Ok(_userService.BanAccount(UserId));
        }
        #endregion

        #region//Follow
        [HttpPost("/api/auth/Following/{UserIDWantFollow}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Following([FromRoute] int UserIDWantFollow)
        {
            return Ok(_relationship.FollowingUser(UserIDWantFollow));
        }

        [HttpPost("/api/auth/UnFollowing")]
        public IActionResult UnFollowing([FromForm]int UserIDWantUnFollow)
        {
            return Ok(_relationship.UnFollow(UserIDWantUnFollow));
        }

        [HttpGet("/api/auth/GetFollowing")]
        public IActionResult GetFollowing([FromForm] int UserIDWantUnFollow)
        {
            return Ok(_relationship.GetRelationShipOfUser());
        }
        #endregion

        
    }
}
