using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Utilities
{
    public class HttpHelper
    {
        private static string HOST;

        static HttpHelper()
        {
            HOST = "";
        }

        public HttpResult Delete(string url)
        {
            return _Delete(url, null, System.Text.Encoding.UTF8);
        }

        public HttpResult Delete(string url, Dictionary<string, string> heads)
        {
            return _Delete(url, heads, System.Text.Encoding.UTF8);
        }

        public HttpResult Get(string url, string host = "")
        {
            return Get(url, System.Text.ASCIIEncoding.Default, host);
        }

        public HttpResult Get(string url, Encoding encoding, string host = "")
        {
            if (string.IsNullOrEmpty(host))
                host = HOST;
            return _Get(host + url, encoding);
        }

        public Task<HttpResult> GetAsync(string url, string host = "")
        {
            return Task.Run<HttpResult>(() =>
            {
                return Get(url, System.Text.ASCIIEncoding.Default, host);
            });
        }

        public Task<HttpResult> GetAsync(string url, Encoding encoding, string host = "")
        {
            return Task.Run<HttpResult>(() =>
            {
                if (string.IsNullOrEmpty(host))
                    host = HOST;
                return _Get(host + url, encoding);
            });
        }

        public HttpResult Post(string url, Dictionary<string, string> param, string host = "")
        {
            if (string.IsNullOrEmpty(host))
                host = HOST;

            return Post(url, param, System.Text.ASCIIEncoding.Default, host);
        }

        public HttpResult Post(string url, Dictionary<string, string> param, Encoding encoding, string host = "")
        {
            if (!string.IsNullOrEmpty(host))
                HOST = host;
            return _Post(HOST + url, param, encoding);
        }

        public Task<HttpResult> PostAsync(string url, Dictionary<string, string> param, string host = "")
        {
            return Task.Run<HttpResult>(() =>
            {
                if (string.IsNullOrEmpty(host))
                    host = HOST;

                return Post(url, param, System.Text.ASCIIEncoding.Default, host);
            });
        }

        public Task<HttpResult> PostAsync(string url, Dictionary<string, string> param, Encoding encoding, string host = "")
        {
            return Task.Run<HttpResult>(() =>
            {
                if (!string.IsNullOrEmpty(host))
                    HOST = host;
                return _Post(HOST + url, param, encoding);
            });
        }

        private HttpResult _Delete(string url, Dictionary<string, string> heads, Encoding encoding)
        {
            HttpResult r = new HttpResult();
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.Proxy = null;
                httpWebRequest.Method = "Delete";
                foreach (var head in heads)
                {
                    httpWebRequest.Headers.Add(head.Key, head.Value);
                }
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                r.result = true;
                r.html = html;
                return r;
            }
            catch (Exception e)
            {
                r.html = e.ToString();
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                return r;
            }
        }

        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private HttpResult _Get(string url, Encoding encoding)
        {
            HttpResult r = new HttpResult();
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.Proxy = null;
                httpWebRequest.Method = "GET";
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                r.result = true;
                r.html = html;
                return r;
            }
            catch (Exception e)
            {
                r.html = e.ToString();
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                return r;
            }
        }

        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private HttpResult _Post(string url, Dictionary<string, string> paramss, Encoding encoding)
        {
            //System.IO.File.AppendAllText(@"d:\a.txt", url);
            HttpResult r = new HttpResult();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Proxy = null;
            try
            {
                string param = "";
                foreach (string p in paramss.Keys)
                {
                    param += (p + "=" + paramss[p] + "&");
                }
                byte[] bs = Encoding.UTF8.GetBytes(param);
                string responseData = String.Empty;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = bs.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        responseData = reader.ReadToEnd().ToString();
                    }
                    r.result = true;
                    r.html = responseData;
                }
            }
            catch (Exception e)
            {
                r.html = e.ToString();
                if (req != null) req.Abort();
                return r;
            }
            return r;
        }
    }

    public class HttpResult
    {
        public string html;
        public bool result;
    }

    public class WebMobileResult<T>
    {
        public string Message { get; set; } = string.Empty;

        public T Obj { get; set; }

        public bool Result { get; set; } = false;
    }
}
