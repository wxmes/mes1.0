using System.Linq;
using System.Threading.Tasks;
using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Utils;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Paging;
using EIP.System.DataAccess.Permission;
using EIP.System.Models.Dtos.Permission;
using EIP.System.Models.Entities;
using EIP.System.Models.Enums;
using EIP.System.Models.Resx;

namespace EIP.System.Business.Permission
{
    /// <summary>
    ///     ϵͳ�ֶ���ҵ���߼�
    /// </summary>
    public class SystemFieldLogic : DapperAsyncLogic<SystemField>, ISystemFieldLogic
    {
        #region ���캯��

        private readonly ISystemFieldRepository _fieldRepository;
        private readonly ISystemPermissionRepository _permissionRepository;

        public SystemFieldLogic(ISystemFieldRepository fieldRepository, ISystemPermissionRepository permissionRepository)
            : base(fieldRepository)
        {
            _fieldRepository = fieldRepository;
            _permissionRepository = permissionRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     ���ݲ˵�Id��ȡ�ֶ���Ϣ
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public async Task<PagedResults<SystemFieldOutput>> GetFieldByMenuId(SystemFieldPagingInput paging)
        {
            return await _fieldRepository.GetFieldByMenuId(paging);
        }

        /// <summary>
        ///     �����ֶ���Ϣ
        /// </summary>
        /// <param name="field">�ֶ���Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveField(SystemField field)
        {
            if (field.FieldId.IsEmptyGuid())
            {
                field.FieldId = CombUtil.NewComb();
                return await InsertAsync(field);
            }
            return await UpdateAsync(field);
        }

        /// <summary>
        ///     ɾ���ֶ���Ϣ
        /// </summary>
        /// <param name="input">�ֶ�Id</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteField(IdInput input)
        {
            var operateStatus = new OperateStatus();
            //�鿴�ù������Ƿ��ѱ�����ռ��
            var permissions = await _permissionRepository.GetSystemPermissionsByPrivilegeAccessAndValue(EnumPrivilegeAccess.�ֶ�, input.Id);
            if (permissions.Any())
            {
                operateStatus.ResultSign = ResultSign.Error;
                operateStatus.Message = ResourceSystem.�ѱ�����Ȩ��;
                return operateStatus;
            }
            return await DeleteAsync(new SystemField { FieldId = input.Id });
        }

        #endregion
    }
}