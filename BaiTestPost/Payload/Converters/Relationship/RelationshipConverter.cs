using BaiTestPost.Data;
using BaiTestPost.Payload.DataResponses.Relationship;

namespace BaiTestPost.Payload.Converters.Relationship
{
    public class RelationshipConverter
    {
        private readonly AppDbContext _Context;
        public RelationshipConverter(AppDbContext context)
        {
            _Context = context;
        }
        public Data_Relationship FollowToDTO(int IDUser)
        {
            return new Data_Relationship
            {
                Followers = _Context.relationships.Count(x => x.FollowingId == IDUser),
                Following = _Context.relationships.Count(x => x.FollowerId == IDUser)
            };
        }
    }
}
