using ProduceTools.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Interface
{
    public interface ICrawler
    {
        event EventHandler<OnStartEventArgs> OnStart;//爬虫启动事件

        event EventHandler<OnCompletedEventArgs> OnCompleted;//爬虫完成事件

        event EventHandler<OnErrorEventArgs> OnError;//爬虫出错事件
        Task<string> Start(string proxy =null, string inputUri = null); //异步爬虫
        void RunSimpleCrawlerExtension(IServiceProvider sp, string[] args);//执行爬虫
    }
}
