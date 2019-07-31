using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Resource;
using EIP.Common.Core.Utils;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Dtos.Reports;
using EIP.Common.Models.Paging;
using EIP.System.Business.Permission;
using EIP.System.DataAccess.Identity;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using EIP.System.Models.Resx;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EIP.System.Business.Identity
{
    /// <summary>
    ///     �û�ҵ���߼�ʵ��
    /// </summary>
    public class SystemUserInfoLogic : DapperAsyncLogic<SystemUserInfo>, ISystemUserInfoLogic
    {
        #region ���캯��

        private readonly ISystemOrganizationLogic _organizationLogic;
        private readonly ISystemPermissionUserLogic _permissionUserLogic;
        private readonly ISystemUserInfoRepository _userInfoRepository;
        private readonly IOptions<EIPConfig> _configOptions;
        public SystemUserInfoLogic(ISystemUserInfoRepository userInfoRepository,
            ISystemPermissionUserLogic permissionUserLogic, IOptions<EIPConfig> configOptions, ISystemOrganizationLogic organizationLogic)
            : base(userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
            _permissionUserLogic = permissionUserLogic;
            _configOptions = configOptions;
            _organizationLogic = organizationLogic;
        }

        public SystemUserInfoLogic(IOptions<EIPConfig> configOptions, ISystemOrganizationLogic organizationLogic)
        {
            _configOptions = configOptions;
            _organizationLogic = organizationLogic;
            _userInfoRepository = new SystemUserInfoRepository();
        }

        #endregion

        #region ����

        /// <summary>
        ///     ���ݵ�¼����������ѯ�û���Ϣ
        /// </summary>
        /// <param name="input">��¼���������</param>
        /// <returns></returns>
        public async Task<OperateStatus<SystemUserLoginOutput>> CheckUserByCodeAndPwd(SystemUserLoginInput input)
        {
            var operateStatus = new OperateStatus<SystemUserLoginOutput>();
            //��������������
            var encryptPwd = DEncryptUtil.Encrypt(input.Pwd,  _configOptions.Value.PasswordKey);
            //��ѯ��Ϣ
            input.Pwd = encryptPwd;
            var data = await _userInfoRepository.CheckUserByCodeAndPwd(input);
            //�Ƿ����
            if (data == null)
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.�û������������;
                return operateStatus;
            }
            //�Ƿ񶳽�
            if (data.IsFreeze)
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.��¼�û��Ѷ���;
                return operateStatus;
            }
            //�ɹ�
            operateStatus.ResultSign = ResultSign.Successful;
            operateStatus.Message = "/";
            
            if (data.FirstVisitTime == null)
            {
                //�����û����һ�ε�¼ʱ��
                _userInfoRepository.UpdateFirstVisitTime(new IdInput(data.UserId));
            }
            //�����û����һ�ε�¼ʱ��
            _userInfoRepository.UpdateLastLoginTime(new IdInput(data.UserId));
            data.LoginId = CombUtil.NewComb();
            operateStatus.Data = data;
            return operateStatus;
        }


        /// <summary>
        ///     ��ҳ��ѯ
        /// </summary>
        /// <param name="paging">��ҳ����</param>
        /// <returns></returns>
        public async Task<PagedResults<SystemUserOutput>> PagingUserQuery(SystemUserPagingInput paging)
        {
            var data = await _userInfoRepository.PagingUserQuery(paging);
            var allOrgs = (await _organizationLogic.GetAllEnumerableAsync()).ToList();
            foreach (var user in data.Data)
            {
                var organization = allOrgs.FirstOrDefault(w => w.OrganizationId == user.OrganizationId);
                if (organization != null && !organization.ParentIds.IsNullOrEmpty())
                {
                    foreach (var parent in organization.ParentIds.Split(','))
                    {
                        //�����ϼ�
                        var dicinfo = allOrgs.FirstOrDefault(w => w.OrganizationId.ToString() == parent);
                        if (dicinfo != null) user.OrganizationNames += dicinfo.Name + ">";
                    }
                    if (!user.OrganizationNames.IsNullOrEmpty())
                        user.OrganizationNames = user.OrganizationNames.TrimEnd('>');
                }
            }
            return data;
        }

        /// <summary>
        ///     Excel������ʽ
        /// </summary>
        /// <param name="paging">��ѯ����</param>
        /// <param name="excelReportDto"></param>
        /// <returns></returns>
        public async Task<OperateStatus> ReportExcelUserQuery(SystemUserPagingInput paging,
            ExcelReportDto excelReportDto)
        {
            var operateStatus = new OperateStatus();
            try
            {
                //��װ����
                IList<SystemUserOutput> dtos = (await _userInfoRepository.PagingUserQuery(paging)).Data.ToList();
                var tables = new Dictionary<string, DataTable>(StringComparer.OrdinalIgnoreCase);
                //��װ��Ҫ��������

                operateStatus.ResultSign = ResultSign.Successful;
            }
            catch (Exception)
            {
                operateStatus.ResultSign = ResultSign.Error;
            }
            return operateStatus;
        }

        /// <summary>
        ///     �������������Ƿ��Ѿ������ظ���
        /// </summary>
        /// <param name="input">��Ҫ��֤�Ĳ���</param>
        /// <returns></returns>
        public async Task<OperateStatus> CheckUserCode(CheckSameValueInput input)
        {
            var operateStatus = new OperateStatus();
            if (await _userInfoRepository.CheckUserCode(input))
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = string.Format(Chs.HaveCode, input.Param);
            }
            else
            {
                operateStatus.ResultSign = ResultSign.Successful;
                operateStatus.Message = Chs.CheckSuccessful;
            }
            return operateStatus;
        }

        /// <summary>
        ///     ������Ա��Ϣ
        /// </summary>
        /// <param name="input">��Ա��Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveUser(SystemUserSaveInput input)
        {
            OperateStatus operateStatus;
            if (input.UserId.IsEmptyGuid())
            {
                //����
                input.CreateTime = DateTime.Now;
                input.UserId = Guid.NewGuid();
                if (!input.Code.IsNullOrEmpty())
                {
                    input.Password = DEncryptUtil.Encrypt("123456", _configOptions.Value.PasswordKey);
                }
                SystemUserInfo userInfoMap = input.MapTo<SystemUserInfo>();
                operateStatus = await InsertAsync(userInfoMap);
                if (operateStatus.ResultSign == ResultSign.Successful)
                {
                    //����û�����֯����
                    operateStatus = await
                            _permissionUserLogic.SavePermissionUser(EnumPrivilegeMaster.��֯����, input.OrganizationId,
                                new List<Guid> { input.UserId });
                    if (operateStatus.ResultSign == ResultSign.Successful)
                    {
                        return operateStatus;
                    }
                }
                else
                {
                    return operateStatus;
                }
            }
            else
            {
                //ɾ����Ӧ��֯����
                operateStatus = await _permissionUserLogic.DeletePrivilegeMasterUser(input.UserId, EnumPrivilegeMaster.��֯����);
                if (operateStatus.ResultSign == ResultSign.Successful)
                {
                    //����û�����֯����
                    operateStatus = await _permissionUserLogic.SavePermissionUser(EnumPrivilegeMaster.��֯����, input.OrganizationId, new List<Guid> { input.UserId });
                    if (operateStatus.ResultSign == ResultSign.Successful)
                    {
                        var userInfo = await GetByIdAsync(input.UserId);
                        input.CreateTime = userInfo.CreateTime;
                        input.Password = userInfo.Password;
                        input.UpdateTime = DateTime.Now;
                        input.UpdateUserId = userInfo.CreateUserId;
                        input.UpdateUserName = input.CreateUserName;
                        SystemUserInfo userInfoMap = input.MapTo<SystemUserInfo>();
                        return await UpdateAsync(userInfoMap);
                    }
                }
            }
            return operateStatus;
        }

        /// <summary>
        ///     ��ȡ�����û�
        /// </summary>
        /// <param name="input">�Ƿ񶳽�</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemChosenUserOutput>> GetChosenUser(FreezeInput input = null)
        {
            return await _userInfoRepository.GetChosenUser(input);
        }

        /// <summary>
        /// ��ȡ�û�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemUserInfo>> GetUser(FreezeInput input = null)
        {
            return await _userInfoRepository.GetUser(input);
        }

        /// <summary>
        ///     ɾ���û���Ϣ
        /// </summary>
        /// <param name="input">�û�id</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteUser(IdInput input)
        {
            await _permissionUserLogic.DeletePermissionUser(input);
            return await DeleteAsync(new SystemUserInfo
            {
                UserId = input.Id
            });
        }

        /// <summary>
        ///     �����û�Id��ȡ���û���Ϣ
        /// </summary>
        /// <param name="input">�û�Id</param>
        /// <returns></returns>
        public async Task<SystemUserDetailOutput> GetDetailByUserId(IdInput input)
        {
            //��ȡ�û�������Ϣ
            var userDto = (await _userInfoRepository.FindByIdAsync(input.Id)).MapTo<SystemUserOutput>();
            //ת��
            var userDetailDto = userDto.MapTo<SystemUserDetailOutput>();
            //��ȡ��ɫ���顢��λ����
            IList<SystemPrivilegeDetailListOutput> privilegeDetailDtos = (await
                _permissionUserLogic.GetSystemPrivilegeDetailOutputsByUserId(input)).ToList();
            var allOrgs = (await _organizationLogic.GetAllEnumerableAsync()).ToList();
            foreach (var dto in privilegeDetailDtos)
            {
                string description = string.Empty;
                var organization = allOrgs.FirstOrDefault(w => w.OrganizationId == dto.OrganizationId);
                if (organization != null && !organization.ParentIds.IsNullOrEmpty())
                {
                    foreach (var parent in organization.ParentIds.Split(','))
                    {
                        //�����ϼ�
                        var dicinfo = allOrgs.FirstOrDefault(w => w.OrganizationId.ToString() == parent);
                        if (dicinfo != null) description += dicinfo.Name + ">";
                    }
                    if (!description.IsNullOrEmpty())
                        description = description.TrimEnd('>');
                }
                dto.Organization = description;
            }

            //��ɫ
            userDetailDto.Role = privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��ɫ).ToList();
            //��
            userDetailDto.Group = privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��).ToList();
            //��λ
            userDetailDto.Post = privilegeDetailDtos.Where(w => w.PrivilegeMaster == EnumPrivilegeMaster.��λ).ToList();
            return userDetailDto;
        }

        /// <summary>
        ///     �����û�Id����ĳ������
        /// </summary>
        /// <param name="input">�û�Id</param>
        /// <returns></returns>
        public async Task<OperateStatus> ResetPassword(SystemUserResetPasswordInput input)
        {
            var operateStatus = new OperateStatus();
            //��������������
            var encryptPwd = DEncryptUtil.Encrypt(input.EncryptPassword, _configOptions.Value.PasswordKey);
            if (await _userInfoRepository.ResetPassword(new SystemUserResetPasswordInput
            {
                EncryptPassword = encryptPwd,
                Id = input.Id
            }))
            {
                operateStatus.ResultSign = ResultSign.Successful;
                operateStatus.Message = string.Format(ResourceSystem.��������ɹ�, input.EncryptPassword);
            }
            return operateStatus;
        }

        /// <summary>
        ///     �����û�ͷ��
        /// </summary>
        /// <param name="input">�û�ͷ��</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveHeadImage(SystemUserSaveHeadImageInput input)
        {
            var operateStatus = new OperateStatus();
            if (await _userInfoRepository.SaveHeadImage(input))
            {
                operateStatus.ResultSign = ResultSign.Successful;
                operateStatus.Message = Chs.Successful;
            }
            return operateStatus;
        }

        /// <summary>
        /// �����޸ĺ�������Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveChangePassword(SystemUserChangePasswordInput input)
        {
            var operateStatus = new OperateStatus();
            //��̨�ٴ���֤�Ƿ�һ��
            if (!input.NewPassword.Equals(input.ConfirmNewPassword))
            {
                operateStatus.Message =  "¼����������ȷ�����벻һ�¡�";
                return operateStatus;
            }
            //�������Ƿ���ȷ
            operateStatus = await CheckOldPassword(new CheckSameValueInput { Id = input.Id, Param = input.OldPassword });
            if (operateStatus.ResultSign == ResultSign.Error)
            {
                return operateStatus;
            }
            //��������������
            var encryptPwd = DEncryptUtil.Encrypt(input.NewPassword, _configOptions.Value.PasswordKey);
            if (await _userInfoRepository.ResetPassword(new SystemUserResetPasswordInput
            {
                EncryptPassword = encryptPwd,
                Id = input.Id
            }))
            {
                operateStatus.ResultSign = ResultSign.Successful;
                operateStatus.Message = string.Format(ResourceSystem.��������ɹ�, input.NewPassword);
            }
            return operateStatus;
        }

        /// <summary>
        ///     ��֤�������Ƿ�������ȷ
        /// </summary>
        /// <param name="input">��Ҫ��֤�Ĳ���</param>
        /// <returns></returns>
        public async Task<OperateStatus> CheckOldPassword(CheckSameValueInput input)
        {
            var operateStatus = new OperateStatus();
            input.Param = DEncryptUtil.Encrypt(input.Param, _configOptions.Value.PasswordKey);
            if (!await _userInfoRepository.CheckOldPassword(input))
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = "�����벻��ȷ";
            }
            else
            {
                operateStatus.ResultSign = ResultSign.Successful;
            }
            return operateStatus;
        }

        /// <summary>
        /// ����Id��ȡ�û���Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SystemUserGetByIdOutput> GetById(IdInput input)
        {
            return await _userInfoRepository.GetById(input);
        }

        #endregion
    }
}