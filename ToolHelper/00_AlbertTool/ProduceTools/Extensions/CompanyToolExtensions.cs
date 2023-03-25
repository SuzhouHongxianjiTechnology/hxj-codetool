using Albert.Commons.Helpers;
using Albert.Interface;
using Albert.Model;
using Albert.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Albert.Extensions
{
    internal class CompanyToolExtensions:ICompanyTool
    {
        private string CompanyToolEnlistmentPath { get; set; }
        private string Src { get; set; }
        private readonly IOptionsSnapshot<ProduceToolEntity> options; //依赖注入可选项
        private readonly ILogger<GitExtension> loggers; //依赖注入日志服务

        public CompanyToolExtensions(IOptionsSnapshot<ProduceToolEntity> options, ILogger<GitExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
            //@"^\$.*\$" 这样的规则匹配也可以
            this.CompanyToolEnlistmentPath =Regex.Replace(options.Value.Repo.CompanyToolEnlistmentPath, @"\$[^$]+\$", options.Value.Repo.DefaultPath, RegexOptions.IgnoreCase);
            this.Src = options.Value.Repo.DefaultPath.Replace('\\','/');
        }

        public void EnterProducePath(string path)
        {
            CommandHelper.ExecuteCmd("cd \\", options.Value.Repo.DefaultPath);
            CommandHelper.ExecuteCmd($"cd {path}", options.Value.Repo.DefaultPath);
        }
        public void ExcuteProduceNetcore() => CommandHelper.ExecuteCmd("produce netcore", options.Value.Repo.DefaultPath);
        public void ExcuteGdepsF() => CommandHelper.ExecuteCmd("gdeps -f", options.Value.Repo.DefaultPath);
        public void ExcuteMsbuildRestore() => CommandHelper.ExecuteCmd("msbuild -t:restore", options.Value.Repo.DefaultPath);
        public void ExcuteBcc() => CommandHelper.ExecuteCmd("bcc", options.Value.Repo.DefaultPath);
        public void ExcuteBccr() => CommandHelper.ExecuteCmd("bccr", options.Value.Repo.DefaultPath);
        public void ExcuteGetDepsFlavorRetail() => CommandHelper.ExecuteCmd("getdeps /flavors:retail", options.Value.Repo.DefaultPath);


        public async Task RunCompanyToolExtensions(IServiceProvider sp, string[] args)
        {
            var argsStr = string.Join(" ", args);
            var companyToolExtensions = sp.GetRequiredService<ICompanyTool>();
            var producePathsTxtFile = AppDomain.CurrentDomain.BaseDirectory + "Configs\\ProducePaths.txt";
            var bccrPathsTxtFile = AppDomain.CurrentDomain.BaseDirectory + "Configs\\BccrPaths.txt";
            //进入公司开发流程
            if ((args.Length > 0) && args[0].Contains("company"))
            {
                //albert company produce
                //批量Produce,采取读取Txt的方式，来批量操作相应的流程
                if ((args.Length > 1) && args[1].Contains("produce"))
                {
                    if (File.Exists(producePathsTxtFile))
                    {
                        //读取文件路径，进入对应的Produce netcore目录下
                        var producePathList = await File.ReadAllLinesAsync(producePathsTxtFile);
                        List<string> strList = new List<string>();
                        //执行bat脚本，启动Enlistment
                        strList.Add(CompanyToolEnlistmentPath);
                        foreach (var item in producePathList)
                        {
                            //将路径格式变为正确的
                            item.Replace('\\', '/');
                            //进入到Produce NetFX目录
                            strList.Add($"cd {this.Src + "/" + item}");
                            //执行produce netcore指令
                            strList.Add("produce netcore");
                            //进入到Produce NetCore目录
                            strList.Add($"cd {this.Src + "/" + item}.NetCore");
                            //进行依赖项下载 gdeps -f
                            strList.Add("gdeps -f");
                            //进行msbuild -t:restore进行项目依赖项恢复
                            strList.Add("msbuild -t:restore");
                            //进行编译bcc
                            strList.Add("bcc");
                        }
                        CommandHelper.ExecuteCmd(strList, options.Value.Repo.DefaultPath);
                    }
                    else
                    {
                        loggers.LogWarning("文件不存在");
                    }              
                }  
                //albert company bccr
                else if((args.Length > 1) && args[1].Contains("bccr"))
                {                   
                    if (File.Exists(bccrPathsTxtFile))
                    {
                        //读取文件路径，获取需要bccr的项目路径
                        var bccrPathList = await File.ReadAllLinesAsync(bccrPathsTxtFile);
                        List<string> strList = new List<string>();
                        //执行bat脚本，启动Enlistment
                        //strList.Add(CompanyToolEnlistmentPath);
                        foreach (var item in bccrPathList)
                        {
                            item.Replace('\\', '/');
                            string itemNetCore = $"{ this.Src + "/" + item }.NetCore";
                            if (!Directory.Exists(itemNetCore))
                            {
                                itemNetCore = $"{ this.Src + "/" + item }.NetStd";
                            }                            
                            //执行到NetCore/NetStd目录
                            strList.Add($"cd {itemNetCore}");
                            //执行依赖项下载 gdeps -f
                            strList.Add("gdeps -f");
                            //执行msbuild -t:restore进行项目依赖项恢复
                            strList.Add("msbuild -t:restore");
                            //执行getdeps /flavors:retail
                            strList.Add("getdeps /flavors:retail");
                            //执行bcc编译
                            strList.Add("build -cC");
                            //执行bccr编译
                            strList.Add("build -cC retail");
                        }
                        CommandHelper.ExecuteCmd(strList, options.Value.Repo.DefaultPath);
                    }
                    else
                    {
                        loggers.LogWarning("文件不存在");
                    }
                }
            }
        }

    }
}
