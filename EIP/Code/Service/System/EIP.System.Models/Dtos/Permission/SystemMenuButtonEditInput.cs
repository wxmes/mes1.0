using System;
using EIP.Common.Models.Dtos;

namespace EIP.System.Models.Dtos.Permission
{
    public class SystemMenuButtonEditInput:IdInput
    {
        public Guid? MenuId { get; set; }

        public Guid? MenuButtonId { get; set; }
    }
}