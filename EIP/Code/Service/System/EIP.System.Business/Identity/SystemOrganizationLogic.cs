using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Resource;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Business.Permission;
using EIP.System.DataAccess.Identity;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using EIP.System.Models.Resx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIP.System.Business.Identity
{
    public class SystemOrganizationLogic : DapperAsyncLogic<SystemOrganization>, ISystemOrganizationLogic
    {
        #region ���캯��

        private readonly ISystemOrganizationRepository _organizationRepository;
        private readonly ISystemPermissionUserLogic _permissionUserLogic;
        private readonly ISystemPermissionLogic _permissionLogic;
        private readonly ISystemGroupLogic _groupLogic;
        private readonly ISystemRoleLogic _roleLogic;
        private readonly ISystemPostLogic _postLogic;
        public SystemOrganizationLogic(ISystemOrganizationRepository organizationRepository,
            ISystemPermissionUserLogic permissionUserLogic,
            ISystemPermissionLogic permissionLogic,
            ISystemGroupLogic groupLogic,
            ISystemRoleLogic roleLogic,
            ISystemPostLogic postLogic)
            : base(organizationRepository)
        {
            _permissionUserLogic = permissionUserLogic;
            _permissionLogic = permissionLogic;
            _groupLogic = groupLogic;
            _roleLogic = roleLogic;
            _postLogic = postLogic;
            _organizationRepository = organizationRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     �첽��ȡ������
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetOrganizationTreeAsync(IdInput input)
        {
            var lists = (await _organizationRepository.GetSystemOrganizationByPid(input)).ToList();
            foreach (var list in lists)
            {
                var info = (await _organizationRepository.GetSystemOrganizationByPid(input)).ToList();
                if (info.Count > 0)
                {
                    list.parent = true;
                }
            }
            return lists;
        }

        /// <summary>
        ///     ��ȡ������
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetSystemOrganizationByPid(IdInput input)
        {
            return (await _organizationRepository.GetSystemOrganizationByPid(input)).ToList();
        }

        /// <summary>
        ///     ͬ����ȡ����������
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetOrganizationTree()
        {
            return await _organizationRepository.GetSystemOrganization();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SystemOrganizationChartOutput>> GetOrganizationChart()
        {
            var allOrgs = (await GetAllEnumerableAsync()).ToList();
            var topOrgs = allOrgs.Where(w => w.ParentId == Guid.Empty).ToList();
            //��ģ��
            IList<SystemOrganizationChartOutput> outputs = new List<SystemOrganizationChartOutput>(topOrgs.Count);
            foreach (var root in topOrgs)
            {
                outputs.Add(new SystemOrganizationChartOutput
                {
                    id = root.OrganizationId,
                    name = root.Name,
                    title = root.MainSupervisor.IsNullOrEmpty() ? "" : root.MainSupervisor
                });
            }
            //������ģ��
            foreach (var permission in outputs)
            {
                //�ж��ж��ٸ�ģ��
                IList<SystemOrganization> perRoots =
                    allOrgs.Where(f => f.ParentId.ToString() == permission.id.ToString()).ToList();
                IList<SystemOrganizationChartOutput> trees = new List<SystemOrganizationChartOutput>();
                SystemOrganizationChartOutput tree = null;
                foreach (var treeEntity in perRoots)
                {
                    tree = new SystemOrganizationChartOutput
                    {
                        name = treeEntity.Name,
                        title = treeEntity.MainSupervisor.IsNullOrEmpty() ? "" : treeEntity.MainSupervisor,
                        children = GetWdChildNodes(ref allOrgs, treeEntity)
                    };
                    trees.Add(tree);
                }
                permission.children = trees;
                tree = null;
            }
            return outputs;
        }

        /// <summary>
        ///     ���ݵ�ǰ�ڵ㣬�����ӽڵ�
        /// </summary>
        /// <param name="treeEntitys">TreeEntity�ļ���</param>
        /// <param name="currTreeEntity">��ǰ�ڵ�</param>
        private IList<SystemOrganizationChartOutput> GetWdChildNodes(ref List<SystemOrganization> treeEntitys,
            SystemOrganization currTreeEntity)
        {
            IList<SystemOrganization> childNodes =
                treeEntitys.Where(f => f.ParentId.ToString() == currTreeEntity.OrganizationId.ToString()).ToList();
            if (childNodes.Count <= 0)
            {
                return null;
            }
            IList<SystemOrganizationChartOutput> childTrees = new List<SystemOrganizationChartOutput>(childNodes.Count);
            SystemOrganizationChartOutput tree = null;
            foreach (var treeEntity in childNodes)
            {
                tree = new SystemOrganizationChartOutput
                {
                    name = treeEntity.Name,
                    title = treeEntity.MainSupervisor.IsNullOrEmpty() ? "" : treeEntity.MainSupervisor,
                    children = GetWdChildNodes(ref treeEntitys, treeEntity)
                };
                childTrees.Add(tree);
            }
            return childTrees;
        }

        /// <summary>
        ///     ��������Ȩ����֯����
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetOrganizationDataPermissionTree(SystemOrganizationDataPermissionTreeInput input)
        {
            var datas = (await _organizationRepository.GetOrganizationDataPermissionTree(input)).ToList();
            foreach (var d in datas)
            {
                if (d.parent.ToString() == Guid.Empty.ToString())
                {
                    d.parent = "#";
                }
                d.state = new JsTreeStateEntity();

            }
            return datas;
        }

        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemOrganizationOutput>> GetOrganizationsByParentId(SystemOrganizationDataPermissionTreeInput input)
        {
            var allOrgs = (await GetAllEnumerableAsync()).ToList();
            IList<SystemOrganizationOutput> organizations = (await _organizationRepository.GetOrganizationsByParentId(input)).ToList();
            foreach (var organization in organizations)
            {
                if (!organization.ParentIds.IsNullOrEmpty())
                {

                    foreach (var parent in organization.ParentIds.Split(','))
                    {
                        //�����ϼ�
                        var dicinfo = allOrgs.FirstOrDefault(w => w.OrganizationId.ToString() == parent);
                        if (dicinfo != null) organization.ParentNames += dicinfo.Name + ">";
                    }
                    if (!organization.ParentNames.IsNullOrEmpty())
                        organization.ParentNames = organization.ParentNames.TrimEnd('>');
                }
            }
            return organizations;
        }

        /// <summary>
        ///     ������֯����
        /// </summary>
        /// <param name="organization">��֯����</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveOrganization(SystemOrganization organization)
        {
            OperateStatus operateStatus;
            if (organization.OrganizationId.IsEmptyGuid())
            {
                organization.CreateTime = DateTime.Now;
                organization.OrganizationId = Guid.NewGuid();
                operateStatus = await InsertAsync(organization);
            }
            else
            {
                organization.UpdateTime = DateTime.Now;
                organization.UpdateUserId = organization.CreateUserId;
                organization.UpdateUserName = organization.CreateUserName;
                SystemOrganization systemOrganization = await GetByIdAsync(organization.OrganizationId);
                organization.CreateTime = systemOrganization.CreateTime;
                organization.CreateUserId = systemOrganization.CreateUserId;
                organization.CreateUserName = systemOrganization.CreateUserName;
                operateStatus = await UpdateAsync(organization);
            }
            GeneratingParentIds();
            return operateStatus;
        }

        /// <summary>
        ///     ɾ����֯�����¼�����
        ///     ɾ������:
        ///     1:û���¼��˵�
        ///     2:û��Ȩ������
        ///     3:û����Ա
        /// </summary>
        /// <param name="input">��֯����id</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteOrganization(IdInput input)
        {
            var operateStatus = new OperateStatus();
            //�ж��¼��˵�
            IList<JsTreeEntity> orgs = (await _organizationRepository.GetSystemOrganizationByPid(input)).ToList();
            if (orgs.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.�����¼���;
                return operateStatus;
            }

            //�ж��Ƿ������Ա
            var permissionUsers = await
                _permissionUserLogic.GetPermissionUsersByPrivilegeMasterAdnPrivilegeMasterValue(
                    EnumPrivilegeMaster.��֯����,
                    input.Id);
            if (permissionUsers.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.������Ա;
                return operateStatus;
            }

            //�ж��Ƿ��н�ɫ
            var orgRole = await
               _roleLogic.GetRolesByOrganizationId(new SystemRolesGetByOrganizationId { Id = input.Id });
            if (orgRole.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.���н�ɫ;
                return operateStatus;
            }

            //�ж��Ƿ�����
            var orgGroup = await
                _groupLogic.GetGroupByOrganizationId(new SystemGroupGetGroupByOrganizationIdInput
                {
                    Id = input.Id
                });
            if (orgGroup.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.������;
                return operateStatus;
            }

            //�ж��Ƿ��и�λ
            var orgPost = await
               _postLogic.GetPostByOrganizationId(new SystemPostGetByOrganizationId { Id = input.Id });
            if (orgPost.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.���и�λ;
                return operateStatus;
            }

            //�ж��Ƿ���а�ťȨ��
            var functionPermissions = await
                _permissionLogic.GetPermissionByPrivilegeMasterValue(
                  new SystemPermissionByPrivilegeMasterValueInput()
                  {
                      PrivilegeAccess = EnumPrivilegeAccess.�˵���ť,
                      PrivilegeMasterValue = input.Id,
                      PrivilegeMaster = EnumPrivilegeMaster.��֯����
                  });
            if (functionPermissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.���й�����Ȩ��;
                return operateStatus;
            }
            //�ж��Ƿ���в˵�Ȩ��
            var menuPermissions = await
                _permissionLogic.GetPermissionByPrivilegeMasterValue(
                 new SystemPermissionByPrivilegeMasterValueInput()
                 {
                     PrivilegeAccess = EnumPrivilegeAccess.�˵�,
                     PrivilegeMasterValue = input.Id,
                     PrivilegeMaster = EnumPrivilegeMaster.��֯����
                 });
            if (menuPermissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.���в˵�Ȩ��;
                return operateStatus;
            }
            //����ɾ������
            return await DeleteAsync(new SystemOrganization()
            {
                OrganizationId = input.Id
            });
        }
        /// <summary>
        ///     �������ɴ���
        /// </summary>
        /// <returns></returns>
        public async Task<OperateStatus> GeneratingParentIds()
        {
            OperateStatus operateStatus = new OperateStatus();
            try
            {
                var dics = (await GetAllEnumerableAsync()).ToList();
                var topOrgs = dics.Where(w => w.ParentId == Guid.Empty);
                foreach (var org in topOrgs)
                {
                    org.ParentIds = org.OrganizationId.ToString();
                    await UpdateAsync(org);
                    await GeneratingParentIds(org, dics.ToList(), "");
                }
            }
            catch (Exception ex)
            {
                operateStatus.Message = ex.Message;
                return operateStatus;
            }
            operateStatus.Message = Chs.Successful;
            operateStatus.ResultSign = ResultSign.Successful;
            return operateStatus;
        }
        /// <summary>
        /// �ݹ��ȡ����
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="organizations"></param>
        /// <param name="orgId"></param>
        private async Task GeneratingParentIds(SystemOrganization organization, IList<SystemOrganization> organizations, string orgId)
        {
            string parentIds = organization.OrganizationId.ToString();
            //��ȡ�¼�
            var nextOrgs = organizations.Where(w => w.ParentId == organization.OrganizationId).ToList();
            if (nextOrgs.Any())
            {
                parentIds = orgId.IsNullOrEmpty() ? parentIds : orgId + "," + parentIds;
            }
            foreach (var or in nextOrgs)
            {
                or.ParentIds = parentIds + "," + or.OrganizationId;
                await UpdateAsync(or);
                await GeneratingParentIds(or
                    , organizations, parentIds);
            }
        }

        /// <summary>
        /// ����Ȩ����֯������
        /// </summary>
        ///  <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetOrganizationResultByDataPermission(IdInput<string> input)
        {
            return await _organizationRepository.GetOrganizationResultByDataPermission(input);
        }
        #endregion
    }
}