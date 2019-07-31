using System;
using EIP.Common.Core.Auth;
using EIP.Common.Models.Dtos;

namespace EIP.System.Models.Dtos.Identity
{
    /// <summary>
    /// 获取
    /// </summary>
    public class SystemOrganizationDataPermissionTreeInput : SearchDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 登录人信息
        /// </summary>
        public PrincipalUser PrincipalUser { get; set; }

        public bool HaveSelf { get; set; }


    }
}