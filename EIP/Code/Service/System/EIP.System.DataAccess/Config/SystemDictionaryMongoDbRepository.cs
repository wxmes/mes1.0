using EIP.Common.DataAccess;
using EIP.System.Models.Entities;

namespace EIP.System.DataAccess.Config
{
    /// <summary>
    ///     �ֵ����ݷ��ʽӿ�ʵ��
    /// </summary>
    public class SystemDictionaryMongoDbRepository : MongoDbAsyncRepository<SystemDictionary>, ISystemDictionaryMongoDbRepository
    {
        
    }
}