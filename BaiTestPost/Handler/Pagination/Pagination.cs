namespace BaiTestPost.Handler.Pagination
{
    public class Pagination
    {
        public static PageResult<T> GetPagedData<T> (IQueryable<T> data, int pageNumber, int pageSize )
        {
            int totalItems = data.Count();
            int totalPages = pageSize==-1?1: (int)Math.Ceiling((decimal) totalItems/pageSize);
            var pagedData = pageSize == -1 ? data : data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new PageResult<T>(pagedData, totalItems, totalPages, pageNumber, pageSize);
        }
    }
}
