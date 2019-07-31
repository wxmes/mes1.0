using EIP.Common.WebApi.Attribute;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Dtos.Reports;
using EIP.Common.Models.Paging;
using EIP.Common.WebApi;
using EIP.System.Business.Log;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIP.Common.Core.Extensions;
using EIP.System.Models.Dtos.Log;
using EIP.System.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using EIP.Common.Core.Utils;

namespace EIP.System.Api
{
    /// <summary>
    ///     日志管理控制器
    /// </summary>
    [Authorize]
    public class LogController : BaseController
    {
        #region 构造函数

        private readonly ISystemExceptionLogLogic _exceptionLogLogic;
        private readonly ISystemLoginLogLogic _loginLogLogic;
        private readonly ISystemOperationLogLogic _operationLogLogic;
        private readonly ISystemDataLogLogic _dataLogLogic;
        private readonly ISystemSqlLogLogic _sqlLogLogic;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionLogLogic"></param>
        /// <param name="loginLogLogic"></param>
        /// <param name="operationLogLogic"></param>
        /// <param name="dataLogLogic"></param>
        /// <param name="sqlLogLogic"></param>
        public LogController(ISystemExceptionLogLogic exceptionLogLogic,
            ISystemLoginLogLogic loginLogLogic,
            ISystemOperationLogLogic operationLogLogic,
            ISystemDataLogLogic dataLogLogic, ISystemSqlLogLogic sqlLogLogic)
        {
            _operationLogLogic = operationLogLogic;
            _dataLogLogic = dataLogLogic;
            _sqlLogLogic = sqlLogLogic;
            _exceptionLogLogic = exceptionLogLogic;
            _loginLogLogic = loginLogLogic;
        }

        #endregion

        #region 数据日志

