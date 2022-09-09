namespace ProcessAnalyser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Database;
    using Mint.Database.Remote;
    using Mint.Substrate;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Utilities;

    public static class ProcessDataCache
    {
        private static LookupTable LookupTable = new LookupTable();

        private static readonly HashSet<string> IgnoreNuGetDlls = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft.Flighting.Runtime.dll",
            "Microsoft.CSharp.dll",
            "System.Web.dll",
            "Microsoft.WindowsAzure.Storage.v4.3.0.dll",
        };

        private static readonly HashSet<string> MarkAsProduced = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft.Forefront.AntiSpam.XMI",
        };

        public static void CacheProcessData(Process process, string? version = null)
        {
            using (Timer.TimeThis)
            {
                // ----------------------------------------------------------------
                ConsoleLog.Title($"\nCaching {process} NonSub Data:");

                var nonsubCache = new List<NonSubDllData>();

                if (string.IsNullOrEmpty(version))
                {
                    version = Versions.GetLatestVersionAsync().Result;
                }
                var allNonSubDlls = ProcessAssemblies.RequestAsync(version, 1, process).Result;
                var getNonSubTasks = allNonSubDlls.Select(assembly => GetNonSubData(process, assembly, version));
                var results = AsyncUtils.WhenAll<NonSubDllData>(getNonSubTasks, workers: 64).Result;

                foreach (var result in results)
                {
                    if (result != null)
                    {
                        nonsubCache.Add(result);
                    }
                }
                // + security
                nonsubCache.Add(new NonSubDllData
                {
                    DllName = "Microsoft.Exchange.Security.dll",
                    PackageName = "Microsoft.Exchange.Security",
                    IsCompatible = true
                });

                string json = JsonSerializer.Serialize<List<NonSubDllData>>(nonsubCache);
                FileUtils.WriteJson(Files.nonsubCacheMap[process], json);

                ConsoleLog.Title($"Total: {allNonSubDlls.Count()}");

                // ----------------------------------------------------------------
                ConsoleLog.Title($"\nCaching {process} Substrate Data:");

                var subCache = new List<SubDllData>();

                if (string.IsNullOrEmpty(version))
                {
                    version = Versions.GetLatestVersionAsync().Result;
                }
                var allSubDlls = ProcessAssemblies.RequestAsync(version, 2, process).Result;
                var getSubTasks = allSubDlls.Select(assembly => GetSubData(process, assembly, version));
                var subResult = AsyncUtils.WhenAll<SubDllData>(getSubTasks, workers: 64).Result;

                foreach (var result in subResult)
                {
                    if (result != null)
                    {
                        subCache.Add(result);
                    }
                }

                string subJson = JsonSerializer.Serialize<List<SubDllData>>(subCache);
                FileUtils.WriteJson(Files.subCacheMap[process], subJson);

                ConsoleLog.Title($"Total: {allSubDlls.Count()}");
            }
        }

        public static List<NonSubDllData> ReadNonSubCache(Process process)
        {
            string json = FileUtils.ReadJson(Files.nonsubCacheMap[process]);
            return JsonSerializer.Deserialize<List<NonSubDllData>>(json);
        }

        public static List<SubDllData> ReadSubCache(Process process)
        {
            string json = FileUtils.ReadJson(Files.subCacheMap[process]);
            return JsonSerializer.Deserialize<List<SubDllData>>(json);
        }

        private static async Task<NonSubDllData?> GetNonSubData(Process process, string assembly, string version)
        {
            var detail = await AssemblyDetails.RequestAsync(assembly, version: version, process: process);
            if (detail == null)
            {
                ConsoleLog.Error($"Cannot get dll detail, check on DGT. ({assembly})");
                return default;
            }

            string dllName = detail.AssemblyName;
            string packageName = detail.PackageName;
            string hasNetCore = "";
            var replacement = new List<string>();
            if (detail.Properties != null && detail.Properties.NuGetInfo != null)
            {
                hasNetCore = detail.Properties.NuGetInfo.HasNetCoreVersion;
                replacement = detail.Properties.NuGetInfo.PackageInfo;
            }

            return new NonSubDllData
            {
                DllName = dllName,
                PackageName = packageName,
                IsCompatible = hasNetCore.EqualsIgnoreCase("True"),
                Replacenment = replacement
            };
        }

        private static async Task<SubDllData?> GetSubData(Process process, string assembly, string version)
        {
            var detail = await AssemblyDetails.RequestAsync(assembly, version: version, process: process);

            if (detail == null)
            {
                ConsoleLog.Error($"Cannot get assembly detail, check on DGT. ({assembly})");
                return default;
            }

            string filePath = Path.Combine(Repo.Paths.SrcDir, detail.SourcePath.Split(@"Sources\").Last());
            if (!File.Exists(filePath))
            {
                ConsoleLog.Error($"Project file not exists in local, is it newly added on master?\n(File {filePath})");
                return default;
            }

            string assemblyName = detail.AssemblyName;
            if (assemblyName.EndsWithIgnoreCase(".dll") || assemblyName.EndsWithIgnoreCase(".exe"))
            {
                assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
            }

            bool isProduced = MarkAsProduced.Contains(assemblyName) ||
                              LookupTable.IsProducedProject(assemblyName, out _) ||
                              filePath.EndsWithIgnoreCase(".vcxproj") ||
                              filePath.EndsWithIgnoreCase(".noproj");

            var buildFile = Repo.Load<BuildFile>(filePath);
            var refResolver = new ReferenceResolver(filePath, LookupTable);
            var references = buildFile.GetReferences(refResolver);

            var blockedSub = new List<string>();
            if (!isProduced)
            {
                blockedSub = references.Where(r => r.Type == ReferenceType.Substrate && !r.Unnecessary &&
                                                   !LookupTable.IsProducedProject(r.ReferenceName, out _) &&
                                                   !LookupTable.IsCppProject(r.ReferenceName, out _) &&
                                                   !MarkAsProduced.Contains(r.ReferenceName))
                                       .Select(r => r.ReferenceName)
                                       .ToList();
            }

            var nonSubDict = ProcessDataCache.ReadNonSubCache(process).ToDictionary(d => d.DllName, d => d);
            var packageDlls = references.Where(r => r.Type == ReferenceType.NuGet)
                                        .Select(r => r.ReferenceDll)
                                        .ToList();

            var blockedNonSub = new List<string>();
            try
            {
                foreach (var dllName in packageDlls)
                {
                    if (string.IsNullOrEmpty(dllName))
                    {
                        continue;
                    }
                    if (nonSubDict.ContainsKey(dllName))
                    {
                        var data = nonSubDict[dllName];
                        if (!data.IsCompatible && !data.Replacenment.Any())
                        {
                            if (data.PackageName.EqualsIgnoreCase("Unknown"))
                            {
                                blockedNonSub.Add(data.DllName);
                            }
                            else
                            {
                                if (!data.PackageName.EqualsIgnoreCase(@"Microsoft.Fast.Search"))
                                {
                                    blockedNonSub.Add(data.PackageName);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!IgnoreNuGetDlls.Contains(dllName))
                        {
                            blockedNonSub.Add(dllName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleLog.Error($"{filePath}\n{e.Message}");
            }

            var incompatibleAPIs = detail.ReferencedIncompatibleAPIs ?? new List<string>();
            var filteredAPIs = detail.FilteredAPIs ?? new List<string>();

            if (isProduced)
            {
                blockedSub = new List<string>();
                blockedNonSub = new List<string>();
            }

            return new SubDllData
            {
                FilePath = filePath,
                AssemblyName = assemblyName,
                IsProduced = isProduced,
                BlockedSub = blockedSub,
                BlockedNonSub = blockedNonSub,
                IncompatibleAPIs = incompatibleAPIs,
                FilteredAPIs = filteredAPIs
            };
        }

        public static void FindPerProcessor(Process process, string? version = null)
        {
            using (Timer.TimeThis)
            {

                if (string.IsNullOrEmpty(version))
                {
                    version = Versions.GetLatestVersionAsync().Result;
                }

                var pop3Assemblies = ProcessAssemblies.RequestAsync(version, 2, Process.Pop3).Result;
                var imapAssemblies = ProcessAssemblies.RequestAsync(version, 2, Process.Imap4).Result;
                var mapiAssemblies = ProcessAssemblies.RequestAsync(version, 2, process).Result;

                ConsoleLog.Highlight($"Pop3 assembleis: {pop3Assemblies.Count}");
                ConsoleLog.Highlight($"Imap assembleis: {imapAssemblies.Count}");
                ConsoleLog.Highlight($"Mapi assembleis: {mapiAssemblies.Count}");
                var checkList = pop3Assemblies.Union(imapAssemblies).Intersect(mapiAssemblies);
                ConsoleLog.Warning($"Common dll: {checkList.Count()}");

                var prduced = new HashSet<string>();
                var allProduced = Repo.RestoreEntry.GetProjects(Repo.Paths.SrcDir, new ProjectResolver());

                foreach (var name in checkList)
                {
                    string rawname = Path.GetFileNameWithoutExtension(name);
                    if (allProduced.TryGetProject(rawname, out IProject? project))
                    {
                        string filePath = project.FilePath;
                        filePath = MSBuildUtils.InferNFBuildFileByPath(filePath);
                        if (filePath.EndsWith(".vcxproj"))
                        {
                            continue;
                        }
                        string parentFolder = Directory.GetParent(filePath).FullName;
                        var file = Repo.Load<BuildFile>(filePath);

                        bool ppInCsproj = false;

                        // Csproj file  ------------------------------------------------------------
                        foreach (var line in File.ReadLines(filePath))
                        {
                            if (line.Contains("NETCOREAPP") || line.Contains("NETSTANDARD") || line.Contains("NETFRAMEWORK"))
                            {
                                ppInCsproj = true;
                                break;
                            }
                        }

                        // Source Codes ------------------------------------------------------------

                        var rawCompiles = file.Document.GetAll(Tags.Compile)
                                                       .Where(c => c.HasAttribute(Tags.Include))
                                                       .Select(c => c.GetAttribute(Tags.Include).Value);
                        var sourceList = new List<string>();
                        foreach (var relPath in rawCompiles)
                        {
                            if (relPath.StartsWithIgnoreCase(@"$(O)") || relPath.StartsWithIgnoreCase(@"$(IntermediateOutputPath)"))
                            {
                                continue;
                            }

                            if (MSBuildUtils.TryResolveBuildVariables(parentFolder, relPath, out string absPath))
                            {

                            }
                            else
                            {
                                absPath = PathUtils.GetAbsolutePath(parentFolder, relPath);
                            }

                            if (!File.Exists(absPath))
                            {
                                ConsoleLog.Error($"{filePath}\n    File not found: {absPath}\n");
                                continue;
                            }

                            foreach (var line in File.ReadLines(absPath))
                            {
                                if (line.Trim().StartsWithIgnoreCase(@"#if") && (line.ContainsIgnoreCase("NETCOREAPP") || line.ContainsIgnoreCase("NETSTANDARD")))
                                {
                                    sourceList.Add(absPath);
                                    break;
                                }
                            }
                        }


                        // ------------------------------------------------------------
                        if (ppInCsproj || sourceList.Count > 0)
                        {
                            ConsoleLog.Warning(filePath);
                            ConsoleLog.Ignore("------------------------------------------------------------");
                            if (ppInCsproj)
                            {
                                ConsoleLog.InLine("  csproj: ");
                                ConsoleLog.Success("Yes");
                            }
                            if (sourceList.Count > 0)
                            {
                                ConsoleLog.Message("  source:");
                                foreach (var source in sourceList)
                                {
                                    ConsoleLog.Success($"    {source}");
                                }
                            }
                            ConsoleLog.Ignore("");
                        }
                    }
                }
            }
        }

    }
}
