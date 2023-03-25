namespace MapiAnalyser.Cache
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Mint.Common;
    using Mint.Database.APIs;
    using Mint.Database.Enums;
    using Mint.Substrate;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Utilities;

    public static class MapiCache
    {
        public static void CacheAssemblyData()
        {
            Timer.Start();
            var allAssemblies = FileUtils.ReadLines(Files.MAPI_ALL_ASSEMBLIES);
            int total = allAssemblies.Count;
            int current = 0;
            ConsoleLog.Title("Caching MapiHttp Assembly Datas:");
            var cache = new List<AssemblyData>();
            foreach (var assembly in allAssemblies)
            {
                ConsoleLog.Ignore($"{++current,3}/{total} {assembly}");
                var detail = AssemblyDetails.RequestAsync(assembly, process: Process.MapiHttp).Result;
                if (detail == null)
                {
                    ConsoleLog.Error("Cannot get assembly detail, check on DGT." + Environment.NewLine +
                                 $"(Assembly {assembly})");
                    continue;
                }

                var projectPath = Path.Combine(DF.SrcDir, detail.SourcePath.Split(@"Sources\").Last());
                if (!File.Exists(projectPath))
                {
                    ConsoleLog.Error("Project file not exists in DF, is it newly added on master?" + Environment.NewLine +
                                 $"(File {projectPath})");
                    continue;
                }

                string assemblyName = detail.AssemblyName;
                var isProduced = DF.RestoreEntry.IsProduced(assemblyName);
                var blockedBy = GetBlockedList(projectPath);
                var incompatibleAPIs = detail.ReferencedIncompatibleAPIs ?? new List<string>();
                var filteredAPIs = detail.FilteredAPIs ?? new List<string>();

                cache.Add(new AssemblyData
                {
                    FilePath = projectPath,
                    AssemblyName = assemblyName,
                    IsProduced = isProduced,
                    BlockedBy = blockedBy,
                    IncompatibleAPIs = incompatibleAPIs,
                    FilteredAPIs = filteredAPIs
                });
            }
            var json = JsonSerializer.Serialize<List<AssemblyData>>(cache);
            FileUtils.WriteJson(Files.MAPI_CACHE, json);
            Timer.Stop();
        }

        public static List<AssemblyData> ReadCache()
        {
            var json = FileUtils.ReadJson(Files.MAPI_CACHE);
            return JsonSerializer.Deserialize<List<AssemblyData>>(json);
        }

        private static List<string> GetBlockedList(string projectPath)
        {
            var blocked = new List<string>();

            if (!File.Exists(projectPath))
            {
                ConsoleLog.Error("Cannot get block list, file not exists." + Environment.NewLine +
                             $"{projectPath}");
                return blocked;
            }

            var csproj = new NetFrameworkProjectFile(projectPath);
            foreach (var reference in csproj.References)
            {
                if (reference.Type == ReferenceType.Substrate)
                {
                    string assembly = reference.Name;
                    if (!DF.RestoreEntry.IsProduced(assembly))
                    {
                        blocked.Add(assembly);
                    }
                }
            }

            foreach (var reference in csproj.ProjectReferences)
            {
                string assembly = reference.Name;
                if (!DF.RestoreEntry.IsProduced(assembly))
                {
                    blocked.Add(assembly);
                }
            }

            return blocked;
        }
    }
}
