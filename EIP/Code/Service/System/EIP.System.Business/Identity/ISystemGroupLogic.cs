using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.System.Models.Enums;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;

namespace EIP.System.Business.Identity
{
    public interface ISystemGroupLogic : IAsyncLogic<SystemGroup>
    {
        /// <summary>
        ///     ������֯������ȡ����Ϣ
        /// </summary>
        /// <param name="input">��֯����</param>
        /// <returns></returns>
        Task<IEnumerable<SystemGroupOutput>> GetGroupByOrganizationId(SystemGroupGetGroupByOrganizationIdInput input);

        /// <summary>
        ///     ɾ������Ϣ
        /// </summary>
        /// <param name="input">��Id</param>
        /// <returns></returns>
       Task< OperateStatus> DeleteGroup(IdInput input);

        /// <summary>
        ///     ��������Ϣ
        /// </summary>
        /// <param name="group">��λ��Ϣ</param>
        /// <param name="belongTo">����</param>
        /// <returns></returns>
        Task<OperateStatus> SaveGroup(SystemGroup group,
            EnumGroupBelongTo belongTo);

        /// <summary>
        ///     ����
        /// </summary>
        /// <param name="input">������Ϣ</param>
        /// <returns></returns>
        Task<OperateStatus> CopyGroup(SystemCopyInput input);
    }
}