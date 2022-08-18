/*
 源码己托管:http://git.oschina.net/kuiyu/dotnetcodes
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DotNet.Utilities.网站安全
{
    /// <summary>
    /// 对url进鉴权
    /// 与阿里相关算法保持一致
    /// </summary>
    public class AuthUrl
    {
        /// <summary>
        /// 验证鉴权地址
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="AuthKey"></param>
        /// <returns></returns>
        public static bool CheckUrl(string Url, string AuthKey)
        {
            string privateKey = ConfigurationManager.AppSettings["AuthUrlKey"].ToString();
            string[] array = AuthKey.Split('-');
            string timestamp = array[0];
            string rand = array[1];
            string uid = array[2];
            string md5value = array[3];
            string s = string.Format("{0}-{1}-{2}-{3}-{4}", Url, timestamp, 0, 0, privateKey);

            //如果手机时间与当前服务器时间相差5分钟，鉴权失败
            if (!ValidateDateTime(timestamp))
            {
                return false;
            }

            s = Md5Sum(s);
            if (s == md5value)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证客户端与服务端的有效时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static bool ValidateDateTime(string timestamp)
        {

            long jsTimeStamp = Convert.ToInt64(timestamp);
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime clientTime = startTime.AddMilliseconds(jsTimeStamp);


            if (System.Math.Abs((DateTime.Now - clientTime).TotalMinutes) > 10)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建授权url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string CreatAuthUrl(string url, string domain)
        {
            string privateKey = ConfigurationManager.AppSettings["AuthUrlKey"].ToString();
            string timeStamp = ConvertDateTime(DateTime.Now);


            string s = string.Format("{0}-{1}-{2}-{3}-{4}", url, timeStamp, 0, 0, privateKey);
            var authKey = Md5Sum(s);

            string result = string.Format("{0}-{1}-{2}-{3}", timeStamp, 0, 0, authKey);
            if (url.Contains("?"))
            {
                return url + "&auth_key=" + result;
            }
            else
            {
                return url + "?auth_key=" + result;
            }

        }

        static string Md5Sum(string strToEncrypt)
        {
            byte[] bs = UTF8Encoding.UTF8.GetBytes(strToEncrypt);
            return Md5Sum(bs);
        }
        static string Md5Sum(byte[] bs)
        {
            // 创建md5 对象  
            System.Security.Cryptography.MD5 md5;
            md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

            // 生成16位的二进制校验码  
            byte[] hashBytes = md5.ComputeHash(bs);

            // 转为32位字符串  
            string hashString = "";
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }


        static string ConvertDateTime(System.DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区

            long timeStamp = (long)(dt - startTime).TotalMilliseconds; // 相差毫秒数

            return timeStamp.ToString();
        }
    }
}
