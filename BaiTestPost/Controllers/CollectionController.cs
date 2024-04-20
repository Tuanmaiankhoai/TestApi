
using BaiTestPost.Services.Implement;
using BaiTestPost.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaiTestPost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly IPostCollectionService _collectionService;
        public CollectionController(IPostCollectionService collectionService)
        {
            _collectionService = collectionService;
        }
        [HttpPost("api/collection/createcollection")]
        public IActionResult CreateCollection([FromForm] string collection_title, [FromForm] string collection_name)
        {
            return Ok(_collectionService.CreateCollection(collection_title, collection_name));
        }
        [HttpPut("api/collection/updatecollection")]
        public IActionResult UpdateCollection([FromForm] int idcollection, [FromForm] string collection_title, [FromForm] string collection_name)
        {
            return Ok(_collectionService.UpdateCollection(idcollection, collection_title, collection_name));
        }
        [HttpDelete("api/collection/deletecollection")]
        public IActionResult DeleteCollection([FromForm] int idcollection)
        {
            return Ok(_collectionService.DeleteCollection(idcollection));
        }
        [HttpGet("api/collection/getcollection")]
        public IActionResult GetCollection()
        {
            return Ok(_collectionService.GetCollection());
        }
        [HttpPost("api/collection/addpostincollection")]
        public IActionResult AddPostInCollection([FromForm] int idPost, [FromForm] int idCollection)
        {
            return Ok(_collectionService.AddPostInCollection(idPost, idCollection));
        }
        [HttpDelete("api/collection/deletepostincollection")]
        public IActionResult DeletePostInCollection([FromForm] int idPost, [FromForm] int idCollection)
        {
            return Ok(_collectionService.DeletePostInCollection(idPost, idCollection));
        }

    }
}
