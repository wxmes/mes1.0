using EIP.Common.Core.Extensions;
using EIP.Common.Dapper;
using EIP.Common.DataAccess;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EIP.System.DataAccess.Identity
{
    public class SystemGroupRepository : DapperAsyncRepository<SystemGroup>, ISystemGroupRepository
    {
        /// <summary>
        ///     ��ѯ����ĳ��֯�µ�����Ϣ
        /// </summary>
        /// <param name="input">��֯����Id</param>
        /// <returns>����Ϣ</returns>
        public Task<IEnumerable<SystemGroupOutput>> GetGroupByOrganizationId(SystemGroupGetGroupByOrganizationIdInput input)
        {
            var sql = new StringBuilder();
            sql.Append(
                @"SELECT gr.GroupId,gr.Name,gr.BelongTo,gr.BelongToUserId,gr.[State],gr.IsFreeze,gr.OrderNo,gr.Remark,gr.CreateTime,gr.CreateUserName,gr.UpdateTime,gr.UpdateUserName,org.OrganizationId,org.Name OrganizationName
                         FROM System_Group gr LEFT JOIN System_Organization org ON org.OrganizationId=gr.OrganizationId WHERE 1=1 ");
            if (!input.Id.IsNullOrEmptyGuid())
            {
                sql.Append(" AND gr.OrganizationId=@orgId");
            }
            sql.Append(input.Sql);
            sql.Append(" ORDER BY gr.OrganizationId");
            return SqlMapperUtil.SqlWithParams<SystemGroupOutput>(sql.ToString(), new {orgId = input.Id});
        }

    }
}