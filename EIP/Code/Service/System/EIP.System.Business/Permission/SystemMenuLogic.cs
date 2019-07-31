using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.Common.Core.Resource;
using EIP.Common.Core.Utils;
using EIP.System.DataAccess.Permission;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using EIP.System.Models.Resx;

namespace EIP.System.Business.Permission
{
    public class SystemMenuLogic : DapperAsyncLogic<SystemMenu>, ISystemMenuLogic
    {
        #region ���캯��

        private readonly ISystemMenuRepository _menuRepository;
        private readonly ISystemMenuButtonRepository _menuButtonRepository;
        private readonly ISystemPermissionLogic _permissionLogic;

        public SystemMenuLogic(ISystemMenuRepository menuRepository,
            ISystemPermissionLogic permissionLogic, ISystemMenuButtonRepository menuButtonRepository)
            : base(menuRepository)
        {
            _menuButtonRepository = menuButtonRepository;
            _permissionLogic = permissionLogic;
            _menuRepository = menuRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     ����״̬ΪTrue�Ĳ˵���Ϣ
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetAllMenuTree()
        {
            return await _menuRepository.GetAllMenuTree();
        }

        /// <summary>
        ///     ����״̬ΪTrue�Ĳ˵���Ϣ
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SystemMenu>> GetMeunuByPId(IdInput input)
        {
            return await _menuRepository.GetMeunuByPId(input);
        }

        /// <summary>
        ///     ����˵�
        /// </summary>
        /// <param name="menu">�˵���Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveMenu(SystemMenu menu)
        {
            OperateStatus result;
            menu.CanbeDelete = true;
            if (menu.MenuId.IsEmptyGuid())
            {
                menu.MenuId = CombUtil.NewComb();
                result = await InsertAsync(menu);
            }
            else
            {
                result = await UpdateAsync(menu);
            }
            await GeneratingParentIds();
            return result;
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
                var alls = (await GetAllEnumerableAsync()).ToList();
                var tops = alls.Where(w => w.ParentId == Guid.Empty);
                foreach (var org in tops)
                {
                    org.ParentIds = org.MenuId.ToString();
                    await UpdateAsync(org);
                    await GeneratingParentIds(org, alls.ToList(), "");
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
        /// <param name="menu"></param>
        /// <param name="menus"></param>
        /// <param name="orgId"></param>
        private async Task GeneratingParentIds(SystemMenu menu, IList<SystemMenu> menus, string orgId)
        {
            string parentIds = menu.MenuId.ToString();
            //��ȡ�¼�
            var next = menus.Where(w => w.ParentId == menu.MenuId).ToList();
            if (next.Any())
            {
                parentIds = orgId.IsNullOrEmpty() ? parentIds : orgId + "," + parentIds;
            }
            foreach (var or in next)
            {
                or.ParentIds = parentIds + "," + or.MenuId;
                await UpdateAsync(or);
                await GeneratingParentIds(or
                    , menus, parentIds);
            }
        }

        /// <summary>
        ///     ɾ���˵����¼�����
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteMenu(IdInput input)
        {
            var operateStatus = new OperateStatus();

            //�жϸ����ܷ����ɾ��
            var menu = await GetByIdAsync(input.Id);

            if (menu != null && !menu.CanbeDelete)
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = Chs.CanotDelete;
                return operateStatus;
            }
            //�鿴�Ƿ�����¼�
            if ((await GetMeunuByPId(input)).Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.�����¼���;
                return operateStatus;
            }
            //�鿴�ò˵��Ƿ��ѱ�����ռ��
            var permissions = await _permissionLogic.GetSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess.�˵�, input.Id);
            if (permissions.Any())
            {
                //��ȡ��ռ�����ͼ�ֵ
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.�ѱ�����Ȩ��;
                return operateStatus;
            }
            return await DeleteAsync(new SystemMenu { MenuId = input.Id });
        }


        /// <summary>
        ///     ��ѯ���о��в˵�Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetHaveMenuPermissionMenu()
        {
            return await _menuRepository.GetHaveMenuPermissionMenu();
        }

        /// <summary>
        ///     ��ѯ���о�������Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetHaveDataPermissionMenu()
        {
            return await _menuRepository.GetHaveDataPermissionMenu();
        }

        /// <summary>
        ///     ��ѯ���о����ֶ�Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetHaveFieldPermissionMenu()
        {
            return await _menuRepository.GetHaveFieldPermissionMenu();
        }

        /// <summary>
        ///     ��ѯ���о��й�����Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetHaveMenuButtonPermissionMenu()
        {
            return await _menuRepository.GetHaveMenuButtonPermissionMenu();
        }

        /// <summary>
        /// ��ȡ��ʾ�ڲ˵��б�������
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SystemMenuGetMenuByParentIdOutput>> GetMenuByParentId(SystemMenuGetMenuByParentIdInput input)
        {
            var menus = (await GetAllEnumerableAsync()).ToList();
            IList<SystemMenuGetMenuByParentIdOutput> systemMenus = (await _menuRepository.GetMenuByParentId(input)).ToList();
            foreach (var menu in systemMenus)
            {
                if (!menu.ParentIds.IsNullOrEmpty())
                {
                    foreach (var parent in menu.ParentIds.Split(','))
                    {
                        //�����ϼ�
                        var dicinfo = menus.FirstOrDefault(w => w.MenuId.ToString() == parent);
                        if (dicinfo != null) menu.ParentNames += dicinfo.Name + ">";
                    }
                    if (!menu.ParentNames.IsNullOrEmpty())
                        menu.ParentNames = menu.ParentNames.TrimEnd('>');
                }
            }
            return systemMenus.OrderBy(o => o.ParentNames).ToList();
        }


        #region ����ɾ��Demo

        /// <summary>
        ///     ɾ���˵����¼�����
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteMenuAndChilds(IdInput<string> input)
        {
            var operateStatus = new OperateStatus();
            //�жϸ����ܷ����ɾ��
            var menu = await GetByIdAsync(input.Id);
            if (!menu.CanbeDelete)
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = Chs.CanotDelete;
                return operateStatus;
            }
            foreach (var id in input.Id.Split(','))
            {
                Guid menuId = Guid.Parse(id);
                MenuDeletGuid.Add(menuId);
                await GetMenuDeleteGuid(menuId);
                foreach (var delete in MenuDeletGuid)
                {
                    await _permissionLogic.DeleteSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess.�˵�, delete);
                    //ɾ����Ӧ��ť����ťȨ��
                    var functions = await _menuButtonRepository.GetMenuButtonByMenuId(new SystemMenuGetMenuButtonByMenuIdInput{ Id = delete });
                    foreach (var item in functions)
                    {
                        await _permissionLogic.DeleteSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess.�˵���ť, item.MenuButtonId);
                        await _menuButtonRepository.DeleteAsync(new SystemMenuButton { MenuButtonId = item.MenuButtonId });
                    }
                    await DeleteAsync(new SystemMenu
                    {
                        MenuId = delete
                    });
                }
            }
            operateStatus.ResultSign = ResultSign.Successful;
            operateStatus.Message = Chs.Successful;
            return operateStatus;
        }

        /// <summary>
        ///     ɾ����������
        /// </summary>
        public IList<Guid> MenuDeletGuid = new List<Guid>();

        /// <summary>
        ///     ��ȡɾ��������Ϣ
        /// </summary>
        /// <param name="guid"></param>
        private async Task GetMenuDeleteGuid(Guid guid)
        {
            //��ȡ�¼�
            var menus = await _menuRepository.GetMeunuByPId(new IdInput(guid));
            if (menus.Count() > 0)
            {
                foreach (var dic in menus)
                {
                    MenuDeletGuid.Add(dic.MenuId);
                    await GetMenuDeleteGuid(dic.MenuId);
                }
            }
        }

        #endregion

        #endregion


    }
}