using Albert.Interface;
using Albert.Model;
using Albert.Utilities;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.Common;
using NPOI.XWPF.UserModel;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Albert.Extensions
{
    [Command("tool",Description= "Some good features that I implemented myself.")]
    public class ToolExtension : ICommand
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options; //依赖注入可选项
        private readonly ILogger<ToolExtension> loggers; //依赖注入日志服务

        public ToolExtension(IOptionsSnapshot<ProduceToolEntity> options, ILogger<ToolExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
        }

        [CommandParameter(0, Description = "")]
        public ToolSupportFunc SupportF { get; set; }

        [CommandOption("Source", 's',Description = "SourcePath")]
        public string SourcePath { get; set; }

        [CommandOption("deleteVersion", 't', Description = "DestinationPath")]
        public string DestinationPath { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.WithColors(ConsoleColor.White, ConsoleColor.Black);     
            switch (SupportF)
            {
                case ToolSupportFunc.cp:

                    if (string.IsNullOrEmpty(SourcePath) || string.IsNullOrEmpty(DestinationPath))
                    {
                        throw new InvalidOperationException("Please set remote url in configs/producetool.json or environment.");
                    }

                    var copyFilePaths = new List<string>();
                    CopyDirectory(SourcePath, DestinationPath, copyFilePaths);
                    var optionsProgressBar = new ProgressBarOptions
                    {
                        ForegroundColor = ConsoleColor.Yellow,
                        ForegroundColorDone = ConsoleColor.DarkGreen,
                        BackgroundColor = ConsoleColor.DarkGray,
                        BackgroundCharacter = '\u2593'
                    };
                    using (var pbar = new ProgressBar(copyFilePaths.Count, $"Copy", optionsProgressBar))
                    {
                        Parallel.ForEach(copyFilePaths, filePath =>
                        {
                            File.Copy(filePath.Split(",")[0], filePath.Split(",")[1], true);
                            pbar.Tick();
                        });
                    }
                    break;
                case ToolSupportFunc.cptxt:
                    var listCopyPaths = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory+"Configs\\ListCopyPaths.txt");
                    listCopyPaths.ToList().ForEach(path=>{
                        if (string.IsNullOrEmpty(path))
                        {
                            return;
                        }
                    var copyFilePaths = new List<string>();
                    CopyDirectory(path.Split(",")[0], path.Split(",")[1], copyFilePaths);
                    var optionsProgressBar = new ProgressBarOptions
                    {
                        ForegroundColor = ConsoleColor.Yellow,
                        ForegroundColorDone = ConsoleColor.DarkGreen,
                        BackgroundColor = ConsoleColor.DarkGray,
                        BackgroundCharacter = '\u2593'
                    };
                    using (var pbar = new ProgressBar(copyFilePaths.Count, $"Copy", optionsProgressBar))
                    {
                        Parallel.ForEach(copyFilePaths, filePath =>
                        {
                            File.Copy(filePath.Split(",")[0], filePath.Split(",")[1], true);
                            pbar.Tick();
                        });
                    }
                    });
                    break;
                case ToolSupportFunc.md:
                    //这种方式也可以加载所有的程序集，但是在模式分离的情况下会找不到下一级
                    //解决方案递归查找：https://www.cnblogs.com/qianxingmu/p/13363193.html
                    //var assembly = Assembly.GetEntryAssembly().GetReferencedAssemblies();

                    //维护一个Dic，先从md文件中读取，Key为 包名_版本号 Value为注释
                    Dictionary<string,string> dic = new Dictionary<string,string>();
                    string mkdownPath = AppDomain.CurrentDomain.BaseDirectory + "Configs\\NugetPackageDescription.md";
                    var packageInfos = await File.ReadAllLinesAsync(mkdownPath);
                    foreach (var package in packageInfos)
                    {
                        var packageInfo = package.Split(":")[0];
                        var packageNote = package.Split(":")[1];
                        dic.Add(packageInfo, packageNote);
                    }

                    var projDir = await File.ReadAllLinesAsync(AppDomain.CurrentDomain.BaseDirectory + "Configs\\ProjectDir.txt");
                    List<string> listProjPaths = new List<string>();
                    //将所有指定目录下的*.csproj路径扫描出来
                    projDir.ToList().ForEach(path => Directory.EnumerateFiles(path, "*.csproj").ToList().ForEach(e => listProjPaths.Add(e)));

                    if(listProjPaths.Count < 0)
                    {
                        loggers.LogInformation("所有路径下面都没有项目文件，请检查");
                    }                  

                    try
                    {
                        foreach (var projPath in listProjPaths)
                        {
                            var file = await XDocument.LoadAsync(File.OpenRead(projPath), LoadOptions.None, console.RegisterCancellationHandler());
                            var references = file.Root.Descendants().Where(x => x.Name == "PackageReference");
                            foreach (XElement item in references)
                            {
                                var include = item.Attribute("Include").Value;
                                var version = item.Attribute("Version").Value;
                                var packageInfo = include + "_" + version;
                                var packageNote = "";
                                //如果dic里面没有包含检索出来的键值，则添加
                                if (!dic.ContainsKey(packageInfo))
                                {
                                    dic.Add(packageInfo, packageNote);
                                }
                            }
                        }

                        var writeTexts = dic.Select(e => $"{e.Key}:{e.Value}");
                        File.WriteAllLines(mkdownPath, writeTexts);
                        loggers.LogInformation($"Success:{mkdownPath}");
                    }
                    catch (Exception ex)
                    {

                       loggers.LogInformation(ex.ToString());
                    }                    
                    break;
                default:
                    goto case ToolSupportFunc.cp;
            }
        }

        private void CopyDirectory(string sourcePath, string destPath,List<string> copyFilePaths)
        {
            string floderName = Path.GetFileName(sourcePath);
            string[] files = Directory.GetFileSystemEntries(sourcePath);
            DirectoryInfo di = new DirectoryInfo(Path.Combine(destPath, floderName));

            if (!Directory.Exists(Path.Combine(destPath, floderName)))
            {
                di = Directory.CreateDirectory(Path.Combine(destPath, floderName));
            }

            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    CopyDirectory(file, di.FullName,copyFilePaths);
                }
                else
                {
                    copyFilePaths.Add(file + "," + Path.Combine(di.FullName, Path.GetFileName(file)));
                }
            }
        }
    }

    public enum ToolSupportFunc
    {
        cp,
        cptxt,
        md
    }
}
