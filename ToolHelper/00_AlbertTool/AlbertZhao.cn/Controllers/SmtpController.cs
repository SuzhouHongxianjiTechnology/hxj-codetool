using AlbertZhao.cn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AlbertZhao.cn.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SmtpController : ControllerBase
    {
        private readonly ILogger<SmtpController> logger;//日志服务
        private readonly IOptionsSnapshot<SmtpSettings> options;//配置选项服务
        private readonly IConnectionMultiplexer connectionMultiplexer;//Redis服务

        public SmtpController(ILogger<SmtpController> logger, IOptionsSnapshot<SmtpSettings> options, IConnectionMultiplexer connectionMultiplexer)
        {
            this.logger = logger;
            this.options = options;
            this.connectionMultiplexer = connectionMultiplexer;
        }


        [HttpGet]
        public ActionResult<SmtpSettings?> GetSmtpInfo()
        {
            logger.LogInformation("开始获取数据");
            return new SmtpSettings() { ServerName = options.Value.ServerName ,
                UserName = options.Value.UserName ,
                Password = options.Value.Password
            };
        }

        [HttpGet]
        public ActionResult<string?> GetRedisInfo()
        {
            logger.LogInformation("开始测试Redis");
            var ping = connectionMultiplexer.GetDatabase(0).Ping();
            return ping.ToString();
        }

    }
}
