using System.Reflection;
using EIP.Common.Dapper.Attributes;
using EIP.Common.Models.CustomAttributes;
using ColumnAttribute = System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;

namespace EIP.Common.Dapper.SqlGenerator
{
    /// <summary>
    ///     Metadata from PropertyInfo
    /// </summary>
    public class SqlPropertyMetadata
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlPropertyMetadata(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            var alias = PropertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (!string.IsNullOrEmpty(alias?.Name))
            {
                Alias = alias.Name;
                ColumnName = Alias;
            }
            else
            {
                ColumnName = PropertyInfo.Name;
            }
            var ignoreColumn = PropertyInfo.GetCustomAttribute<IgnoreColumnAttribute>();
            if (ignoreColumn != null)
                IgnoreUpdate = true;
        }

        /// <summary>
        ///     Original PropertyInfo
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        ///     Alias for ColumnName
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        ///     ColumnName
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///     Exclude property from update
        /// </summary>
        public bool IgnoreUpdate { get; set; }

        /// <summary>
        ///     PropertyName
        /// </summary>
        public virtual string PropertyName => PropertyInfo.Name;
    }
}