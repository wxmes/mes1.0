using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EIP.Common.Dapper;
using EIP.Common.DataAccess;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Config;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Config
{
    /// <summary>
    ///     �ֵ����ݷ��ʽӿ�ʵ��
    /// </summary>
    public class SystemDictionaryRepository : DapperAsyncRepository<SystemDictionary>, ISystemDictionaryRepository
    {
        /// <summary>
        ///     ���������ֶ���Ϣ
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetDictionaryTree()
        {
            var sql = new StringBuilder();
            sql.Append("SELECT DictionaryId id,ParentId parent,name text FROM System_Dictionary ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString());
        }

        /// <summary>
        ///     �����ֵ�����ȡ��Ӧ�¼�ֵ
        /// </summary>
        /// <param name="input">����ֵ</param>
        /// <returns></returns>
        public Task<IEnumerable<SystemDictionaryGetByParentIdOutput>> GetDictionariesParentId(SystemDictionaryGetByParentIdInput input)
        {
            var sql = new StringBuilder();
            sql.Append("select *,(select name from System_Dictionary d where d.DictionaryId=dic.ParentId) ParentName from System_Dictionary dic WHERE 1=1 ");
            if (input.Id != null)
            {
                sql.Append(" and dic.ParentIds  like '%" + (input.Id + ",").Replace(",", @"\,") + "%" + "' escape '\\'");
            }
            sql.Append(input.Sql);
            sql.Append(" ORDER BY dic.OrderNo");
            return SqlMapperUtil.SqlWithParams<SystemDictionaryGetByParentIdOutput>(sql.ToString());
        }

        /// <summary>
        /// ����ParentIds��ȡ�����¼�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<JsTreeEntity>> GetDictionaryTreeByParentIds(IdInput input)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT DictionaryId id,ParentId parent,name text FROM System_Dictionary WHERE ParentIds like '%" +input.Id + "%' ORDER BY OrderNo");
            return SqlMapperUtil.SqlWithParams<JsTreeEntity>(sql.ToString());
        }
    }
}