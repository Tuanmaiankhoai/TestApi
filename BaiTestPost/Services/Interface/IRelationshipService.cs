using BaiTestPost.Payload.DataResponses.Relationship;
using BaiTestPost.Payload.Responses;

namespace BaiTestPost.Services.Interface
{
    public interface IRelationshipService
    {
        ResponseObject<Data_Relationship> FollowingUser(int idUserWantFollow);
        ResponseObject<Data_Relationship> GetRelationShipOfUser();
        ResponseObject<Data_Relationship> UnFollow(int idUserWantUnFollow);
    }
}
