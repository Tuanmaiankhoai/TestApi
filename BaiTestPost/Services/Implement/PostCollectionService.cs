using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.Converters.PostCollectionConverter;
using BaiTestPost.Payload.Converters.PostConverter;
using BaiTestPost.Payload.DataResponses.Post;
using BaiTestPost.Payload.DataResponses.PostCollection;
using BaiTestPost.Payload.Responses;
using BaiTestPost.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaiTestPost.Services.Implement
{
    public class PostCollectionService : IPostCollectionService
    {
        #region Private readonly
        private readonly AppDbContext _context;
        private readonly CollectionConverter _collectionConverter;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion
        #region public 
        public PostCollectionService(AppDbContext context,
                                 IHttpContextAccessor contextAccessor,
                                 CollectionConverter collectionConverter)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _collectionConverter = collectionConverter;
        }
        #endregion
        #region public func
        // tạo bộ sưu tập
        public ResponseObject<Data_Collection> CreateCollection(string collectiontitle, string collectionname)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            Collection collection = new Collection
            {
                UserId = idUser,
                CollectionTitle = collectiontitle,
                CollectionName = collectionname
            };
            _context.collections.Add(collection);
            _context.SaveChanges();
            return new ResponseObject<Data_Collection>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tạo bộ sưu tập thành công",
                Data = _collectionConverter.collectionToDTO(collection)
            };
        }
        //Xóa bộ sưu tập
        public ResponseObject<IEnumerable<Data_Collection>> DeleteCollection(int idcollection)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                var collectionA = _context.collections.FirstOrDefault(x => x.Id == idcollection);
                if (collectionA == null)
                {
                    return new ResponseObject<IEnumerable<Data_Collection>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bộ sưu tập này không tồn tại trong cơ sở dữ liệu",
                        Data = null
                    };
                }
                var postcollection = _context.postCollections.FirstOrDefault(x => x.CollectionId == idcollection);
                
                _context.postCollections.Remove(postcollection);
                _context.collections.Remove(collectionA);
                _context.SaveChanges();
                var ListcollectionA = _context.collections
                    .Include(x => x.PostCollections).ToList()
                    .Select(x => _collectionConverter.collectionToDTO(x)).AsQueryable();
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Xóa bộ sưu tập thành công",
                    Data = ListcollectionA
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            var collection = _context.collections.FirstOrDefault(x => x.Id == idcollection && x.UserId == idUser);
            if (collection == null)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không có bộ sưu tập này",
                    Data = null
                };
            }
            _context.collections.Remove(collection);
            _context.SaveChanges();
            var Listcollection = _context.collections
                .Include(x => x.PostCollections).ToList()
                .Where(y => y.UserId == idUser)
                .Select(x => _collectionConverter.collectionToDTO(x)).AsQueryable();
            return new ResponseObject<IEnumerable<Data_Collection>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Xóa bộ sưu tập thành công",
                Data = Listcollection
            };
        }
        //sửa bộ sưu tập
        public ResponseObject<Data_Collection> UpdateCollection(int idcollection, string collectiontitle, string collectionname)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                var collectionA = _context.collections.FirstOrDefault(x => x.Id == idcollection);
                if (collectionA == null)
                {
                    return new ResponseObject<Data_Collection>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bộ sưu tập này không tồn tại trong cơ sở dữ liệu",
                        Data = null
                    };
                }
                collectionA.CollectionName = collectionname;
                collectionA.CollectionTitle = collectiontitle;
                _context.SaveChanges();
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Sửa thành công",
                    Data = _collectionConverter.collectionToDTO(collectionA)
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            var collection = _context.collections.FirstOrDefault(x => x.Id == idcollection && x.UserId == idUser);
            if (collection == null)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không có bộ sưu tập này",
                    Data = null
                };
            }
            collection.CollectionName = collectionname;
            collection.CollectionTitle = collectiontitle;
            _context.SaveChanges();
            return new ResponseObject<Data_Collection>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tạo bộ sưu tập thành công",
                Data = _collectionConverter.collectionToDTO(collection)
            };
        }
        //Get Collection
        public ResponseObject<IEnumerable<Data_Collection>> GetCollection()
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                //lấy ra danh sách collection + postcollection
                var Collections = _context.collections.Include(x => x.PostCollections).ToList();
                //chuyển qua kiểu ResponseCollection
                var ListcollectionA = Collections.Select(x => _collectionConverter.collectionToDTO(x));
                if (!ListcollectionA.Any())
                {
                    return new ResponseObject<IEnumerable<Data_Collection>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Danh sách trống",
                        Data = ListcollectionA
                    };
                }
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy danh sách bộ sưu tập thành công",
                    Data = ListcollectionA
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            //lấy ra danh sách collection + postcollection
            var Collection = _context.collections.Include(x => x.PostCollections).Where(y => y.UserId == idUser).ToList();
            //chuyển qua kiểu ResponseCollection
            var Listcollection = Collection.Select(x => _collectionConverter.collectionToDTO(x));
            if (!Listcollection.Any())
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Danh sách trống",
                    Data = Listcollection
                };
            }
            return new ResponseObject<IEnumerable<Data_Collection>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách thành công",
                Data = Listcollection
            };
        }
        //Thêm bài viết vào bộ sưu tập
        public ResponseObject<Data_Collection> AddPostInCollection(int idPost, int idCollection)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                var collectionA = _context.collections.Include(x => x.PostCollections).FirstOrDefault(x => x.Id == idCollection);
                if (collectionA == null)
                {
                    return new ResponseObject<Data_Collection>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bộ sưu tập này không tồn tại hoặc đã bị xóa khỏi dữ liệu",
                        Data = null
                    };
                }
                var postA = _context.posts.FirstOrDefault(x => x.Id == idPost);
                if (collectionA == null)
                {
                    return new ResponseObject<Data_Collection>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bài viết này không tồn tại hoặc đã bị xóa khỏi dữ liệu",
                        Data = null
                    };
                }
                if (_context.postCollections.Any(x => x.CollectionId == idCollection && x.PostId == idPost))
                {
                    return new ResponseObject<Data_Collection>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bài viết đã nằm trong bộ sưu tập này rồi!",
                        Data = null
                    };
                }
                PostCollection postCollectionA = new PostCollection
                {
                    CollectionId = idCollection,
                    PostId = idPost
                };
                _context.postCollections.Add(postCollectionA);
                _context.SaveChanges();
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Thêm bài viết vào bộ sưu tập thành công!",
                    Data = _collectionConverter.collectionToDTO(collectionA)
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            var collection = _context.collections.Include(x => x.PostCollections).FirstOrDefault(x => x.UserId == idUser && x.Id == idCollection);
            if (collection == null)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy bộ sưu tập này trong dữ liệu của bạn",
                    Data = null
                };
            }
            var post = _context.posts.FirstOrDefault(x => x.UserId == idUser && x.Id == idPost);
            if (collection == null)
            {
                return new ResponseObject<Data_Collection>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy bài viết này trong dữ liệu của bạn",
                    Data = null
                };
            }
            PostCollection postCollection = new PostCollection
            {
                CollectionId = idCollection,
                PostId = idPost
            };
            _context.postCollections.Add(postCollection);
            _context.SaveChanges();
            return new ResponseObject<Data_Collection>
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Thêm bài viết vào bộ sưu tập thành công!",
                Data = _collectionConverter.collectionToDTO(collection)
            };
        }
        //Xóa bài viết khỏi bộ sưu tập
        public ResponseObject<IEnumerable<Data_Collection>> DeletePostInCollection(int idPost, int idCollection)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng chưa xác thực",
                    Data = null
                };
            }
            if (currentUser.IsInRole("Admin"))
            {
                var collectionA = _context.collections.FirstOrDefault(x => x.Id == idCollection);
                if (collectionA == null)
                {
                    return new ResponseObject<IEnumerable<Data_Collection>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bộ sưu tập này không tồn tại hoặc đã bị xóa trong dữ liệu!",
                        Data = null
                    };
                }
                var postA = _context.posts.FirstOrDefault(x => x.Id == idPost);
                if (postA == null)
                {
                    return new ResponseObject<IEnumerable<Data_Collection>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bài viết này không tồn tại hoặc đã bị xóa trong dữ liệu!",
                        Data = null
                    };
                }
                var postcollectionA = _context.postCollections.FirstOrDefault(x => x.CollectionId == idCollection && x.PostId == idPost);
                if (postcollectionA == null)
                {
                    return new ResponseObject<IEnumerable<Data_Collection>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Bài viết không nằm trong bộ sưu tập này!",
                        Data = null
                    };
                }
                _context.postCollections.Remove(postcollectionA);
                _context.SaveChanges();
                var ListcollectionA = _context.collections
                    .Include(x => x.PostCollections).ToList()
                    .Select(x => _collectionConverter.collectionToDTO(x)).AsQueryable();
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đã xóa bài viết ra khỏi bộ sưu tập",
                    Data = ListcollectionA
                };
            }
            var claim = currentUser.FindFirst("ID");
            var idUser = int.Parse(claim.Value);
            var user = _context.users.FirstOrDefault(x => x.Id == idUser);
            
            if (user.IsActive==false)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã dừng hoạt động!",
                    Data = null
                };
            }
            if (user.IsLocked==true)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài khoản của bạn đã bị quản trị viên Ban do vi phạm chính sách!",
                    Data = null
                };
            }
            var collection = _context.collections.FirstOrDefault(x => x.Id == idCollection && x.UserId == idUser);
            if (collection == null)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Bộ sưu tập này không tồn tại hoặc đã bị xóa trong dữ liệu!",
                    Data = null
                };
            }
            var post = _context.posts.FirstOrDefault(x => x.Id == idPost && x.UserId == idUser);
            if (post == null)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Bài viết này không tồn tại hoặc đã bị xóa trong dữ liệu!",
                    Data = null
                };
            }
            var postcollection = _context.postCollections.FirstOrDefault(x => x.CollectionId == idCollection && x.PostId == idPost);
            if (postcollection == null)
            {
                return new ResponseObject<IEnumerable<Data_Collection>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Bài viết này không nằm trong bộ sưu tập này!",
                    Data = null
                };
            }
            _context.postCollections.Remove(postcollection);
            _context.SaveChanges();
            var Listcollection = _context.collections
                .Include(x => x.PostCollections).ToList()
                .Where(y => y.UserId == idUser)
                .Select(x => _collectionConverter.collectionToDTO(x)).AsQueryable();
            return new ResponseObject<IEnumerable<Data_Collection>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Đã xóa bài viết ra khỏi bộ sưu tập",
                Data = Listcollection
            };
        }
        #endregion
    }
}
