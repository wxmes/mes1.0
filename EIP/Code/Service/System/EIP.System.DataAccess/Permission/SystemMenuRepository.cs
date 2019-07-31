using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EIP.Common.Core.Extensions;
using EIP.Common.Dapper;
using EIP.Common.DataAccess;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Permission
{
    public class SystemMenuRepository : DapperAsyncRepository<SystemMenu>, ISystemMenuRepository
    {
        /// <summary>
        ///     ��ѯ���в˵�
        /// </summary>
        /// <param name="haveUrl">�Ƿ���в˵�</param>
        /// <param name="isMenuShow">�Ƿ�˵���ʾ</param>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetAllMenuTree(bool haveUrl = false,
            bool isMenuShow = false)
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT menu.MenuId id,menu.ParentId parent,menu.name text,menu.icon,menu.remark");
            if (haveUrl)
            {
                sql.Append(",menu.OpenUrl url");
            }
            sql.Append(" FROM System_Menu menu ");
            if (isMenuShow)
            {
                sql.Append(" WHERE menu.IsShowMenu='true'");
            }
            sql.Append(" ORDER BY menu.OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString());
        }

        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SystemMenu>> GetMeunuByPId(IdInput input)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT * FROM System_Menu WHERE ParentId=@pid ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<SystemMenu>(sql.ToString(), new { pid = input.Id });
        }

        /// <summary>
        ///     ��ѯ���о��в˵�Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetHaveMenuPermissionMenu()
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT MenuId id,ParentId parent,name text,icon FROM System_Menu WHERE HaveMenuPermission=@haveMenuPermission AND IsFreeze=@isFreeze ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString(),
                new { haveMenuPermission = true, isFreeze = false });
        }

        /// <summary>
        ///     ��ѯ���о�������Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetHaveDataPermissionMenu()
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT MenuId id,ParentId parent,name text,icon FROM System_Menu WHERE HaveDataPermission=@haveDataPermission AND IsFreeze=@isFreeze ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString(),
                new { haveDataPermission = true, isFreeze = false });
        }

        /// <summary>
        ///     ��ѯ���о����ֶ�Ȩ�޵Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetHaveFieldPermissionMenu()
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT MenuId id,ParentId parent,name text,icon FROM System_Menu WHERE HaveFieldPermission=@haveFieldPermission AND IsFreeze=@isFreeze ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString(),
                new { haveFieldPermission = true, isFreeze = false });
        }

        /// <summary>
        ///     ��ѯ���о��й�����Ĳ˵�
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetHaveMenuButtonPermissionMenu()
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT MenuId id,ParentId parent,name text,icon FROM System_Menu WHERE HaveFunctionPermission=@haveFunctionPermission AND IsFreeze=@isFreeze ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString(),
                new { haveFunctionPermission = true, isFreeze = false });
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemMenuGetMenuByParentIdOutput>> GetMenuByParentId(SystemMenuGetMenuByParentIdInput input)
        {
            var sql = new StringBuilder();
            sql.Append("select *,(select name from System_Menu o where o.MenuId=menu.ParentId) ParentName from System_Menu menu Where 1=1 ");
            if (input.Id != null)
            {
                sql.Append(" And menu.ParentIds  like '%" + (input.Id + ",").Replace(",", @"\,") + "%" + "' escape '\\'");
            }
            sql.Append(input.Sql);
            return SqlMapperUtil.SqlWithParams<SystemMenuGetMenuByParentIdOutput>(sql.ToString(), new { pId = input.Id });
        }
    }
}