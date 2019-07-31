using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.DataAccess;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Config;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Config
{
    /// <summary>
    /// �ֵ����ݷ��ʽӿ�
    /// </summary>
    public interface ISystemDictionaryRepository : IAsyncRepository<SystemDictionary>
    {
        /// <summary>
        ///     ���������ֶ���Ϣ
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetDictionaryTree();

        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        Task<IEnumerable<SystemDictionaryGetByParentIdOutput>> GetDictionariesParentId(SystemDictionaryGetByParentIdInput input);
        
        /// <summary>
        ///   ����ParentId��ȡ�����¼�
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetDictionaryTreeByParentIds(IdInput input);
        
    }
}