using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EIP.System.Business.Permission
{
    public interface ISystemPermissionLogic : IAsyncLogic<SystemPermission>
    {
        /// <summary>
        /// ����״̬ΪTrue�Ĳ˵���Ϣ
        /// </summary>
        /// <param name="input">Ȩ������</param>
        /// <returns></returns>
        Task<IEnumerable<SystemPermission>> GetPermissionByPrivilegeMasterValue(SystemPermissionByPrivilegeMasterValueInput input);

        /// <summary>
        ///     ����Ȩ����Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<OperateStatus> SavePermission(SystemPermissionSaveInput input);

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
        ///     ������Ȩ���Ͳ�ѯ��Ӧӵ�еĹ�����˵���Ϣ
        /// </summary>
        /// <param name="privilegeMasterValue">��Ȩid</param>
        /// <param name="privilegeMaster">��Ȩö��</param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButton>> GetFunctionByPrivilegeMaster(Guid privilegeMasterValue,
            EnumPrivilegeMaster privilegeMaster);

        /// <summary>
        ///     ��ȡ��¼��Ա��Ӧ�˵��µĹ�����
        /// </summary>
        /// <param name="mvcRote">·����Ϣ</param>
        /// <param name="userId">�û�Id</param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButton>> GetFunctionByMenuIdAndUserId(MvcRote mvcRote,
            Guid userId);

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
    }
}