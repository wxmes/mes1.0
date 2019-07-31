using System.Collections.Generic;


namespace EIP.System.Models.Dtos.Identity
{
    public class SystemUserGetRoleUsersOutput
    {
        /// <summary>
        /// 已有人员
        /// </summary>
        public IList<string> HaveUser { get; set; }
    }
}