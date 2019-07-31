using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Models.Paging;
using MongoDB.Driver;

namespace EIP.Common.DataAccess
{
    public interface IAsyncMongoDbRepository<T> where T : class
    {
        bool Insert(T t);

        Task<bool> InsertAsync(T t);

        bool BulkInsert(IEnumerable<T> t);

        Task<bool> BulkInsertAsync(IEnumerable<T> t);

        UpdateResult Update(T t, FilterDefinition<T> filter);
        
        Task<UpdateResult> UpdateAsync(T t, FilterDefinition<T> filter);

        UpdateResult UpdateManay(Dictionary<string, string> dic, FilterDefinition<T> filter);

        Task<UpdateResult> UpdateManayAsync(Dictionary<string, string> dic, FilterDefinition<T> filter);

        DeleteResult DeleteMany(FilterDefinition<T> filter);

        Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter);

        long Count(FilterDefinition<T> filter);

        Task<long> CountAsync(FilterDefinition<T> filter);

        T FindOne(FilterDefinition<T> filter, string[] field = null);

        Task<T> FindOneAsync(FilterDefinition<T> filter, string[] field = null);

        IEnumerable<T> GetAllEnumerable(FilterDefinition<T> filter = null, string[] field = null, SortDefinition<T> sort = null);

        Task<IEnumerable<T>> GetAllEnumerableAsync(FilterDefinition<T> filter = null, string[] field = null, SortDefinition<T> sort = null);

        PagedResults<T> FindListByPage(FilterDefinition<T> filter, int pageIndex, int pageSize,
            string[] field = null, SortDefinition<T> sort = null);

        Task<PagedResults<T>> FindListByPageAsync(FilterDefinition<T> filter, int pageIndex, int pageSize,
            string[] field = null, SortDefinition<T> sort = null);
    }
}