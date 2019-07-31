using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Utils;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.System.DataAccess.Permission;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIP.System.Business.Permission
{
    /// <summary>
    ///     ������ҵ���߼�
    /// </summary>
    public class SystemMenuButtonLogic : DapperAsyncLogic<SystemMenuButton>, ISystemMenuButtonLogic
    {
        #region ���캯��
        private readonly ISystemMenuRepository _menuRepository;
        private readonly ISystemMenuButtonRepository _functionRepository;
        private readonly ISystemPermissionLogic _permissionLogic;

        public SystemMenuButtonLogic(ISystemMenuButtonRepository functionRepository,
            ISystemPermissionLogic permissionLogic, ISystemMenuRepository menuRepository)
            : base(functionRepository)
        {
            _menuRepository = menuRepository;
            _permissionLogic = permissionLogic;
            _functionRepository = functionRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     ���ݲ˵���ȡ��������Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(SystemMenuGetMenuButtonByMenuIdInput input)
        {
            var functions = (await _functionRepository.GetMenuButtonByMenuId(input)).ToList();
            var menus = (await _menuRepository.FindAllAsync()).ToList();
            foreach (var item in functions)
            {
                var menu = menus.FirstOrDefault(w => w.MenuId == item.MenuId);
                if (menu != null && !menu.ParentIds.IsNullOrEmpty())
                {
                    foreach (var parent in menu.ParentIds.Split(','))
                    {
                        //�����ϼ�
                        var dicinfo = menus.FirstOrDefault(w => w.MenuId.ToString() == parent);
                        if (dicinfo != null) item.MenuNames += dicinfo.Name + ">";
                    }
                    if (!item.MenuNames.IsNullOrEmpty())
                        item.MenuNames = item.MenuNames.TrimEnd('>');
                }
            }
            return functions.OrderBy(o => o.MenuNames).ThenBy(b => b.OrderNo).ToList();
        }

        /// <summary>
        ///     ���湦������Ϣ
        /// </summary>
        /// <param name="input">��������Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveMenuButton(SystemMenuButtonSaveInput input)
        {
            SystemMenuButton button = input.MapTo<SystemMenuButton>();
            if (button.MenuButtonId.IsEmptyGuid())
            {
                button.MenuButtonId = CombUtil.NewComb();
                return await InsertAsync(button);
            }
            return await UpdateAsync(button);
        }

        /// <summary>
        ///     ɾ��������
        /// </summary>
        /// <param name="input">��������Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteMenuButton(IdInput input)
        {
            var operateStatus = new OperateStatus();
            //�鿴�ù������Ƿ��ѱ�����ռ��
            var permissions = await _permissionLogic.DeleteSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess.�˵���ť, input.Id);
            return await DeleteAsync(new SystemMenuButton
            {
                MenuButtonId = input.Id
            });
        }

        /// <summary>
        ///     ��ȡ��¼��Ա��Ӧ�˵��µĹ�����
        /// </summary>
        /// <param name="mvcRote">·����Ϣ</param>
        /// <param name="userId">�û�Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemMenuButton>> GetMenuButtonByMenuIdAndUserId(MvcRote mvcRote,
            Guid userId)
        {
            return (await _functionRepository.GetMenuButtonByMenuIdAndUserId(mvcRote, userId)).ToList();
        }

        #endregion
    }
}