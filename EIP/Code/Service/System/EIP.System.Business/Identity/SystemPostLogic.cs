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
    public class SystemPostLogic : DapperAsyncLogic<SystemPost>, ISystemPostLogic
    {
        #region ���캯��

        private readonly ISystemPostRepository _postRepository;
        private readonly ISystemPermissionUserLogic _permissionUserLogic;
        private readonly ISystemPermissionLogic _permissionLogic;
        private readonly ISystemOrganizationRepository _organizationRepository;
        public SystemPostLogic(ISystemPostRepository postRepository,
            ISystemPermissionUserLogic permissionUserLogic,
            ISystemPermissionLogic permissionLogic, ISystemOrganizationRepository organizationRepository)
            : base(postRepository)
        {
            _permissionUserLogic = permissionUserLogic;
            _permissionLogic = permissionLogic;
            _organizationRepository = organizationRepository;
            _postRepository = postRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     ������֯������ȡ��λ��Ϣ
        /// </summary>
        /// <param name="input">��֯����Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemPostOutput>> GetPostByOrganizationId(SystemPostGetByOrganizationId input)
        {
            var data = (await _postRepository.GetPostByOrganizationId(input)).ToList();
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
        ///     ����
        /// </summary>
        /// <param name="input">������Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> CopyPost(SystemCopyInput input)
        {
            var operateStatus = new OperateStatus();
            try
            {
                //��ȡ��Ϣ
                var role = await GetByIdAsync(input.Id);
                role.PostId = CombUtil.NewComb();
                role.Name = input.Name;
                role.CreateTime = DateTime.Now;

                //��ȡӵ�е�Ȩ�޼���Ա
                var allUser = (await _permissionUserLogic.GetPermissionUsersByPrivilegeMasterAdnPrivilegeMasterValue(EnumPrivilegeMaster.��λ,
                    input.Id)).ToList();
                var allPer = (await _permissionLogic.GetSystemPermissionsByPrivilegeMasterValueAndValue(EnumPrivilegeMaster.��λ, input.Id)).ToList();
                foreach (var user in allUser)
                {
                    user.PrivilegeMasterValue = role.PostId;
                }
                foreach (var per in allPer)
                {
                    per.PrivilegeMasterValue = role.PostId;
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
        ///     �����λ��Ϣ
        /// </summary>
        /// <param name="post">��λ��Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> SavePost(SystemPost post)
        {
            if (post.PostId.IsEmptyGuid())
            {
                post.CreateTime = DateTime.Now;
                post.PostId = CombUtil.NewComb();
                return await InsertAsync(post);
            }
            SystemPost systemPost =await GetByIdAsync(post.PostId);
            post.CreateTime = systemPost.CreateTime;
            post.CreateUserId = systemPost.CreateUserId;
            post.CreateUserName = systemPost.CreateUserName;
            post.UpdateTime = DateTime.Now;
            post.UpdateUserId = post.CreateUserId;
            post.UpdateUserName = post.CreateUserName;
            return await UpdateAsync(post);
        }

        /// <summary>
        ///     ɾ����λ��Ϣ
        /// </summary>
        /// <param name="input">��λ��ϢId</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeletePost(IdInput input)
        {
            var operateStatus = new OperateStatus();
            //�ж��Ƿ������Ա
            var permissionUsers =await  _permissionUserLogic.GetPermissionUsersByPrivilegeMasterAdnPrivilegeMasterValue(EnumPrivilegeMaster.��λ,
                    input.Id);
            if (permissionUsers.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.������Ա;
                return operateStatus;
            }
            //�ж��Ƿ���а�ťȨ��
            var functionPermissions =await 
                _permissionLogic.GetPermissionByPrivilegeMasterValue(
                new SystemPermissionByPrivilegeMasterValueInput
                {
                    PrivilegeAccess = EnumPrivilegeAccess.�˵���ť,
                    PrivilegeMasterValue = input.Id,
                    PrivilegeMaster = EnumPrivilegeMaster.��λ
                });
            if (functionPermissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.���й�����Ȩ��;
                return operateStatus;
            }
            //�ж��Ƿ���в˵�Ȩ��
            var menuPermissions =await 
                _permissionLogic.GetPermissionByPrivilegeMasterValue(
                new SystemPermissionByPrivilegeMasterValueInput
                {
                    PrivilegeAccess = EnumPrivilegeAccess.�˵�,
                    PrivilegeMasterValue = input.Id,
                    PrivilegeMaster = EnumPrivilegeMaster.��λ
                });
            if (menuPermissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message =  ResourceSystem.���в˵�Ȩ��;
                return operateStatus;
            }
            return await DeleteAsync(new SystemPost
            {
                PostId = input.Id
            });
        }

        #endregion
    }
}