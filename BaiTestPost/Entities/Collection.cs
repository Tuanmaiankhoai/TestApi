namespace BaiTestPost.Entities
{
    public class Collection:BaseId
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public string CollectionTitle { get; set; }
        public string CollectionName { get; set; }
        public IEnumerable<PostCollection> PostCollections { get; set; }

    }
}
