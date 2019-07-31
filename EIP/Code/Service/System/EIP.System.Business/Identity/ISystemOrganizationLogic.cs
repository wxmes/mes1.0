using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;

namespace EIP.System.Business.Identity
{
    public interface ISystemOrganizationLogic : IAsyncLogic<SystemOrganization>
    {
        /// <summary>
        ///     �첽��ȡ������
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetOrganizationTreeAsync(IdInput input);

        /// <summary>
        /// ��ȡ��֯������Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetSystemOrganizationByPid(IdInput input);

        /// <summary>
        /// ��ȡ��������Ȩ�޵���֯��������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetOrganizationDataPermissionTree(
            SystemOrganizationDataPermissionTreeInput input);

        /// <summary>
        ///     ͬ����ȡ����������
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetOrganizationTree();

        /// <summary>
        ///     ͬ����ȡ����������
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SystemOrganizationChartOutput>> GetOrganizationChart();

        /// <summary>
        ///     ���ݸ�����ѯ�¼�
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        Task<IEnumerable<SystemOrganizationOutput>> GetOrganizationsByParentId(SystemOrganizationDataPermissionTreeInput input);

        /// <summary>
        ///     ������֯����
        /// </summary>
        /// <param name="organization">��֯����</param>
        /// <returns></returns>
        Task<OperateStatus> SaveOrganization(SystemOrganization organization);

        /// <summary>
        ///     ɾ����֯�����¼�����
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        Task<OperateStatus> DeleteOrganization(IdInput input);

        /// <summary>
        /// ����Ȩ����֯������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<JsTreeEntity>> GetOrganizationResultByDataPermission(IdInput<string> input);

    }
}