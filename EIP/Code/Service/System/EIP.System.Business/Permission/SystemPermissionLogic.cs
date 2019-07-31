using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Resource;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.DataAccess.Identity;
using EIP.System.DataAccess.Permission;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using Newtonsoft.Json;

namespace EIP.System.Business.Permission
{
    /// <summary>
    ///     Ȩ��ҵ���߼�
    /// </summary>
    public class SystemPermissionLogic : DapperAsyncLogic<SystemPermission>, ISystemPermissionLogic
    {
        #region ���캯��

        /// <summary>
        ///     �޲ι��캯��
        /// </summary>
        public SystemPermissionLogic(ISystemOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
            _menuButtonRepository = new SystemMenuButtonRepository();
            _userInfoRepository = new SystemUserInfoRepository();
        }

        #region ����ע��

        private readonly ISystemMenuButtonRepository _menuButtonRepository;
        private readonly ISystemPermissionRepository _permissionRepository;
        private readonly ISystemPermissionUserRepository _permissionUsernRepository;
        private readonly ISystemUserInfoRepository _userInfoRepository;
        private readonly ISystemMenuRepository _menuRepository;
        
        private readonly ISystemOrganizationRepository _organizationRepository;
        public SystemPermissionLogic(ISystemMenuButtonRepository menuButtonRepository,
            ISystemPermissionRepository permissionRepository,
            ISystemPermissionUserRepository permissionUserRepository,
            ISystemUserInfoRepository userInfoRepository,
            ISystemMenuRepository menuRepository,
             ISystemOrganizationRepository organizationRepository)
            : base(permissionRepository)
        {
            _menuButtonRepository = menuButtonRepository;
            _permissionRepository = permissionRepository;
            _permissionUsernRepository = permissionUserRepository;
            _userInfoRepository = userInfoRepository;
            _menuRepository = menuRepository;
            
            _organizationRepository = organizationRepository;
        }

        #endregion

        #endregion

        #region ����

        /// <summary>
        ///     ����״̬ΪTrue�Ĳ˵���Ϣ
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SystemPermission>> GetPermissionByPrivilegeMasterValue(SystemPermissionByPrivilegeMasterValueInput input)
        {
            return (await _permissionRepository.GetPermissionByPrivilegeMasterValue(input)).ToList();
        }

        /// <summary>
        ///     ����Ȩ����Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OperateStatus> SavePermission(SystemPermissionSaveInput input)
        {
            var operateStatus = new OperateStatus();
            try
            {
                IList<SystemPermission> systemPermissions = input.Permissiones.Select(per => new SystemPermission
                {
                    PrivilegeAccess = (byte)input.PrivilegeAccess,
                    PrivilegeAccessValue = per,
                    PrivilegeMasterValue = input.PrivilegeMasterValue,
                    PrivilegeMaster = (byte)input.PrivilegeMaster,
                    PrivilegeMenuId = input.PrivilegeMenuId
                }).ToList();

                //ɾ���ý�ɫ��Ȩ����Ϣ
                await _permissionRepository.DeletePermissionByPrivilegeMasterValue(input.PrivilegeAccess, input.PrivilegeMasterValue, input.PrivilegeMenuId);
                if (input.PrivilegeMaster == EnumPrivilegeMaster.��Ա)
                {
                    //ɾ����Ӧ��Ա����
                    await _permissionUsernRepository.DeletePermissionUser(input.PrivilegeMaster, input.PrivilegeMasterValue);
                    //�ж��Ƿ����Ȩ��
                    if (!systemPermissions.Any())
                    {
                        operateStatus.ResultSign = ResultSign.Successful;
                        operateStatus.Message = Chs.Successful;
                        return operateStatus;
                    }
                    //����Ȩ����Ա����
                    var permissionUser = new SystemPermissionUser
                    {
                        PrivilegeMaster = (byte)input.PrivilegeMaster,
                        PrivilegeMasterUserId = input.PrivilegeMasterValue,
                        PrivilegeMasterValue = input.PrivilegeMasterValue
                    };
                    await _permissionUsernRepository.InsertAsync(permissionUser);
                }

                //�Ƿ����Ȩ������
                if (!systemPermissions.Any())
                {
                    operateStatus.ResultSign = ResultSign.Successful;
                    operateStatus.Message = Chs.Successful;
                    return operateStatus;
                }
                //�������ݿ�
                await _permissionRepository.BulkInsertAsync(systemPermissions);
                operateStatus.ResultSign = ResultSign.Successful;
                operateStatus.Message = Chs.Successful;
                return operateStatus;
            }
            catch (Exception ex)
            {
                operateStatus.Message = ex.Message;
                return operateStatus;
            }
        }

