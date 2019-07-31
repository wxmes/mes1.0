using EIP.Common.Dapper;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Fixture
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// 字典信息表
        /// </summary>
        IDapperRepository<SystemDictionary> SystemDictionary { get; }
        /// <summary>
        /// 省市区县
        /// </summary>
        IDapperRepository<SystemDistrict> SystemDistrict { get; }
        /// <summary>
        /// 组信息维护
        /// </summary>
        IDapperRepository<SystemGroup> SystemGroup { get; }
        /// <summary>
        /// 系统菜单
        /// </summary>
        IDapperRepository<SystemMenu> SystemMenu { get; }
        /// <summary>
        /// 菜单按钮记录表
        /// </summary>
        IDapperRepository<SystemMenuButton> SystemMenuButton { get; }
        /// <summary>
        /// 组织机构信息表
        /// </summary>
        IDapperRepository<SystemOrganization> SystemOrganization { get; }
        /// <summary>
        /// 权限记录表
        /// </summary>
        IDapperRepository<SystemPermission> SystemPermission { get; }
        /// <summary>
        /// 权限用户记录表:组织机构、角色、岗位、组下的人员
        /// </summary>
        IDapperRepository<SystemPermissionUser> SystemPermissionUser { get; }
        /// <summary>
        /// 岗位信息记录表
        /// </summary>
        IDapperRepository<SystemPost> SystemPost { get; }
        /// <summary>
        /// 系统角色记录表
        /// </summary>
        IDapperRepository<SystemRole> SystemRole { get; }
        /// <summary>
        /// 系统使用人员
        /// </summary>
        IDapperRepository<SystemUserInfo> SystemUserInfo { get; }
    }
}