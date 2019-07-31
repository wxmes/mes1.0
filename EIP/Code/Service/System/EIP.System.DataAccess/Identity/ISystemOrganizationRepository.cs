using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.DataAccess;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Identity
{
    public interface ISystemOrganizationRepository : IAsyncRepository<SystemOrganization>
    {
        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetSystemOrganizationByPid(IdInput input);

        /// <summary>
        ///     ��ȡ������֯������Ϣ
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetSystemOrganization();

        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemOrganizationOutput>> GetOrganizationsByParentId(SystemOrganizationDataPermissionTreeInput input);

        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemOrganizationOutput>> GetOrganizationsByParentId(SystemOrganizationsByParentIdInput input);

        /// <summary>
        /// ����Ȩ����֯������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetOrganizationResultByDataPermission(IdInput<string> input);

        /// <summary>
        /// ��ȡ��������Ȩ�޵���֯��������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetOrganizationDataPermissionTree(
            SystemOrganizationDataPermissionTreeInput input);
    }
}