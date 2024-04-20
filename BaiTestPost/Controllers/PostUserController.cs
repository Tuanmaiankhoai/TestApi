using BaiTestPost.Entities;
using BaiTestPost.Payload.DataRequests.CommentPost;
using BaiTestPost.Payload.DataRequests.LikeComment;
using BaiTestPost.Payload.DataRequests.LikePost;
using BaiTestPost.Payload.DataRequests.Post;
using BaiTestPost.Payload.DataRequests.ReportType;
using BaiTestPost.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaiTestPost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostUserController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IUserLikePostService _userLikePost;
        private readonly IUserCommentPost _userCommentPost;
        private readonly IUserLikeCommentPostService _userLikeComment;
        
        public PostUserController(IPostService postService , IUserLikePostService userLikePost , IUserCommentPost userCommentPost ,IUserLikeCommentPostService userLikeComment)
        {
            _postService = postService;
            _userLikePost = userLikePost;
            _userCommentPost = userCommentPost;
            _userLikeComment = userLikeComment;
        }
        #region//Thêm Post

        [HttpPost("/api/auth/CreatePost")]
        public async Task<IActionResult> createPost( [FromForm] Request_CreatePost request)
        {
            return Ok(await _postService.CreatePost(request));
        }
        [HttpPut("/api/auth/UpdatePost")]
        public async Task<IActionResult> updatePost([FromForm] Request_UpdatePost request)
        {
            return Ok(await _postService.UpdatePost(request));
        }
        [HttpDelete("/api/auth/DeletePost")]
        public async Task<IActionResult> DeletePost([FromForm]int PostId)
        {
            return Ok(await _postService.DeletePost(PostId));
        }
        [HttpGet("/api/auth/getAllPost")]
        public async Task<IActionResult> getAll(int pageNumber=1, int pageSize=10)
        {
            return Ok(await _postService.GetAll(pageNumber, pageSize));
        }
#endregion

        #region// User like Post
        [HttpPost("/api/auth/LikePost")]
        public async Task<IActionResult> Likepost([FromForm] Request_LikePost request)
        {
            return Ok(await _userLikePost.CreateLikeOrUnlikePost(request));
        }

        [HttpGet("/api/auth/GetAllLike")]
        public async Task<IActionResult> GetAllLike(int pageNumber=1, int pageSize=10)
        {
            return Ok(await _userLikePost.GetAll(pageNumber, pageSize));
        }
        #endregion

        #region//CommnetPost
        [HttpPost("/api/auth/CreateComment")]
        public async Task<IActionResult> CreateCommnet([FromForm]Request_CommentPost request)
        {
            return Ok(await _userCommentPost.CreateComment(request));
        }
        [HttpPost("/api/auth/UpdateComment")]
        public async Task<IActionResult> UpdateCommnet([FromForm] int CommentId,[FromForm] Request_UpdateComment request)
        {
            return Ok(await _userCommentPost.UpdateComment(CommentId, request));
        }
        [HttpPost("/api/auth/RemoveComment")]
        public async Task<IActionResult> ReMoveCommnet([FromForm]int postId,int IdComment)
        {
            return Ok(await _userCommentPost.RemoveComment(postId, IdComment));
        }
        /*
        [HttpGet("/api/auth/getAllComment")]
        public async Task<IActionResult> GetAllCommnet([FromForm]int PostId,int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _userCommentPost.GetAllComment(PostId,pageNumber, pageSize));
        }*/

        #endregion


        #region// User like Comment
        [HttpPost("/api/auth/LikeOrUnlikeComment")]
        public async Task<IActionResult> LikeOrUnlike([FromForm] Request_LikeComment request)
        {
            return Ok(await _userLikeComment.CreateLikeOrUnlikeComment(request));
        }
        
        [HttpGet("/api/auth/GetAllLikeComment")]
        public async Task<IActionResult> GetAllLikeComment(int commentId, int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _userLikeComment.GetAllLikeComment(commentId, pageNumber, pageSize));
        }
        #endregion

        #region//Report Post
        [HttpPost("api/post/report-post")]
        public  IActionResult ReportPost([FromForm] Request_ReportType request)
        {
            return Ok( _postService.ReportPost(request));
        }

        [HttpGet("api/post/get-all-report")]
        public IActionResult GetAllReport()
        {
            return Ok(_postService.GetAllReport());
        }
        #endregion
    }
}
