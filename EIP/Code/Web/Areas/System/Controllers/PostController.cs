﻿using Microsoft.AspNetCore.Mvc;

namespace EIP.Areas.System.Controllers
{
    [Area("System")]
    public class PostController : BaseController
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public IActionResult List()
        {
            return View();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit()
        {
            return View();
        }
    }
}