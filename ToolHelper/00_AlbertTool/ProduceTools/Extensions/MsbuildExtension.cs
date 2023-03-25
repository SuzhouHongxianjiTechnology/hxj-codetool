using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Albert.Extensions
{
    public class MsbuildExtension
    {
        public MsbuildExtension()
        {
        }
        public MsbuildExtension(string src) => this.Src = src;
        private string Src { get; set; }
        public List<string> buildResult { get; set; }

        public void Restore() => MsbuildCommandExcute(this.Src, "msbuild -t:restore");
        public void Clean() => MsbuildCommandExcute(this.Src, "msbuild -t:clean");
        public void Msbuild() => MsbuildCommandExcute(this.Src, "msbuild");

        private void MsbuildCommandExcute(string path, string command)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = path;
                process.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                this.buildResult = null;
                process.OutputDataReceived += new DataReceivedEventHandler(OutputEventHandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(ErrorEventHandler);
                process.Start();

                process.StandardInput.WriteLine("\"%ProgramFiles(x86)%\\Microsoft Visual Studio\\2022\\Preview\\Common7\\Tools\\VsDevCmd.bat\"");
                process.StandardInput.WriteLine(command);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.StandardInput.WriteLine("exit");
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
        }
        private void OutputEventHandler(Object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Length <= 1000)
                {
                    Match message = Regex.Match(e.Data, ".*error\\s+nu.*", RegexOptions.IgnoreCase);
                    if (message.Success)
                    {
                        string errorMessage = Regex.Replace(message.Value.TrimStart(), @"\d+>", "");
                        if (this.buildResult == null) this.buildResult = new List<string>();
                        if (!buildResult.Select(x => x.ToString().Equals(errorMessage, StringComparison.OrdinalIgnoreCase)).Contains(true)) this.buildResult.Add(errorMessage);
                    }
                }
            }
            Console.WriteLine(e.Data);
        }
        private void ErrorEventHandler(Object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Length <= 1000)
                {
                    Match message = Regex.Match(e.Data, ".*error\\s+nu.*", RegexOptions.IgnoreCase);
                    if (message.Success)
                    {
                        string errorMessage = Regex.Replace(message.Value.TrimStart(), @"\d+>", "");
                        if (this.buildResult == null) this.buildResult = new List<string>();
                        if (!buildResult.Select(x => x.ToString().Equals(errorMessage, StringComparison.OrdinalIgnoreCase)).Contains(true)) this.buildResult.Add(errorMessage);
                    }
                }
            }
            Console.WriteLine(e.Data);

        }
    }
}
