﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Models;
using EIP.Common.Models.Paging;
using MongoDB.Driver;

namespace EIP.Common.Business
{
    public interface IAsyncMongoDbLogic<T> where T : class
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">新增实体</param>
        /// <returns></returns>
        Task<OperateStatus> InsertAsync(T entity);

        /// <summary>
        ///     插入可返回自增id值
        /// </summary>
        /// <param name="entity">实体信息</param>
        /// <returns></returns>
        Task<OperateStatus> InsertScalarAsync(T entity);

        /// <summary>
        /// Dapper批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<OperateStatus> InsertMultipleDapperAsync(IEnumerable<T> list);

        /// <summary>
        /// SqlBulkCopy批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<OperateStatus> InsertMultipleAsync(IEnumerable<T> list);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="current">更新实体</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<OperateStatus> UpdateAsync(T current, FilterDefinition<T> filter);

        /// <summary>
        /// 删除所有
        /// </summary>
        /// <returns></returns>
        Task<OperateStatus> DeleteAsync(FilterDefinition<T> filter);

        /// <summary>
        /// 获取集合数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllEnumerableAsync(FilterDefinition<T> filter = null, string[] field = null, SortDefinition<T> sort = null);

        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<T> FindOneAsync(FilterDefinition<T> filter, string[] field = null);

        /// <summary>
        /// 存储过程分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="queryParam"></param>
        /// <param name="field"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<PagedResults<T>> PagingQueryProcAsync(FilterDefinition<T> filter,QueryParam queryParam, SortDefinition<T> sort = null, string[] field = null);
    }
}