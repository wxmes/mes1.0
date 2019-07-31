using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Dtos.Reports;
using EIP.Common.Models.Paging;
using EIP.System.Models.Dtos.Identity;
using EIP.System.Models.Entities;

namespace EIP.System.Business.Identity
{
    /// <summary>
    ///     �û�ҵ���߼�
    /// </summary>
    public interface ISystemUserInfoLogic : IAsyncLogic<SystemUserInfo>
    {
        /// <summary>
        ///     ���ݵ�¼����������ѯ�û���Ϣ
        /// </summary>
        /// <param name="input">�û����������</param>
        /// <returns></returns>
        Task<OperateStatus<SystemUserLoginOutput>> CheckUserByCodeAndPwd(SystemUserLoginInput input);

        /// <summary>
        /// ����Id��ȡ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SystemUserGetByIdOutput> GetById(IdInput input);

        /// <summary>
        ///     ��ҳ��ѯ
        /// </summary>
        /// <param name="paging">��ҳ����</param>
        /// <returns></returns>
        Task<PagedResults<SystemUserOutput>> PagingUserQuery(SystemUserPagingInput paging);

        /// <summary>
        ///     Excel������ʽ
        /// </summary>
        /// <param name="paging">��ѯ����</param>
        /// <param name="excelReportDto"></param>
        /// <returns></returns>
        Task<OperateStatus> ReportExcelUserQuery(SystemUserPagingInput paging,
            ExcelReportDto excelReportDto);

        /// <summary>
        ///     �������������Ƿ��Ѿ������ظ���
        /// </summary>
        /// <param name="input">��Ҫ��֤�Ĳ���</param>
        /// <returns></returns>
        Task<OperateStatus> CheckUserCode(CheckSameValueInput input);

        /// <summary>
        ///     ������Ա��Ϣ
        /// </summary>
        /// <param name="input">��Ա��Ϣ</param>
        /// <returns></returns>
        Task<OperateStatus> SaveUser(SystemUserSaveInput input);

        /// <summary>
        ///     ��ȡ�����û�
        /// </summary>
        /// <param name="input">�Ƿ񶳽�</param>
        /// <returns></returns>
        Task<IEnumerable<SystemChosenUserOutput>> GetChosenUser(FreezeInput input = null);

        /// <summary>
        ///     ɾ���û���Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<OperateStatus> DeleteUser(IdInput input);

        /// <summary>
        ///     �����û�Id����ĳ������
        /// </summary>
        /// <param name="input">�û�Id</param>
        /// <returns></returns>
        Task<OperateStatus> ResetPassword(SystemUserResetPasswordInput input);

        /// <summary>
        ///     �����û�ͷ��
        /// </summary>
        /// <param name="input">�û�ͷ��</param>
        /// <returns></returns>
        Task<OperateStatus> SaveHeadImage(SystemUserSaveHeadImageInput input);

        /// <summary>
        /// �����޸ĺ�������Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
       Task< OperateStatus> SaveChangePassword(SystemUserChangePasswordInput input);

        /// <summary>
        ///     ��֤�������Ƿ�������ȷ
        /// </summary>
        /// <param name="input">��Ҫ��֤�Ĳ���</param>
        /// <returns></returns>
        Task<OperateStatus> CheckOldPassword(CheckSameValueInput input);

        /// <summary>
        ///     �����û�Id��ȡ���û���Ϣ
        /// </summary>
        /// <param name="input">�û�Id</param>
        /// <returns></returns>
        Task<SystemUserDetailOutput> GetDetailByUserId(IdInput input);
    }
}