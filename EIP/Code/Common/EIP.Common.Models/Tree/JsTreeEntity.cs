using System;
using Newtonsoft.Json;

namespace EIP.Common.Models.Tree
{
    public class JsTreeEntity
    {
        /// <summary>
        ///     主键
        /// </summary>
        public Object id { get; set; }

        /// <summary>
        ///     父级
        /// </summary>
        public Object parent { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        public string text { get; set; }

        /// <summary>
        ///     图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public JsTreeStateEntity state { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [JsonIgnore]
        public string url { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonIgnore]
        public string remark { get; set; }

        /// <summary>
        /// 设置
        /// </summary>
        public bool children { get; set; }
    }

    public class JsTreeStateEntity
    {
        /// <summary>
        /// 是否打开
        /// </summary>
        public bool opened { get; set; } = true;

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool disabled { get; set; } = false;
    }
}