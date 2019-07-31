using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Core.Resource;
using EIP.Common.Core.Utils;
using EIP.System.Business.Permission;
using EIP.System.DataAccess.Identity;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using EIP.System.Models.Resx;

namespace EIP.System.Business.Identity
{
    public class SystemRoleLogic : DapperAsyncLogic<SystemRole>, ISystemRoleLogic
    {
        #region ���캯��

        private readonly ISystemRoleRepository _roleRepository;
        private readonly ISystemPermissionUserLogic _permissionUserLogic;
        private readonly ISystemPermissionLogic _permissionLogic;
        private readonly ISystemOrganizationRepository _organizationRepository;
        public SystemRoleLogic(ISystemRoleRepository roleRepository,
            ISystemPermissionUserLogic permissionUserLogic,
            ISystemPermissionLogic permissionLogic,
            ISystemOrganizationRepository organizationRepository)
            : base(roleRepository)
        {
            _permissionUserLogic = permissionUserLogic;
            _permissionLogic = permissionLogic;
            _organizationRepository = organizationRepository;
            _roleRepository = roleRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     ������֯����Id��ѯ��ɫ��Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemRoleOutput>> GetRolesByOrganizationId(SystemRolesGetByOrganizationId input)
        {
            var data = (await _roleRepository.GetRolesByOrganizationId(input)).ToList();
            var allOrgs = (await _organizationRepository.FindAllAsync()).ToList();
            foreach (var user in data)
            {
                var organization = allOrgs.FirstOrDefault(w => w.OrganizationId == user.OrganizationId);
                if (organization != null && !organization.ParentIds.IsNullOrEmpty())
                {
                    foreach (var parent in organization.ParentIds.Split(','))
                    {
                        //�����ϼ�
                        var dicinfo = allOrgs.FirstOrDefault(w => w.OrganizationId.ToString() == parent);
                        if (dicinfo != null) user.OrganizationNames += dicinfo.Name + ">";
                    }
                    if (!user.OrganizationNames.IsNullOrEmpty())
                        user.OrganizationNames = user.OrganizationNames.TrimEnd('>');
                }
            }
            return data;
        }

       

        /// <summary>
        ///     �����λ��Ϣ
        /// </summary>
        /// <param name="role">��λ��Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveRole(SystemRole role)
        {
            role.CanbeDelete = true;
            if (role.RoleId.IsEmptyGuid())
            {
                role.CreateTime = DateTime.Now;
                role.RoleId = Guid.NewGuid();
                return await InsertAsync(role);
            }
            var systemRole = await GetByIdAsync(role.RoleId);
            role.CreateTime = systemRole.CreateTime;
            role.CreateUserId = systemRole.CreateUserId;
            role.CreateUserName = systemRole.CreateUserName;
            role.UpdateTime = DateTime.Now;
            role.UpdateUserId = role.CreateUserId;
            role.UpdateUserName = role.CreateUserName;
            return await UpdateAsync(role);
        }

        /// <summary>
        ///     ��ɫ����
        /// </summary>
        /// <param name="input">��ɫId</param>
        /// <returns></returns>
        public async Task<OperateStatus> CopyRole(SystemCopyInput input)
        {
            var operateStatus = new OperateStatus();
            try
            {
                //��ȡ��ɫ��Ϣ
                var role = await GetByIdAsync(input.Id);
                role.RoleId = CombUtil.NewComb();
                role.Name = input.Name;
                role.CreateTime=DateTime.Now;
                
                //��ȡ�ý�ɫӵ�е�Ȩ�޼���Ա
                var allUser = (await _permissionUserLogic.GetPermissionUsersByPrivilegeMasterAdnPrivilegeMasterValue(EnumPrivilegeMaster.��ɫ,
                    input.Id)).ToList();
                var allPer = (await _permissionLogic.GetSystemPermissionsByPrivilegeMasterValueAndValue(EnumPrivilegeMaster.��ɫ, input.Id)).ToList();
                foreach (var user in allUser)
                {
                    user.PrivilegeMasterValue = role.RoleId;
                }
                foreach (var per in allPer)
                {
                    per.PrivilegeMasterValue = role.RoleId;
                }
                //��������
                operateStatus = await _permissionUserLogic.InsertMultipleAsync(allUser);
                operateStatus = await _permissionLogic.InsertMultipleAsync(allPer);
                operateStatus = await InsertAsync(role);
                operateStatus.Message = Chs.Successful;
                operateStatus.ResultSign = ResultSign.Successful;
            }
            catch (Exception e)
            {
                operateStatus.Message = e.Message;
            }
            return operateStatus;
        }

        /// <summary>
        ///     ��ȡ���û��Ѿ����еĽ�ɫ��Ϣ
        /// </summary>
        /// <param name="privilegeMaster"></param>
        /// <param name="userId">��Ҫ��ѯ���û�id</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemRole>> GetHaveUserRole(EnumPrivilegeMaster privilegeMaster,
            Guid userId)
        {
            return await _roleRepository.GetHaveUserRole(privilegeMaster, userId);
        }

        /// <summary>
        ///     ɾ����ɫ��Ϣ
        /// </summary>
        /// <param name="input">��ɫId</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteRole(IdInput input)
        {
            var operateStatus = new OperateStatus();
            //�ж��Ƿ������Ա
            var permissionUsers = await
                _permissionUserLogic.GetPermissionUsersByPrivilegeMasterAdnPrivilegeMasterValue(EnumPrivilegeMaster.��ɫ,
                    input.Id);
            if (permissionUsers.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message =  ResourceSystem.������Ա;
                return operateStatus;
            }
            //�ж��Ƿ���а�ťȨ��
            var functionPermissions = await
                _permissionLogic.GetPermissionByPrivilegeMasterValue(
                    new SystemPermissionByPrivilegeMasterValueInput
                    {
                        PrivilegeAccess = EnumPrivilegeAccess.�˵���ť,
                        PrivilegeMasterValue = input.Id,
                        PrivilegeMaster = EnumPrivilegeMaster.��ɫ
                    });
            if (functionPermissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message =  ResourceSystem.���й�����Ȩ��;
                return operateStatus;
            }
            //�ж��Ƿ���в˵�Ȩ��
            var menuPermissions = await
                _permissionLogic.GetPermissionByPrivilegeMasterValue(
                    new SystemPermissionByPrivilegeMasterValueInput
                    {
                        PrivilegeAccess = EnumPrivilegeAccess.�˵�,
                        PrivilegeMasterValue = input.Id,
                        PrivilegeMaster = EnumPrivilegeMaster.��ɫ
                    });
            if (menuPermissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message =  ResourceSystem.���в˵�Ȩ��;
                return operateStatus;
            }
            return await DeleteAsync(new SystemRole
            {
                RoleId = input.Id
            });
        }

        #endregion
    }
}