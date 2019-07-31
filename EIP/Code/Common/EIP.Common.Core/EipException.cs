using System;
using System.Runtime.Serialization;

namespace EIP.Common.Core
{
    /// <summary>
    /// EIP异常
    /// </summary>
    [Serializable]
    public class EipException:Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public EipException()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public EipException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">异常消息</param>
        public EipException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public EipException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}