using Microsoft.AspNetCore.Mvc;

namespace EIP.Areas.System.Controllers
{
    /// <summary>
    /// 监控
    /// </summary>
    [Area("System")]
    public class MonitorController : BaseController
    {
        /// <summary>
        /// 程序集
        /// </summary>
        /// <returns></returns>
        public IActionResult Assemblies()
        {
            return View();
        }
        /// <summary>
        /// 所有接口
        /// </summary>
        /// <returns></returns>
        public IActionResult Apis()
        {
            return View();
        }
    }
}