using System;

namespace EIP.Common.Models.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class DbAttribute : BaseAttribute
    {
        public DbAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Name { get; set; }
    }
}