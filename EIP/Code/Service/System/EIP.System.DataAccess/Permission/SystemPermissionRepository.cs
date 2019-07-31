using EIP.Common.Core.Extensions;
using EIP.Common.Dapper;
using EIP.Common.DataAccess;
using EIP.Common.Models;
using EIP.Common.Models.Tree;
using EIP.System.DataAccess.Common;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EIP.System.DataAccess.Permission
{
    public class SystemPermissionRepository : DapperAsyncRepository<SystemPermission>, ISystemPermissionRepository
    {
        /// <summary>
        ///     ����Ȩ�޹���Id��ѯ�˵�Ȩ����Ϣ
        /// </summary>
        /// <param name="input">Ȩ������:�˵�����������ݡ��ֶΡ��ļ�</param>
        /// <returns></returns>
        public Task<IEnumerable<SystemPermission>> GetPermissionByPrivilegeMasterValue(
            SystemPermissionByPrivilegeMasterValueInput input)
        {
            var sql =
                new StringBuilder(
                    "SELECT PrivilegeAccessValue FROM System_Permission WHERE PrivilegeAccess=@privilegeAccess");
            sql.Append(
                input.PrivilegeMaster == EnumPrivilegeMaster.��Ա
                    ? " AND PrivilegeMasterValue IN (SELECT PrivilegeMasterValue FROM System_PermissionUser WHERE PrivilegeMasterUserId=@privilegeMasterValue)"
                    : " AND PrivilegeMasterValue=@privilegeMasterValue");
            if (!input.PrivilegeMenuId.IsNullOrEmptyGuid())
            {
                sql.Append(" AND PrivilegeMenuId=@privilegeMenuId");
            }
            sql.Append(" GROUP BY PrivilegeAccessValue");
            return SqlMapperUtil.SqlWithParams<SystemPermission>(sql.ToString(),
                new
                {
                    privilegeAccess = (byte)input.PrivilegeAccess,
                    privilegeMasterValue = input.PrivilegeMasterValue,
                    privilegeMenuId = input.PrivilegeMenuId
                });
        }

        /// <summary>
        ///     ����Ȩ�޹���Idɾ���˵�Ȩ����Ϣ
        /// </summary>
        /// <param name="privilegeAccess">Ȩ������:�˵�����������ݡ��ֶΡ��ļ�</param>
        /// <param name="privilegeMasterValue"></param>
        /// <param name="privilegeMenuId"></param>
        /// <returns></returns>
        public Task<bool> DeletePermissionByPrivilegeMasterValue(EnumPrivilegeAccess? privilegeAccess,
            Guid privilegeMasterValue,
            Guid? privilegeMenuId)
        {
            var deleteSql =
                new StringBuilder("DELETE FROM System_Permission WHERE PrivilegeMasterValue=@privilegeMasterValue ");

            if (privilegeAccess != null)
            {
                deleteSql.Append(" AND PrivilegeAccess=@privilegeAccess");
                if (privilegeMenuId != null)
                {
                    deleteSql.Append(" AND PrivilegeMenuId=@privilegeMenuId");
                }
                return
                    SqlMapperUtil.InsertUpdateOrDeleteSqlBool<SystemUserInfo>(deleteSql.ToString(),
                        new { privilegeAccess = (int)privilegeAccess, privilegeMasterValue, privilegeMenuId });
            }

            return SqlMapperUtil.InsertUpdateOrDeleteSqlBool<SystemUserInfo>(deleteSql.ToString(), new { privilegeMasterValue });
        }

        /// <summary>
        ///     �����û�Id��ȡ�û����еĲ˵�Ȩ��
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetSystemPermissionMenuByUserId(Guid userId)
        {
            var sql =
                new StringBuilder(
                    @"SELECT menu.MenuId id,menu.ParentId parent,menu.name text,menu.icon,menu.OpenUrl url,menu.remark FROM System_Menu menu
                                                    LEFT JOIN System_Permission per ON per.PrivilegeAccessValue=menu.MenuId
                                                    LEFT JOIN System_PermissionUser perUser ON per.PrivilegeMasterValue=perUser.[PrivilegeMasterValue]                                                   
                                                    WHERE per.PrivilegeAccess=@privilegeAccess and menu.IsShowMenu=@isShowMenu and menu.IsFreeze=@isFreeze and perUser.[PrivilegeMasterUserId]=@userId 
                                                    GROUP BY menu.MenuId,menu.ParentId,menu.name,menu.icon,menu.OpenUrl,menu.OrderNo,menu.remark
                                                    ORDER BY menu.OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString(),
                new { privilegeAccess = (byte)EnumPrivilegeAccess.�˵�, isShowMenu = true, isFreeze = false, userId });
        }

        /// <summary>
        ///     ���ݽ�ɫId,��λId,��Id,��ԱId��ȡ���еĲ˵���Ϣ
        /// </summary>
        /// <param name="input">�������</param>
        /// <returns>���β˵���Ϣ</returns>
        /// GetMenuPermissionByPrivilegeMasterValue
        public Task<IEnumerable<JsTreeEntity>> GetMenuHavePermissionByPrivilegeMasterValue(SystemPermissiontMenuHaveByPrivilegeMasterValueInput input)
        {
            var sql =
                new StringBuilder("SELECT MenuId id,ParentId parent,name text,icon FROM System_Menu menu WHERE MenuId IN( SELECT PrivilegeAccessValue  FROM System_Permission WHERE PrivilegeAccess=@privilegeAccess AND ");
            sql.Append(
                input.PrivilegeMaster == EnumPrivilegeMaster.��Ա
                    ? " PrivilegeMasterValue IN (SELECT PrivilegeMasterValue FROM System_PermissionUser WHERE PrivilegeMasterUserId=@privilegeMasterValue) "
                    : " PrivilegeMasterValue=@privilegeMasterValue  ");
            sql.Append(" GROUP BY PrivilegeAccessValue) AND menu.IsFreeze=@isFreeze");
            if (input.PrivilegeAccess != null)
            {
                switch (input.PrivilegeAccess)
                {
                    case EnumPrivilegeAccess.�˵���ť:
                        sql.Append(" AND menu.HaveFunctionPermission='true'");
                        break;
                    case EnumPrivilegeAccess.����Ȩ��:
                        sql.Append(" AND menu.HaveDataPermission='true'");
                        break;
                    case EnumPrivilegeAccess.�ֶ�:
                        sql.Append(" AND menu.HaveFieldPermission='true'");
                        break;
                    case EnumPrivilegeAccess.�˵�:
                        sql.Append(" AND menu.HaveMenuPermission='true'");
                        break;
                }
            }
            sql.Append("  ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString(),
                new
                {
                    privilegeAccess = EnumPrivilegeAccess.�˵�,
                    privilegeMasterValue = input.PrivilegeMasterValue,
                    isFreeze = false
                });
        }

        /// <summary>
        /// ��ȡ�˵���������ȱ�ʹ�õ�Ȩ����Ϣ
        /// </summary>
        /// <param name="privilegeAccess">����:�˵���������</param>
        /// <param name="privilegeAccessValue">��Ӧֵ</param>
        /// <returns></returns>
        public Task<IEnumerable<SystemPermission>> GetSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess privilegeAccess,
            Guid? privilegeAccessValue = null)
        {
            StringBuilder sql =
                new StringBuilder(
                    "SELECT * FROM System_Permission WHERE PrivilegeAccess=@privilegeAccess");
            if (privilegeAccessValue != null)
            {
                sql.Append(" AND PrivilegeAccessValue=@privilegeAccessValue");
            }
            return SqlMapperUtil.SqlWithParams<SystemPermission>(sql.ToString(),
                new { privilegeAccess, privilegeAccessValue });
        }

        /// <summary>
        /// ��ȡ��ɫ����Ⱦ��е�Ȩ��
        /// </summary>
        /// <param name="privilegeMaster">����:��ɫ����Ա�����</param>
        /// <param name="privilegeMasterValue">��Ӧֵ</param>
        /// <returns></returns>
        public Task<IEnumerable<SystemPermission>> GetSystemPermissionsByPrivilegeMasterValueAndValue(EnumPrivilegeMaster privilegeMaster,
            Guid? privilegeMasterValue = null)
        {
            StringBuilder sql =
                new StringBuilder(
                    "SELECT * FROM System_Permission WHERE PrivilegeMaster=@privilegeMaster");
            if (privilegeMasterValue != null)
            {
                sql.Append(" AND PrivilegeMasterValue=@privilegeMasterValue");
            }
            return SqlMapperUtil.SqlWithParams<SystemPermission>(sql.ToString(),
                new { privilegeMaster, privilegeMasterValue });
        }

        /// <summary>
        /// ���ݹ������ȡȨ����Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemPermission>> GetSystemPermissionsMvcRote(SystemPermissionsByMvcRoteInput input)
        {
            StringBuilder sql = new StringBuilder(string.Format(@"SELECT per.PrivilegeAccess FROM dbo.System_Permission per
                                                    LEFT JOIN dbo.System_PermissionUser perUser ON perUser.PrivilegeMasterValue = per.PrivilegeMasterValue WHERE perUser.PrivilegeMasterUserId=@userId
                                                    AND per.PrivilegeAccessValue IN(
                                                    SELECT menuButtonFunc.MenuButtonId FROM dbo.System_MenuButtonFunction menuButtonFunc
                                                    LEFT JOIN  dbo.System_Function func ON func.FunctionId = menuButtonFunc.FunctionId
                                                    WHERE Area=@area AND Controller=@controller AND [Action]=@action)
                                                    AND per.PrivilegeAccess={0}", (byte)EnumPrivilegeAccess.�˵���ť));
            sql.Append(" UNION ALL ");
            sql.Append(string.Format(@" SELECT per.PrivilegeAccess FROM dbo.System_Permission per
                                                    LEFT JOIN dbo.System_PermissionUser perUser ON perUser.PrivilegeMasterValue = per.PrivilegeMasterValue WHERE perUser.PrivilegeMasterUserId=@userId
                                                    AND per.PrivilegeAccessValue IN(
                                                    SELECT menu.MenuId FROM dbo.System_Menu menu
                                                    WHERE menu.Area=@area AND Controller=@controller AND [Action]=@action)
                                                    AND per.PrivilegeAccess={0}", (byte)EnumPrivilegeAccess.�˵�));
            return SqlMapperUtil.SqlWithParams<SystemPermission>(sql.ToString(), new
            {
                userId = input.UserId,
                area = input.Area,
                controller = input.Controller,
                action = input.Action
            });
        }

        /// <summary>
        /// ��ȡ�ֶ�Ȩ��Sql
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemField>> GetFieldPermission(SystemPermissionSqlInput input)
        {
            //���ô洢���̻�ȡ�ֶ���Ϣ
            const string procName = @"System_Proc_GetFiledSql";
            PermissionRouteConvert convert = new PermissionRouteConvert();
            MvcRote rote = convert.RoteConvert(input.EnumPermissionRoteConvert);
            return SqlMapperUtil.StoredProcWithParams<SystemField>(procName,
                 new
                 {
                     input.PrincipalUser.UserId,
                     rote.Area,
                     rote.Controller,
                     rote.Action
                 });
        }


        public Task<int> DeleteSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess privilegeAccess, Guid? privilegeAccessValue = null)
        {
            StringBuilder sql =
                new StringBuilder(
                    "DELETE FROM System_Permission WHERE PrivilegeAccess=@privilegeAccess");
            if (privilegeAccessValue != null)
            {
                sql.Append(" AND PrivilegeAccessValue=@privilegeAccessValue");
            }
            return (SqlMapperUtil.InsertUpdateOrDeleteSql<SystemPermission>(sql.ToString(), new { privilegeAccess, privilegeAccessValue }));
        }
    }
}