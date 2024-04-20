namespace BaiTestPost.Entities
{
    public class PostCollection:BaseId
    {
        public int PostId { get; set; }
        public Post? Post { get; set; }
        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }
    }
}
