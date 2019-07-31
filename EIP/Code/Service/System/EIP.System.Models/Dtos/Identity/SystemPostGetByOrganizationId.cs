using System;
using EIP.Common.Models.Dtos;

namespace EIP.System.Models.Dtos.Identity
{
    public class SystemPostGetByOrganizationId : SearchDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }
    }
}