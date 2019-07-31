using System;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Paging;
using EIP.System.Models.Enums;

namespace EIP.System.Models.Dtos.Identity
{
    /// <summary>
    /// 用户信息Dto
    /// </summary>
    public class SystemUserPagingInput : QueryParam, IInputDto
    {
        /// <summary>
        /// 对应Id值
        /// </summary>
        public Guid? PrivilegeMasterValue { get; set; }

        /// <summary>
        /// 归属人员类型:组织机构、角色、岗位、组
        /// </summary>
        public EnumPrivilegeMaster PrivilegeMaster { get; set; }
    }
}