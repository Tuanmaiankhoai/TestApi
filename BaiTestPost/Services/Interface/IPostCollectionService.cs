using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.DataResponses.Post;
using BaiTestPost.Payload.DataResponses.PostCollection;
using BaiTestPost.Payload.Responses;

namespace BaiTestPost.Services.Interface
{
    public interface IPostCollectionService
    {
        

        ResponseObject<Data_Collection> CreateCollection(string collectiontitle, string collectionname);
        ResponseObject<Data_Collection> UpdateCollection(int idcollection, string collectiontitle, string collectionname);
        ResponseObject<IEnumerable<Data_Collection>> DeleteCollection(int idcollection);
        ResponseObject<IEnumerable<Data_Collection>> GetCollection();
        ResponseObject<Data_Collection> AddPostInCollection(int idPost, int idCollection);
        ResponseObject<IEnumerable<Data_Collection>> DeletePostInCollection(int idPost, int idCollection);
    }
}
