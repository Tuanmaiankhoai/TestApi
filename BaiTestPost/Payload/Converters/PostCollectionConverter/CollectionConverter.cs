using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Payload.DataResponses.PostCollection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaiTestPost.Payload.Converters.PostCollectionConverter
{
    public class CollectionConverter
    {
        private readonly AppDbContext _context;
        private readonly PostCollectionConverter _postCollectionConverter;
        public CollectionConverter(AppDbContext context, PostCollectionConverter postCollectionConverter)
        {
            _context = context;
            _postCollectionConverter = postCollectionConverter;
        }
        public Data_Collection collectionToDTO(Collection collection)
        {
            return new Data_Collection
            {
                OwnerCollection = _context.users.FirstOrDefault(x => x.Id == collection.UserId).Username,
                CollectionName = collection.CollectionName,
                CollectionTitle = collection.CollectionTitle,
                PostCollections = _context.postCollections.ToList()
                                    .Where(x => x.CollectionId == collection.Id)
                                    .Select(x => _postCollectionConverter.PostCollectionToDTO(x))
                                    .AsQueryable()
            };
        }
    }
}
