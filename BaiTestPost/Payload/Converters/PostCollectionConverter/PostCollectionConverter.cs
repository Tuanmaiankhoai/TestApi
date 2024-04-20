using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataResponses.PostCollection;

namespace BaiTestPost.Payload.Converters.PostCollectionConverter
{
    public class PostCollectionConverter
    {
        private readonly AppDbContext _context;
        public PostCollectionConverter(AppDbContext context)
        {
            _context = context;
        }
        public Data_PostCollection PostCollectionToDTO(PostCollection postCollection)
        {
            return new Data_PostCollection
            {
                PostTitle = _context.posts.SingleOrDefault(x => x.Id == postCollection.PostId).Title,
                CollectionTitle = _context.collections.SingleOrDefault(x => x.Id == postCollection.CollectionId).CollectionTitle,
            };
        }
    }
}
