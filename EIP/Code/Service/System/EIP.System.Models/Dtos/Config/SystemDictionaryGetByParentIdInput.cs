﻿using System;
using EIP.Common.Models.Dtos;

namespace EIP.System.Models.Dtos.Config
{
    /// <summary>
    /// 根据父级查询
    /// </summary>
    public class SystemDictionaryGetByParentIdInput : SearchDto
    {
        public Guid? Id { get; set; }
    }
}