using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.DataAccess;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;

namespace EIP.System.DataAccess.Permission
{
    public interface ISystemPermissionRepository : IAsyncRepository<SystemPermission>
    {
        /// <summary>
        ///     ����Ȩ�޹���Id��ѯ�˵�Ȩ����Ϣ
        /// </summary>
        /// <param name="input">Ȩ������:�˵�����������ݡ��ֶΡ��ļ�</param>
        /// <returns></returns>
        Task<IEnumerable<SystemPermission>> GetPermissionByPrivilegeMasterValue(SystemPermissionByPrivilegeMasterValueInput input);

        /// <summary>
        ///     ����Ȩ�޹���Idɾ���˵�Ȩ����Ϣ
        /// </summary>
        /// <param name="privilegeAccess">Ȩ������:�˵�����������ݡ��ֶΡ��ļ�</param>
        /// <param name="privilegeMasterValue"></param>
        /// <param name="privilegeMenuId"></param>
        /// <returns></returns>
        Task<bool> DeletePermissionByPrivilegeMasterValue(EnumPrivilegeAccess? privilegeAccess,
            Guid privilegeMasterValue,
            Guid? privilegeMenuId);

        /// <summary>
        ///     �����û�Id��ȡ�û����еĲ˵�Ȩ��
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetSystemPermissionMenuByUserId(Guid userId);

        /// <summary>
        ///     ���ݽ�ɫId,��λId,��Id,��ԱId��ȡ���еĲ˵���Ϣ
        /// </summary>
        /// <param name="input">�������</param>
        /// <returns>���β˵���Ϣ</returns>
        Task<IEnumerable<JsTreeEntity>> GetMenuHavePermissionByPrivilegeMasterValue(SystemPermissiontMenuHaveByPrivilegeMasterValueInput input);

        /// <summary>
        /// ��ȡ�˵���������ȱ�ʹ�õ�Ȩ����Ϣ
        /// </summary>
        /// <param name="privilegeAccess">����:�˵���������</param>
        /// <param name="privilegeAccessValue">��Ӧֵ</param>
        /// <returns></returns>
        Task<IEnumerable<SystemPermission>> GetSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess privilegeAccess,
            Guid? privilegeAccessValue = null);

        /// <summary>
        /// ��ȡ��ɫ����Ⱦ��е�Ȩ��
        /// </summary>
        /// <param name="privilegeMaster">����:��ɫ����Ա�����</param>
        /// <param name="privilegeMasterValue">��Ӧֵ</param>
        /// <returns></returns>
        Task<IEnumerable<SystemPermission>> GetSystemPermissionsByPrivilegeMasterValueAndValue(
            EnumPrivilegeMaster privilegeMaster,
            Guid? privilegeMasterValue = null);

        /// <summary>
        /// ɾ����ȡ�˵���������ȱ�ʹ�õ�Ȩ����Ϣ
        /// </summary>
        /// <param name="privilegeAccess">����:�˵���������</param>
        /// <param name="privilegeAccessValue">��Ӧֵ</param>
        /// <returns></returns>
        Task<int> DeleteSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess privilegeAccess,
            Guid? privilegeAccessValue = null);

        /// <summary>
        /// ���ݹ������ȡȨ����Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemPermission>> GetSystemPermissionsMvcRote(SystemPermissionsByMvcRoteInput input);

        /// <summary>
        /// ��ȡ�ֶ�Ȩ��Sql
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemField>> GetFieldPermission(SystemPermissionSqlInput input);

       
    }
}