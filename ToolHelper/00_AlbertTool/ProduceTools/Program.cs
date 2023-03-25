using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Serilog;
using Serilog.Formatting.Json;
using Albert.Interface;
using Exceptionless;
using Albert.Model;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CliFx;

namespace Albert
{
    class Program
    {
        private static ServiceCollection service = new ServiceCollection();

        /// <summary>
        /// <para>初始化DI:<see cref="InitService"/>
        /// </para>
        /// <para>Git拓展:<see cref="Extensions.GitExtension.ExecuteAsync(CliFx.Infrastructure.IConsole)"/>
        /// </para>
        /// <para>Baget拓展:<see cref="Extensions.BagetExtension.ExecuteAsync(CliFx.Infrastructure.IConsole)"/>
        /// </para>
        /// <para>Tool拓展:<see cref="Extensions.ToolExtension.ExecuteAsync(CliFx.Infrastructure.IConsole)"/>
        /// </para>
        /// <para>Produce自动化:<see cref="Extensions.ProduceExtension.RunProduceExtensions"/>
        /// </para>
        /// <para>常规网站爬虫：<see cref="Extensions.SimpleCrawlerExtension.RunSimpleCrawlerExtension"/>
        /// </para>
        /// <para>Azure云API分析：<see cref="Extensions.AzureDevOpsExtension.RunAzureDevOpsExtension"/>
        /// </para>
        /// <para>公司流程:<see cref="Extensions.CompanyToolExtensions.RunCompanyToolExtensions"/>
        /// </para>
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>
        /// 目前支持五类工具:
        /// 1.Git拓展：简化git流程，不需要整一大堆指令;
        /// 2.Nuget包上传删掉罗列
        /// 3.私有工具：快速拷贝整个文件夹
        /// 4.常规Produce流程：在readme.md文件中已描述；
        /// 5.常规网站的爬虫程序；
        /// 6.Azure云反爬虫的爬虫程序；
        /// 7.公司流程自动化执行。
        /// </remarks>
        static async Task Main(string[] args)
        {          
            InitService();
            //将注入的对象创建成实例service.BuildServiceProvider()
            using (var sp = service.BuildServiceProvider())
            {
                sp.GetRequiredService<ICrawler>().RunSimpleCrawlerExtension(sp, args);
                sp.GetRequiredService<IHelper>().RunHelperInfoExtension(sp, args);
                await sp.GetRequiredService<ICompanyTool>().RunCompanyToolExtensions(sp, args);

                //使用的前置条件是service要将实现ICommand接口的对象注入进去
                //InitService()方法中:service.AddGitExtensions();实现注入
                //从当前程序集注入所有实现ICommand接口的CMD指令
                var application = new CliApplicationBuilder()
               .AddCommandsFromThisAssembly()
               .UseTypeActivator(t =>sp.GetRequiredService(t))
               .Build();
                //执行CMD
                await application.RunAsync();
            }
        }

        static void InitService()
        {
            service.AddGitExtensions();
            service.AddBagetExtensions();
            service.AddToolExtensions();
            service.AddSimpleCrawlerExtensions();
            service.AddSerilogExtensions();
            service.AddGetHelperExtensions();
            service.AddCompanyToolExtensions();

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();          
            configurationBuilder.AddJsonFile("Configs\\ProduceTool.Json", false, true);
            
            //If not judge, configurationBuilder.Build() will error:can't find user-secrets
            //EnvironmentVariableTarget.Machine is used for system variable value.
            if(string.Equals(Environment.
                GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT",EnvironmentVariableTarget.Machine),"Development",
                StringComparison.OrdinalIgnoreCase))
            {
                //从sqlserver数据库中获取数据，暂时先手写连接字符串,设置超时时间从默认15s变为5s
                string strConfigFromSqlserver = "Server = .; Database = AlbertConfigDb; Trusted_Connection = True;MultipleActiveResultSets=true;Connect Timeout=500";
                SqlConnection sqlConnection = null;
                bool sqlConnectionStatus = true;

                try
                {
                    using (sqlConnection = new SqlConnection(strConfigFromSqlserver))
                    {
                        sqlConnection.Open();
                    }
                }
                catch (Exception ex)
                {
                    sqlConnectionStatus = false;
                    Console.WriteLine(ex.Message);
                }

                if (sqlConnectionStatus)
                {
                    configurationBuilder.AddDbConfiguration(() => new SqlConnection(strConfigFromSqlserver),
                        reloadOnChange: true,
                        reloadInterval: TimeSpan.FromSeconds(2),
                        tableName: "ProduceToolConfig");
                }

                configurationBuilder.AddUserSecrets<Program>();//防止机密信息上传到Github
            }
           
            var rootConfig = configurationBuilder.Build();
            service.AddOptions().Configure<ProduceToolEntity>(e => rootConfig.Bind(e))
                .Configure<Repo>(e => rootConfig.GetSection("Repo").Bind(e))
                .Configure<MsBuild>(e => rootConfig.GetSection("MsBuild").Bind(e))
                .Configure<AzureDevOps>(e => rootConfig.GetSection("AzureDevOps").Bind(e))
                .Configure<PersonalCrawling>(e => rootConfig.GetSection("PersonalCrawling").Bind(e))
                .Configure<HelperInfo>(e => rootConfig.GetSection("HelperInfo").Bind(e))
                .Configure<BagetRule>(e => rootConfig.GetSection("BagetRule").Bind(e))
                .Configure<SqlServer>(e=>rootConfig.GetSection("SqlServer").Bind(e))
                .Configure<SqlServer>(e => rootConfig.GetSection("RedisServer").Bind(e));

            //ToDo:Serilog Write Information to File
            using (var sp = service.BuildServiceProvider())
            {
                var serilogExtension = sp.GetRequiredService<ISeriLog>();
                if (serilogExtension.OpenExceptionlessClient())
                {
                    //配置ExceptionlessClient启动密钥,从UserSecrets-ProduceTool.Json获取
                    //数据将记录到Exceptionless，网址为：https://be.exceptionless.io/frequent
                    ExceptionlessClient.Default.Startup(serilogExtension.ExceptionlessClientDefaultStartUpKey);
                    ExceptionlessClient.Default.Configuration.SetDefaultMinLogLevel(Exceptionless.Logging.LogLevel.Trace);
                    service.AddLogging(e => {
                        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(new JsonFormatter())
                        .WriteTo.Exceptionless()
                        .CreateLogger();
                        e.AddSerilog();
                    });
                }
                else
                {
                    service.AddLogging(e => {
                        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(new JsonFormatter())
                        .CreateLogger();
                        e.AddSerilog();
                    });
                }
            }            
        }   
    }
}
