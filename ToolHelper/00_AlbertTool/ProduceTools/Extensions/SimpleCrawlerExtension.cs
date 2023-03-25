using Albert.Interface;
using Albert.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProduceTools.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#pragma warning disable SYSLIB0014


namespace Albert.Extensions
{
    public class SimpleCrawlerExtension : ICrawler
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options;//依赖注入可选项

        private readonly ILogger<SimpleCrawlerExtension> loggers; //依赖注入日志

        public event EventHandler<OnStartEventArgs> OnStart;//爬虫启动事件

        public event EventHandler<OnCompletedEventArgs> OnCompleted;//爬虫完成事件

        public event EventHandler<OnErrorEventArgs> OnError;//爬虫出错事件

        public CookieContainer CookiesContainer { get; set; }//定义Cookie容器

        public SimpleCrawlerExtension(IOptionsSnapshot<ProduceToolEntity> options, ILogger<SimpleCrawlerExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
        }

        /// <summary>
        /// 异步创建爬虫
        /// </summary>
        /// <param name="uri">爬虫URL地址</param>
        /// <param name="proxy">代理服务器</param>
        /// <returns>网页源代码</returns>
        public async Task<string> Start(string proxy = null,string inputUri = null)
        {
            var personalCrawling = options.Value.PersonalCrawling;
            Uri uri;
            if (string.IsNullOrEmpty(inputUri))
            {
                uri = new Uri(personalCrawling.PersonalCrawlingSite);
            }
            else
            {
                uri = new Uri(inputUri);
            }           
            return await Task.Run(() =>
            {
                var pageSource = string.Empty;
                try
                {
                    if (this.OnStart != null) this.OnStart(this, new OnStartEventArgs(uri));
                    var watch = new Stopwatch();
                    watch.Start();
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Accept = "*/*";
                    request.ServicePoint.Expect100Continue = personalCrawling.Expect100Continue;//加快载入速度
                    request.ServicePoint.UseNagleAlgorithm = personalCrawling.UseNagleAlgorithm;//禁止Nagle算法加快载入速度
                    request.AllowWriteStreamBuffering = personalCrawling.AllowWriteStreamBuffering;//禁止缓冲加快载入速度
                    request.Headers.Add(HttpRequestHeader.AcceptEncoding, personalCrawling.AcceptEncoding);//定义gzip压缩页面支持
                    request.ContentType = personalCrawling.ContentType;//定义文档类型及编码
                    request.AllowAutoRedirect = personalCrawling.AllowAutoRedirect;//禁止自动跳转,服务重定向                  
                    request.UserAgent = personalCrawling.UserAgent;//设置User-Agent，伪装成Google Chrome浏览器
                    request.Timeout = personalCrawling.Timeout;//定义请求超时时间为5秒
                    request.KeepAlive = personalCrawling.KeepAlive;//启用长连接
                    request.Method = personalCrawling.Method;//定义请求方式为GET              
                    if (proxy != null) request.Proxy = new WebProxy(proxy);//设置代理服务器IP，伪装请求地址
                    request.CookieContainer = this.CookiesContainer;//附加Cookie容器
                    request.ServicePoint.ConnectionLimit = int.MaxValue;//定义最大连接数

                    using (var response = (HttpWebResponse)request.GetResponse())//获取请求响应
                    {
                        foreach (Cookie cookie in response.Cookies) this.CookiesContainer.Add(cookie);//将Cookie加入容器，保存登录状态

                        if (response.ContentEncoding.ToLower().Contains("gzip"))//解压
                        {
                            using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    pageSource = reader.ReadToEnd();
                                }
                            }
                        }
                        else if (response.ContentEncoding.ToLower().Contains("deflate"))//解压
                        {
                            using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    pageSource = reader.ReadToEnd();
                                }

                            }
                        }
                        else
                        {
                            using (Stream stream = response.GetResponseStream())//原始
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {

                                    pageSource = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    request.Abort();
                    watch.Stop();
                    var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;//获取当前任务线程ID
                    var milliseconds = watch.ElapsedMilliseconds;//获取请求执行时间
                    if (this.OnCompleted != null) this.OnCompleted(this, new OnCompletedEventArgs(uri, threadId, milliseconds, pageSource));
                }
                catch (Exception ex)
                {
                    if (this.OnError != null) this.OnError(this, new OnErrorEventArgs(uri, ex));
                }
                return pageSource;
            });
        }

        public void RunSimpleCrawlerExtension(IServiceProvider sp, string[] args)
        {
            ///普通网站的爬虫，重定向问题需要单独配置相关的设置
            if ((args.Length > 0) && args[0].Contains("crawl"))
            {          
                var simpleCrawlerExtension = sp.GetRequiredService<ICrawler>();
                simpleCrawlerExtension.OnStart += (s, e) =>
                {
                    Console.WriteLine("爬虫开始抓取地址：" + e.Uri.ToString());
                    loggers.LogInformation("爬虫开始抓取地址：{@url}", e.Uri.ToString());
                };
                simpleCrawlerExtension.OnError += (s, e) =>
                {
                    Console.WriteLine("爬虫抓取出现错误：" +e.Exception.Message);
                    loggers.LogError("爬虫抓取出现错误：" + e.Exception.Message);
                };
                simpleCrawlerExtension.OnCompleted += (s, e) =>
                {
                    Console.WriteLine(e.PageSource);
                    //使用正则表达式清洗网页源代码中的数据
                    var links = Regex.Matches(e.PageSource, @"<a[^>]+href=""*(?<href>/hotel/[^>\s]+)""\s*[^>]*>(?<text>(?!.*img).*?)</a>", RegexOptions.IgnoreCase);
                    foreach (Match match in links) { }
                    Console.WriteLine("===============================================");
                    Console.WriteLine($"爬虫抓取任务完成！\n耗时：{e.Milliseconds}毫秒\n线程：{e.ThreadId}\n地址：{e.Uri.ToString()}");
                    loggers.LogInformation($"爬虫抓取任务完成！\n耗时：{e.Milliseconds}毫秒\n线程：{e.ThreadId}\n地址：{e.Uri.ToString()}");
                };              
                if (args[0].Contains("crawl-"))
                {
                    var normalWebSite = args[0].Split("-")[(args[0].Split("-")).Length - 1];
                    simpleCrawlerExtension.Start(null, normalWebSite).Wait();
                }
                else
                {
                    simpleCrawlerExtension.Start().Wait();
                }
                
            }
        }
    }
}
