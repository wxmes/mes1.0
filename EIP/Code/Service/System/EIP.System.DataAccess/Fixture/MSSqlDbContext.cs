using System.Data.SqlClient;
using EIP.Common.Dapper;
using EIP.Common.Dapper.DbContext;
using EIP.Common.Dapper.SqlGenerator;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Fixture
{
    /// <summary>
    /// 
    /// </summary>
    public class MsSqlDbContext : DapperDbContext, IDbContext
    {
        private readonly SqlGeneratorConfig _config = new SqlGeneratorConfig
        {
            SqlConnector = ESqlConnector.MSSQL,
            UseQuotationMarks = true
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public MsSqlDbContext(string connectionString)
            : base(new SqlConnection(connectionString))
        {
        }

        private IDapperRepository<SystemDictionary> _systemDictionary;
        /// <summary>
        /// 字典信息表
        /// </summary>
        public IDapperRepository<SystemDictionary> SystemDictionary => _systemDictionary ?? (_systemDictionary = new DapperRepository<SystemDictionary>(Connection, _config));

        private IDapperRepository<SystemDistrict> _systemDistrict;
        /// <summary>
        /// 省市区县
        /// </summary>
        public IDapperRepository<SystemDistrict> SystemDistrict => _systemDistrict ?? (_systemDistrict = new DapperRepository<SystemDistrict>(Connection, _config));

        private IDapperRepository<SystemGroup> _systemGroup;
        /// <summary>
        /// 组信息维护
        /// </summary>
        public IDapperRepository<SystemGroup> SystemGroup => _systemGroup ?? (_systemGroup = new DapperRepository<SystemGroup>(Connection, _config));

        private IDapperRepository<SystemMenu> _systemMenu;
        /// <summary>
        /// 系统菜单
        /// </summary>
        public IDapperRepository<SystemMenu> SystemMenu => _systemMenu ?? (_systemMenu = new DapperRepository<SystemMenu>(Connection, _config));

        private IDapperRepository<SystemMenuButton> _systemMenuButton;
        /// <summary>
        /// 菜单按钮记录表
        /// </summary>
        public IDapperRepository<SystemMenuButton> SystemMenuButton => _systemMenuButton ?? (_systemMenuButton = new DapperRepository<SystemMenuButton>(Connection, _config));

        private IDapperRepository<SystemOrganization> _systemOrganization;
        /// <summary>
        /// 组织机构信息表
        /// </summary>
        public IDapperRepository<SystemOrganization> SystemOrganization => _systemOrganization ?? (_systemOrganization = new DapperRepository<SystemOrganization>(Connection, _config));

        private IDapperRepository<SystemPermission> _systemPermission;
        /// <summary>
        /// 权限记录表
        /// </summary>
        public IDapperRepository<SystemPermission> SystemPermission => _systemPermission ?? (_systemPermission = new DapperRepository<SystemPermission>(Connection, _config));

        private IDapperRepository<SystemPermissionUser> _systemPermissionUser;
        /// <summary>
        /// 权限用户记录表:组织机构、角色、岗位、组下的人员
        /// </summary>
        public IDapperRepository<SystemPermissionUser> SystemPermissionUser => _systemPermissionUser ?? (_systemPermissionUser = new DapperRepository<SystemPermissionUser>(Connection, _config));

        private IDapperRepository<SystemPost> _systemPost;
        /// <summary>
        /// 岗位信息记录表
        /// </summary>
        public IDapperRepository<SystemPost> SystemPost => _systemPost ?? (_systemPost = new DapperRepository<SystemPost>(Connection, _config));

        private IDapperRepository<SystemRole> _systemRole;
        /// <summary>
        /// 系统角色记录表
        /// </summary>
        public IDapperRepository<SystemRole> SystemRole => _systemRole ?? (_systemRole = new DapperRepository<SystemRole>(Connection, _config));

        private IDapperRepository<SystemUserInfo> _systemUserInfo;
        /// <summary>
        /// 系统使用人员
        /// </summary>
        public IDapperRepository<SystemUserInfo> SystemUserInfo => _systemUserInfo ?? (_systemUserInfo = new DapperRepository<SystemUserInfo>(Connection, _config));
    }
}