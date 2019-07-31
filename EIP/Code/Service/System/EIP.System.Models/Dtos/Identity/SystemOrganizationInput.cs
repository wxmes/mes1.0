using System;
using EIP.Common.Models.Dtos;

namespace EIP.System.Models.Dtos.Identity
{
    public class SystemOrganizationInput:IdInput
    {
        /// <summary>
        /// 父级Id
        /// </summary>
        public Guid ParentId { get; set; }
    }
}