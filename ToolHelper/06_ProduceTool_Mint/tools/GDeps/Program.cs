namespace GDeps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    class Program
    {
        private const string TargetPathDir = @"$(TargetPathDir)";

        private const string TargetDevPath1 = @"$(TargetPathDir)";

        private const string TargetDevPath2 = @"$(TargetPathDir)\";

        private const string RemoteRootPath = @"//redmond/exchange/Build/SUBSTRATE/LATEST/target/";

        private const string FlavorPlatformDir = @"$(FlavorPlatformDir)";

        private const string BuildArchitecture = @"$(BuildType)\$(BuildArchitecture)";

        private const string DebugAmd64 = @"debug\amd64";

        static void Main(string[] args)
        {
            string? buildFilePath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj")?[0];
            if (buildFilePath == null)
            {
                ConsoleLog.Error($"Project file not found in current directory.");
                return;
            }

            UserInputParser.Parse(args, out string command, out string _, out string _);
            bool force = command == "-f";

            GetAllPaths(buildFilePath)
                .Where(path => path.StartsWithIgnoreCase(TargetPathDir))
                .Select(path => NormalizePath(path))
                .ForEachAsync(DownloadDllAsync, force).GetAwaiter().GetResult();
        }

        private static HashSet<string> GetAllPaths(string buildFilePath)
        {
            var targetPaths = new HashSet<string>();
            var allNodes = XDocument.Load(buildFilePath).Descendants();
            allNodes.Where(e => e.HasAttribute(Tags.Include)).ToList()
                    .ForEach(i => targetPaths.Add(i.GetAttribute(Tags.Include).Value));
            allNodes.Where(e => e.Is(Tags.HintPath)).ToList()
                    .ForEach(h => targetPaths.Add(h.Value));
            return targetPaths;
        }

        private static string NormalizePath(string path)
        {
            return path.Replace(FlavorPlatformDir, DebugAmd64)
                       .Replace(BuildArchitecture, DebugAmd64)
                       .Replace(TargetDevPath1, "")
                       .Replace(TargetDevPath2, "");
        }

        private static async Task DownloadDllAsync(string dllPath, bool force)
        {
            string remoteFile = (RemoteRootPath + dllPath).Replace('/', '\\');
            string localFile = Path.Combine(Repo.Paths.SrcDir, "target", dllPath);

            if (File.Exists(localFile) && !force)
            {
                Console.WriteLine($"Exists: {localFile}");
            }
            else if (!File.Exists(remoteFile))
            {
                Console.WriteLine($"Missing: {remoteFile}");
            }
            else
            {
                Console.WriteLine($"Downloading: {dllPath}");
                try
                {
                    using (var input = File.OpenRead(remoteFile))
                    {
                        using (var output = FileUtils.Create(localFile))
                        {
                            await input.CopyToAsync(output);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Fail downlaod: '{remoteFile}'.\n{e}");
                }
            }
        }
    }
}
