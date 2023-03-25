using Albert.Interface;
using Albert.Model;
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
using System.Threading.Tasks;

namespace Albert.Extensions
{
    [Command("git",Description="It's easier to push your code to remote repo.")]
    public class GitExtension : ICommand
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options; //依赖注入可选项
        private readonly ILogger<GitExtension> loggers; //依赖注入日志服务

        public GitExtension(IOptionsSnapshot<ProduceToolEntity> options, ILogger<GitExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
        }
       
        [CommandOption("push", 'p', Description = "Push your code to remote repo directly")]
        public string? PushCodeDirectly { get; set; }

        [CommandParameter(0, Description = "some comments of your commit record.")]
        public string? Comments { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (string.IsNullOrWhiteSpace(Comments))
            {
                throw new InvalidOperationException("please write comments");
            }
            console.Output.WriteLine("Starting ....");
            GitCommand.ChangeSrc();
            GitCommand.GitAdd();
            GitCommand.Commit(Comments);
            GitCommand.Push();
            //命令行输出
            console.Output.WriteLine("Done!");
            //记录日志
            loggers.LogInformation("Run Successfully!");
        }
    }

    public static class GitCommand
    {
        public static string Src { get; set; }
        public static void ChangeSrc()
        {
            GitCommandExcute(Src, "cd \\");
            GitCommandExcute(Src, $"cd {Src}");
        }
        public static void OpenInput(string cmd) => GitCommandExcute(Src, cmd);
        public static void GitAdd() => GitCommandExcute(Src, "git add .");
        public static void GetGitVersion() => GitCommandExcute(Src, "git --version");
        public static void Clone(string repo) => GitCommandExcute(Src, "git clone " + repo + " .");
        public static void Chekcout(string branch) => GitCommandExcute(Src, $"git checkout {branch}");
        public static void NewBranch(string branchName) => GitCommandExcute(Src, $"git checkout -b {branchName}");
        public static void Pull() => GitCommandExcute(Src, "git pull");
        public static void Commit() => GitCommandExcute(Src, "git commit -am update");
        public static void Commit(string commitNote) => GitCommandExcute(Src, $"git commit -m \"{commitNote}\"");
        public static void Stash() => GitCommandExcute(Src, "git stash");
        public static void Push() => GitCommandExcute(Src, "git push");
        public static void Push(string branchName) => GitCommandExcute(Src, $"git push --set-upstream origin {branchName}");
        public static void DeleteBranch(string branchName)
        {
            GitCommandExcute(Src, $"git branch -D {branchName}");
            GitCommandExcute(Src, $"git push origin --delete {branchName}");
        }
        public static void ProduceNetCore() => GitCommandExcute(Src, "produce netcore");

        //Fix everycommand execute exit command.
        //批量执行最后一步退出方法在CompanyToolExtensions中已实现
        public static void GitCommandExcute(string path, string command)
        {
            using (Process process = new Process())
            {
                //Process对象如果是exe必须设置为false
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = path;           
                process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");
                //标准重定向流
                Console.WriteLine(process.StartInfo.FileName);
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //抓取事件
                process.OutputDataReceived += new DataReceivedEventHandler(OutputEventHandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(ErrorEventHandler);
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                //这个仅仅用于进去Ensliment目录下
                //process.StandardInput.WriteLine("\"%ProgramFiles(x86)%\\Microsoft Visual Studio\\2022\\Preview\\Common7\\Tools\\VsDevCmd.bat\"");
                process.StandardInput.WriteLine(command + "&exit");
                process.StandardInput.AutoFlush = true;
                process.WaitForExit();
                process.Close();
            }
        }
        private static void OutputEventHandler(Object sender, DataReceivedEventArgs e) => Console.WriteLine(e.Data);
        private static void ErrorEventHandler(Object sender, DataReceivedEventArgs e) => Console.WriteLine(e.Data);
    }
}
