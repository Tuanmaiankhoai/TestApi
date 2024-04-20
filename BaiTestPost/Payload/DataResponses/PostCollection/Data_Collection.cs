namespace BaiTestPost.Payload.DataResponses.PostCollection
{
    public class Data_Collection
    {
        public string OwnerCollection { get; set; }
        public string CollectionTitle { get; set; }
        public string CollectionName { get; set; }
        public IEnumerable<Data_PostCollection> PostCollections { get; set; }
    }
}
