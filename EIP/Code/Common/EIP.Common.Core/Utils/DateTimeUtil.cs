using System;

namespace EIP.Common.Core.Utils
{
    public static class DateTimeUtil
    {
        /// <summary>
        /// 取得某月的第一天
        /// </summary>
        /// <param name="datetime">要取得月份第一天的时间</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        /// <summary>
        /// 取得某月的最后一天
        /// </summary>
        /// <param name="datetime">要取得月份最后一天的时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 取得上个月第一天
        /// </summary>
        /// <param name="datetime">要取得上个月第一天的当前时间</param>
        /// <returns></returns>
        public static DateTime FirstDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(-1);
        }

        /// <summary>
        /// 取得上个月的最后一天
        /// </summary>
        /// <param name="datetime">要取得上个月最后一天的当前时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfPrdviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1);
        }

        /// <summary>
        /// 将客户端时间转换为服务端本地时间
        /// </summary>
        /// <param name="clientTime">客户端时间</param>
        /// <returns>返回服务端本地时间</returns>
        public static DateTime ToServerLocalTime(this DateTime clientTime)
        {
            return TimeZoneInfo.ConvertTime(clientTime, TimeZoneInfo.Local);
        }

        /// <summary>
        /// 将服务端本地时间转换为Utc时间
        /// </summary>
        /// <param name="serverTime">服务端时间</param>
        /// <returns>返回服务端本地时间</returns>
        public static DateTime ToUtcTime(this DateTime serverTime)
        {
            return TimeZoneInfo.ConvertTime(serverTime, TimeZoneInfo.Utc);
        }
    }
}