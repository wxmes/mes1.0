using Microsoft.AspNetCore.Mvc;

namespace EIP.Areas.Common.Controllers
{
    [Area("Common")]
    public class ErrorController : BaseController
    {
        public IActionResult NotFind()
        {
            return View();
        }
    }
}