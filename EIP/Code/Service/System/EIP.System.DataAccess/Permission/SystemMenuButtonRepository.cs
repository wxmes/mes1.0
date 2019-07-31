using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EIP.Common.Core.Extensions;
using EIP.Common.Dapper;
using EIP.Common.DataAccess;
using EIP.Common.Models;
using EIP.System.Models.Entities;
using EIP.Common.Models.Dtos;
using EIP.System.Models.Dtos.Permission;

namespace EIP.System.DataAccess.Permission
{
    public class SystemMenuButtonRepository : DapperAsyncRepository<SystemMenuButton>, ISystemMenuButtonRepository
    {
        /// <summary>
        ///     ���ݲ˵���ȡ��������Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(SystemMenuGetMenuButtonByMenuIdInput input = null)
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT f.*,menu.Name MenuName FROM System_MenuButton f LEFT JOIN System_Menu menu ON menu.MenuId=f.MenuId WHERE 1=1 ");

            if (input != null)
            {
                sql.Append(input.Sql);
                if (!input.Id.IsNullOrEmptyGuid())
                {
                    sql.Append(" AND f.MenuId=@menuId");
                }
                return SqlMapperUtil.SqlWithParams<SystemMenuButtonOutput>(sql.ToString(), new { menuId = input.Id });
            }
            return  SqlMapperUtil.SqlWithParams<SystemMenuButtonOutput>(sql.ToString());
        }

        /// <summary>
        ///     ����·����Ϣ��ȡ�˵���Ϣ
        /// </summary>
        /// <param name="mvcRote"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemMenuButton>> GetMenuButtonByMvcRote(MvcRote mvcRote)
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT * FROM System_MenuButton func " +
                "WHERE func.MenuId IN (SELECT MenuId FROM System_Menu WHERE Area=@area AND Controller=@controller AND Action=@action) ORDER BY func.OrderNo");
            return  SqlMapperUtil.SqlWithParams<SystemMenuButton>(sql.ToString(),
                new
                {
                    area = mvcRote.Area,
                    controller = mvcRote.Controller,
                    action = mvcRote.Action
                });
        }

        /// <summary>
        ///     ���ݲ˵�Id���û�Id��ȡ��ťȨ������
        /// </summary>
        /// <param name="mvcRote"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemMenuButton>> GetMenuButtonByMenuIdAndUserId(MvcRote mvcRote,
            Guid userId)
        {
            const string procName = @"System_Proc_GetMenuButtonPermissions";
            return  SqlMapperUtil.StoredProcWithParams<SystemMenuButton>(procName,
                new { UserId = userId, mvcRote.Area, mvcRote.Controller, mvcRote.Action });
        }
    }
}