        /// <summary>
        ///     获取所有数据日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("数据日志-方法-列表-获取所有数据日志")]
        public async Task<JsonResult> GetPagingDataLog(SystemDataLogGetPagingInput paging)
        {
            var list = new List<FilterDefinition<SystemDataLog>>
            {
                Builders<SystemDataLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            if (!paging.Name.IsNullOrEmpty())
                list.Add(Builders<SystemDataLog>.Filter.Where(w => w.CreateUserName.Contains(paging.Name)));

            var filter = Builders<SystemDataLog>.Filter.And(list);
            return JsonForGridPaging(await _dataLogLogic.PagingQueryProcAsync(filter, paging));
        }

        /// <summary>
        ///     根据主键获取数据日志
        /// </summary>
        /// <param name="input">主键Id</param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("数据日志-方法-列表-根据主键获取数据日志")]
        public async Task<JsonResult> GetDataLogById(IdInput input)
        {
            var filter = Builders<SystemDataLog>.Filter.Where(w => w.DataLogId == input.Id);
            return Json(await _dataLogLogic.FindOneAsync(filter));
        }
        #endregion

        #region 异常日志

        /// <summary>
        ///     获取所有异常信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-获取所有异常信息")]
        public async Task<JsonResult> GetPagingExceptionLog(SystemExceptionLogGetPagingInput paging)
        {
            var list = new List<FilterDefinition<SystemExceptionLog>>
            {
                Builders<SystemExceptionLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            if (!paging.Name.IsNullOrEmpty())
                list.Add(Builders<SystemExceptionLog>.Filter.Where(w => w.CreateUserName.Contains(paging.Name)));
            if (!paging.Code.IsNullOrEmpty())
                list.Add(Builders<SystemExceptionLog>.Filter.Where(w => w.CreateUserCode.Contains(paging.Code)));
            if (!paging.CreateTime.IsNullOrEmpty())
                list.Add(Builders<SystemExceptionLog>.Filter.Where(w => w.CreateTime <= paging.EndCreateTime && w.CreateTime >= paging.BeginCreateTime));

            var filter = Builders<SystemExceptionLog>.Filter.And(list);
            var sort = Builders<SystemExceptionLog>.Sort.Descending(d => d.CreateTime);
            var datas = await _exceptionLogLogic.PagingQueryProcAsync(filter, paging, sort);
            foreach (var data in datas.Data)
            {
                data.CreateTime = data.CreateTime.ToServerLocalTime();
            }
            return JsonForGridPaging(datas);
        }

        /// <summary>
        ///     根据主键获取异常明细
        /// </summary>
        /// <param name="input">主键Id</param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键获取异常明细")]
        public async Task<JsonResult> GetExceptionLogById(IdInput input)
        {
            var filter = Builders<SystemExceptionLog>.Filter.Where(w => w.ExceptionLogId == input.Id);
            return Json(await _exceptionLogLogic.FindOneAsync(filter));
        }

        /// <summary>
        ///     根据主键删除异常信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除异常信息")]
        public async Task<JsonResult> DeleteExceptionLogById(IdInput<string> input)
        {
            var list = new List<FilterDefinition<SystemExceptionLog>>();
            foreach (var id in input.Id.Split(','))
            {
                list.Add(Builders<SystemExceptionLog>.Filter.Where(w => w.ExceptionLogId == Guid.Parse(id)));
            }
            var filter = Builders<SystemExceptionLog>.Filter.And(list);
            return Json(await _exceptionLogLogic.DeleteAsync(filter));
        }

        /// <summary>
        ///     根据主键删除异常信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除异常信息")]
        public async Task<JsonResult> DeleteExceptionLogAll()
        {
            var list = new List<FilterDefinition<SystemExceptionLog>>
            {
                Builders<SystemExceptionLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            var filter = Builders<SystemExceptionLog>.Filter.And(list);
            return Json(await _exceptionLogLogic.DeleteAsync(filter));
        }

        /// <summary>
        ///     导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-导出到Excel")]
        public async Task<FileResult> ExportExcelToExceptionLog(QueryParam paging)
        {
            ExcelReportDto excelReportDto = new ExcelReportDto()
            {
                //TemplatePath = Server.MapPath("/") + "DataUser/Templates/System/Log/异常日志.xlsx",
                DownTemplatePath = "异常日志" + string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + ".xlsx",
                Title = "异常日志.xlsx"
            };
            await _exceptionLogLogic.ReportExcelExceptionLogQuery(paging, excelReportDto);
            //return File(new FileStream(excelReportDto.DownPath, FileMode.Open), "application/octet-stream", Server.UrlEncode(excelReportDto.Title));
            return null;
        }

        #endregion

        #region 登录日志

        /// <summary>
        ///     获取所有登录日志信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("登录日志-方法-列表-获取所有登录日志信息")]
        public async Task<JsonResult> GetPagingLoginLog(SystemLoginLogGetPagingInput paging)
        {
            var list = new List<FilterDefinition<SystemLoginLog>>
            {
                Builders<SystemLoginLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            if (!paging.Name.IsNullOrEmpty())
                list.Add(Builders<SystemLoginLog>.Filter.Where(w => w.CreateUserName.Contains(paging.Name)));
            if (!paging.Code.IsNullOrEmpty())
                list.Add(Builders<SystemLoginLog>.Filter.Where(w => w.CreateUserCode.Contains(paging.Code)));
            if (!paging.CreateTime.IsNullOrEmpty())
                list.Add(Builders<SystemLoginLog>.Filter.Where(w => w.CreateTime <= paging.EndCreateTime && w.CreateTime >= paging.BeginCreateTime));

            var filter = Builders<SystemLoginLog>.Filter.And(list);
            var sort = Builders<SystemLoginLog>.Sort.Descending(d => d.CreateTime);
            var datas = await _loginLogLogic.PagingQueryProcAsync(filter, paging, sort);
            foreach (var data in datas.Data)
            {
                data.CreateTime = data.CreateTime.ToServerLocalTime();
            }

            return JsonForGridPaging(datas);
        }

        /// <summary>
        ///     获取所有登录日志信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("登录日志-方法-列表-获取所有登录日志分析")]
        public async Task<JsonResult> GetLoginLogAnalysis(SystemLoginLogGetPagingInput paging)
        {
            var list = new List<FilterDefinition<SystemLoginLog>>
            {
                Builders<SystemLoginLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            if (!paging.Name.IsNullOrEmpty())
                list.Add(Builders<SystemLoginLog>.Filter.Where(w => w.CreateUserName.Contains(paging.Name)));
            if (!paging.Code.IsNullOrEmpty())
                list.Add(Builders<SystemLoginLog>.Filter.Where(w => w.CreateUserCode.Contains(paging.Code)));
            if (!paging.CreateTime.IsNullOrEmpty())
                list.Add(Builders<SystemLoginLog>.Filter.Where(w => w.CreateTime <= paging.EndCreateTime && w.CreateTime >= paging.BeginCreateTime));
            var filter = Builders<SystemLoginLog>.Filter.And(list);
            var sort = Builders<SystemLoginLog>.Sort.Descending(d => d.CreateTime);
            var datas = (await _loginLogLogic.GetAllEnumerableAsync(filter, null, sort)).ToList();
            IList<string> xdata = new List<string>();
            IList<int> ydata = new List<int>();
            if (datas.Any())
            {
                int days = ((!paging.CreateTime.IsNullOrEmpty() ? Convert.ToDateTime(paging.EndCreateTime.ToString("yyyy-MM-dd")) : DateTime.Now) - Convert.ToDateTime(datas.Min(m => m.CreateTime).ToString("yyyy-MM-dd"))).Days;
                for (int i = 0; i < days + 1; i++)
                {
                    var time = datas.Min(m => m.CreateTime).AddDays(i);
                    time = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));
                    xdata.Add(time.ToString("MM-dd"));
                    ydata.Add(datas.Where(w =>
                        w.CreateTime >= time.AddDays(0).Date && w.CreateTime <= time.AddDays(1).Date).Count());
                }
            }
            return Json(new
            {
                analysis = new
                {
                    xdata,
                    ydata
                }
            });
        }


        /// <summary>
        ///     根据主键删除登录日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除登录日志")]
        public async Task<JsonResult> DeleteLoginLogById(IdInput<string> input)
        {
            var list = new List<FilterDefinition<SystemLoginLog>>();
            foreach (var id in input.Id.Split(','))
            {
                list.Add(Builders<SystemLoginLog>.Filter.Where(w => w.LoginLogId == Guid.Parse(id)));
            }
            var filter = Builders<SystemLoginLog>.Filter.And(list);
            return Json(await _loginLogLogic.DeleteAsync(input.Id));
        }

        /// <summary>
        ///     根据主键删除登录日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除登录日志")]
        public async Task<JsonResult> DeleteLoginLogAll()
        {
            var list = new List<FilterDefinition<SystemLoginLog>>
            {
                Builders<SystemLoginLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            var filter = Builders<SystemLoginLog>.Filter.And(list);
            return Json(await _loginLogLogic.DeleteAsync(filter));
        }

        /// <summary>
        ///     导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("登录日志-方法-列表-导出到Excel")]
        public async Task<FileResult> ExportExcelToLoginLog(QueryParam paging)
        {
            //ExcelReportDto excelReportDto = new ExcelReportDto()
            //{
            //    TemplatePath = Server.MapPath("/") + "DataUser/Templates/System/Log/登录日志.xlsx",
            //    DownTemplatePath = "登录日志" + string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + ".xlsx",
            //    Title = "登录日志.xlsx"
            //};
            //await _loginLogLogic.ReportExcelLoginLogQuery(paging, excelReportDto);
            //return File(new FileStream(excelReportDto.DownPath, FileMode.Open), "application/octet-stream", Server.UrlEncode(excelReportDto.Title));
            return null;
        }
        #endregion

        #region 操作日志

        /// <summary>
        ///     获取所有操作日志信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("操作日志-方法-列表-获取所有操作日志信息")]
        public async Task<JsonResult> GetPagingOperationLog(SystemOperationLogGetPagingInput paging)
        {
            var list = new List<FilterDefinition<SystemOperationLog>>
            {
                Builders<SystemOperationLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            if (!paging.Name.IsNullOrEmpty())
                list.Add(Builders<SystemOperationLog>.Filter.Where(w => w.CreateUserName.Contains(paging.Name)));
            if (!paging.Describe.IsNullOrEmpty())
                list.Add(Builders<SystemOperationLog>.Filter.Where(w => w.Describe.Contains(paging.Describe)));
            if (!paging.Code.IsNullOrEmpty())
                list.Add(Builders<SystemOperationLog>.Filter.Where(w => w.CreateUserCode.Contains(paging.Code)));
            if (!paging.CreateTime.IsNullOrEmpty())
                list.Add(Builders<SystemOperationLog>.Filter.Where(w => w.CreateTime <= paging.EndCreateTime && w.CreateTime >= paging.BeginCreateTime));

            var filter = Builders<SystemOperationLog>.Filter.And(list);
            var sort = Builders<SystemOperationLog>.Sort.Descending(d => d.CreateTime);
            var datas = await _operationLogLogic.PagingQueryProcAsync(filter, paging, sort);
            foreach (var data in datas.Data)
            {
                data.CreateTime = data.CreateTime.ToServerLocalTime();
            }
            return JsonForGridPaging(datas);
        }

        /// <summary>
        ///     根据主键获取操作日志信息明细
        /// </summary>
        /// <param name="input">主键Id</param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("操作日志-方法-列表-根据主键获取操作日志信息明细")]
        public async Task<JsonResult> GetOperationLogById(IdInput input)
        {
            var filter = Builders<SystemOperationLog>.Filter.Where(w => w.OperationLogId == input.Id);
            return Json(await _operationLogLogic.FindOneAsync(filter));
        }

        /// <summary>
        ///     根据主键删除操作日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除操作日志")]
        public async Task<JsonResult> DeleteOperationLogById(IdInput<string> input)
        {
            var list = new List<FilterDefinition<SystemOperationLog>>();
            foreach (var id in input.Id.Split(','))
            {
                list.Add(Builders<SystemOperationLog>.Filter.Where(w => w.OperationLogId == Guid.Parse(id)));
            }
            var filter = Builders<SystemOperationLog>.Filter.And(list);
            return Json(await _operationLogLogic.DeleteAsync(input.Id));
        }

        /// <summary>
        ///     根据主键删除操作日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除登录日志")]
        public async Task<JsonResult> DeleteOperationLogAll()
        {
            var list = new List<FilterDefinition<SystemOperationLog>>
            {
                Builders<SystemOperationLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            var filter = Builders<SystemOperationLog>.Filter.And(list);
            return Json(await _operationLogLogic.DeleteAsync(filter));
        }

        /// <summary>
        ///     导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("操作日志-方法-列表-导出到Excel")]
        public async Task<FileResult> ExportExcelToOperationLog(QueryParam paging)
        {
            //ExcelReportDto excelReportDto = new ExcelReportDto()
            //{
            //    TemplatePath = Server.MapPath("/") + "DataUser/Templates/System/Log/操作日志.xlsx",
            //    DownTemplatePath = "操作日志" + string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + ".xlsx",
            //    Title = "操作日志.xlsx"
            //};
            //await _operationLogLogic.ReportExcelOperationLogQuery(paging, excelReportDto);
            //return File(new FileStream(excelReportDto.DownPath, FileMode.Open), "application/octet-stream", Server.UrlEncode(excelReportDto.Title));
            return null;

        }
        #endregion

        #region Sql日志

        /// <summary>
        ///     获取所有数据日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("Sql日志-方法-列表-获取所有Sql日志")]
        public async Task<JsonResult> GetPagingSqlLog(QueryParam paging)
        {
            var list = new List<FilterDefinition<SystemSqlLog>>
            {
                Builders<SystemSqlLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            var filter = Builders<SystemSqlLog>.Filter.And(list);
            return JsonForGridPaging(await _sqlLogLogic.PagingQueryProcAsync(filter, paging));
        }

        /// <summary>
        ///     根据主键获取数据日志
        /// </summary>
        /// <param name="input">主键Id</param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("Sql日志-方法-列表-根据主键获取Sql日志")]
        public async Task<JsonResult> GetSqlLogById(IdInput input)
        {
            var filter = Builders<SystemSqlLog>.Filter.Where(w => w.SqlLogId == input.Id);
            return Json(await _sqlLogLogic.FindOneAsync(filter));
        }

        /// <summary>
        ///     根据主键删除Sql日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除Sql日志")]
        public async Task<JsonResult> DeleteSqlLogById(IdInput<string> input)
        {
            var list = new List<FilterDefinition<SystemSqlLog>>();
            foreach (var id in input.Id.Split(','))
            {
                list.Add(Builders<SystemSqlLog>.Filter.Where(w => w.SqlLogId == Guid.Parse(id)));
            }
            var filter = Builders<SystemSqlLog>.Filter.And(list);
            return Json(await _sqlLogLogic.DeleteAsync(input.Id));
        }

        /// <summary>
        ///     根据主键删除Sql日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("异常日志-方法-列表-根据主键删除Sql日志")]
        public async Task<JsonResult> DeleteSqlLogAll()
        {
            var list = new List<FilterDefinition<SystemSqlLog>>
            {
                Builders<SystemSqlLog>.Filter.Lt("CreateTime", DateTime.Now)
            };
            var filter = Builders<SystemSqlLog>.Filter.And(list);
            return Json(await _sqlLogLogic.DeleteAsync(filter));
        }
        #endregion

        #region 日志分析

        #region 浏览器分析


        /// <summary>
        /// 浏览器分析
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("孙泽伟")]
        [Remark("日志分析-方法-获取浏览器分析数据")]
        public async Task<JsonResult> GetAnalysisForBrowser()
        {
            return Json(await _loginLogLogic.GetBrowserAnalysis());
        }
        #endregion

        #endregion
    }
}