﻿using System;
using EIP.Common.Models.Dtos;

namespace EIP.System.Models.Dtos.Permission
{
    public class SystemMenuGetMenuByParentIdInput:SearchDto
    {
        public Guid? Id { get; set; }
    }
}