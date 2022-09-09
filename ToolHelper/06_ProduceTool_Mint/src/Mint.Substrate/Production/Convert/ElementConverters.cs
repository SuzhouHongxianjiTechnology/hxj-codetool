namespace Mint.Substrate.Production
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Utilities;

    internal delegate ConvertResult ElementConverter(XElement element, PortingConfig config, out List<XElement> result);

    internal static class ElementConverters
    {
        private static OrderedDictionary RequiredProperties = new OrderedDictionary
        {
            {"AssemblyName", "AssemblyName"},
            {"OutputType", "Library"},
            {"TargetFramework", ""},
            {"GenerateAssemblyInfo", "false"},
            {"PlatformTarget", "x64"},
        };

        // ------------------------------------------------------------

        internal static ElementConverter GetConverter(string localName)
        {
            switch (localName)
            {
                case Tags.PropertyGroup:
                    return ConvertPropertyGroup;
                case Tags.ItemGroup:
                    return ConvertItemGroup;
                case Tags.Reference:
                    return ConvertReferenceItem;
                case Tags.ProjectReference:
                    return ConvertProjectReferenceItem;
                case Tags.Import:
                    return ConvertImportItem;
                case Tags.Target:
                    return ConvertTargetItem;
                default:
                    return ConvertDefaultItem;
            }
        }

        // ------------------------------------------------------------

        private static ConvertResult ConvertPropertyGroup(XElement element, PortingConfig config, out List<XElement> result)
        {
            result = new List<XElement>();
            var clone = element.Clone();

            if (element.HasAttribute(Tags.Condition) ||
                element.Element(Tags.AssemblyName) == null)
            {
                result.Add(clone);
                return ConvertResult.NotChanged;
            }

            // make sure all required properties are added
            var properties = new List<XElement>();
            foreach (string name in RequiredProperties.Keys)
            {
                string value = string.Empty;
                if (name == "TargetFramework")
                {
                    value = config.TargetFramework;
                }
                else
                {
                    value = clone.GetFirst(name)?.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        value = RequiredProperties[name].ToString();
                    }
                }
                properties.Add(XElement.Parse($@"<{name}>{value}</{name}>"));
            }

            // add other properties if any
            foreach (var property in clone.Elements().ToList())
            {
                string name = property.Name.LocalName;
                if (!RequiredProperties.Contains(name) &&
                    !NetFrameworkConsts.NetFrameworkOnylProperties.Contains(name))
                {
                    properties.Add(XElement.Parse($@"<{name}>{property.Value}</{name}>"));
                }
            }

            // add property 'LocalizeResourceFolder' if NetFramework proejct has a loc folder
            string netFrameworkLocPath = Path.Combine(config.NetFrameworkParentPath, "loc");
            if (Directory.Exists(netFrameworkLocPath))
            {
                string relatedLocPath = $"..\\{config.NetFrameworkParentName}\\loc";
                properties.Add(XElement.Parse($@"<LocalizeResourceFolder>{relatedLocPath}</LocalizeResourceFolder>"));
            }

            // Cleanup
            clone.ReplaceAll(properties);
            result.Add(clone);
            return ConvertResult.Replaced;
        }

        // ------------------------------------------------------------

        private static ConvertResult ConvertItemGroup(XElement element, PortingConfig config, out List<XElement> result)
        {
            result = new List<XElement>();
            var clone = element.Clone();

            foreach (var child in element.Elements().ToList())
            {
                ConvertibleElement.Parse(child, config).Produce();
                child.RemoveNamespace();
            }

            result.Add(clone);
            return ConvertResult.NotChanged;
        }

        // ------------------------------------------------------------

        private static ConvertResult ConvertReferenceItem(XElement element, PortingConfig config, out List<XElement> result)
        {
            result = new List<XElement>();
            var clone = element.Clone();

            var reference = new NetFrameworkReference(element);

            if (reference.Type == ReferenceType.SDK)
            {
                return ConvertResult.Removed;
            }

            else if (reference.Type == ReferenceType.Nuget)
            {
                string packageName = reference.Name;

                if (SpecialCases.PackageReplacement.ContainsKey(packageName))
                {
                    packageName = SpecialCases.PackageReplacement[packageName];
                    if (string.IsNullOrEmpty(packageName))
                    {
                        return ConvertResult.Removed;
                    }
                }
                XElement packageReferenceElement;
                if (DF.PackagesProps.HasPackage(packageName))
                {
                    packageReferenceElement = XElement.Parse($@"<PackageReference Include=""{packageName}"" />");
                }
                else
                {
                    packageReferenceElement = XElement.Parse($@"<PackageReference Include=""{packageName}"" />");
                    packageReferenceElement.SetAttributeValue(Tags.Undefined, true);
                }
                result.Add(packageReferenceElement);
                return ConvertResult.Replaced;
            }

            else if (reference.Type == ReferenceType.Substrate)
            {
                string assemblyName = reference.Name;
                XElement referenceElement;
                if (DF.RestoreEntry.TryGetProjectPathsByName(assemblyName, out string projectPath, out string targetPath))
                {
                    referenceElement = XElement.Parse($@"<Reference Include=""{assemblyName}""><HintPath>{targetPath}</HintPath></Reference>");
                }
                else
                {
                    referenceElement = XElement.Parse($@"<Reference Include=""{assemblyName}""><HintPath>NOT_PRODUCED</HintPath></Reference>");
                    referenceElement.SetAttributeValue(Tags.Blocked, true);
                }
                result.Add(referenceElement);
                return ConvertResult.Replaced;
            }

            else
            {
                result.Add(clone);
                return ConvertResult.NotChanged;
            }
        }

        // ------------------------------------------------------------

        private static ConvertResult ConvertProjectReferenceItem(XElement element, PortingConfig config, out List<XElement> result)
        {
            result = new List<XElement>();

            string relativepath = element.GetAttribute(Tags.Include)?.Value;
            string netFrameworkPath = SubstrateUtils.ResolveItemFullPath(config.NetFrameworkParentPath, relativepath);
            string assemblyName = new NetFrameworkProjectFile(netFrameworkPath).AssemblyName;

            XElement referenceElement;
            if (DF.RestoreEntry.TryGetProjectPathsByName(assemblyName, out string _, out string targetPath))
            {
                referenceElement = XElement.Parse($@"<Reference Include=""{assemblyName}""><HintPath>{targetPath}</HintPath></Reference>");
            }
            else
            {
                referenceElement = XElement.Parse($@"<Reference Include=""{assemblyName}""><HintPath>UNKNOWN</HintPath></Reference>");
                referenceElement.SetAttributeValue(Tags.Blocked, true);
            }

            result.Add(referenceElement);
            return ConvertResult.Replaced;
        }

        // ------------------------------------------------------------

        private static ConvertResult ConvertImportItem(XElement element, PortingConfig config, out List<XElement> result)
        {
            result = new List<XElement>();
            var clone = element.Clone();

            string attrProject = clone.GetAttribute(Tags.Project)?.Value;

            // replace 'Localize.targets'
            if (StringUtils.EqualsIgnoreCase(attrProject, @"$(BranchTargetsPath)\Localization\Localize.targets") ||
                StringUtils.EqualsIgnoreCase(attrProject, @"$(ExchangeBuildExtensionsPath)\Localize.targets"))
            {
                clone.SetAttributeValue(Tags.Project, @"$(BranchTargetsPath)\Localization\NetStdNetCoreLocalize.targets");
                XElement locImport = XElement.Parse(@"<Import Project=""$(ExchangeBuildExtensionsPath)\Localize.targets"" />");
                result.Add(clone);
                result.Add(locImport);
                return ConvertResult.Replaced;
            }

            // replace 'MC-XmlStyler.targets'
            else if (StringUtils.EqualsIgnoreCase(attrProject, @"$(BranchTargetsPath)\XmlStyler\MC-XmlStyler.targets") ||
                     StringUtils.EqualsIgnoreCase(attrProject, @"$(ExtendedTargetsPath)\MC-XmlStyler.targets"))
            {
                clone.SetAttributeValue(Tags.Project, @"$(MSBuildExtensionsPath)\Override\MC-XmlStyler.targets");
                result.Add(clone);
                return ConvertResult.Replaced;
            }

            // remove 'EnvironmentConfig', 'Microsoft.CSharp.targets'
            else if (StringUtils.EqualsIgnoreCase(attrProject, "$(EnvironmentConfig)") ||
                     StringUtils.EqualsIgnoreCase(attrProject, @"$(ExtendedTargetsPath)\Microsoft.CSharp.targets"))
            {
                return ConvertResult.Removed;
            }

            result.Add(clone);
            return ConvertResult.NotChanged;
        }

        // ------------------------------------------------------------

        private static ConvertResult ConvertTargetItem(XElement element, PortingConfig config, out List<XElement> result)
        {
            result = new List<XElement>();
            var clone = element.Clone();

            string name = element.GetAttribute(Tags.Name)?.Value;
            // remove '_GenerateRestoreGraphProjectEntry' and  '_GenerateProjectRestoreGraph'
            if (StringUtils.EqualsIgnoreCase(name, "_GenerateRestoreGraphProjectEntry") ||
                StringUtils.EqualsIgnoreCase(name, "_GenerateProjectRestoreGraph"))
            {
                return ConvertResult.Removed;
            }

            result.Add(clone);
            return ConvertResult.NotChanged;
        }

        // ------------------------------------------------------------

        private static ConvertResult ConvertDefaultItem(XElement element, PortingConfig config, out List<XElement> result)
        {
            result = new List<XElement>();
            var clone = element.Clone();
            if (element.HasAttribute(Tags.Include))
            {
                string oldPath = clone.GetAttribute(Tags.Include).Value;
                string fullPath = SubstrateUtils.ResolveItemFullPath(config.NetFrameworkParentPath, oldPath);
                string newPath = fullPath.StartsWith("$") || fullPath.StartsWith("@") ? fullPath
                                                                                      : PathUtils.GetRelativePath(config.ProduceParentPath, fullPath);
                clone.SetAttributeValue(Tags.Include, newPath);
                result.Add(clone);
                return ConvertResult.Replaced;
            }

            result.Add(clone);
            return ConvertResult.NotChanged;
        }
    }
}
