using Albert.Commons.Helpers;
using Albert.Interface;
using Albert.Model;
using Albert.Utilities;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttpHelper = Albert.Utilities.HttpHelper;

namespace Albert.Extensions
{
    [Command("baget", Description = "Manage baget in remote service by command.")]
    public class BagetExtension : ICommand
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options; //依赖注入可选项
        private readonly ILogger<BagetExtension> loggers; //依赖注入日志服务

        public BagetExtension(IOptionsSnapshot<ProduceToolEntity> options, ILogger<BagetExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
        }

        [CommandParameter(0, Description = "")]
        public SupportFunc SupportF { get; set; }

        [CommandOption("deleteName", 'n', Description = "delete nuget package name.")]
        public string DeleteName { get; set; }

        [CommandOption("deleteVersion", 'v', Description = "delete nuget package version.")]
        public string DeleteVersion { get; set; }

        [CommandOption("pushPath", 'p', Description = "push local package to remote")]
        public string PushPath { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.WithColors(ConsoleColor.White, ConsoleColor.Black);
            if (string.IsNullOrEmpty(this.options.Value.BagetRule.NugetWebUrl))
            {
                throw new InvalidOperationException("Please set remote url in configs/producetool.json or environment.");
            }

            switch (SupportF)
            {
                case SupportFunc.list:
                    HttpResult _httpResult = new HttpHelper().Get(
                    string.Format(this.options.Value.BagetRule.SearchBagUrl, this.options.Value.BagetRule.NugetWebUrl));
                    if (!_httpResult.result)
                    {
                        throw new InvalidOperationException("Query package failed due to:" + _httpResult.html);
                    }
                    SearchBag bags = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchBag>(_httpResult.html);
                    if (bags.totalHits > 0)
                    {
                        foreach (var data in bags.data)
                        {
                            foreach (var version in data.versions)
                            {
                                console.WithColors(ConsoleColor.Blue, ConsoleColor.Black);
                                console.Output.WriteLine($"PackageName:{data.id}\t PackageVersion:{version.version}");
                            }
                        }
                    }
                    else
                    {
                        console.WithColors(ConsoleColor.DarkRed, ConsoleColor.Black);
                        console.Output.WriteLine("No package in remote services.");
                    }
                    break;
                case SupportFunc.push:
                    if (string.IsNullOrEmpty(this.options.Value.BagetRule.NugetKey))
                    {
                        throw new InvalidOperationException("Please set NugetKey in configs/producetool.json or environment etc.");
                    }
                    if (string.IsNullOrEmpty(PushPath))
                    {
                        throw new InvalidOperationException("Please input your local package directory");
                    }
                    //检索输入路径下所有文件，推送后缀为.nupkg的文件                    
                    var files = Directory.GetFiles(PushPath).Where(e => Path.GetExtension(e).ToLower() == ".nupkg");
                    foreach (var item in files)
                    {
                        var cmd = $"dotnet nuget push -s {this.options.Value.BagetRule.NugetWebUrl} -k " +
                            $"{this.options.Value.BagetRule.NugetKey} {item}";
                        CommandHelper.DataReceiveList.Add(cmd);
                    }
                    CommandHelper.ExecuteCmd(CommandHelper.DataReceiveList, PushPath);
                    console.Output.WriteLine($"Push successfully.");

                    break;
                case SupportFunc.del:
                    if (string.IsNullOrEmpty(this.options.Value.BagetRule.NugetKey))
                    {
                        throw new InvalidOperationException("Please set NugetKey in configs/producetool.json or environment etc.");
                    }
                    if (string.IsNullOrEmpty(DeleteName) || string.IsNullOrEmpty(DeleteVersion))
                    {
                        throw new InvalidOperationException("Please input nuget name and version");
                    }
                    console.Output.WriteLine($"You want to delete the package info:{DeleteName}-{DeleteVersion}");
                    console.Output.WriteLine($"Please input yes/no:");
                    var readKey = console.Input.ReadLine();
                    if (readKey.Contains("y", StringComparison.OrdinalIgnoreCase))
                    {
                        console.Output.WriteLine($"Starting delete....");
                        Dictionary<string, string> heads = new Dictionary<string, string>() { { "X-NuGet-ApiKey", this.options.Value.BagetRule.NugetKey } };
                        _httpResult = new HttpHelper().Delete(
                            string.Format(
                                this.options.Value.BagetRule.DelteBagUrl,
                                this.options.Value.BagetRule.NugetWebUrl,
                                DeleteName,
                                DeleteVersion), heads);
                        if (!_httpResult.result)
                        {
                            throw new InvalidOperationException("delete package failed due to:" + _httpResult.html);
                        }
                        else
                        {
                            console.Output.WriteLine($"Delete successfully.");
                        }
                    }
                    break;
                default:
                    goto case SupportFunc.list;
            }
        }
    }

    public enum SupportFunc
    {
        list,
        push,
        del,
    }
}
