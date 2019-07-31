using Microsoft.AspNetCore.Mvc;

namespace EIP
{
    /// <summary>
    /// 添加缓存
    /// </summary>
    [ResponseCache(CacheProfileName = "EipCacheProfiles")]
    public class BaseController : Controller
    {

    }
}