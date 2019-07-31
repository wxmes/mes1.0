using EIP.Common.DataAccess;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EIP.System.DataAccess.Identity
{
    public interface ISystemGroupRepository : IAsyncRepository<SystemGroup>
    {
        /// <summary>
        ///     ��ѯ����ĳ��֯�µ�����Ϣ
        /// </summary>
        /// <param name="input">��֯����PostId</param>
        /// <returns>����Ϣ</returns>
        Task<IEnumerable<SystemGroupOutput>> GetGroupByOrganizationId(SystemGroupGetGroupByOrganizationIdInput input);

    }
}