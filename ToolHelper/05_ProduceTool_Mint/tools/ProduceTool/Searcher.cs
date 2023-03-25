namespace ProduceTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Mint.Common;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    internal static class Searcher
    {
        internal static void SearchString(string[] args)
        {
            string keyword = args.Length > 1 ? args[1] : string.Empty;
            if (string.IsNullOrEmpty(keyword))
            {
                ConsoleLog.Error("Please specify a search keyword.");
                return;
            }

            bool foundAny = false;
            foreach (var projectPath in DF.RestoreEntry.ProjectPaths)
            {
                if (!File.Exists(projectPath)) continue;

                int lineNum = 0;
                bool isFirst = true;
                bool anyInFile = false;
                foreach (string line in File.ReadAllLines(projectPath))
                {
                    lineNum++;
                    if (line.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        foundAny = anyInFile = true;
                        if (isFirst)
                        {
                            ConsoleLog.Path($"{projectPath}");
                            isFirst = false;
                        }
                        ConsoleLog.Success($"line: {lineNum}", inLine: true);
                        ConsoleLog.Warning($"  {line.Trim()}");
                    }
                }
                if (anyInFile)
                {
                    ConsoleLog.Ignore("----------------------------------------------------------------");
                }
            }
            if (!foundAny)
            {
                ConsoleLog.Error($"Can't find '{keyword}' in all NetStd/NetCore projects.");
            }
        }

        internal static void SearchNoWarn(string warningCode)
        {
            var result = new Dictionary<string, List<string>>();

            foreach (var projectPath in DF.RestoreEntry.ProjectPaths)
            {
                var project = new NetCoreProjectFile(projectPath);
                foreach (var packageReference in project.PackageReferences)
                {
                    if (packageReference.HasNoWarn(warningCode))
                    {
                        string pkgName = packageReference.Name;
                        if (result.ContainsKey(pkgName))
                        {
                            result[pkgName].Add(project.FullPath);
                        }
                        else
                        {
                            result.Add(pkgName, new List<string> { project.FullPath });
                        }
                    }
                }
            }

            ConsoleLog.Message($"Result for {warningCode}:");
            foreach (var key in result.Keys)
            {
                ConsoleLog.Ignore("----------------------------------------------------------------");
                ConsoleLog.Warning(key);
                var projects = result[key];
                foreach (var proj in projects)
                {
                    ConsoleLog.Path(proj);
                }
            }
        }

    }
}
