/*
 源码己托管:http://git.oschina.net/kuiyu/dotnetcodes
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Utilities.时间戳
{
   public class TimeHelper
    {
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”></param>
        /// <returns></returns>
        public static DateTime GetTimeByJavascript(long jsTimeStamp)
        {  
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
            return dt;
        }

        /// <summary>
        /// 把时间转换为javascript所使用的时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertDateTimeByJavascript(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(dt - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }

        /// <summary>
        /// 把时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”></param>
        /// <returns></returns>
        public long  ConvertDateTimeByUnix(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }

        /// <summary>
        /// 把Unix时间戳转化为时间
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public DateTime GetTimeByUnix(long unixTimeStamp)
        { 
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }

        /// <summary>
		/// 获取以0点0分0秒开始的日期
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static DateTime GetStartDateTime(DateTime d)
		{
			if (d.Hour != 0)
			{
				var year = d.Year;
				var month = d.Month;
				var day = d.Day;
				var hour = "0";
				var minute = "0";
				var second = "0";
				d = Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second));
			}
			return d;
		}

		/// <summary>
		/// 获取以23点59分59秒结束的日期
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static DateTime GetEndDateTime(DateTime d)
		{
			if (d.Hour != 23)
			{
				var year = d.Year;
				var month = d.Month;
				var day = d.Day;
				var hour = "23";
				var minute = "59";
				var second = "59";
				d = Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second));
			}
			return d;
		}
    }
}
