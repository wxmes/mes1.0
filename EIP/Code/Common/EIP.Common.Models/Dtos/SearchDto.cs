using EIP.Common.Models.Paging;

namespace EIP.Common.Models.Dtos
{
    /// <summary>
    /// 查询基础Dto
    /// </summary>
    public class SearchDto
    {
        /// <summary>
        /// 过滤
        /// </summary>
        public string Filters { get; set; }

        /// <summary>
        /// 生成的Sql
        /// </summary>
        public string Sql => SearchFilterUtil.ConvertFilters(Filters);
    }
}