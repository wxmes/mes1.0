using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.DataAccess;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;

namespace EIP.System.DataAccess.Identity
{
    public interface ISystemRoleRepository : IAsyncRepository<SystemRole>
    {
        /// <summary>
        ///     ������֯������ȡ��ɫ��Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemRoleOutput>> GetRolesByOrganizationId(SystemRolesGetByOrganizationId input);
        
        /// <summary>
        ///     ��ȡ���û��Ѿ����еĽ�ɫ��Ϣ
        /// </summary>
        /// <param name="privilegeMaster"></param>
        /// <param name="userId">��Ҫ��ѯ���û�id</param>
        /// <returns></returns>
        Task<IEnumerable<SystemRoleOutput>> GetHaveUserRole(EnumPrivilegeMaster privilegeMaster,
            Guid userId);
    }
}