        /// <summary>
        ///     �����û�Id��ȡ�û����еĲ˵�Ȩ��
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetSystemPermissionMenuByUserId(Guid userId)
        {
            IList<JsTreeEntity> treeEntities = new List<JsTreeEntity>();
            //�жϸ��û��Ƿ�Ϊ��������Ա:���ǳ�������Ա����ʾ���в˵�
            var userInfo = await _userInfoRepository.FindByIdAsync(userId);
            if (userInfo != null)
            {
                //����ǳ�������Ա
                if (userInfo.IsAdmin)
                {
                    treeEntities = (await _menuRepository.GetAllMenuTree(true, true)).ToList();
                    return treeEntities;
                }
                treeEntities = (await _permissionRepository.GetSystemPermissionMenuByUserId(userId)).ToList();
            }
            return treeEntities;
        }

        /// <summary>
        ///     ���ݽ�ɫId,��λId,��Id,��ԱId��ȡ���еĲ˵���Ϣ
        /// </summary>
        /// <param name="input">�������</param>
        /// <returns>���β˵���Ϣ</returns>
        public async Task<IEnumerable<JsTreeEntity>> GetMenuHavePermissionByPrivilegeMasterValue(SystemPermissiontMenuHaveByPrivilegeMasterValueInput input)
        {
            return (await _permissionRepository.GetMenuHavePermissionByPrivilegeMasterValue(input)).ToList();
        }

        /// <summary>
        ///     ��ѯ��Ӧӵ�еĹ�����˵���Ϣ
        /// </summary>
        /// <param name="privilegeMasterValue">��Ϣ</param>
        /// <param name="privilegeMaster"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemMenuButton>> GetFunctionByPrivilegeMaster(Guid privilegeMasterValue,
            EnumPrivilegeMaster privilegeMaster)
        {
            IList<SystemMenuButton> functions = new List<SystemMenuButton>();
            //��ȡӵ�еĲ˵���Ϣ
            var menus = await GetMenuHavePermissionByPrivilegeMasterValue(new SystemPermissiontMenuHaveByPrivilegeMasterValueInput
            {
                PrivilegeMasterValue = privilegeMasterValue,
                PrivilegeMaster = privilegeMaster,
                PrivilegeAccess = EnumPrivilegeAccess.�˵���ť
            });

            //��ȡӵ�еĹ�������Ϣ
            IList<SystemPermission> haveFunctions =
              (await GetPermissionByPrivilegeMasterValue(
                new SystemPermissionByPrivilegeMasterValueInput()
                {
                    PrivilegeAccess = EnumPrivilegeAccess.�˵���ť,
                    PrivilegeMasterValue = privilegeMasterValue,
                    PrivilegeMaster = privilegeMaster
                })).ToList();

            //��ȡ���й�����
            IList<SystemMenuButtonOutput> functionDtos = (await _menuButtonRepository.GetMenuButtonByMenuId()).ToList();
            foreach (var menu in menus)
            {
                var function = functionDtos.Where(w => w.MenuId == (Guid)menu.id).OrderBy(o => o.OrderNo);
                foreach (var f in function)
                {
                    var selectFunction = haveFunctions.Where(w => w.PrivilegeAccessValue == f.MenuButtonId);
                    f.Remark = selectFunction.Any() ? "selected" : "";
                    functions.Add(f);
                }
            }
            return functions;
        }

