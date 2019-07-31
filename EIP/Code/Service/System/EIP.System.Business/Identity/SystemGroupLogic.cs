using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Utils;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Core.Resource;
using EIP.System.Business.Permission;
using EIP.System.DataAccess.Identity;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using EIP.System.Models.Resx;

namespace EIP.System.Business.Identity
{
    public class SystemGroupLogic : DapperAsyncLogic<SystemGroup>, ISystemGroupLogic
    {
        #region ���캯��

        private readonly ISystemGroupRepository _groupRepository;
        private readonly ISystemPermissionUserLogic _permissionUserLogic;
        private readonly ISystemPermissionLogic _permissionLogic;
        private readonly ISystemOrganizationRepository _organizationRepository;
        public SystemGroupLogic(ISystemGroupRepository groupRepository,
            ISystemPermissionUserLogic permissionUserLogic,
            ISystemPermissionLogic permissionLogic, ISystemOrganizationRepository organizationRepository)
            : base(groupRepository)
        {
            _permissionUserLogic = permissionUserLogic;
            _permissionLogic = permissionLogic;
            _organizationRepository = organizationRepository;
            _groupRepository = groupRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     ������֯������ȡ����Ϣ
        /// </summary>
        /// <param name="input">��֯����</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemGroupOutput>> GetGroupByOrganizationId(SystemGroupGetGroupByOrganizationIdInput input)
        {
            var groups = (await _groupRepository.GetGroupByOrganizationId(input)).ToList();
            var allOrgs = (await _organizationRepository.FindAllAsync()).ToList();
            foreach (var group in groups)
            {
                group.BelongToName = EnumUtil.GetName(typeof(EnumGroupBelongTo), group.BelongTo);
                var organization = allOrgs.FirstOrDefault(w => w.OrganizationId == group.OrganizationId);
                if (organization != null && !organization.ParentIds.IsNullOrEmpty())
                {
                    foreach (var parent in organization.ParentIds.Split(','))
                    {
                        //�����ϼ�
                        var dicinfo = allOrgs.FirstOrDefault(w => w.OrganizationId.ToString() == parent);
                        if (dicinfo != null) group.OrganizationNames += dicinfo.Name + ">";
                    }
                    if (!group.OrganizationNames.IsNullOrEmpty())
                        group.OrganizationNames = group.OrganizationNames.TrimEnd('>');
                }
            }
            return groups;
        }

        /// <summary>
        ///     ��������Ϣ
        /// </summary>
        /// <param name="group">��λ��Ϣ</param>
        /// <param name="belongTo">����</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveGroup(SystemGroup group,
            EnumGroupBelongTo belongTo)
        {
            group.BelongTo = (byte)belongTo;
            if (group.GroupId.IsEmptyGuid())
            {
                group.CreateTime = DateTime.Now;
                group.GroupId = CombUtil.NewComb();
                return await InsertAsync(group);
            }
            var systemGroup = await GetByIdAsync(group.GroupId);
            group.CreateTime = systemGroup.CreateTime;
            group.CreateUserId = systemGroup.CreateUserId;
            group.CreateUserName = systemGroup.CreateUserName;
            group.UpdateTime = DateTime.Now;
            group.UpdateUserId = group.CreateUserId;
            group.UpdateUserName = group.CreateUserName;
            return await UpdateAsync(group);
        }

        /// <summary>
        ///     ɾ������Ϣ
        /// </summary>
        /// <param name="input">��Id</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteGroup(IdInput input)
        {
            var operateStatus = new OperateStatus();
            //�ж��Ƿ������Ա
            var permissionUsers = await
                _permissionUserLogic.GetPermissionUsersByPrivilegeMasterAdnPrivilegeMasterValue(EnumPrivilegeMaster.��,
                    input.Id);
            if (permissionUsers.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.������Ա;
                return operateStatus;
            }
            //�ж��Ƿ���а�ťȨ��
            var functionPermissions = await
                _permissionLogic.GetPermissionByPrivilegeMasterValue(
                    new SystemPermissionByPrivilegeMasterValueInput
                    {
                        PrivilegeAccess = EnumPrivilegeAccess.�˵���ť,
                        PrivilegeMasterValue = input.Id,
                        PrivilegeMaster = EnumPrivilegeMaster.��
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
                        PrivilegeMaster = EnumPrivilegeMaster.��
                    });

            if (menuPermissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message =  ResourceSystem.���в˵�Ȩ��;
                return operateStatus;
            }
            return await DeleteAsync(new SystemGroup { GroupId = input.Id });
        }


        /// <summary>
        ///     ����
        /// </summary>
        /// <param name="input">������Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> CopyGroup(SystemCopyInput input)
        {
            var operateStatus = new OperateStatus();
            try
            {
                //��ȡ��Ϣ
                var role = await GetByIdAsync(input.Id);
                role.GroupId = CombUtil.NewComb();
                role.Name = input.Name;
                role.CreateTime = DateTime.Now;

                //��ȡӵ�е�Ȩ�޼���Ա
                var allUser = (await _permissionUserLogic.GetPermissionUsersByPrivilegeMasterAdnPrivilegeMasterValue(EnumPrivilegeMaster.��,
                    input.Id)).ToList();
                var allPer = (await _permissionLogic.GetSystemPermissionsByPrivilegeMasterValueAndValue(EnumPrivilegeMaster.��, input.Id)).ToList();
                foreach (var user in allUser)
                {
                    user.PrivilegeMasterValue = role.GroupId;
                }
                foreach (var per in allPer)
                {
                    per.PrivilegeMasterValue = role.GroupId;
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
        #endregion
    }
}