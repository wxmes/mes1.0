using EIP.Common.DataAccess;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Config
{
    /// <summary>
    /// �ֵ����ݷ��ʽӿ�
    /// </summary>
    public interface ISystemDictionaryMongoDbRepository : IAsyncMongoDbRepository<SystemDictionary>
    {
    }
}