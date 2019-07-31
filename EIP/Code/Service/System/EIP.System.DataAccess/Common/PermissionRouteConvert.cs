using EIP.Common.Models;
using EIP.System.Models.Enums;

namespace EIP.System.DataAccess.Common
{
    /// <summary>
    /// 权限路由转换
    /// </summary>
    public class PermissionRouteConvert
    {
        /// <summary>
        /// 路由转换
        /// </summary>
        /// <param name="roteConvert">转换类型枚举</param>
        public virtual MvcRote RoteConvert(EnumPermissionRoteConvert roteConvert)
        {
            MvcRote mvcRote = new MvcRote();
            switch (roteConvert)
            {
                case EnumPermissionRoteConvert.组织机构数据权限:
                    mvcRote.AppCode = "EIP";
                    mvcRote.Area = "System";
                    mvcRote.Controller = "Organization";
                    mvcRote.Action = "List";
                    break;
                case EnumPermissionRoteConvert.人员数据权限:
                    mvcRote.AppCode = "EIP";
                    mvcRote.Area = "System";
                    mvcRote.Controller = "User";
                    mvcRote.Action = "List";
                    break;
            }
            return mvcRote;
        }
    }
}