        /// <summary>
        ///     ��ȡ��¼��Ա��Ӧ�˵��µĹ�����
        /// </summary>
        /// <param name="mvcRote">·����Ϣ</param>
        /// <param name="userId">�û�Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemMenuButton>> GetFunctionByMenuIdAndUserId(MvcRote mvcRote,
            Guid userId)
        {
            //�жϵ�ǰ��Ա�Ƿ�Ϊ��������Ա���ǳ�������Ա��������Ȩ��
            IList<SystemMenuButton> functions = new List<SystemMenuButton>();
            //�жϸ��û��Ƿ�Ϊ��������Ա:���ǳ�������Ա����ʾ���в˵�
            var userInfo = await _userInfoRepository.FindByIdAsync(userId);
            if (userInfo != null)
            {
                //����ǳ�������Ա
                if (userInfo.IsAdmin)
                {
                    return (await _menuButtonRepository.GetMenuButtonByMvcRote(mvcRote)).ToList();
                }
                functions = (await _menuButtonRepository.GetMenuButtonByMenuIdAndUserId(mvcRote, userId)).ToList();
            }
            return functions;
        }

        /// <summary>
        ///     ��ȡ�˵���������ȱ�ʹ�õ�Ȩ����Ϣ
        /// </summary>
        /// <param name="privilegeAccess">����:�˵���������</param>
        /// <param name="privilegeAccessValue">��Ӧֵ</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemPermission>> GetSystemPermissionsByPrivilegeAccessAndValue(
            EnumPrivilegeAccess privilegeAccess,
            Guid? privilegeAccessValue = null)
        {
            return (await _permissionRepository.GetSystemPermissionsByPrivilegeAccessAndValue(privilegeAccess, privilegeAccessValue)).ToList();
        }

        /// <summary>
        /// ��ȡ��ɫ����Ⱦ��е�Ȩ��
        /// </summary>
        /// <param name="privilegeMaster">����:��ɫ����Ա�����</param>
        /// <param name="privilegeMasterValue">��Ӧֵ</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemPermission>> GetSystemPermissionsByPrivilegeMasterValueAndValue(EnumPrivilegeMaster privilegeMaster,
            Guid? privilegeMasterValue = null)
        {
            return (await _permissionRepository.GetSystemPermissionsByPrivilegeMasterValueAndValue(privilegeMaster, privilegeMasterValue)).ToList();
        }

        /// <summary>
        /// ���ݹ������ȡȨ����Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemPermission>> GetSystemPermissionsMvcRote(SystemPermissionsByMvcRoteInput input)
        {
            return (await _permissionRepository.GetSystemPermissionsMvcRote(input)).ToList();
        }

        /// <summary>
        /// ��ȡ�ֶ�Ȩ��Sql
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> GetFieldPermissionSql(SystemPermissionSqlInput input)
        {
            StringBuilder sql = new StringBuilder();
            //ƴ���ֶ�Ȩ��Sql
            IList<SystemField> fields = (await _permissionRepository.GetFieldPermission(input)).ToList();
            //�Ƿ�����ֶ�Ȩ��
            foreach (var field in fields)
            {
                sql.Append(field.SqlField + ",");
            }
            return sql.Length > 0 ? sql.ToString().TrimEnd(',') : "*";
        }

        
        /// <summary>
        /// �滻����Sql
        /// </summary>
        /// <param name="ruleSql"></param>
        /// <param name="userId">�û�Id</param>
        /// <returns></returns>
        public virtual async Task<string> GetRuleSql(string ruleSql, Guid userId)
        {
            //��ȡ��ɫ���顢��λ����
            IList<SystemPrivilegeDetailListOutput> privilegeDetailDtos = (await _permissionUsernRepository.GetSystemPrivilegeDetailOutputsByUserId(new IdInput { Id = userId })).ToList();

            if (ruleSql.Contains("{����}"))
            {
                ruleSql = ruleSql.Replace("{����}", "1=1");
            }
            if (ruleSql.Contains("{��ǰ�û�}"))
            {
                ruleSql = ruleSql.Replace("{��ǰ�û�}", userId.ToString());
            }
            if (ruleSql.Contains("{������֯}"))
            {
                //��ȡ��ǰ��Ա������֯
                ruleSql = ruleSql.Replace("{������֯}", privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��֯����).Select(d => d.PrivilegeMasterValue).ToList().ExpandAndToString().InSql());
            }
            if (ruleSql.Contains("{������֯���¼���֯}"))
            {
                //���һ���
                var orgId = privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��֯����)
                    .Select(d => d.PrivilegeMasterValue);
                //��ȡ��ǰ��Ա������֯���¼���֯
                ruleSql = ruleSql.Replace("{������֯���¼���֯}", (await _organizationRepository.GetOrganizationsByParentId(new SystemOrganizationsByParentIdInput { Id = orgId.FirstOrDefault() })).Select(s => s.OrganizationId).ToList().ExpandAndToString().InSql());
            }
            if (ruleSql.Contains("{������֯����}"))
            {
                //��ȡ��ǰ��Ա������֯
                ruleSql = ruleSql.Replace("{������֯����}", privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��֯����).Select(d => d.PrivilegeMasterValue).ToList().ExpandAndToString().InSql());
            }
            if (ruleSql.Contains("{���ڸ�λ}"))
            {
                //��ȡ��ǰ��Ա���ڸ�λ
                ruleSql = ruleSql.Replace("{���ڸ�λ}", privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��λ).Select(d => d.PrivilegeMasterValue).ToList().ExpandAndToString().InSql());
            }
            if (ruleSql.Contains("{���ڹ�����}"))
            {
                //��ȡ��ǰ��Ա���ڹ�����
                ruleSql = ruleSql.Replace("{���ڹ�����}", privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��).Select(d => d.PrivilegeMasterValue).ToList().ExpandAndToString().InSql());
            }
            return ruleSql;
        }

        public async Task<int> DeleteSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess privilegeAccess, Guid? privilegeAccessValue = null)
        {
            return await _permissionRepository.DeleteSystemPermissionsByPrivilegeAccessAndValue(privilegeAccess, privilegeAccessValue);
        }
        #endregion
    }
}