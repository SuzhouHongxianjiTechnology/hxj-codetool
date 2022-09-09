namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Constants;

    internal class DefaultFormatter : IFormatter<NetCoreProjectFile>
    {
        private NetCoreProjectFile _file;

        internal DefaultFormatter(NetCoreProjectFile file)
        {
            this._file = file;
        }

        public void Format()
        {
            // Remove NetFramework only properties
            {
                var properties = this._file.Document.GetAll(Tags.PropertyGroup)
                                                    .SelectMany(p => p.Elements());
                foreach (var property in properties)
                {
                    if (NetFrameworkConsts.NetFrameworkOnylProperties.Contains(property.Name.LocalName))
                    {
                        property.TryRemove();
                    }
                }
            }

            // Add a 'compile remove' if necessary
            {
                var compiles = this._file.Document.GetAll(Tags.Compile);

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
                        this._file.Document.GetFirst(Tags.Compile).AddBeforeSelf(removeCompile);
                    }
                    else if (hasCompileRemove && !hasGeneratedFile)
                    {
                        compiles.Where(c => c.HasAttribute(Tags.Remove)).Remove();
                    }
                }
            }

            // Remove packages if has a VariantConfiguration
            {
                if (this._file.ContainsVariantConfig)
                {
                    foreach (var package in this._file.PackageReferences.ToList())
                    {
                        if (DF.VariantConfig.HasPackage(package.Name))
                        {
                            this._file.PackageReferences.Remove(package);
                            package.TryRemove();
                        }
                    }
                }
            }

            // Group and sort Reference
            {
                var refParent = this._file.Document.GetFirst(Tags.Reference)?.Parent;
                if (refParent != null)
                {
                    var newGroup = new XElement(Tags.ItemGroup);
                    var referenceSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var reference in this._file.Document.GetAll(Tags.Reference))
                    {
                        string include = reference.GetAttribute(Tags.Include)?.Value;
                        if (!referenceSet.Contains(include))
                        {
                            var refClone = new XElement(reference);
                            var hintPath = refClone.GetFirst(Tags.HintPath);
                            if (hintPath != null)
                            {
                                string oldPath = hintPath.Value;
                                hintPath.SetValue(oldPath.Replace("$(TargetPathDir)\\", "$(TargetPathDir)"));
                            }
                            newGroup.Add(refClone);
                            referenceSet.Add(include);
                        }
                        reference.TryRemove();
                    }
                    newGroup.SortByAttribute(Tags.Include);
                    refParent.AddAfterSelf(newGroup);
                }
            }

            // Group and sort PackageReference
            {
                var pkgParent = this._file.Document.GetFirst(Tags.PackageReference)?.Parent;
                if (pkgParent != null)
                {
                    var newGroup = new XElement(Tags.ItemGroup);
                    var packageSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var pkgReference in this._file.Document.GetAll(Tags.PackageReference))
                    {
                        string include = pkgReference.GetAttribute(Tags.Include)?.Value;
                        if (!packageSet.Contains(include))
                        {
                            newGroup.Add(new XElement(pkgReference));
                            packageSet.Add(include);
                        }
                        pkgReference.TryRemove();
                    }
                    newGroup.SortByAttribute(Tags.Include);
                    pkgParent.AddAfterSelf(newGroup);
                }
            }

            // Remove empty groups
            {
                this._file.Document.GetAll(Tags.PropertyGroup).ForEach(group => group.RemoveIfEmpty());
                this._file.Document.GetAll(Tags.ItemGroup).ForEach(group => group.RemoveIfEmpty());
            }
        }
    }
}
