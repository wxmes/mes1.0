using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.System.Models.Entities;

namespace EIP.System.Business.Permission
{
    /// <summary>
    /// 功能项业务逻辑接口
    /// </summary>
    public interface ISystemFunctionLogic : IAsyncLogic<SystemFunction>
    {
        /// <summary>
        /// 保存功能项信息
        /// </summary>
        /// <param name="rotes"></param>
        /// <returns></returns>
        Task<OperateStatus> SaveFunction(IList<MvcRote> rotes);

        /// <summary>
        /// 根据代码获取该系统的功能项信息
        /// </summary>
        /// <param name="input">代码值</param>
        /// <returns></returns>
        Task<IEnumerable<SystemFunction>> GetFunctionsByAppCode(IdInput<string> input = null);
    }
}