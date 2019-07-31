using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.DataAccess;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Permission
{
    public interface ISystemMenuRepository : IAsyncRepository<SystemMenu>
    {
       
        /// <summary>
        ///     ��ѯ���в˵�
        /// </summary>
        /// <param name="haveUrl">�Ƿ���в˵�</param>
        /// <param name="isMenuShow"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetAllMenuTree(bool haveUrl = false,
            bool isMenuShow = false);

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
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SystemMenu>> GetMeunuByPId(IdInput input);

        /// <summary>
        /// ���ݸ�����ȡ����˵�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuGetMenuByParentIdOutput>> GetMenuByParentId(SystemMenuGetMenuByParentIdInput input);
    }
}