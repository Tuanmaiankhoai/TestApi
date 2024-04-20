using System.ComponentModel.DataAnnotations;

namespace BaiTestPost.Payload.DataRequests.Post
{
    public class Request_UpdatePost
    {
        public int PostId { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
