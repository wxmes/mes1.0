using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EIP.Common.Core.Auth;
using EIP.Common.Core.Log;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.WebApi;
using EIP.Common.WebApi.Attribute;
using EIP.Common.WebApi.Jwt;
using EIP.System.Business.Identity;
using EIP.System.Business.Log;
using EIP.System.Models.Dtos.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EIP.System.Api
{
    /// <summary>
    /// �ʺſ�����
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly IOptions<JwtConfiguration> _jwtConfig;
        private readonly ISystemUserInfoLogic _userInfoLogic;
        private readonly ISystemOrganizationLogic _organizationLogic;
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="jwtConfig"></param>
        /// <param name="userInfoLogic"></param>
        /// <param name="organizationLogic"></param>
        /// <param name="accessor"></param>
        /// <param name="loginLogLogic"></param>
        public AccountController(IOptions<JwtConfiguration> jwtConfig,
            ISystemUserInfoLogic userInfoLogic,
            ISystemOrganizationLogic organizationLogic,
            IHttpContextAccessor accessor,
            ISystemLoginLogLogic loginLogLogic)
        {
            _jwtConfig = jwtConfig;
            _userInfoLogic = userInfoLogic;
            _organizationLogic = organizationLogic;
            _accessor = accessor;
        }

        #region Login
        /// <summary>
        /// ��¼
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [CreateBy("����ΰ")]
        [Remark("�ʺŹ���-����-��¼")]
        public async Task<JsonResult> Login(SystemUserLoginInput input)
        {
            var operateStatus = new OperateStatus<SystemUserLoginOutput>();
            string tokenData = String.Empty;
            //��֤���ݿ���Ϣ
            var info = await _userInfoLogic.CheckUserByCodeAndPwd(input);
            if (info.Data != null)
            {
                ICollection<string> roles = new List<string>();
                if (info.Data.IsAdmin)
                {
                    //��ѯ������֯����
                    var orgs = (await _organizationLogic.GetSystemOrganizationByPid(new IdInput(Guid.Empty))).FirstOrDefault();
                    if (orgs != null)
                    {
                        info.Data.OrganizationId = Guid.Parse(orgs.id.ToString());
                        info.Data.OrganizationName = orgs.text;
                    }
                    roles.Add("Admin");
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Value.Secret));
                var header = new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
                var issuer = _jwtConfig.Value.Issuer;
                var loginTime = DateTime.Now;
                var claims = new[]
                {
                        new Claim("Name", info.Data.Name),
                        new Claim("Code", info.Data.Code),
                        new Claim("OrganizationId", info.Data.OrganizationId==Guid.Empty?"":info.Data.OrganizationId.ToString()),
                        new Claim("OrganizationName", info.Data.OrganizationName ?? ""),
                        new Claim("LoginId",info.Data.LoginId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, info.Data.UserId.ToString())
                    }.Concat(roles.Select(role => new Claim("role", role)));
                JwtPayload payload = input.Remberme ? new JwtPayload(issuer, null, claims, null, loginTime.AddYears(1)) : new JwtPayload(issuer, null, claims, null, loginTime.AddMinutes(60));
                var token = new JwtSecurityToken(header, payload);
                operateStatus.ResultSign = ResultSign.Successful;
                tokenData = new JwtSecurityTokenHandler().WriteToken(token);
                WriteLoginLog(info.Data);
            }
            else
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = info.Message;
            }
            if (operateStatus.ResultSign == ResultSign.Successful)
            {
                if (info.Data != null)
                {
                    info.Data.Token = tokenData;
                }
                operateStatus.Data = info.Data;
            }
            return Json(operateStatus);
        }
        #endregion

        /// <summary>
        /// д��¼��־
        /// </summary>
        /// <returns></returns> 
        [HttpPost]
        [CreateBy("����ΰ")]
        [Remark("�ʺŹ���-����-��д��־")]
        private async void WriteLoginLog(SystemUserLoginOutput input)
        {
            //�ͻ���Ip
            LoginLogHandler handler = new LoginLogHandler(new PrincipalUser
            {
                Code = input.Code,
                UserId = input.UserId,
                Name = input.Name
            }, _accessor, input.LoginId);
            handler.WriteLog();
        }
    }
}