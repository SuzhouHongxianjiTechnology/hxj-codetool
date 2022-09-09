namespace SyncTool
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Data;

    internal class UpdateManager
    {
        private LookupTable lookupTable;
        private RepoPaths paths;

        public UpdateManager(LookupTable lookupTable)
        {
            this.lookupTable = lookupTable;
            this.paths = RepoPaths.Load();
        }

        internal void UpdateSubstratePackages()
        {
            var innerPackages = Repo.InnerCorext.GetPackages();
            var outerPackages = Repo.OuterCorext.GetPackages();

            var props = Repo.PackagesProps;
            var packages = props.GetPackages();

            packages.Upgrade(innerPackages, out var innerFails);
            foreach (var fail in innerFails)
            {
                ConsoleLog.Warning($"[SKIP] PKG: {fail.Name}", inLine: true);
                ConsoleLog.Ignore($" {fail.CurrentVersion}  >>  {fail.NewVersion}");
            }

            packages.Upgrade(outerPackages, out var outerFails);
            foreach (var fail in outerFails)
            {
                ConsoleLog.Warning($"[SKIP] PKG: {fail.Name}", inLine: true);
                ConsoleLog.Ignore($" {fail.CurrentVersion}  >>  {fail.NewVersion}");
            }

            props.Save();
        }

        internal void UpdateNuGetPackages()
        {
            foreach (string nugetInetPath in this.lookupTable.SubstrateNuspecs)
            {
                string nugetFullPath = nugetInetPath.ReplaceIgnoreCase("$(Inetroot)", this.paths.SrcDir);
                ConsoleLog.Ignore(nugetFullPath, inLine: true);

                var nuspec = Repo.Load(nugetFullPath);
                nuspec.Document.GetFirst(Tags.Version).SetAttributeValue(Tags.Version, AppSettings.PackageVersion);

                var description = nuspec.Document.GetFirst(Tags.Description);
                description.SetValue(Regex.Replace(description.Value, @"\d+.\d+.\d+.\d+", AppSettings.BuildVersion));

                OrderedDictionary dependencies = ResolveAllDependencies(nuspec);

                // For now, we only replace: <group targetFramework=".NETCoreApp3.1">
                var dependencyGroup = nuspec.Document.GetFirst(Tags.Group);
                dependencyGroup.RemoveNodes();
                foreach (var name in dependencies.Keys)
                {
                    var element = new XElement(nuspec.Namespace + Tags.dependency,
                                                new XAttribute(Tags.id, name),
                                                new XAttribute(Tags.version, dependencies[name]));
                    dependencyGroup.Add(element);
                }

                nuspec.Save();
            }
        }

        internal void UpdateHardCodedbldver()
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
            content = string.Format(content, AppSettings.PackageVersion.Split(".")[2]);
            File.WriteAllText(this.paths.HardCodedBuildVersion, content);
        }

        private OrderedDictionary ResolveAllDependencies(XMLFile nuspec)
        {
            var dependencies = new OrderedDictionary();

            /*
                1. scan all included assemblies, find all package references' name
            */
            var assemblies = nuspec.Document.GetAll(Tags.File)
                                            .Where(file => file.HasAttribute(Tags.Target, @"lib\netcoreapp3.1") || file.HasAttribute(Tags.Target, @"lib\netstandard2.0"))
                                            .Select(file => file.GetAttribute(Tags.Src).Value)
                                            .Select(f => f.Replace(".dll", ""));

            var packageNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool hasVarConfigV1 = false;
            bool hasVarConfigV2 = false;
            var producedProjects = this.lookupTable.GetProducedProjects();
            foreach (string assembly in assemblies)
            {
                var projectPath = producedProjects.GetFilePath(assembly);
                if (projectPath != null)
                {
                    var projectFile = Repo.Load<BuildFile>(projectPath);

                    if (projectFile.ContainsVariantConfigV1)
                    {
                        hasVarConfigV1 = true;
                    }

                    if (projectFile.ContainsVariantConfigV2)
                    {
                        hasVarConfigV2 = true;
                    }

                    foreach (var pkgReference in projectFile.Document.GetAll(Tags.PackageReference))
                    {
                        packageNames.Add(pkgReference.GetAttribute(Tags.Include).Value);
                    }
                }
                else
                {
                    // can't find project with this name
                    // throw new Exception($"Cannot find project with AssemblyName: '{assembly}'");
                }
            }

            if (hasVarConfigV1)
            {
                packageNames.UnionWith(this.lookupTable.VarConfigV1Refs);
            }

            if (hasVarConfigV2)
            {
                packageNames.UnionWith(this.lookupTable.VarConfigV2Refs);
            }

            /*
                2. apply special rules
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

            if (string.Equals(nuspec.Document.GetFirst(Tags.Version).Value, "Microsoft.Exchange.XSOCommon"))
            {
                packageNames.Add("Microsoft.M365.KVCache.Interface");
            }

            /*
                3. find all package version in packages.props
            */
            var sortedNames = packageNames.OrderBy(n => n);

            foreach (string name in sortedNames)
            {
                if (this.lookupTable.IsDefinedNuGet(name, out string? actualName, out string? version))
                {
                    dependencies.Add(actualName, version);
                }
                else
                {
                    // can't find package in packages.props, this should NOT happen.
                    ConsoleLog.Error($"\nPackage doesn't definded in Packages.Props. (Padckage {name})");
                }
            }

            /*
                4. add substrate published nuget if any
            */
            var nuGets = this.lookupTable.SelfPublishedNuGetNames();

            foreach (var package in nuspec.Document.GetAll(Tags.Dependency))
            {
                string name = package.GetAttribute(Tags.Id).Value;

                if (nuGets.Contains(name))
                {
                    dependencies.Add(name, string.Format("[{0}]", AppSettings.PackageVersion));
                }
            }

            return dependencies;
        }
    }
}
