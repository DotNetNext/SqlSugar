using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.TDengineAdo
{
    internal class Helper
    {

        //public static bool HasMicrosecondPrecision(DateTime dateTime)
        //{
        //    const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

        //    // Extract the microseconds by shifting and masking the ticks value
        //    int microseconds = (int)((dateTime.Ticks / ticksPerMicrosecond) % 1000000);

        //    return microseconds>0; 
        //}
        /// <summary>
        /// 将时间转为19位纳秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToLong19(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset;

            if (dateTime.Kind == DateTimeKind.Utc)
            {
                dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero);
            }
            else
            {
                dateTimeOffset = new DateTimeOffset(dateTime.ToUniversalTime());
            }

            long unixTimeSeconds = dateTimeOffset.ToUnixTimeSeconds();
            long nanoseconds = dateTimeOffset.Ticks % TimeSpan.TicksPerSecond * 100;

            return unixTimeSeconds * 1000000000 + nanoseconds;
        }
        /// <summary>
        /// 获取时间根据16位Long
        /// </summary>
        /// <param name="timestampInMicroseconds"></param>
        /// <returns></returns>
        public static DateTime Long16ToDateTime(long timestampInMicroseconds)
        {
            // 计算秒和微秒部分
            long seconds = timestampInMicroseconds / 1000000;
            long microseconds = timestampInMicroseconds % 1000000;

            // 创建 DateTimeOffset 对象，设置秒和微秒部分
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds).AddTicks(microseconds * 10);

            // 转换为本地时间
            DateTime localDateTime = dateTimeOffset.LocalDateTime;

            return localDateTime;
        }

        /// <summary>
        /// 获取时间根据19位Long
        /// </summary>
        /// <param name="timestampInMicroseconds"></param>
        /// <returns></returns>
        public static DateTime Long19ToDateTime(long timestampInNanoseconds)
        {
            // 计算秒和纳秒部分
            long seconds = timestampInNanoseconds / 1000000000;
            long nanoseconds = timestampInNanoseconds % 1000000000;

            // 创建 DateTimeOffset 对象，设置秒和纳秒部分
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds).AddTicks(nanoseconds / 100);

            // 转换为本地时间
            DateTime localDateTime = dateTimeOffset.LocalDateTime;

            return localDateTime;
        }

        /// <summary>
        /// 将时间转为16位
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToLong16(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset;

            if (dateTime.Kind == DateTimeKind.Utc)
            {
                dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero);
            }
            else
            {
                dateTimeOffset = new DateTimeOffset(dateTime.ToUniversalTime());
            }

            long unixTimeSeconds = dateTimeOffset.ToUnixTimeSeconds();
            long microseconds = dateTimeOffset.Ticks % TimeSpan.TicksPerSecond / (TimeSpan.TicksPerMillisecond / 1000);

            return unixTimeSeconds * 1000000 + microseconds;
        }



        // Method to convert a local DateTime to Unix timestamp (long)
        public static long ToUnixTimestamp(DateTime dateTime)
        {
            // If the DateTime is Utc, use ToUnixTimeMilliseconds directly
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            }
            else
            {
                // Convert local DateTime to Utc before converting to Unix timestamp
                return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
            }
        }
    }
}
