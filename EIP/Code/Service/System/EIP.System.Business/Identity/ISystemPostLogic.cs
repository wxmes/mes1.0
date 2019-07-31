using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;

namespace EIP.System.Business.Identity
{
    public interface ISystemPostLogic : IAsyncLogic<SystemPost>
    {
        /// <summary>
        ///     ������֯������ȡ��λ��Ϣ
        /// </summary>
        /// <param name="input">��֯����Id</param>
        /// <returns></returns>
        Task<IEnumerable<SystemPostOutput>> GetPostByOrganizationId(SystemPostGetByOrganizationId input);

        /// <summary>
        ///     ɾ����λ��Ϣ
        /// </summary>
        /// <param name="input">��λ��ϢId</param>
        /// <returns></returns>
        Task<OperateStatus> DeletePost(IdInput input);

        /// <summary>
        ///     �����λ��Ϣ
        /// </summary>
        /// <param name="post">��λ��Ϣ</param>
        /// <returns></returns>
        Task<OperateStatus> SavePost(SystemPost post);

        /// <summary>
        ///     ����
        /// </summary>
        /// <param name="input">������Ϣ</param>
        /// <returns></returns>
        Task<OperateStatus> CopyPost(SystemCopyInput input);
    }
}