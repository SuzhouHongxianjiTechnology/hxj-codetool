namespace MergeTool.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using MergeTool.Common;
    using Mint.Substrate;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Construction;
    using Mint.Common;

    internal static class NuspecUpdater
    {
        internal static List<string> SubstrateNugetNames = new List<string>
        {
            "Microsoft.Exchange.Imap4.ModelA",
            "Microsoft.Exchange.InMemoryMapiImplementation",
            "Microsoft.Exchange.MapiAbstraction",
            "Microsoft.Exchange.MapiImplementation",
            "Microsoft.Exchange.Pop3",
            "Microsoft.Exchange.Pop3.ModelB2",
            "Microsoft.Exchange.XSO",
            "Microsoft.Exchange.XSOCommon",
            "Microsoft.Exchange.Azure.SecretsProvider.Standard"
        };

        internal static List<string> SubstrateNuspecList = new List<string>
        {
            @"sources\dev\PopImap\nupkg\Imap4.ModelA\Microsoft.Exchange.Imap4.ModelA.nuspec",
            @"sources\dev\PopImap\nupkg\InMemoryMapiImplementation\Microsoft.Exchange.InMemoryMapiImplementation.nuspec",
            @"sources\dev\PopImap\nupkg\MapiAbstraction\Microsoft.Exchange.MapiAbstraction.nuspec",
            @"sources\dev\PopImap\nupkg\MapiImplementation\Microsoft.Exchange.MapiImplementation.nuspec",
            @"sources\dev\PopImap\nupkg\Pop3\Microsoft.Exchange.Pop3.nuspec",
            @"sources\dev\PopImap\nupkg\Pop3.ModelB2\Microsoft.Exchange.Pop3.ModelB2.nuspec",
            @"sources\dev\PopImap\nupkg\XSO\Microsoft.Exchange.XSO.nuspec",
            @"sources\dev\PopImap\nupkg\XSOCommon\Microsoft.Exchange.XSOCommon.nuspec",
            @"sources\dev\Azure\nupkg\Microsoft.Exchange.Azure.SecretsProvider.Standard\Microsoft.Exchange.Azure.SecretsProvider.Standard.nuspec"
        };

        internal static void UpdateNuspecFiles()
        {
            foreach (var filePath in SubstrateNuspecList)
            {
                string fullPath = Path.Combine(Settings.DFSrc, filePath);
                ConsoleLog.Ignore(fullPath, inLine: true);
                NuspecUpdater.UpdateNuspecFile(fullPath);
                ConsoleLog.Success(" [DONE]");
            }
        }

        internal static void UpdateHardCodeH()
        {
            string content = @"#ifndef _BLDVER_H_" + Environment.NewLine +
                             @"#define _BLDVER_H_" + Environment.NewLine + Environment.NewLine +
                             @"#define PRODUCT_MAJOR          ""15""" + Environment.NewLine +
                             @"#define rmj                    15" + Environment.NewLine +
                             @"#define PRODUCT_MINOR          ""20""" + Environment.NewLine +
                             @"#define rmn                    20" + Environment.NewLine +
                             @"#define BUILD_MAJOR            ""{0}""" + Environment.NewLine +
                             @"#define rmm                    {0}" + Environment.NewLine +
                             @"#define BUILD_MINOR            ""000""" + Environment.NewLine +
                             @"#define rup                    0" + Environment.NewLine + Environment.NewLine +
                             @"#endif" + Environment.NewLine;
            content = string.Format(content, Settings.PackageVersion.Split(".")[2]);
            File.WriteAllText(ConstFiles.HardCodeH, content);
        }

        private static void UpdateNuspecFile(string filePath)
        {
            using (var nuspec = new NuspecProjectFile(filePath))
            {
                nuspec.Version = Settings.PackageVersion;

                string description = nuspec.Description;
                nuspec.Description = Regex.Replace(description, @"\d+.\d+.\d+.\d+", Settings.BuildVersion);

                var dependencies = NuspecUpdater.ResolveAllDependencies(nuspec);
                nuspec.ReplaceDependencies(dependencies);
            }
        }

        private static OrderedDictionary ResolveAllDependencies(NuspecProjectFile nuspec)
        {
            var dependencies = new OrderedDictionary();

            /*
                1. scan all included assemblies, find all package references' name
            */
            var assemblies = nuspec.IncludedFiles.Select(f => f.Replace(".dll", ""));
            var packageNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool containsVariantConfig = false;
            foreach (var assembly in assemblies)
            {
                if (DF.RestoreEntry.TryGetProjectPathsByName(assembly, out string projectPath, out string _))
                {
                    var projectFile = new NetCoreProjectFile(projectPath);
                    if (projectFile.ContainsVariantConfig)
                    {
                        containsVariantConfig = true;
                    }
                    foreach (var packageReference in projectFile.PackageReferences)
                    {
                        packageNames.Add(packageReference.Name);
                    }
                }
                else
                {
                    // can't find project with this name
                    // throw new Exception($"Cannot find project with AssemblyName: '{assembly}'");
                }
            }
            if (containsVariantConfig)
            {
                foreach (var packageName in DF.VariantConfig.Packages.Keys)
                {
                    packageNames.Add(packageName);
                }
            }

            /*
                2. sort package names and apply special rules
            */
            packageNames.Remove("MsBuild.Corext");

            if (packageNames.Contains("Bond.NET", StringComparer.OrdinalIgnoreCase))
            {
                packageNames.Add("Bond.Rpc.NET");
            }

            if (packageNames.Contains("Microsoft.M365.Authentication", StringComparer.OrdinalIgnoreCase))
            {
                packageNames.Add("Microsoft.Extensions.Caching.Memory");
                packageNames.Add("RPSv6.7");
            }

            if (string.Equals(nuspec.Id, "Microsoft.Exchange.XSOCommon"))
            {
                packageNames.Add("Microsoft.M365.KVCache.Interface");
            }

            /*
                3. find all package version in packages.props
            */
            var sortedNames = packageNames.ToList().OrderBy(name => name);

            foreach (var packageName in sortedNames)
            {
                string name = DF.PackagesProps.GetPackageName(packageName);
                string version = DF.PackagesProps.GetPackageVersion(packageName);
                if (!string.IsNullOrEmpty(version))
                {
                    dependencies.Add(name, version);
                }
                else
                {
                    // can't find package in packages.props, this should not happen.
                    // throw new Exception($"Package '{packageName}' doesn't definded in Packages.Props.");
                    ConsoleLog.Error(Environment.NewLine +
                                 $"Package doesn't definded in Packages.Props. (Padckage {packageName})");
                }
            }

            /*
                4. add substrate published nuget if any
            */
            foreach (var package in nuspec.Packages)
            {
                string version = string.Format("[{0}]", Settings.PackageVersion);
                if (SubstrateNugetNames.Contains(package.Key, StringComparer.OrdinalIgnoreCase))
                {
                    dependencies.Add(package.Key, version);
                }
            }

            return dependencies;
        }
    }
}
