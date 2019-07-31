using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;

namespace EIP.System.Business.Permission
{
    public interface ISystemMenuLogic : IAsyncLogic<SystemMenu>
    {
        #region �˵�
        
        /// <summary>
        ///     ����״̬ΪTrue�Ĳ˵���Ϣ
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetAllMenuTree();

        /// <summary>
        ///     ��ѯ���о��в˵�Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetHaveMenuPermissionMenu();

        /// <summary>
        ///     ��ѯ���о�������Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetHaveDataPermissionMenu();

        /// <summary>
        ///     ��ѯ���о����ֶ�Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetHaveFieldPermissionMenu();

        /// <summary>
        ///     ��ѯ���о��й�����Ĳ˵�
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetHaveMenuButtonPermissionMenu();

        /// <summary>
        ///     ����״̬ΪTrue�Ĳ˵���Ϣ
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SystemMenu>> GetMeunuByPId(IdInput input);

        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <param name="menu">����id</param>
        /// <returns></returns>
        Task<OperateStatus> SaveMenu(SystemMenu menu);

        /// <summary>
        ///     ɾ���˵����¼�����
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        Task<OperateStatus> DeleteMenu(IdInput input);
        /// <summary>
        ///     ɾ���˵����¼�����
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        Task<OperateStatus> DeleteMenuAndChilds(IdInput<string> input);
      
        /// <summary>
        /// ��ȡ��ʾ�ڲ˵��б�������
        /// </summary>
        /// <returns></returns>
        Task<IList<SystemMenuGetMenuByParentIdOutput>> GetMenuByParentId(SystemMenuGetMenuByParentIdInput input);
        #endregion
    }
}