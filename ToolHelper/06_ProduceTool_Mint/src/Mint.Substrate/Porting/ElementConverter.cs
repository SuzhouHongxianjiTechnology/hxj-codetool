namespace Mint.Substrate.Porting
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Utilities;

    public class ElementConverter
    {
        private readonly PortingMetadata metadata;

        private readonly LookupTable lookupTable;

        private readonly ReferenceResolver resolver;

        public ElementConverter(PortingMetadata metadata,
                                LookupTable lookupTable,
                                ReferenceResolver resolver)
        {
            this.metadata = metadata;
            this.lookupTable = lookupTable;
            this.resolver = resolver;
        }

        public PreConvertedElement? Convert(in XElement element)
        {
            if (element == null) return null;

            XElement? clone;
            ConvertResult result;

            switch (element.Name.LocalName)
            {
                case Tags.PropertyGroup:
                    result = ConvertPropertyGroup(element, out clone);
                    break;

                case Tags.ItemGroup:
                    result = ConvertItemGroup(element, out clone);
                    break;

                case Tags.Reference:
                case Tags.ProjectReference:
                    result = ConvertReference(element, out clone);
                    break;

                case Tags.Import:
                    result = ConvertImport(element, out clone);
                    break;

                case Tags.Target:
                    result = ConvertTarget(element, out clone);
                    break;

                default:
                    result = ConvertGeneralItem(element, out clone);
                    break;
            }

            /*
                NOTE:
                There might be some other conditions we need keep.
                Keep an eye on this.
            */
            if (element.HasAttribute(Tags.Condition, "$(DefineConstants.Contains('NETFRAMEWORK'))"))
            {
                clone = null;
                result = ConvertResult.Removed;
            }

            if (element.HasAttribute(Tags.Condition, "!$(DefineConstants.Contains('NETFRAMEWORK'))"))
            {
                clone = clone ?? element.Clone();
                clone.GetAttribute(Tags.Condition)?.Remove();
                result = ConvertResult.Changed;
            }

            return new PreConvertedElement(element, result, clone);
        }

        private ConvertResult ConvertPropertyGroup(in XElement element, out XElement? clone)
        {
            bool isMainGroup = element.GetFirst(Tags.AssemblyName) != null ||
                               element.GetFirst(Tags.RootNamespace) != null;

            return isMainGroup ? ConvertMainPropertyGroup(element, out clone)
                               : ConvertSubPropertyGroup(element, out clone);
        }

        private ConvertResult ConvertMainPropertyGroup(in XElement element, out XElement? clone)
        {
            clone = element.Clone();
            var included = this.lookupTable.IncludedProps;

            // make sure all included properties are added
            List<XElement> properties = new List<XElement>();
            foreach (string key in included.Keys)
            {
                string value = key == Tags.TargetFramework
                    ? this.metadata.Framework
                    : clone.GetFirst(key)?.Value ?? included[key].ToString();
                properties.Add(XElement.Parse($@"<{key}>{value}</{key}>"));
            }

            // add other properties if any
            foreach (var property in clone.Elements().ToList())
            {
                string key = property.Name.LocalName;
                if (!this.lookupTable.IsIncludedProperty(key, out _) &&
                    !this.lookupTable.IsExcludedProperty(key))
                {
                    properties.Add(XElement.Parse($@"<{key}>{property.Value}</{key}>"));
                }
            }

            // add property 'LocalizeResourceFolder' if NetFramework proejct has a loc folder
            string netFrameworkLocPath = Path.Combine(this.metadata.NFBuildFolder, "loc");
            if (Directory.Exists(netFrameworkLocPath))
            {
                string relatedLocPath = $"..\\{this.metadata.NFBuildFolderName}\\loc";
                properties.Add(XElement.Parse($@"<LocalizeResourceFolder>{relatedLocPath}</LocalizeResourceFolder>"));
            }

            clone.ReplaceAll(properties);
            return ConvertResult.Changed;
        }

        private ConvertResult ConvertSubPropertyGroup(in XElement element, out XElement? clone)
        {
            var overallResult = ConvertResult.NotChanged;
            clone = element.Clone();

            foreach (var property in clone.Elements().ToList())
            {
                if (this.lookupTable.IsExcludedProperty(property.Name.LocalName))
                {
                    property.TryRemove();
                    overallResult = ConvertResult.Changed;
                }
            }

            if (!clone.Elements().Any())
            {
                clone = null;
                return ConvertResult.Removed;
            }

            clone = ConvertResult.NotChanged == overallResult ? null : clone;
            return overallResult;
        }

        private ConvertResult ConvertItemGroup(in XElement element, out XElement? clone)
        {
            var overallResult = ConvertResult.NotChanged;
            clone = element.Clone();

            foreach (var child in clone.Elements().ToList())
            {
                var childClone = Convert(child);
                if (childClone != null && childClone.ConvertResult != ConvertResult.NotChanged)
                {
                    overallResult = ConvertResult.Changed;
                }
                childClone?.Commit();
            }

            if (!clone.Elements().Any())
            {
                clone = null;
                return ConvertResult.Removed;
            }

            clone = ConvertResult.NotChanged == overallResult ? null : clone;
            return overallResult;
        }

        private ConvertResult ConvertReference(in XElement element, out XElement? clone)
        {
            var reference = this.resolver.Resolve(element);
            string name = reference.ReferenceName;

            switch (reference.Type)
            {
                case ReferenceType.SDK:
                    clone = null;
                    return ConvertResult.Removed;

                case ReferenceType.NuGet:
                    if (this.lookupTable.IsExcludedNuGet(name, out _))
                    {
                        clone = null;
                        return ConvertResult.Removed;
                    }
                    if (this.lookupTable.IsReplaceNuGet(name, out string? replaceName))
                    {
                        name = replaceName;
                    }

                    // TODO:
                    // need a ceres dll map, replace ceres references

                    if (this.lookupTable.IsDefinedNuGet(name, out string? actualNuGetName, out _))
                    {
                        clone = XElement.Parse($@"<PackageReference Include=""{actualNuGetName}"" />");
                    }
                    else
                    {
                        clone = XElement.Parse($@"<PackageReference Include=""{name}"" {Tags.Undefined}=""true"" />");
                    }
                    return ConvertResult.Changed;

                case ReferenceType.Substrate:
                    if (this.lookupTable.IsProducedProject(name, out IProject? project))
                    {
                        clone = XElement.Parse($@"<Reference Include=""{project.Name}""><HintPath>{project.TargetPath}</HintPath></Reference>");
                    }
                    else
                    {
                        clone = XElement.Parse($@"<Reference Include=""{name}"" {Tags.Blocked}=""true""><HintPath>unknown</HintPath></Reference>");
                    }
                    return ConvertResult.Changed;

                default:
                    clone = null;
                    return ConvertResult.NotChanged;
            }
        }

        private ConvertResult ConvertImport(in XElement element, out XElement? clone)
        {
            if (element.HasAttribute(Tags.Project, @"$(EnvironmentConfig)") ||
                element.HasAttribute(Tags.Project, @"$(ExtendedTargetsPath)\Microsoft.CSharp.targets"))
            {
                clone = null;
                return ConvertResult.Removed;
            }

            if (element.HasAttribute(Tags.Project, @"$(BranchTargetsPath)\XmlStyler\MC-XmlStyler.targets") ||
                element.HasAttribute(Tags.Project, @"$(ExtendedTargetsPath)\MC-XmlStyler.targets"))
            {
                clone = element.Clone();
                clone.SetAttributeValue(Tags.Project, @"$(MSBuildExtensionsPath)\Override\MC-XmlStyler.targets");
                return ConvertResult.Changed;
            }

            string project = element.GetAttribute(Tags.Project).Value;
            if (project.StartsWithIgnoreCase("$(Pkg"))
            {
                var dummy = XElement.Parse($@"<Reference Include=""{project}"" />");
                return ConvertReference(in dummy, out clone);
            }

            clone = null;
            return ConvertResult.NotChanged;
        }

        private ConvertResult ConvertTarget(in XElement element, out XElement? clone)
        {
            clone = null;

            if (element.HasAttribute(Tags.Name, "_GenerateRestoreGraphProjectEntry") ||
                element.HasAttribute(Tags.Name, "_GenerateProjectRestoreGraph"))
            {
                return ConvertResult.Removed;
            }

            return ConvertResult.NotChanged;
        }

        private ConvertResult ConvertGeneralItem(in XElement element, out XElement? clone)
        {
            if (element.HasAttribute(Tags.Include))
            {
                string oldPath = element.GetAttribute(Tags.Include).Value;
                string newPath = MSBuildUtils.GetRelativePath(this.metadata.NFBuildFolder, this.metadata.ProducedFolder, oldPath);
                if (oldPath.EqualsIgnoreCase(newPath))
                {
                    clone = null;
                    return ConvertResult.NotChanged;
                }
                else
                {
                    clone = element.Clone();
                    clone.SetAttributeValue(Tags.Include, newPath);
                    return ConvertResult.Changed;
                }
            }

            clone = null;
            return ConvertResult.NotChanged;
        }

    }
}
