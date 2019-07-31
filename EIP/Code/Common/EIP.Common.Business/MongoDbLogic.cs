using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Resource;
using EIP.Common.DataAccess;
using EIP.Common.Models;
using EIP.Common.Models.Paging;
using MongoDB.Driver;

namespace EIP.Common.Business
{
    public class MongoDbLogic<T> : IAsyncMongoDbLogic<T> where T : class, new()
    {
        public MongoDbLogic() { }

        public IAsyncMongoDbRepository<T> Repository;

        public MongoDbLogic(IAsyncMongoDbRepository<T> repository)
        {
            Repository = repository ?? throw new ArgumentNullException("repository", "repository为空");
        }

        public async Task<OperateStatus> InsertAsync(T entity)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.InsertAsync(entity);
                operateStatus.ResultSign = resultNum ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = resultNum ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        public async Task<OperateStatus> InsertScalarAsync(T entity)
        {
            var operateStatus = new OperateStatus<int>();
            try
            {
                var resultNum = await Repository.InsertAsync(entity);
                operateStatus.ResultSign = resultNum ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = resultNum ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        public async Task<OperateStatus> InsertMultipleDapperAsync(IEnumerable<T> list)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.BulkInsertAsync(list);
                operateStatus.ResultSign = resultNum ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = resultNum ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        public async Task<OperateStatus> InsertMultipleAsync(IEnumerable<T> list)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.BulkInsertAsync(list);
                operateStatus.ResultSign = resultNum ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = resultNum ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        public async Task<OperateStatus> UpdateAsync(T current, FilterDefinition<T> filter)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var updateResult = await Repository.UpdateAsync(current, filter);
                operateStatus.ResultSign = updateResult.IsAcknowledged ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = updateResult.IsAcknowledged ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        public async Task<OperateStatus> DeleteAsync(FilterDefinition<T> filter)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var result = await Repository.DeleteManyAsync(filter);
                operateStatus.ResultSign = result.DeletedCount > 0 ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = result.DeletedCount > 0 ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="field"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> GetAllEnumerableAsync(FilterDefinition<T> filter = null, string[] field = null, SortDefinition<T> sort = null)
        {
            return Repository.GetAllEnumerableAsync(filter,field,sort);
        }

        /// <summary>
        /// 分页获取
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="queryParam"></param>
        /// <param name="field"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<PagedResults<T>> PagingQueryProcAsync(FilterDefinition<T> filter, QueryParam queryParam, SortDefinition<T> sort = null, string[] field = null)
        {
            return await Repository.FindListByPageAsync(filter, queryParam.Page, queryParam.Limit, field, sort);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<T> FindOneAsync(FilterDefinition<T> filter,string[] field = null)
        {
            return await Repository.FindOneAsync(filter, field);
        }
    }
}