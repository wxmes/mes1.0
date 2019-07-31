using System;

namespace EIP.Common.Models.CustomAttributes
{
    /// <summary>
    /// 忽略字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreColumnAttribute : BaseAttribute
    {
    }
}
