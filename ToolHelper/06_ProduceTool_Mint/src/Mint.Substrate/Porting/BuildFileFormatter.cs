namespace Mint.Substrate.Porting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate.Construction;

    public class BuildFileFormater
    {
        private readonly LookupTable lookupTable;

        public BuildFileFormater(LookupTable lookupTable)
        {
            this.lookupTable = lookupTable;
        }

        public void Format(BuildFile file)
        {
            RemoveUnnecessaryProperties(file);
            RemovePackagesIfHasVariantConfig(file);
            RemoveContent(file);
            RemovePass2Publish(file);
            FormatCompileGroup(file);
            AdditionalRules(file);
            FormatReferences(file);
            FormatPackageReferences(file);
            ResolveLocalizeFile(file);
            ChangeResourceFolder(file);
            AddTargets(file);
            RemoveEmptyGroups(file);
        }

        private void RemoveUnnecessaryProperties(BuildFile file)
        {
            var properties = file.Document.GetAll(Tags.PropertyGroup).SelectMany(p => p.Elements());
            foreach (var property in properties)
            {
                if (this.lookupTable.IsExcludedProperty(property.Name.LocalName))
                {
                    property.TryRemove();
                }
            }
        }

        private void RemovePackagesIfHasVariantConfig(BuildFile file)
        {
            bool configV1 = file.ContainsVariantConfigV1;
            bool configV2 = file.ContainsVariantConfigV2;

            if (!configV1 && !configV2) return;

            var pkgReferences = file.Document.GetAll(Tags.PackageReference).ToList();
            foreach (var pkgReference in pkgReferences)
            {
                string name = pkgReference.GetAttribute(Tags.Include).Value;
                if (configV1 && this.lookupTable.IsDefinedInVariantConfigV1(name))
                {
                    pkgReference.TryRemove();
                }
                else if (configV2 && this.lookupTable.IsDefinedInVariantConfigV2(name))
                {
                    pkgReference.TryRemove();
                }
            }
        }

        private void RemoveContent(BuildFile file)
        {
            // <Content Include="..\Common\loc\Microsoft.Exchange.Data.Common.dll.lci" />
            var contents = file.Document.GetAll(Tags.Content);
            foreach (var content in contents.ToList())
            {
                var include = content.GetAttribute(Tags.Include)?.Value;
                if (include != null)
                {
                    string lci = include.Split("\\").LastOrDefault();
                    if (lci != null && lci.EqualsIgnoreCase("Microsoft.Exchange.Data.Common.dll.lci"))
                    {
                        content.Remove();
                    }
                }
            }
        }

        private void RemovePass2Publish(BuildFile file)
        {
            var p2p = file.Document.GetAll("Pass2Publish");
            foreach (var p in p2p.ToList())
            {
                p.Remove();
            }
        }

        private void FormatCompileGroup(BuildFile file)
        {
            var compiles = file.Document.GetAll(Tags.Compile);

            if (compiles.Any())
            {
                bool hasCompileRemove = compiles.Where(c => c.HasAttribute(Tags.Remove))
                                                .Any();

                bool hasGeneratedFile = compiles.Where(c => c.HasAttribute(Tags.Include))
                                                .Where(c => c.GetAttribute(Tags.Include).Value.StartsWith("$"))
                                                .Any();

                if (!hasCompileRemove && hasGeneratedFile)
                {
                    var removeCompile = new XElement(Tags.Compile);
                    removeCompile.SetAttributeValue(Tags.Remove, @".\**\*.cs");
                    file.Document.GetFirst(Tags.Compile).AddBeforeSelf(removeCompile);
                }
                else if (hasCompileRemove && !hasGeneratedFile)
                {
                    compiles.Where(c => c.HasAttribute(Tags.Remove)).Remove();
                }
            }
        }

        private void AdditionalRules(BuildFile file)
        {
            var pkgReference = file.Document.GetAll(Tags.PackageReference);
            foreach (var pkgRef in pkgReference.ToList())
            {
                string include = pkgRef.Attribute(Tags.Include).Value;
                if (include.EqualsIgnoreCase("Microsoft.Exchange.Security"))
                {
                    if (this.lookupTable.IsProducedProject("Microsoft.Exchange.Security", out IProject? project))
                    {
                        string? hintpath = project.TargetPath;
                        var actualReference = XElement.Parse($@"<Reference Include=""Microsoft.Exchange.Security""><HintPath>{hintpath}</HintPath></Reference>");
                        pkgRef.AddAfterSelf(actualReference);
                        pkgRef.TryRemove();
                    }
                }
            }
        }

        private void FormatReferences(BuildFile file)
        {
            var refParent = file.Document.GetFirst(Tags.Reference)?.Parent;
            if (refParent != null)
            {
                var newGroup = new XElement(Tags.ItemGroup);
                var referenceSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var reference in file.Document.GetAll(Tags.Reference).ToList())
                {
                    string refName = reference.GetAttribute(Tags.Include).Value;
                    if (!referenceSet.Contains(refName))
                    {
                        var refClone = reference.Clone();
                        var hintPath = refClone.GetFirst(Tags.HintPath);
                        if (hintPath != null)
                        {
                            string oldPath = hintPath.Value;
                            hintPath.SetValue(oldPath.ReplaceIgnoreCase("$(TargetPathDir)\\", "$(TargetPathDir)"));
                        }
                        newGroup.Add(refClone);
                        referenceSet.Add(refName);
                    }
                    reference.TryRemove();
                }
                newGroup.SortByAttribute(Tags.Include);
                refParent.AddAfterSelf(newGroup);
            }
        }

        private void FormatPackageReferences(BuildFile file)
        {
            XElement? pkgParent = null;

            var itemGroups = file.Document.GetAll(Tags.ItemGroup);
            foreach (var iGroup in itemGroups)
            {
                if (iGroup.GetFirst(Tags.PackageReference) != null)
                {
                    pkgParent = iGroup;
                    break;
                }
            }

            if (pkgParent != null)
            {
                var newGroup = new XElement(Tags.ItemGroup);
                var packageSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var pkgReference in file.Document.GetAll(Tags.PackageReference).ToList())
                {
                    string pkgName = pkgReference.GetAttribute(Tags.Include).Value;
                    if (!packageSet.Contains(pkgName))
                    {
                        newGroup.Add(pkgReference.Clone());
                        packageSet.Add(pkgName);
                    }
                    pkgReference.TryRemove();
                }
                newGroup.SortByAttribute(Tags.Include);
                pkgParent.AddAfterSelf(newGroup);
            }
        }

        private void ResolveLocalizeFile(BuildFile file)
        {
            bool containsLocalize = false;
            var imports = file.Document.GetAll(Tags.Import);
            foreach (var import in imports.ToList())
            {
                if (import.HasAttribute(Tags.Project, "$(BranchTargetsPath)\\Localization\\NetStdNetCoreLocalize.targets") ||
                    import.HasAttribute(Tags.Project, "$(BranchTargetsPath)\\Localization\\Localize.targets") ||
                    import.HasAttribute(Tags.Project, "$(ExchangeBuildExtensionsPath)\\Localize.targets"))
                {
                    import.Remove();
                    containsLocalize = true;
                }
            }
            if (containsLocalize)
            {
                // <GenerateLocalization>true</GenerateLocalization>
                // <WriteLocalizationConfiguration>false</WriteLocalizationConfiguration>
                var mainGroup = file.Document.GetAll(Tags.PropertyGroup)
                                             .Where(group => group.GetFirst(Tags.AssemblyName) != null)
                                             .First();
                mainGroup.Add(new XElement("GenerateLocalization", "true"));
                mainGroup.Add(new XElement("WriteLocalizationConfiguration", "false"));
            }
        }

        private void ChangeResourceFolder(BuildFile file)
        {
            var resourceFolder = file.Document.GetFirst("LocalizeResourceFolder");
            if (resourceFolder != null)
            {
                // $(SRCDIR)\dev\CTS\src\Server\Common\Loc
                string filePath = file.FilePath;
                string parent = Directory.GetParent(filePath).ToString();
                string value = resourceFolder.Value;
                string absolutePath = PathUtils.GetAbsolutePath(parent, value);
                string newPath = absolutePath.ReplaceIgnoreCase($"{Repo.Paths.SrcDir}\\sources", "$(SRCDIR)");
                resourceFolder.Name = "LocalizationResourceRootFolder";
                resourceFolder.Value = newPath;
            }
        }

        private void AddTargets(BuildFile file)
        {
            // bool hasPerfCounters = file.ContainsPerfCounters;
            // if (hasPerfCounters)
            // {
            //     var perfCounters = new XElement(Tags.Import,
            //                            new XAttribute(Tags.Project, @"$(MSBuildExtensionsPath)\Override\PerfCounters.targets"));
            //     var lastTarget = file.Document.GetLast(Tags.Import);
            //     if (lastTarget != null)
            //     {
            //         lastTarget.AddAfterSelf(perfCounters);
            //     }
            //     else
            //     {
            //         file.Document.Root.Add(perfCounters);
            //     }
            // }
        }

        private void RemoveEmptyGroups(BuildFile file)
        {
            file.Document.GetAll(Tags.PropertyGroup).ToList()
                         .ForEach(group => group.RemoveIfEmpty());

            file.Document.GetAll(Tags.ItemGroup).ToList()
                         .ForEach(group => group.RemoveIfEmpty());
        }

    }
}
