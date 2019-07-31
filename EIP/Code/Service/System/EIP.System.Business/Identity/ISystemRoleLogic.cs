using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;

namespace EIP.System.Business.Identity
{
    public interface ISystemRoleLogic : IAsyncLogic<SystemRole>
    {
        /// <summary>
        ///     ������֯����Id��ѯ��ɫ��Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemRoleOutput>> GetRolesByOrganizationId(SystemRolesGetByOrganizationId input);

        /// <summary>
        ///     �����λ��Ϣ
        /// </summary>
        /// <param name="role">��λ��Ϣ</param>
        /// <returns></returns>
        Task<OperateStatus> SaveRole(SystemRole role);

        /// <summary>
        ///     ɾ����ɫ��Ϣ
        /// </summary>
        /// <param name="input">��ɫId</param>
        /// <returns></returns>
        Task<OperateStatus> DeleteRole(IdInput input);

        /// <summary>
        ///     ��ɫ����
        /// </summary>
        /// <param name="input">��ɫId</param>
        /// <returns></returns>
        Task<OperateStatus> CopyRole(SystemCopyInput input);

        /// <summary>
        ///     ��ȡ���û��Ѿ����еĽ�ɫ��Ϣ
        /// </summary>
        /// <param name="privilegeMaster"></param>
        /// <param name="userId">��Ҫ��ѯ���û�id</param>
        /// <returns></returns>
        Task<IEnumerable<SystemRole>> GetHaveUserRole(EnumPrivilegeMaster privilegeMaster,
            Guid userId);
    }
}