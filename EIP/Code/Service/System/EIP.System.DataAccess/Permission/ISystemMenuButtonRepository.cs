using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.DataAccess;
using EIP.Common.Models;
using EIP.System.Models.Entities;
using EIP.Common.Models.Dtos;
using EIP.System.Models.Dtos.Permission;

namespace EIP.System.DataAccess.Permission
{
    public interface ISystemMenuButtonRepository : IAsyncRepository<SystemMenuButton>
    {
        /// <summary>
        ///     ���ݲ˵���ȡ��������Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(SystemMenuGetMenuButtonByMenuIdInput input = null);

        /// <summary>
        ///     ����·����Ϣ��ȡ��������Ϣ
        /// </summary>
        /// <param name="mvcRote"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButton>> GetMenuButtonByMvcRote(MvcRote mvcRote);

        /// <summary>
        ///     ���ݲ˵�Id���û�Id��ȡ��ťȨ������
        /// </summary>
        /// <param name="mvcRote"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButton>> GetMenuButtonByMenuIdAndUserId(MvcRote mvcRote,
            Guid userId);
    }
}