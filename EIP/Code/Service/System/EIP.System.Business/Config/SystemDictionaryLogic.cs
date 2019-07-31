using EIP.Common.Business;
using EIP.Common.Core.Extensions;
using EIP.Common.Core.Resource;
using EIP.Common.Core.Utils;
using EIP.Common.Models;
using EIP.Common.Models.Dtos;
using EIP.Common.Models.Tree;
using EIP.System.DataAccess.Config;
using EIP.System.Models.Dtos.Config;
using EIP.System.Models.Entities;
using EIP.System.Models.Resx;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIP.System.Business.Config
{
    /// <summary>
    ///     �ֵ�ҵ���߼�ʵ��
    /// </summary>
    public class SystemDictionaryLogic : DapperAsyncLogic<SystemDictionary>, ISystemDictionaryLogic
    {
        #region ���캯��

        public SystemDictionaryLogic()
        {
            _dictionaryRepository = new SystemDictionaryRepository();
        }
        private readonly ISystemDictionaryMongoDbRepository _dictionaryMongoDbRepository;
        private readonly ISystemDictionaryRepository _dictionaryRepository;

        public SystemDictionaryLogic(ISystemDictionaryRepository dictionaryRepository, ISystemDictionaryMongoDbRepository dictionaryMongoDbRepository)
            : base(dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
            _dictionaryMongoDbRepository = dictionaryMongoDbRepository;
        }

        #endregion

        #region ����

        /// <summary>
        ///     ��ѯ�����ֵ���Ϣ:Ztree��ʽ
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetDictionaryTree()
        {
            await UpdateDictionaryMongoDb();
            return await _dictionaryRepository.GetDictionaryTree();
        }

        /// <summary>
        /// ����MongoDb�ֵ�����
        /// </summary>
        /// <returns></returns>
        private async Task UpdateDictionaryMongoDb()
        {
            var allDics = (await GetAllEnumerableAsync()).ToList();
            var list = new List<FilterDefinition<SystemDictionary>>
            {
                Builders<SystemDictionary>.Filter.Lt("CreateTime", DateTime.Now)
            };
            var filter = Builders<SystemDictionary>.Filter.And(list);
            //ɾ��MonogoDb����
            await _dictionaryMongoDbRepository.DeleteManyAsync(filter);
            //����MonogoDb����
            _dictionaryMongoDbRepository.BulkInsertAsync(allDics);
        }

        /// <summary>
        ///    ��MogoDb�в�ѯ�����ֵ���Ϣ
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetMongoDbDictionaryTreeByParentIds(IdInput input)
        {
            IList<JsTreeEntity> treeEntities = new List<JsTreeEntity>();
            var builder = Builders<SystemDictionary>.Filter;
            var filter = builder.Where(w => w.ParentIds.Contains(input.Id.ToString()));
            var datas = await _dictionaryMongoDbRepository.GetAllEnumerableAsync(filter);
            foreach (var data in datas)
            {
                treeEntities.Add(new JsTreeEntity
                {
                    id = data.DictionaryId,
                    parent = data.DictionaryId == input.Id ? "#" : data.ParentId.ToString(),
                    text = data.Name,
                    state = new JsTreeStateEntity()
                });
            }
            return treeEntities;
        }

        /// <summary>
        ///    ���ݸ���id��ȡ�¼�(ֻ��һ��)
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemDictionary>> GetDictionarieByParentId(IdInput input)
        {
            return (await _dictionaryRepository.FindAllAsync(f => f.ParentId == input.Id)).OrderBy(o => o.OrderNo);
        }

        /// <summary>
        /// ���ݸ�����ѯ�¼�(�����¼�)
        /// </summary>
        /// <param name="input">����id</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemDictionaryGetByParentIdOutput>> GetDictionariesParentId(SystemDictionaryGetByParentIdInput input)
        {
            var allDics = (await GetAllEnumerableAsync()).ToList();
            var dics = (await _dictionaryRepository.GetDictionariesParentId(input)).ToList();
            foreach (var dic in dics)
            {
                foreach (var parent in dic.ParentIds.Split(','))
                {
                    //�����ϼ�
                    var dicinfo = allDics.FirstOrDefault(w => w.DictionaryId.ToString() == parent);
                    if (dicinfo != null) dic.ParentNames += dicinfo.Name + ">";
                }
                if (!dic.ParentNames.IsNullOrEmpty())
                    dic.ParentNames = dic.ParentNames.TrimEnd('>');
            }
            return dics.OrderBy(o => o.ParentNames);
        }

        /// <summary>
        ///     �����ֵ���Ϣ
        /// </summary>
        /// <param name="dictionary">�ֵ���Ϣ</param>
        /// <returns></returns>
        public async Task<OperateStatus> SaveDictionary(SystemDictionary dictionary)
        {
            OperateStatus operateStatus;
            if (dictionary.DictionaryId.IsEmptyGuid())
            {
                dictionary.CreateTime = DateTime.Now;
                dictionary.CanbeDelete = true;
                dictionary.DictionaryId = CombUtil.NewComb();
                operateStatus = await InsertAsync(dictionary);
            }
            else
            {
                dictionary.UpdateTime = DateTime.Now;
                dictionary.UpdateUserId = dictionary.CreateUserId;
                dictionary.UpdateUserName = dictionary.CreateUserName;
                var dic = await GetByIdAsync(dictionary.DictionaryId);
                dictionary.CanbeDelete = dic.CanbeDelete;
                dictionary.CreateTime = dic.CreateTime;
                dictionary.CreateUserId = dic.CreateUserId;
                dictionary.CreateUserName = dic.CreateUserName;
                operateStatus = await UpdateAsync(dictionary);
            }
            await GeneratingParentIds();
            return operateStatus;
        }

        /// <summary>
        ///     ɾ���ֵ估�¼�����
        /// </summary>
        /// <param name="input">id</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteDictionary(IdInput<string> input)
        {
            var operateStatus = new OperateStatus();
            if (input.Id.IsNullOrEmpty())
            {
                return operateStatus;
            }
            foreach (var id in input.Id.Split(','))
            {
                //�жϸ��ֵ��Ƿ�����ɾ��:������ϵͳ������ֵ�������ɾ��
                var dictionary = await GetByIdAsync(id);
                if (!dictionary.CanbeDelete)
                {
                    operateStatus.ResultSign = ResultSign.Error;
                    operateStatus.Message = Chs.CanotDelete;
                    return operateStatus;
                }
                //�Ƿ��������
                IEnumerable<SystemDictionary> dictionaries = await GetDictionarieByParentId(new IdInput(Guid.Parse(id)));
                if (dictionaries.Any())
                {
                    operateStatus.ResultSign = ResultSign.Error;
                    operateStatus.Message = ResourceSystem.�����¼���;
                    return operateStatus;
                }
            }
            foreach (var id in input.Id.Split(','))
            {
                try
                {
                    operateStatus = await DeleteAsync(new SystemDictionary { DictionaryId = Guid.Parse(id) });
                    if (operateStatus.ResultSign == ResultSign.Error)
                    {
                        return operateStatus;
                    }
                }
                catch (Exception e)
                {
                    operateStatus.Message = e.Message;
                    return operateStatus;
                }
            }
            operateStatus.ResultSign = ResultSign.Successful;
            operateStatus.Message = Chs.Successful;
            return operateStatus;
        }

        /// <summary>
        ///     ����ParentIds��ȡ�����¼�
        /// </summary>
        /// <param name="input">����ֵ</param>
        /// <returns></returns>
        public async Task<IEnumerable<JsTreeEntity>> GetDictionaryTreeByParentIds(IdInput input)
        {
            return await _dictionaryRepository.GetDictionaryTreeByParentIds(input);
        }

        #endregion

        /// <summary>
        ///     �����������¼���ϵ�ֶ�����
        /// </summary>
        /// <returns></returns>
        public async Task<OperateStatus> GeneratingParentIds()
        {
            OperateStatus operateStatus = new OperateStatus();
            try
            {
                //��ȡ�����ֵ���
                var dics = (await GetAllEnumerableAsync()).ToList();
                var topDics = dics.Where(w => w.ParentId == Guid.Empty);
                foreach (var dic in topDics)
                {
                    dic.ParentIds = dic.DictionaryId.ToString();
                    await UpdateAsync(dic);
                    await GeneratingParentIds(dic, dics.ToList(), "");
                }
            }
            catch (Exception ex)
            {
                operateStatus.Message = ex.Message;
                return operateStatus;
            }
            operateStatus.Message = Chs.Successful;
            operateStatus.ResultSign = ResultSign.Successful;
            return operateStatus;
        }

        /// <summary>
        /// �ݹ��ȡ����
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dictionaries"></param>
        /// <param name="dicId"></param>
        private async Task GeneratingParentIds(SystemDictionary dictionary, IList<SystemDictionary> dictionaries, string dicId)
        {
            string parentIds = dictionary.DictionaryId.ToString();
            //��ȡ�¼�
            var nextDic = dictionaries.Where(w => w.ParentId == dictionary.DictionaryId).ToList();
            if (nextDic.Any())
            {
                parentIds = dicId.IsNullOrEmpty() ? parentIds : dicId + "," + parentIds;
            }
            foreach (var dic in nextDic)
            {
                dic.ParentIds = parentIds + "," + dic.DictionaryId;
                await UpdateAsync(dic);
                await GeneratingParentIds(dic, dictionaries, parentIds);
            }
        }

        /// <summary>
        /// ����Id��ȡ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SystemDictionaryEditOutput> GetById(IdInput input)
        {
            SystemDictionaryEditOutput output = (await GetByIdAsync(input.Id)).MapTo<SystemDictionaryEditOutput>();
            //��ȡ������Ϣ
            var parentInfo = await GetByIdAsync(output.ParentId);
            if (parentInfo != null)
            {
                output.ParentName = parentInfo.Name;
            }
            return output;
        }
    }
}