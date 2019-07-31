using EIP.Common.WebApi;
using EIP.Common.WebApi.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace EIP.System.Api
{
    /// <summary>
    /// 监控
    /// </summary>
    public class MonitorController : BaseController
    {
        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("系统监控-方法-获取所有程序集")]
        public JsonResult GetAllAssemblies()
        {
            var list = new List<AssembliesOutput>();
            var deps = DependencyContext.Default;
            //var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");//排除所有的系统程序集、Nuget下载包
            foreach (var lib in deps.CompileLibraries)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    list.Add(new AssembliesOutput
                    {
                        Name = assembly.GetName().Name,
                        Version = assembly.GetName().Version.ToString(),
                        ClrVersion = assembly.ImageRuntimeVersion,
                        Location = assembly.Location
                    });
                }
                catch (Exception)
                {
                }
            }
            return JsonForGridLoadOnce(list);
        }

        /// <summary>
        /// 获取所有Api信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟","2018-04-12")]
        [Remark("用户控件-方法-选择所有下级字典:排除下级")]
        [AllowAnonymous]
        public JsonResult GetAllApi()
        {
            IList<MvcRote> rotes = new List<MvcRote>();
            var deps = DependencyContext.Default;

            foreach (var lib in deps.CompileLibraries)
            {
                if (lib.Name == "EIP")
                {
                    try
                    {
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                        var types = Assembly.LoadFile(assembly.Location).GetTypes();
                        //控制器
                        var baseControllerType = typeof(BaseController);
                        var controllerType = typeof(Controller);

                        //方法
                        var jsonType = typeof(JsonResult);
                        var taskJsonType = typeof(Task<JsonResult>);

                        foreach (var type in types)
                        {
                            //是否为控制器类型:Controller或者是BaseController
                            var isController = controllerType.IsAssignableFrom(type) || baseControllerType.IsAssignableFrom(type);
                            // 跳过不是Controller的类型
                            if (!isController)
                            {
                                continue;
                            }
                            //控制器名称
                            var controller = type.Name.Substring(0, type.Name.Length - "Controller".Length);
                            var methodInfos = type.GetMethods();
                            foreach (var method in methodInfos)
                            {
                                //是否为方法
                                bool isAction = jsonType.IsAssignableFrom(method.ReturnType) ||
                                                taskJsonType.IsAssignableFrom(method.ReturnType);
                                // 跳过不是Action的方法
                                if (!isAction || method.Name.ToLower() == "json")
                                {
                                    continue;
                                }
                                //方法名称
                                string action = method.Name;
                                //该方法、界面的描述
                                string description,
                                    byDeveloperCode = string.Empty,
                                    byDeveloperTime = string.Empty;

                                // 如果打有描述文本标记则使用描述文本作为操作说明，否则试用Action方法名
                                var descriptionAttrs = method.GetCustomAttributes(typeof(RemarkAttribute), false);
                                if (descriptionAttrs.Length > 0)
                                {
                                    description = ((RemarkAttribute)descriptionAttrs[0]).Describe;
                                    if (string.IsNullOrEmpty(description))
                                    {
                                        description = action;
                                    }
                                }
                                else
                                {
                                    description = action;
                                }

                                // 如果打有描述文本标记则使用描述文本作为编写者说明
                                var byAttrs = method.GetCustomAttributes(typeof(CreateByAttribute), false);
                                if (byAttrs.Length > 0)
                                {
                                    byDeveloperCode = ((CreateByAttribute)byAttrs[0]).Name;
                                    byDeveloperTime = ((CreateByAttribute)byAttrs[0]).Time;
                                }
                                rotes.Add(new MvcRote
                                {
                                    Controller = controller,
                                    Action = action,
                                    Description = description,
                                    ByDeveloperCode = byDeveloperCode,
                                    ByDeveloperTime = byDeveloperTime
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return JsonForGridLoadOnce(rotes, ex.Message);
                    }
                }
            }
            return JsonForGridLoadOnce(rotes);
        }
    }
}